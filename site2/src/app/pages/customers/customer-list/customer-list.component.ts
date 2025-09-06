import { CommonModule, DatePipe } from '@angular/common';
import { Component, OnInit, computed, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { PersonService } from '../../../services/person.service';

// Tipos auxiliares para definir claves de ordenamiento y dirección
type SortKey = 'id' | 'firstName' | 'lastName' | 'email' | 'createdAt';
type SortDir = 'asc' | 'desc';

@Component({
  selector: 'app-customer-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, DatePipe],
  templateUrl: './customer-list.component.html',
  styles: [`
    .placeholder { display:inline-block; height: 1rem; }
    .spinner-border { display: inline-block; }
  `]
})
export class CustomerListComponent implements OnInit {
  /** ===============================
   * Estado reactivo de la aplicación
   * =============================== */
  persons = signal<any[]>([]);          // Lista de clientes cargados
  isLoading = signal<boolean>(true);    // Bandera de carga
  error = signal<string>('');           // Mensaje de error (si lo hay)

  /** ===============================
   * Estado de la interfaz (UI state)
   * =============================== */
  query = '';                           // Texto de búsqueda
  sortKey: SortKey = 'id';              // Columna por la cual ordenar
  sortDir: SortDir = 'asc';             // Dirección de ordenamiento
  page = 1;                             // Página actual
  pageSize = 10;                        // Tamaño de página

  // Tick de UI para forzar recomputación de computed() cuando cambian props no-signal
  private uiTick = signal<number>(0);

  constructor(private personService: PersonService) {}

  /** Al iniciar el componente se cargan los clientes */
  ngOnInit(): void { this.reload(); }

  /** Recarga los clientes desde el servicio */
  reload(): void {
    this.isLoading.set(true);
    this.error.set('');

    this.personService.getPersons().subscribe({
      next: data => {
        // Normaliza campos y fechas (acepta alias nombre/apellido/correo)
        const mapped = (data || []).map((p: any) => ({
          ...p,
          firstName: p.firstName ?? p.nombre ?? '',
          lastName:  p.lastName  ?? p.apellido ?? '',
          email:     p.email     ?? p.correo   ?? '',
          createdAt: p.createdAt ? new Date(p.createdAt) : null
        }));
        this.persons.set(mapped);
        this.isLoading.set(false);
        this.goToPage(1); // Reinicia a la primera página
      },
      error: err => {
        console.error('Error loading persons:', err);
        this.error.set('Ocurrió un error al cargar los clientes.');
        this.isLoading.set(false);
      }
    });
  }

  /** =================================
   * Computed properties (derivaciones)
   * ================================= */
  // Aplica búsqueda y ordenamiento
  filtered = computed(() => {
    // Registrar dependencia con cambios de UI (query, sortKey, sortDir)
    this.uiTick();
    const q = this.normalize(this.query);
    const list = this.persons();
    if (!q) return this.sort(list); // si no hay búsqueda, solo ordenar

    // Filtra por nombre, apellido o correo
    return this.sort(
      list.filter(p => {
        const fn = this.normalize(p.firstName || p.nombre || '');
        const ln = this.normalize(p.lastName  || p.apellido || '');
        const em = this.normalize(p.email     || p.correo   || '');
        const id = (p.id ?? '').toString();
        return fn.includes(q) || ln.includes(q) || em.includes(q) || id.includes(q);
      })
    );
  });

  // Retorna solo la página actual
  paged = computed(() => {
    // Registrar dependencia con cambios de UI (page, pageSize)
    this.uiTick();
    const start = (this.page - 1) * this.pageSize;
    return this.filtered().slice(start, start + this.pageSize);
  });

  // Total de páginas según cantidad de datos filtrados
  totalPages(): number {
    const len = this.filtered().length;
    return Math.ceil(len / this.pageSize);
  }

  // Índice inicial de la página actual
  startIndex(): number {
    return Math.min((this.page - 1) * this.pageSize, Math.max(this.filtered().length - 1, 0));
  }

  // Índice final de la página actual
  endIndex(): number {
    return Math.min(this.page * this.pageSize, this.filtered().length);
  }

  /** =================================
   * Helpers de UI (paginación y orden)
   * ================================= */
  setDir(dir: SortDir) { this.sortDir = dir; this.goToPage(1); }

  goToPage(n: number) {
    this.page = Math.max(1, Math.min(n, this.totalPages() || 1));
    // Notificar cambio a los computed
    this.uiTick.update(v => v + 1);
  }

  nextPage() { this.goToPage(this.page + 1); }
  prevPage() { this.goToPage(this.page - 1); }

  /** Ordenamiento personalizado */
  private sort(list: any[]): any[] {
    const key = this.sortKey;
    const dir = this.sortDir === 'asc' ? 1 : -1;

    return [...list].sort((a, b) => {
      const va = this.normalize((a?.[key] ?? '').toString());
      const vb = this.normalize((b?.[key] ?? '').toString());

      // Orden especial para ID (numérico) y fecha
      if (key === 'id') return (a.id - b.id) * dir;
      if (key === 'createdAt') {
        const ta = a.createdAt ? new Date(a.createdAt).getTime() : 0;
        const tb = b.createdAt ? new Date(b.createdAt).getTime() : 0;
        return (ta - tb) * dir;
      }

      // Para strings: comparar ignorando mayúsculas/minúsculas
      return va.localeCompare(vb) * dir;
    });
  }

  /** Normaliza texto: minúsculas y sin tildes */
  private normalize(text: string): string {
    return (text || '')
      .toString()
      .toLowerCase()
      .normalize('NFD')
      .replace(/\p{Diacritic}+/gu, '')
      .trim();
  }
}
