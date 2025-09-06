import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { OrderService, OrderReadDto, OrderCreateDto } from '../../../services/order.service';
import { ProductService } from '../../../services/product.service';
import { PersonService } from '../../../services/person.service';
import { Product } from '../../../models/product';
import { Person } from '../../../models/person';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-order-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './order-list.component.html',
  styles: [`
    .status-pending { color: #e67e22; }
    .status-completed { color: #27ae60; }
    .status-cancelled { color: #e74c3c; }
  `]
})
export class OrderListComponent implements OnInit {
  orders: OrderReadDto[] = [];
  filteredOrders: OrderReadDto[] = [];
  isLoading = true;
  isSaving = false;
  error = '';
  searchTerm = '';
  statusFilter = '';
  
  // For new order form
  showNewOrderForm = false;
  newOrder: OrderCreateDto = {
    personId: 0, // Will be set when a person is selected
    createdBy: 1, // Default user ID (you might want to get this from auth service)
    orderDetails: []
  };
  
  selectedPersonId: number | null = null;
  availableProducts: Product[] = [];
  availablePersons: Person[] = [];
  isLoadingProducts = false;
  isLoadingPersons = false;

  constructor(
    private orderService: OrderService,
    private productService: ProductService,
    private personService: PersonService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadOrders();
    this.loadProducts();
    this.loadPersons();
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
        alert('Error al cargar los productos');
        this.isLoadingProducts = false;
      }
    });
  }

  loadPersons(): void {
    this.isLoadingPersons = true;
    this.personService.getAll().subscribe({
      next: (persons: Person[]) => {
        this.availablePersons = persons;
        this.isLoadingPersons = false;
      },
      error: (err: any) => {
        console.error('Error loading persons:', err);
        alert('Error al cargar los clientes');
        this.isLoadingPersons = false;
      }
    });
  }

  onPersonSelect(personId: number | null): void {
    if (personId !== null) {
      this.newOrder.personId = personId;
    } else {
      this.newOrder.personId = 0; // Reset to default value if null
    }
  }

  loadOrders(): void {
    this.isLoading = true;
    this.orderService.getOrders().subscribe({
      next: (orders: OrderReadDto[]) => {
        // Asegurarse de que cada orden tenga un array orderDetails
        this.orders = orders.map(order => ({
          ...order,
          orderDetails: order.orderDetails || []
        }));
        this.filteredOrders = [...this.orders];
        this.isLoading = false;
      },
      error: (err: any) => {
        console.error('Error loading orders:', err);
        this.error = 'Error al cargar las órdenes';
        alert('Error al cargar las órdenes: ' + (err.error?.message || err.message || 'Error desconocido'));
        this.isLoading = false;
      }
    });
  }

  applyFilters(): void {
    if (!this.orders) return;
    
    this.filteredOrders = this.orders.filter(order => {
      const searchTerm = this.searchTerm.toLowerCase();
      const matchesSearch = 
        (order.personName?.toLowerCase().includes(searchTerm) || false) ||
        order.id.toString().includes(searchTerm);
      
      const matchesStatus = !this.statusFilter || 
                          (order.status && order.status.toLowerCase() === this.statusFilter.toLowerCase());
      
      return matchesSearch && matchesStatus;
    });
  }

  onSearchChange(): void {
    this.applyFilters();
  }

  onStatusFilterChange(): void {
    this.applyFilters();
  }

  addOrderItem(): void {
    // Agregar un nuevo ítem con el primer producto seleccionado por defecto
    const firstProduct = this.availableProducts[0];
    if (firstProduct && firstProduct.id !== undefined) {
      this.newOrder.orderDetails.push({ 
        itemId: firstProduct.id, 
        quantity: 1, 
        unitPrice: firstProduct.price || 0, 
        itemName: firstProduct.name || '' 
      });
    }
  }

  removeOrderItem(index: number): void {
    this.newOrder.orderDetails.splice(index, 1);
  }

  onItemChange(index: number): void {
    const item = this.newOrder.orderDetails[index];
    const selectedProduct = this.availableProducts.find(p => p.id === item.itemId);
    if (selectedProduct) {
      item.unitPrice = selectedProduct.price;
      item.itemName = selectedProduct.name || '';
    }
  }

  createOrder(): void {
    if (!this.newOrder.personId) {
      alert('Por favor seleccione un cliente');
      return;
    }

    if (this.newOrder.orderDetails.length === 0) {
      alert('Advertencia: Debe agregar al menos un ítem a la orden');
      return;
    }

    if (confirm('¿Está seguro de crear esta orden?')) {
      this.isSaving = true;
      this.orderService.createOrder(this.newOrder).subscribe({
        next: (order) => {
          this.isSaving = false;
          alert('¡Orden creada exitosamente!');
          this.loadOrders();
          this.resetNewOrderForm();
          this.showNewOrderForm = false;
        },
        error: (err) => {
          this.isSaving = false;
          console.error('Error creating order:', err);
          alert('Error al crear la orden: ' + (err.error?.message || err.message || 'Error desconocido'));
        }
      });
    }
  }

  updateOrderStatus(order: OrderReadDto, status: string): void {
    const updatedOrder = { ...order, status };
    this.orderService.updateOrder(order.id, updatedOrder as any).subscribe({
      next: (success) => {
        if (success) {
          alert('Estado actualizado correctamente');
          this.loadOrders();
        } else {
          alert('Error al actualizar el estado');
        }
      },
      error: (err) => {
        console.error('Error updating order status:', err);
        alert('Error al actualizar el estado: ' + (err.error?.message || err.message || 'Error desconocido'));
      }
    });
  }

  deleteOrder(id: number): void {
    if (confirm('¿Estás seguro de que deseas eliminar esta orden? Esta acción no se puede deshacer.')) {
      this.orderService.deleteOrder(id).subscribe({
        next: (success) => {
          if (success) {
            alert('¡Orden eliminada correctamente!');
            this.loadOrders();
          } else {
            alert('No se pudo eliminar la orden');
          }
        },
        error: (err) => {
          console.error('Error deleting order:', err);
          alert('Error al eliminar la orden: ' + (err.error?.message || err.message || 'Error desconocido'));
        }
      });
    }
  }

  getOrderTotal(order: OrderReadDto): number {
    if (!order.orderDetails || order.orderDetails.length === 0) return 0;
    return order.orderDetails.reduce((total, item) => {
      const quantity = item.quantity || 0;
      const unitPrice = item.unitPrice || 0;
      return total + (quantity * unitPrice);
    }, 0);
  }

  getStatusClass(status: string | undefined): string {
    if (!status) return 'status-pending';
    switch (status.toLowerCase()) {
      case 'pendiente': return 'status-pending';
      case 'completada': return 'status-completed';
      case 'cancelada': return 'status-cancelled';
      default: return 'status-pending';
    }
  }

  private resetNewOrderForm(): void {
    this.newOrder = {
      personId: 0,
      createdBy: 1,
      orderDetails: []
    };
    this.selectedPersonId = null;
  }
}
