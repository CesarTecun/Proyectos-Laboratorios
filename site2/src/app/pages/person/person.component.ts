import { CommonModule } from '@angular/common';
import { Component, inject, ViewChild, AfterViewInit, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatCardModule } from '@angular/material/card';
import Swal from 'sweetalert2';

import { Person } from '../../models/person';
import { PersonService } from '../../services/person.service';
import { PersonEditComponent } from '../../management/person-edit/person-edit.component';

@Component({
  selector: 'app-person',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
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
  styleUrls: ['./person.component.css'] // <-- en plural
})
export class PersonComponent implements OnInit, AfterViewInit {

  private personService = inject(PersonService);
  private dialog = inject(MatDialog);

  // columnas visibles (solo lo que quieres editar)
  displayedColumns: string[] = ['id', 'firstName', 'lastName', 'email', 'action'];
  dataSource = new MatTableDataSource<Person>([]);

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  ngOnInit() {
    // configuración del filtro: busca por nombre completo o correo
    const norm = (s: any) =>
      (s ?? '').toString().toLowerCase().normalize('NFD').replace(/\p{Diacritic}/gu, '');

    this.dataSource.filterPredicate = (data: Person, filter: string) => {
      const f = norm(filter);
      const fullName = `${norm(data.firstName)} ${norm(data.lastName)}`.trim();
      const email = norm(data.email);
      return fullName.includes(f) || email.includes(f);
    };

    // carga inicial
    this.loadPersons();
  }

  ngAfterViewInit() {
    // conecta paginator y sort cuando la vista ya existe
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;

    // ordenamiento robusto SOLO para columnas existentes
    this.dataSource.sortingDataAccessor = (item: Person, property: string): string | number => {
      switch (property) {
        case 'firstName': return (item.firstName ?? '').toLowerCase();
        case 'lastName':  return (item.lastName  ?? '').toLowerCase();
        case 'email':     return (item.email     ?? '').toLowerCase();
        case 'id':        return Number(item.id) || 0;
        default:          return '';
      }
    };
  }

  loadPersons() {
    this.personService.getAll().subscribe({
      next: (res) => {
        const persons = Array.isArray(res) ? res : [];
        this.dataSource.data = persons;

        // reubica al inicio si hay paginador
        if (this.dataSource.paginator) {
          this.dataSource.paginator.firstPage();
        }
      },
      error: (err) => {
        console.error('Error al cargar personas:', err);
        Swal.fire('Error', 'No se pudieron cargar las personas.', 'error');
      }
    });
  }

  applyFilter(event: Event) {
    const value = (event.target as HTMLInputElement).value ?? '';
    this.dataSource.filter = value.trim().toLowerCase();
    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  openDialog(person?: Person) {
    const dialogRef = this.dialog.open(PersonEditComponent, {
      width: '600px',
      maxWidth: '95vw',
      data: person ?? null,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) this.loadPersons();
    });
  }

  deletePerson(id: number) {
    Swal.fire({
      title: '¿Eliminar persona?',
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
            Swal.fire('Eliminado', 'La persona fue eliminada correctamente.', 'success');
          },
          error: (err) => {
            console.error('Error al eliminar persona:', err);
            Swal.fire('Error', 'No se pudo eliminar la persona.', 'error');
          }
        });
      }
    });
  }
}
