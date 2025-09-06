/**
 * Lista de productos con el mismo diseño/UX que PersonComponent:
 * - Búsqueda por nombre/descripcion
 * - Orden por id/name/price/createdAt/updatedAt
 * - Paginación en cliente con signals
 * - Estados de carga, error, vacío y skeleton
 *
 * Notas:
 * - Todo el filtrado/orden/paginación ocurre en memoria (lado cliente).
 * - Si el dataset crece, evaluar paginación/orden/filtrado en servidor.
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

/** Campos soportados para ordenar la grilla. */
type SortKey = 'id' | 'name' | 'price' | 'createdAt' | 'updatedAt';
/** Direcciones de ordenamiento posibles. */
type SortDir = 'asc' | 'desc';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,              // ngModel en toolbar y paginación
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatSnackBarModule,
    DatePipe
  ],
  templateUrl: './product-list.component.html',
  styles: [`
    /* Utilidad para truncar celdas largas sin romper el layout */
    .text-truncate { white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
  `]
})
export class ProductListComponent implements OnInit {
  // ---------------------------------------------------------------------------
  // Estado base (datos + flags)
  // ---------------------------------------------------------------------------

  /** Colección completa proveniente del backend. */
  products = signal<Product[]>([]);
  /** Indicador de carga para mostrar skeleton/spinner en UI. */
  isLoading = signal<boolean>(true);
  /** Mensaje de error a mostrar en UI si falla la carga/operación. */
  error = signal<string>('');

  // ---------------------------------------------------------------------------
  // Estado de UI (filtro/orden/paginación) controlado con signals
  // ---------------------------------------------------------------------------

  /** Texto de búsqueda aplicado a name/description (case-insensitive). */
  query = signal<string>('');
  /** Campo por el que se ordena la tabla. */
  sortKey = signal<SortKey>('id');
  /** Dirección de ordenamiento. */
  sortDir = signal<SortDir>('asc');
  /** Página actual (1-based). */
  page = signal<number>(1);
  /** Tamaño de página (cuántos ítems por página). */
  pageSize = signal<number>(10);

  // ---------------------------------------------------------------------------
  // Derivados (computed) que recalculan vista según cambios en los signals
  // ---------------------------------------------------------------------------

  /**
   * Lista final que se renderiza:
   * 1) Clona products
   * 2) Aplica filtro por query (name/description)
   * 3) Ordena por sortKey/sortDir
   * 4) Corta el tramo de la página actual (page/pageSize)
   */
  paged = computed(() => {
    let list = [...this.products()];

    // Filtro en memoria (optimizar en servidor si el dataset escala).
    const q = this.query().trim().toLowerCase();
    if (q) {
      list = list.filter(p =>
        (p.name?.toLowerCase().includes(q) || p.description?.toLowerCase().includes(q))
      );
    }

    // Orden en memoria por campo/dirección seleccionados.
    const key = this.sortKey();
    const dir = this.sortDir() === 'asc' ? 1 : -1;
    list.sort((a: any, b: any) => {
      const av = a[key] ?? '';
      const bv = b[key] ?? '';
      if (av < bv) return -1 * dir;
      if (av > bv) return  1 * dir;
      return 0;
    });

    // Paginación por "slice" de la colección filtrada/ordenada.
    const start = (this.page() - 1) * this.pageSize();
    return list.slice(start, start + this.pageSize());
  });

  /**
   * Total de elementos teniendo en cuenta el filtro actual.
   * Se usa para calcular rango visible y total de páginas.
   */
  totalItems = computed(() => {
    const q = this.query().trim().toLowerCase();
    if (!q) return this.products().length;
    return this.products().filter(p =>
      (p.name?.toLowerCase().includes(q) || p.description?.toLowerCase().includes(q))
    ).length;
  });

  /** Número total de páginas, mínimo 1 para evitar divisiones por cero. */
  totalPages = computed(() => Math.max(1, Math.ceil(this.totalItems() / this.pageSize())));

  /** Índice del último elemento mostrado (para “Mostrando X–Y de Z”). */
  endItem = computed(() => Math.min(this.page() * this.pageSize(), this.totalItems()));

  // ---------------------------------------------------------------------------
  // Inyección de dependencias
  // ---------------------------------------------------------------------------

  constructor(
    private productService: ProductService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  // ---------------------------------------------------------------------------
  // Ciclo de vida
  // ---------------------------------------------------------------------------

  /**
   * Inicializa el componente cargando los productos desde la API.
   * Se mantiene la lógica en un método separado (loadProducts) para reutilizarla.
   */
  ngOnInit(): void {
    this.loadProducts();
  }

  // ---------------------------------------------------------------------------
  // Acceso a datos
  // ---------------------------------------------------------------------------

  /**
   * Obtiene la lista desde el servicio y gestiona flags de carga/error.
   * Si la API no retorna array, se utiliza lista vacía para robustez.
   */
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
        // Notifica error en UI (snackbar informativo para el usuario).
        this.snackBar.open(this.error(), 'Cerrar', { duration: 5000 });
      }
    });
  }

  // ---------------------------------------------------------------------------
  // Helpers de UI (filtro/orden/paginación/recarga)
  // ---------------------------------------------------------------------------

  /** Reaplica el filtro y vuelve a la primera página para evitar páginas vacías. */
  applyFilter() { this.goToPage(1); }

  /**
   * Cambia la dirección de ordenamiento y reinicia a página 1.
   * @param dir 'asc' | 'desc'
   */
  setDir(dir: SortDir) { this.sortDir.set(dir); this.goToPage(1); }

  /**
   * Modifica el tamaño de página y reinicia a la primera.
   * @param size Nuevo tamaño de página
   */
  setPageSize(size: number) { this.pageSize.set(size); this.goToPage(1); }

  /**
   * Navega a una página válida dentro del rango [1, totalPages].
   * @param n Página destino (1-based)
   */
  goToPage(n: number) { this.page.set(Math.max(1, Math.min(n, this.totalPages()))); }

  /** Avanza una página si no se está en la última. */
  nextPage() { if (this.page() < this.totalPages()) this.page.update(p => p + 1); }

  /** Retrocede una página si no se está en la primera. */
  prevPage() { if (this.page() > 1) this.page.update(p => p - 1); }

  /** Recarga la lista desde el backend (reutiliza loadProducts). */
  reload() { this.loadProducts(); }

  // ---------------------------------------------------------------------------
  // Operaciones CRUD (eliminación con confirmación)
  // ---------------------------------------------------------------------------

  /**
   * Abre un diálogo de confirmación y elimina por id si el usuario acepta.
   * Tras eliminar, recarga la lista y notifica el resultado por snackbar.
   * @param id Identificador del producto a eliminar
   */
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
      if (!ok) return; // Usuario canceló

      this.productService.delete(id).subscribe({
        next: (success) => {
          if (success) {
            this.snackBar.open('Producto eliminado correctamente', 'Cerrar', { duration: 3000 });
            this.loadProducts(); // Refresca la tabla para reflejar cambios
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
