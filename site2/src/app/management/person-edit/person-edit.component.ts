import { CommonModule } from '@angular/common';
import { Component, inject, Inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { PersonService } from '../../services/person.service';
import { Person, CreatePersonDto, UpdatePersonDto } from '../../models/person';
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
      firstName: ['', [Validators.required, Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.maxLength(50)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(100)]],
      phone: [''],
      address: ['']
    });
  }

  ngOnInit() {
    if (this.data) {
      // Split the name into first and last name if it exists
      const personData = { ...this.data };
      if (personData.firstName === undefined && 'name' in personData) {
        // Handle case where we might have the old name field
        const nameParts = (personData as any).name?.split(' ') || ['', ''];
        personData.firstName = nameParts[0];
        personData.lastName = nameParts.slice(1).join(' ');
      }
      this.personForm.patchValue(personData);
    }
  }

  onSubmit() {
    if (this.personForm.invalid) {
      return;
    }

    const formValue = this.personForm.value;
    
    if (this.data?.id) {
      // Update existing person
      const dto: UpdatePersonDto = {
        firstName: formValue.firstName,
        lastName: formValue.lastName,
        email: formValue.email,
        phone: formValue.phone || '',
        address: formValue.address || ''
      };
      
      this.personService.update(this.data.id, dto).subscribe({
        next: () => {
          swal('Success!', 'Person updated successfully!', 'success');
          this.dialogRef.close(true);
        },
        error: (err: any) => {
          console.error('Error updating person:', err);
          swal('Error!', 'Failed to update person', 'error');
        }
      });
    } else {
      // Create new person
      const dto: CreatePersonDto = {
        firstName: formValue.firstName,
        lastName: formValue.lastName,
        email: formValue.email,
        phone: formValue.phone || '',
        address: formValue.address || ''
      };
      
      this.personService.add(dto).subscribe({
        next: () => {
          swal('Success!', 'Person created successfully!', 'success');
          this.dialogRef.close(true);
        },
        error: (err: any) => {
          console.error('Error creating person:', err);
          swal('Error!', 'Failed to create person', 'error');
        }
      });
    }
  }

}
