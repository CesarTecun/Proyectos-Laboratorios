import { NgModule } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatTableModule } from '@angular/material/table';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';

const materialModules = [
  MatCardModule,
  MatButtonModule,
  MatInputModule,
  MatFormFieldModule,
  MatDatepickerModule,
  MatNativeDateModule,
  MatTableModule,
  MatDialogModule,
  MatSlideToggleModule,
  MatIconModule,
  MatProgressSpinnerModule,
  MatSelectModule
];

@NgModule({
  imports: [...materialModules],
  exports: [...materialModules],
})
export class MaterialModule { }
