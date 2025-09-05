/**
 * Componente para mostrar una lista de productos con funcionalidad CRUD.
 * 
 * Este componente implementa:
 * - Visualización de productos en una tabla con paginación y ordenamiento
 * - Eliminación de productos con diálogo de confirmación
 * - Navegación a edición/creación de productos
 * 
 * - Utiliza Angular Material para la interfaz de usuario
 * - Implementa carga perezosa de datos
 * - Manejo de errores con notificaciones
 */
import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { Product } from '../../../models/product';
import { ProductService } from '../../../services/product.service';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog.component';

@Component({
  /**
   * Selector del componente.
   * Se usa con <app-product-list></app-product-list> en las plantillas.
   */
  selector: 'app-product-list',
  /**
   * standalone: true indica que este componente es independiente y no necesita ser declarado en un módulo.
   * imports: Importa los módulos necesarios para el funcionamiento del componente.
   */
  standalone: true,
  imports: [
    CommonModule, 
    RouterModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatDialogModule,
    MatSortModule,
    MatPaginatorModule,
    DatePipe
  ],
  templateUrl: './product-list.component.html',
  styles: [`
    .mat-mdc-table {
      width: 100%;
    }
    
    .mat-mdc-header-cell {
      font-weight: bold;
    }
    
    .mat-mdc-row:hover {
      background-color: #f5f5f5;
      cursor: pointer;
    }
    
    .mat-mdc-cell, .mat-mdc-header-cell {
      padding: 0 16px;
    }
    
    .actions-cell {
      display: flex;
      justify-content: flex-end;
      gap: 8px;
    }
  `]
})
export class ProductListComponent implements OnInit {
  // Columnas a mostrar en la tabla
  displayedColumns: string[] = ['id', 'name', 'price', 'description', 'createdAt', 'updatedAt', 'actions'];
  
  // Fuente de datos para la tabla
  dataSource: Product[] = [];
  
  // Estado de carga
  isLoading = true;
  
  // Mensaje de error, si lo hay
  error = '';

  // Referencias a los componentes de Material para ordenamiento y paginación
  @ViewChild(MatSort) sort!: MatSort;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  /**
   * Constructor del componente.
   * 
   * @param productService Servicio para interactuar con la API de productos
   * @param dialog Servicio para abrir diálogos modales
   * @param snackBar Servicio para mostrar notificaciones
   */
  constructor(
    private productService: ProductService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  /**
   * Método del ciclo de vida de Angular que se ejecuta al inicializar el componente.
   * Carga la lista de productos al iniciar.
   */
  ngOnInit(): void {
    this.loadProducts();
  }

  /**
   * Carga los productos desde el servidor.
   * Actualiza el estado de carga y maneja los errores.
   */
  loadProducts(): void {
    this.isLoading = true;
    this.productService.getAll().subscribe({
      next: (products) => {
        console.log('Productos recibidos de la API:', JSON.stringify(products, null, 2));
        console.log('Primer producto:', products[0] ? {
          id: products[0].id,
          name: products[0].name,
          price: products[0].price,
          description: products[0].description,
          createdAt: products[0].createdAt,
          updatedAt: products[0].updatedAt
        } : 'No hay productos');
        this.dataSource = products;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error al cargar productos:', err);
        this.error = 'Error al cargar la lista de productos';
        this.snackBar.open(this.error, 'Cerrar', { duration: 5000 });
        this.isLoading = false;
      }
    });
  }

  /**
   * Maneja la acción de eliminar un producto.
   * Muestra un diálogo de confirmación antes de proceder con la eliminación.
   * 
   * @param id ID del producto a eliminar
   */
  onDelete(id: number): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '350px',
      data: { 
        title: 'Eliminar Producto',
        message: '¿Está seguro de que desea eliminar este producto?',
        confirmText: 'Eliminar',
        cancelText: 'Cancelar'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.productService.delete(id).subscribe({
          next: (success) => {
            if (success) {
              this.snackBar.open('Producto eliminado correctamente', 'Cerrar', { duration: 3000 });
              this.loadProducts();
            } else {
              this.snackBar.open('Error al eliminar el producto', 'Cerrar', { duration: 5000 });
            }
          },
          error: (err) => {
            console.error('Error al eliminar el producto:', err);
            this.snackBar.open('Error al eliminar el producto', 'Cerrar', { duration: 5000 });
          }
        });
      }
    });
  }
}
