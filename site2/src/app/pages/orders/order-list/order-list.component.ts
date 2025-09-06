import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { OrderService, OrderReadDto, OrderCreateDto, OrderItemDto, BackendOrderCreateDto } from '../../../services/order.service';
import { ProductService } from '../../../services/product.service';
import { Product } from '../../../models/product';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-order-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    MatIconModule,
    MatButtonModule,
    MatTooltipModule
  ],
  templateUrl: './order-list.component.html',
  styles: []
})
export class OrderListComponent implements OnInit {
  // Make Math available in the template
  Math = Math;

  // State management
  orders: OrderReadDto[] = [];
  filteredOrders: OrderReadDto[] = [];
  isLoading = signal(true);
  isSaving = false;
  error = signal('');

  // Pagination
  currentPage = 1;
  pageSize = 10;
  totalItems = 0;

  // Filters and sorting
  searchTerm = '';
  statusFilter = '';
  sortKey = 'createdAt';
  sortDir: 'asc' | 'desc' = 'desc';

  // Data
  availableProducts: Product[] = [];

  // New order form
  newOrder: OrderCreateDto = {
    customerName: '',
    customerEmail: '',
    items: [],
    status: 'PENDING'
  };

  // UI state
  showNewOrderForm = false;
  isLoadingProducts = false;

  constructor(
    private orderService: OrderService,
    private productService: ProductService,
    private router: Router,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.loadOrders();
    this.loadProducts();
  }

  // Calculate order total
  getOrderTotal(order: OrderReadDto): number {
    return order.items?.reduce((total, item) => {
      return total + ((item.quantity || 0) * (item.unitPrice || 0));
    }, 0) || 0;
  }

  // Get status badge class
  getStatusClass(status: string): string {
    switch (status?.toUpperCase()) {
      case 'PENDING':
        return 'bg-warning';
      case 'COMPLETED':
        return 'bg-success text-white';
      case 'CANCELLED':
      case 'CANCELED':
        return 'bg-danger text-white';
      default:
        return 'bg-secondary text-white';
    }
  }

  loadProducts(): void {
    this.isLoadingProducts = true;
    this.productService.getAll().subscribe({
      next: (products: Product[]) => {
        this.availableProducts = products;
        this.isLoadingProducts = false;
      },
      error: (err: any) => {
        console.error('Error loading products:', err);
        this.snackBar.open('Error al cargar los productos', 'Cerrar', { duration: 3000 });
        this.isLoadingProducts = false;
      }
    });
  }

  loadOrders(): void {
    this.isLoading.set(true);
    this.orderService.getOrders().subscribe({
      next: (orders: OrderReadDto[]) => {
        this.orders = orders;
        this.applyFilters();
        this.isLoading.set(false);
      },
      error: (err: any) => {
        console.error('Error loading orders:', err);
        this.error.set('Error al cargar las órdenes');
        this.snackBar.open('Error al cargar las órdenes', 'Cerrar', { duration: 3000 });
        this.isLoading.set(false);
      }
    });
  }

  applyFilters(): void {
    if (!this.orders) return;

    const searchTerm = this.searchTerm.toLowerCase();

    this.filteredOrders = this.orders.filter(order => {
      const matchesSearch =
        (order.customerName?.toLowerCase().includes(searchTerm) || false) ||
        order.id?.toString().includes(searchTerm) ||
        (order.customerEmail?.toLowerCase().includes(searchTerm) || false);

      const matchesStatus = !this.statusFilter ||
        (order.status && order.status.toUpperCase() === this.statusFilter.toUpperCase());

      return matchesSearch && matchesStatus;
    });

    // Sort the results
    this.filteredOrders.sort((a, b) => {
      let aValue: any, bValue: any;

      switch (this.sortKey) {
        case 'createdAt':
          aValue = new Date(a.createdAt || 0).getTime();
          bValue = new Date(b.createdAt || 0).getTime();
          break;
        case 'id':
          aValue = a.id || 0;
          bValue = b.id || 0;
          break;
        case 'customerName':
          aValue = (a.customerName || '').toLowerCase();
          bValue = (b.customerName || '').toLowerCase();
          break;
        case 'total':
          aValue = this.getOrderTotal(a);
          bValue = this.getOrderTotal(b);
          break;
        case 'status':
          aValue = a.status || '';
          bValue = b.status || '';
          break;
        default:
          aValue = a[this.sortKey as keyof OrderReadDto];
          bValue = b[this.sortKey as keyof OrderReadDto];
      }

      if (aValue < bValue) return this.sortDir === 'asc' ? -1 : 1;
      if (aValue > bValue) return this.sortDir === 'asc' ? 1 : -1;
      return 0;
    });

    this.totalItems = this.filteredOrders.length;
    this.currentPage = Math.min(this.currentPage, this.totalPages);
  }

  onSearchChange(): void {
    this.applyFilters();
  }

  onStatusFilterChange(): void {
    this.applyFilters();
  }

  get totalPages(): number {
    return Math.ceil(this.filteredOrders.length / this.pageSize);
  }

  get pagedOrders(): OrderReadDto[] {
    const start = (this.currentPage - 1) * this.pageSize;
    const end = start + this.pageSize;
    return this.filteredOrders.slice(start, end);
  }

  onPageSizeChange(): void {
    this.currentPage = 1;
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
    }
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }

  reload(): void {
    this.isLoading.set(true);
    this.loadOrders();
  }

  setSortKey(key: string): void {
    if (this.sortKey === key) {
      this.sortDir = this.sortDir === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortKey = key;
      this.sortDir = 'asc';
    }
    this.applyFilters();
  }

  updateOrderStatus(orderId: number, status: string): void {
    if (!orderId) return;

    this.isSaving = true;
    this.orderService.updateStatus(orderId, status).subscribe({
      next: (updatedOrder) => {
        if (updatedOrder) {
          const index = this.orders.findIndex(o => o.id === orderId);
          if (index !== -1) {
            this.orders[index] = updatedOrder;
            this.applyFilters();
            this.snackBar.open('Estado de la orden actualizado', 'Cerrar', { duration: 3000 });
          }
        } else {
          this.snackBar.open('No se pudo actualizar el estado de la orden', 'Cerrar', { duration: 3000 });
        }
        this.isSaving = false;
      },
      error: (err) => {
        console.error('Error updating order status:', err);
        this.snackBar.open(
          'Error al actualizar el estado de la orden: ' + (err.error?.message || err.message || 'Error desconocido'),
          'Cerrar',
          { duration: 5000 }
        );
        this.isSaving = false;
      }
    });
  }

  deleteOrder(id: number): void {
    if (!confirm('¿Está seguro de eliminar esta orden? Esta acción no se puede deshacer.')) {
      return;
    }

    this.isSaving = true;
    this.orderService.deleteOrder(id).subscribe({
      next: (success) => {
        if (success) {
          this.orders = this.orders.filter(order => order.id !== id);
          this.applyFilters();
          this.snackBar.open('Orden eliminada correctamente', 'Cerrar', { duration: 3000 });
        } else {
          this.snackBar.open('No se pudo eliminar la orden', 'Cerrar', { duration: 3000 });
        }
        this.isSaving = false;
      },
      error: (err) => {
        console.error('Error deleting order:', err);
        this.snackBar.open(
          'Error al eliminar la orden: ' + (err.error?.message || err.message || 'Error desconocido'),
          'Cerrar',
          { duration: 5000 }
        );
        this.isSaving = false;
      }
    });
  }

  addOrderItem(): void {
    // Agregar un nuevo ítem con el primer producto seleccionado por defecto
    const firstProduct = this.availableProducts[0];
    if (firstProduct && firstProduct.id !== undefined) {
      this.newOrder.items.push({
        itemId: firstProduct.id,
        quantity: 1,
        unitPrice: firstProduct.price || 0,
        itemName: firstProduct.name || ''
      });
    }
  }

  removeOrderItem(index: number): void {
    this.newOrder.items.splice(index, 1);
  }

  onItemChange(index: number): void {
    const item = this.newOrder.items[index];
    const selectedProduct = this.availableProducts.find(p => p.id === item.itemId);
    if (selectedProduct) {
      item.unitPrice = selectedProduct.price;
      item.itemName = selectedProduct.name || '';
    }
  }

  createOrder(): void {
    // Redirigir a la pantalla de creación/edición que ya tiene selector de cliente (personId)
    this.router.navigate(['/orders/new']);
  }

  private resetNewOrderForm(): void {
    this.newOrder = {
      customerName: '',
      customerEmail: '',
      items: [],
      status: 'PENDING'
    };
  }
}
