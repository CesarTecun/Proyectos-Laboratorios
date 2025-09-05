import { CommonModule } from '@angular/common';
import { Component, inject, ViewChild, AfterViewInit } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { FormsModule } from '@angular/forms';
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
  styleUrl: './person.component.css'
})
export class PersonComponent {

  private personService = inject(PersonService);
  private dialog = inject(MatDialog);

  displayedColumns = ['id', 'name', 'email', 'phone', 'address', 'action'];
  dataSource = new MatTableDataSource<Person>([]);
  
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  ngOnInit() {
    this.loadPersons();
  }

  ngAfterViewInit() {
    this.loadPersons();
  }

  loadPersons() {
    this.personService.getAll().subscribe({
      next: (res) => {
        this.dataSource.data = res;
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
      },
      error: (err: any) => {
        console.error('Error loading persons:', err);
        Swal.fire('Error!', 'Failed to load persons', 'error');
      }
    });
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
    
    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  openDialog(person?: Person) {
    const dialogRef = this.dialog.open(PersonEditComponent, {
      width: '99%',
      height: '99%',
      data: person,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.loadPersons();
      }
    });
  }

  deletePerson(id: number) {
    Swal.fire({
      title: 'Are you sure?',
      text: 'You will not be able to recover this person!',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Yes, delete it!',
      cancelButtonText: 'No, keep it'
    }).then((result) => {
      if (result.isConfirmed) {
        this.personService.delete(id).subscribe({
          next: () => {
            this.loadPersons();
            Swal.fire('Deleted!', 'The person has been deleted.', 'success');
          },
          error: (err: any) => {
            console.error('Error deleting person:', err);
            Swal.fire('Error!', 'Failed to delete person.', 'error');
          }
        });
      }
    });
  }

}
