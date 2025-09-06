/**
 * Lista de productos con el mismo diseño/UX que PersonComponent:
 * - Búsqueda por nombre/descripcion
 * - Orden por id/name/price/createdAt/updatedAt
 * - Paginación en cliente con signals
 * - Estados de carga, error, vacío y skeleton
 */
import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Product } from '../../../models/product';
import { ProductService } from '../../../services/product.service';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog.component';

type SortKey = 'id' | 'name' | 'price' | 'createdAt' | 'updatedAt';
type SortDir = 'asc' | 'desc';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,              // para ngModel en toolbar/paginación
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatSnackBarModule,
    DatePipe
  ],
  templateUrl: './product-list.component.html',
  styles: [`
    .text-truncate { white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
  `]
})
export class ProductListComponent implements OnInit {
  // Estado base
  products = signal<Product[]>([]);
  isLoading = signal<boolean>(true);
  error = signal<string>('');

  // Estado de UI
  query = signal<string>('');
  sortKey = signal<SortKey>('id');
  sortDir = signal<SortDir>('asc');
  page = signal<number>(1);
  pageSize = signal<number>(10);

  // Derivados
  paged = computed(() => {
    let list = [...this.products()];

    // Filtro
    const q = this.query().trim().toLowerCase();
    if (q) {
      list = list.filter(p =>
        (p.name?.toLowerCase().includes(q) || p.description?.toLowerCase().includes(q))
      );
    }

    // Orden
    const key = this.sortKey();
    const dir = this.sortDir() === 'asc' ? 1 : -1;
    list.sort((a: any, b: any) => {
      const av = a[key] ?? '';
      const bv = b[key] ?? '';
      if (av < bv) return -1 * dir;
      if (av > bv) return  1 * dir;
      return 0;
    });

    // Paginación
    const start = (this.page() - 1) * this.pageSize();
    return list.slice(start, start + this.pageSize());
  });

  totalItems = computed(() => {
    const q = this.query().trim().toLowerCase();
    if (!q) return this.products().length;
    return this.products().filter(p =>
      (p.name?.toLowerCase().includes(q) || p.description?.toLowerCase().includes(q))
    ).length;
  });

  totalPages = computed(() => Math.max(1, Math.ceil(this.totalItems() / this.pageSize())));
  endItem = computed(() => Math.min(this.page() * this.pageSize(), this.totalItems()));

  constructor(
    private productService: ProductService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  /** Carga desde API */
  loadProducts(): void {
    this.isLoading.set(true);
    this.error.set('');
    this.productService.getAll().subscribe({
      next: (products) => {
        this.products.set(Array.isArray(products) ? products : []);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Error al cargar productos:', err);
        this.error.set('Error al cargar la lista de productos');
        this.isLoading.set(false);
        this.snackBar.open(this.error(), 'Cerrar', { duration: 5000 });
      }
    });
  }

  /** Helpers UI */
  applyFilter() { this.goToPage(1); }
  setDir(dir: SortDir) { this.sortDir.set(dir); this.goToPage(1); }
  setPageSize(size: number) { this.pageSize.set(size); this.goToPage(1); }
  goToPage(n: number) { this.page.set(Math.max(1, Math.min(n, this.totalPages()))); }
  nextPage() { if (this.page() < this.totalPages()) this.page.update(p => p + 1); }
  prevPage() { if (this.page() > 1) this.page.update(p => p - 1); }
  reload() { this.loadProducts(); }

  /** Eliminar con confirmación */
  onDelete(id: number): void {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      width: '350px',
      data: {
        title: 'Eliminar Producto',
        message: '¿Está seguro de que desea eliminar este producto?',
        confirmText: 'Eliminar',
        cancelText: 'Cancelar'
      }
    });

    ref.afterClosed().subscribe(ok => {
      if (!ok) return;
      this.productService.delete(id).subscribe({
        next: (success) => {
          if (success) {
            this.snackBar.open('Producto eliminado correctamente', 'Cerrar', { duration: 3000 });
            this.loadProducts();
          } else {
            this.snackBar.open('Error al eliminar el producto', 'Cerrar', { duration: 5000 });
          }
        },
        error: (err) => {
          console.error('Error al eliminar el producto:', err);
          this.snackBar.open('Error al eliminar el producto', 'Cerrar', { duration: 5000 });
        }
      });
    });
  }
}
