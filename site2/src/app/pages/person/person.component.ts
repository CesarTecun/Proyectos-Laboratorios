import { CommonModule } from '@angular/common';
import { Component, inject, ViewChild, AfterViewInit, signal, computed } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginator, MatPaginatorIntl, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table'; // Nota: no se utiliza en este componente
import { MatTooltipModule } from '@angular/material/tooltip';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import Swal from 'sweetalert2';
import { Person } from '../../models/person';
import { PersonService } from '../../services/person.service';
import { PersonEditComponent } from '../../management/person-edit/person-edit.component';

/** Campos permitidos para ordenar la grilla. */
type SortKey = 'id' | 'firstName' | 'lastName' | 'email' | 'createdAt';
/** Direcciones de ordenamiento. */
type SortDir = 'asc' | 'desc';

/**
 * Configura las etiquetas del paginador de Angular Material en español.
 * Se inyecta por proveedor en el decorador @Component.
 */
function getSpanishPaginatorIntl(): MatPaginatorIntl {
  const intl = new MatPaginatorIntl();
  intl.itemsPerPageLabel = 'Elementos por página';
  intl.nextPageLabel = 'Siguiente página';
  intl.previousPageLabel = 'Página anterior';
  intl.firstPageLabel = 'Primera página';
  intl.lastPageLabel = 'Última página';
  // Etiqueta "X–Y de Z" con control de casos sin datos.
  intl.getRangeLabel = (page: number, pageSize: number, length: number) => {
    if (length === 0 || pageSize === 0) return `0 de ${length}`;
    const start = page * pageSize + 1;
    const end = Math.min((page + 1) * pageSize, length);
    return `${start}–${end} de ${length}`;
  };
  return intl;
}

@Component({
  selector: 'app-person',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatTooltipModule,
    MatCardModule
  ],
  templateUrl: './person.component.html',
  styles: [`
    .placeholder { display:inline-block; height: 1rem; }
    .spinner-border { display: inline-block; }
    .table-responsive { min-height: 300px; }
  `],
  providers: [
    // Proveedor para traducir el paginador a español
    { provide: MatPaginatorIntl, useFactory: getSpanishPaginatorIntl }
  ]
})
export class PersonComponent implements AfterViewInit {
  // Inyección de dependencias (APIs de Angular sin constructor).
  private personService = inject(PersonService);
  private dialog = inject(MatDialog);

  // ------------------------------
  // Estado reactivo (Signals)
  // ------------------------------

  /** Lista completa de personas recibida desde el backend. */
  persons = signal<Person[]>([]);
  /** Flag de carga para mostrar spinners en UI. */
  isLoading = signal<boolean>(true);
  /** Mensaje de error de carga/operaciones. */
  error = signal<string>('');
  
  // ------------------------------
  // Estado de la interfaz (filtro/orden/paginación)
  // ------------------------------

  /** Texto de búsqueda aplicado a firstName/lastName/email. */
  query = signal<string>('');
  /** Campo por el cual se ordena. */
  sortKey = signal<SortKey>('id');
  /** Dirección de ordenamiento. */
  sortDir = signal<SortDir>('asc');
  /** Página actual (1-based). */
  page = signal<number>(1);
  /** Tamaño de página actual. */
  pageSize = signal<number>(10);
  
  // ------------------------------
  // Derivados (computed)
  // ------------------------------

  /**
   * Colección paginada y ordenada que se muestra en la tabla.
   * 1) Clona persons
   * 2) Aplica filtro por query
   * 3) Ordena por sortKey/sortDir
   * 4) Recorta según page/pageSize
   */
  paged = computed(() => {
    let result = [...this.persons()];
    
    // Filtro por texto (case-insensitive) en nombre, apellido y email.
    if (this.query()) {
      const q = this.query().toLowerCase();
      result = result.filter(p => 
        (p.firstName?.toLowerCase().includes(q) || 
         p.lastName?.toLowerCase().includes(q) || 
         p.email?.toLowerCase().includes(q))
      );
    }
    
    // Ordenamiento por campo seleccionado.
    result.sort((a: any, b: any) => {
      const key = this.sortKey();
      const dir = this.sortDir() === 'asc' ? 1 : -1;
      
      if (a[key] < b[key]) return -1 * dir;
      if (a[key] > b[key]) return 1 * dir;
      return 0;
    });
    
    // Paginación por slicing en memoria (cliente).
    const start = (this.page() - 1) * this.pageSize();
    return result.slice(start, start + this.pageSize());
  });
  
  /** Cantidad total de páginas según total y pageSize. */
  totalPages = computed(() => 
    Math.ceil(this.persons().length / this.pageSize())
  );
  
  /** Total de elementos (sin filtro de búsqueda). */
  totalItems = computed(() => this.persons().length);
  
  // ------------------------------
  // Referencias a componentes de Angular Material
  // ------------------------------

  /** Referencia al paginador de Angular Material (se usa para tamaño inicial). */
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  /** Referencia al MatSort para sincronizar sort activo/dirección. */
  @ViewChild(MatSort) matSort!: MatSort;

  // Exponer Math al template para utilidades (p. ej. cálculos básicos).
  Math = Math;
  
  // ------------------------------
  // Utilidades de UI (cálculo de rangos visibles)
  // ------------------------------

  /** Índice de inicio (1-based) del rango visible actual. */
  get currentPageStart(): number {
    return (this.page() - 1) * this.pageSize() + 1;
  }

  /** Índice de fin (1-based) del rango visible actual. */
  get currentPageEnd(): number {
    return Math.min(this.page() * this.pageSize(), this.totalItems());
  }

  // ------------------------------
  // Ciclo de vida
  // ------------------------------

  /** Carga inicial de la tabla. */
  ngOnInit() {
    this.loadPersons();
  }

  /**
   * Sincroniza componentes Material una vez que el template está inicializado.
   * - Ajusta pageSize del paginador
   * - Vincula MatSort para reflejar sortKey/sortDir y reiniciar a la página 1
   */
  ngAfterViewInit() {
    // Configurar el paginador de Material si es necesario
    if (this.paginator) {
      this.paginator.pageSize = this.pageSize();
    }
    
    // Configurar el ordenamiento de Material si es necesario
    if (this.matSort) {
      this.matSort.active = this.sortKey();
      this.matSort.direction = this.sortDir() as 'asc' | 'desc';
      
      this.matSort.sortChange.subscribe(sort => {
        this.sortKey.set(sort.active as SortKey);
        this.sortDir.set(sort.direction as SortDir);
        this.goToPage(1);
      });
    }
  }

  // ------------------------------
  // Acceso a datos
  // ------------------------------

  /**
   * Obtiene personas desde el servicio y actualiza estados de carga/error.
   * Maneja respuesta no-array como lista vacía por seguridad.
   */
  loadPersons() {
    this.isLoading.set(true);
    this.error.set('');
    
    this.personService.getAll().subscribe({
      next: (res) => {
        const persons = Array.isArray(res) ? res : [];
        this.persons.set(persons);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Error al cargar personas:', err);
        this.error.set('No se pudieron cargar las personas. Por favor, intente nuevamente.');
        this.isLoading.set(false);
      }
    });
  }

  // ------------------------------
  // Navegación de páginas (helpers)
  // ------------------------------

  /**
   * Cambia a una página válida dentro de [1, totalPages].
   * @param page Número de página (1-based)
   */
  goToPage(page: number) {
    this.page.set(Math.max(1, Math.min(page, this.totalPages())));
  }
  
  /** Avanza una página si no se está en la última. */
  nextPage() {
    if (this.page() < this.totalPages()) {
      this.page.update(p => p + 1);
    }
  }
  
  /** Retrocede una página si no se está en la primera. */
  prevPage() {
    if (this.page() > 1) {
      this.page.update(p => p - 1);
    }
  }
  
  /**
   * Cambia el tamaño de página y reinicia la navegación a la primera página.
   * @param size Nuevo tamaño de página
   */
  setPageSize(size: number) {
    this.pageSize.set(size);
    this.goToPage(1);
  }
  
  /**
   * Cambia la dirección de ordenamiento y vuelve a la primera página.
   * @param dir Dirección 'asc' | 'desc'
   */
  setDir(dir: SortDir) {
    this.sortDir.set(dir);
    this.goToPage(1);
  }

  /** Reaplica el filtro actual reiniciando la navegación. */
  applyFilter() {
    this.goToPage(1);
  }
  
  /** Recarga la lista desde el backend. */
  reload() {
    this.loadPersons();
  }

  // ------------------------------
  // CRUD (alta/edición/eliminación)
  // ------------------------------

  /**
   * Abre el diálogo de edición/creación de persona.
   * Si el resultado es afirmativo (ok), recarga los datos.
   * @param person Persona a editar; si se omite, se interpreta como "nueva".
   */
  openDialog(person?: Person) {
    const dialogRef = this.dialog.open(PersonEditComponent, {
      width: '99%',
      height: '99%',
      data: person,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe((ok) => {
      if (ok) this.loadPersons();
    });
  }

  /**
   * Confirma y elimina una persona por id. Al finalizar, recarga la lista.
   * Muestra notificaciones de éxito o error mediante SweetAlert2.
   * @param id Identificador de la persona a eliminar
   */
  deletePerson(id: number) {
    Swal.fire({
      title: '¿Eliminar?',
      text: 'Esta acción no se puede deshacer.',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Sí, eliminar',
      cancelButtonText: 'Cancelar'
    }).then((r) => {
      if (r.isConfirmed) {
        this.personService.delete(id).subscribe({
          next: () => {
            this.loadPersons();
            Swal.fire('Eliminado', 'La persona fue eliminada.', 'success');
          },
          error: (err) => {
            console.error('Error deleting person:', err);
            Swal.fire('Error', 'No se pudo eliminar.', 'error');
          }
        });
      }
    });
  }
}
