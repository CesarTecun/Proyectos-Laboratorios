import { CommonModule } from '@angular/common';
import { Component, inject, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { PersonService } from '../../services/person.service';
import { Person, CreatePersonDto, UpdatePersonDto } from '../../models/person';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogActions, MatDialogContent } from '@angular/material/dialog';
import Swal from 'sweetalert2'; // <-- usa sweetalert2, no 'sweetalert'

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
  styleUrls: ['./person-edit.component.css'] // <-- plural
})
export class PersonEditComponent implements OnInit {

  private personService = inject(PersonService);
  private dialogRef = inject(MatDialogRef<PersonEditComponent>);
  private fb = inject(FormBuilder);

  personForm: FormGroup;

  constructor(@Inject(MAT_DIALOG_DATA) public data: Person | null) {
    this.personForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(50)]],
      lastName:  ['', [Validators.required, Validators.maxLength(50)]],
      email:     ['', [Validators.required, Validators.email, Validators.maxLength(100)]],
    });
  }

  ngOnInit() {
    if (this.data) {
      // Compatibilidad por si venía un campo 'name' antiguo
      const p: any = { ...this.data };
      if (p.firstName === undefined && 'name' in p) {
        const parts = (p.name as string)?.trim().split(/\s+/) ?? [];
        p.firstName = parts[0] ?? '';
        p.lastName  = parts.slice(1).join(' ') ?? '';
      }
      this.personForm.patchValue({
        firstName: p.firstName ?? '',
        lastName:  p.lastName  ?? '',
        email:     p.email     ?? '',
      });
    }
  }
  
  onCancel() {
    this.dialogRef.close(false); // puedes pasar false si quieres indicar "cancelado"
  }
  

  onSubmit() {
    if (this.personForm.invalid) return;

    const { firstName, lastName, email } = this.personForm.value;

    if (this.data?.id) {
      // Actualizar
      const dto: UpdatePersonDto = { firstName, lastName, email };
      this.personService.update(this.data.id, dto).subscribe({
        next: () => {
          Swal.fire('Actualizado', 'La persona se actualizó correctamente.', 'success');
          this.dialogRef.close(true);
        },
        error: (err) => {
          console.error('Error al actualizar persona:', err);
          Swal.fire('Error', 'No se pudo actualizar la persona.', 'error');
        }
      });
    } else {
      // Crear
      const dto: CreatePersonDto = { firstName, lastName, email };
      this.personService.add(dto).subscribe({
        next: () => {
          Swal.fire('Creado', 'La persona se creó correctamente.', 'success');
          this.dialogRef.close(true);
        },
        error: (err) => {
          console.error('Error al crear persona:', err);
          Swal.fire('Error', 'No se pudo crear la persona.', 'error');
        }
      });
    }
  }
}
