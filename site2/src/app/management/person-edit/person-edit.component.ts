import { CommonModule } from '@angular/common';
import { Component, inject, Inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { PersonService } from '../../services/person.service';
import { Person } from '../../models/person';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogActions, MatDialogContent } from '@angular/material/dialog';
import swal from 'sweetalert';

@Component({
  selector: 'app-person-edit',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDialogActions,
    MatDialogContent
  ],
  templateUrl: './person-edit.component.html',
  styleUrl: './person-edit.component.css'
})
export class PersonEditComponent {

  private personService = inject(PersonService);
  private dialogRef = inject(MatDialogRef<PersonEditComponent>);
  private fb = inject(FormBuilder);
  personForm: FormGroup;
  constructor(@Inject(MAT_DIALOG_DATA) public data: Person) {
    this.personForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]]
    });
  }

  ngOnInit() {
    if (this.data) this.personForm.patchValue(this.data);
  }

  onSubmit() {
    const val = this.personForm.value as Person;
    swal({
      title: "Are you sure?",
      text: "Once deleted, you will not be able to recover this imaginary file!",
      icon: "warning",
      dangerMode: true,
    })
      .then((willDelete) => {
        if (willDelete) {
          if (this.data?.id) {
            this.personService.update(this.data.id, val).subscribe(() => this.dialogRef.close(true));
          } else {
            this.personService.add(val).subscribe(() => this.dialogRef.close(true));
          }
        }
      });
  }

}
