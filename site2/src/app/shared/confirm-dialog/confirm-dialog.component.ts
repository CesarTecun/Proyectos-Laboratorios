/**
 * Componente de diálogo de confirmación reutilizable.
 * 
 * Este componente muestra un diálogo de confirmación con un título, mensaje y botones personalizables.
 * Se puede utilizar en toda la aplicación para confirmar acciones importantes como eliminaciones.
 * 
 * Uso:
 * 1. Inyectar MatDialog en el componente que necesite el diálogo de confirmación
 * 2. Abrir el diálogo usando:
 *    
 *    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
 *      data: {
 *        title: 'Confirmar eliminación',
 *        message: '¿Está seguro de que desea eliminar este elemento?',
 *        confirmText: 'Eliminar',
 *        cancelText: 'Cancelar'
 *      }
 *    });
 * 
 *    dialogRef.afterClosed().subscribe(result => {
 *      if (result) {
 *        // El usuario confirmó la acción
 *      }
 *    });
 */
import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';

/**
 * Interfaz que define la estructura de datos que recibe el diálogo de confirmación.
 * 
 * @property {string} [title] - Título opcional del diálogo (por defecto: 'Confirmar').
 * @property {string} message - Mensaje de confirmación que se mostrará al usuario.
 * @property {string} [confirmText] - Texto para el botón de confirmación (por defecto: 'Aceptar').
 * @property {string} [cancelText] - Texto para el botón de cancelación (por defecto: 'Cancelar').
 */

export interface ConfirmDialogData {
  title?: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
}

@Component({
  /**
   * Selector del componente.
   * Se usa con <app-confirm-dialog></app-confirm-dialog> cuando se necesita incrustar directamente.
   */
  selector: 'app-confirm-dialog',
  /**
   * standalone: true indica que este componente es independiente y no necesita ser declarado en un módulo.
   * imports: Importa los módulos necesarios para el funcionamiento del componente.
   */
  standalone: true,
  imports: [MatDialogModule, MatButtonModule],
  template: `
    <h2 mat-dialog-title>{{ data.title || 'Confirmar' }}</h2>
    <mat-dialog-content>
      {{ data.message || '¿Está seguro de que desea continuar?' }}
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button [mat-dialog-close]="false" cdkFocusInitial>
        {{ data.cancelText || 'Cancelar' }}
      </button>
      <button mat-raised-button color="warn" [mat-dialog-close]="true">
        {{ data.confirmText || 'Aceptar' }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    :host { display: block; }
    mat-dialog-actions { padding: 16px 24px; }
  `]
})
export class ConfirmDialogComponent {
  constructor(
    public dialogRef: MatDialogRef<ConfirmDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ConfirmDialogData
  ) {}
}
