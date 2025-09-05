// Importaciones necesarias de Angular y Angular Material
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Product, CreateProductDto, UpdateProductDto } from '../../../models/product';
import { ProductService } from '../../../services/product.service';

/**
 * Componente para crear o editar un producto
 */
@Component({
  selector: 'app-product-edit',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatSnackBarModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './product-edit.component.html',
  styleUrls: ['./product-edit.component.css']
})
export class ProductEditComponent implements OnInit {
  // Grupo de formulario para los controles del producto
  productForm: FormGroup;
  
  // Bandera para determinar si estamos en modo edición o creación
  isEditMode = false;
  
  // ID del producto que se está editando (null si es nuevo)
  productId: number | null = null;
  
  // Bandera para mostrar/ocultar el spinner de carga
  isLoading = false;

  /**
   * Constructor del componente
   * @param fb Constructor de formularios
   * @param productService Servicio para operaciones de productos
   * @param route Servicio para acceder a los parámetros de la ruta
   * @param router Servicio para navegación
   * @param snackBar Servicio para mostrar mensajes emergentes
   */
  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private route: ActivatedRoute,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    // Inicialización del formulario con validaciones
    this.productForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      price: ['', [Validators.required, Validators.min(0.01)]],
      description: ['', [Validators.maxLength(500)]]
    });
  }

  /**
   * Método que se ejecuta al inicializar el componente
   */
  ngOnInit(): void {
    // Suscripción a los cambios en los parámetros de la ruta
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        // Si hay un ID, estamos en modo edición
        this.isEditMode = true;
        this.productId = +id;
        this.loadProduct(this.productId);
      }
    });
  }

  /**
   * Carga los datos de un producto existente para editarlo
   * @param id ID del producto a cargar
   */
  loadProduct(id: number): void {
    this.isLoading = true;
    this.productService.getById(id).subscribe({
      next: (product) => {
        if (product) {
          // Actualiza el formulario con los datos del producto
          this.productForm.patchValue({
            name: product.name,
            price: product.price,
            description: product.description || ''
          });
        }
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error al cargar el producto:', err);
        this.snackBar.open('Error al cargar el producto', 'Cerrar', { duration: 5000 });
        this.router.navigate(['/products']);
      }
    });
  }

  /**
   * Maneja el envío del formulario
   */
  onSubmit(): void {
    // Verifica si el formulario es válido
    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    const productData = this.productForm.value;

    // Decide si crear o actualizar según el modo
    if (this.isEditMode && this.productId) {
      const updateData: UpdateProductDto = {
        id: this.productId,
        ...productData
      };
      this.updateProduct(updateData);
    } else {
      this.createProduct(productData);
    }
  }

  /**
   * Crea un nuevo producto
   * @param productData Datos del producto a crear
   */
  private createProduct(productData: CreateProductDto): void {
    this.productService.create(productData).subscribe({
      next: (product) => {
        if (product) {
          this.snackBar.open('Producto creado correctamente', 'Cerrar', { duration: 3000 });
          this.router.navigate(['/products']);
        }
      },
      error: (err) => {
        console.error('Error al crear el producto:', err);
        this.snackBar.open('Error al crear el producto', 'Cerrar', { duration: 5000 });
        this.isLoading = false;
      }
    });
  }

  /**
   * Actualiza un producto existente
   * @param updateData Datos actualizados del producto
   */
  private updateProduct(updateData: UpdateProductDto): void {
    this.productService.update(updateData.id, updateData).subscribe({
      next: (success) => {
        if (success) {
          this.snackBar.open('Producto actualizado correctamente', 'Cerrar', { duration: 3000 });
          this.router.navigate(['/products']);
        } else {
          this.snackBar.open('Error al actualizar el producto', 'Cerrar', { duration: 5000 });
          this.isLoading = false;
        }
      },
      error: (err) => {
        console.error('Error al actualizar el producto:', err);
        this.snackBar.open('Error al actualizar el producto', 'Cerrar', { duration: 5000 });
        this.isLoading = false;
      }
    });
  }

  /**
   * Maneja la acción de cancelar
   */
  onCancel(): void {
    this.router.navigate(['/products']);
  }

  // Métodos de ayuda para acceder a los controles del formulario
  get name() { return this.productForm.get('name'); }
  get price() { return this.productForm.get('price'); }
  get description() { return this.productForm.get('description'); }
}
