import { CommonModule } from '@angular/common';
import { Component, inject, Inject, ViewChild } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { PersonService } from '../../services/person.service';
import { Person } from '../../models/person';
import { PersonEditComponent } from '../../management/person-edit/person-edit.component';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatCardModule } from '@angular/material/card';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-person',
  standalone: true,
  imports: [
     CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatDialogModule,
    FormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatCardModule,
    MatTooltipModule,
    MatIconModule
  ],
  templateUrl: './person.component.html',
  styleUrl: './person.component.css'
})
export class PersonComponent {

  private PersonService = inject(PersonService);
  private dialog = inject(MatDialog);

  displayedColumns = ['id', 'firstName', 'lastName', 'email', 'action'];
  dataSource = new MatTableDataSource<Person>();

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  ngOnInit() {
    this.loadPersons();
  }

  loadPersons() {
    this.PersonService.getAll().subscribe({
      next: (Response) => {
        let data = Response;
        this.dataSource = new MatTableDataSource(data);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
      },
      error: (error) => {
        
      }
    });
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();
  }

  openDialog(Person?: Person) {
    const dialogRef = this.dialog.open(PersonEditComponent, {
      width: '99%',
      height: '99%',
      data: Person,
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) this.loadPersons();
    });
  }

  delete(id: number) {
    if (confirm('Are you sure?')) {
      this.PersonService.delete(id).subscribe(() => this.loadPersons());
    }
  }

}
