/**
 * Módulos necesarios de Angular y Angular Material
 * - Component: Para definir el componente
 * - FormBuilder, FormGroup, Validators, ReactiveFormsModule: Para formularios reactivos
 * - ActivatedRoute, Router, RouterModule: Para navegación y parámetros de ruta
 * - Módulos de Material: Componentes de UI de Angular Material
 * - Servicios y modelos personalizados
 */
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon'; // Para íconos
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Product, CreateProductDto, UpdateProductDto } from '../../../models/product';
import { ProductService } from '../../../services/product.service';

/**
 * Componente para la creación y edición de productos.
 * Maneja el formulario de productos con validaciones y comunicación con el servicio.
 * 
 * Características:
 * - Formulario reactivo con validaciones
 * - Modos de creación y edición
 * - Carga de datos existentes para edición
 * - Manejo de estados de carga
 * - Notificaciones al usuario
 */
@Component({
  selector: 'app-product-edit',
  standalone: true,
  imports: [
    CommonModule,           // Directivas comunes de Angular
    ReactiveFormsModule,    // Para formularios reactivos
    RouterModule,           // Para navegación
    MatButtonModule,        // Botones de Material
    MatCardModule,          // Tarjetas de Material
    MatFormFieldModule,     // Campos de formulario
    MatInputModule,         // Inputs de formulario
    MatIconModule,          // Íconos de Material
    MatSnackBarModule,      // Notificaciones
    MatProgressSpinnerModule // Spinners de carga
  ],
  templateUrl: './product-edit.component.html',
  styleUrls: ['./product-edit.component.css']
})
export class ProductEditComponent implements OnInit {
  /**
   * Grupo de formulario que contiene todos los controles del formulario de producto.
   * Incluye validaciones para cada campo.
   */
  productForm: FormGroup;
  
  /**
   * Indica si el componente está en modo edición (true) o creación (false).
   * Determina el comportamiento del formulario y los mensajes mostrados.
   */
  isEditMode = false;
  
  /**
   * Almacena el ID del producto que se está editando.
   * Es null cuando se está creando un producto nuevo.
   */
  productId: number | null = null;
  
  /**
   * Controla la visualización del spinner de carga.
   * Se activa durante operaciones asíncronas como guardar o cargar datos.
   */
  isLoading = false;

  /**
   * Inicializa una nueva instancia del componente de edición de productos.
   * 
   * @param fb Servicio para construir formularios reactivos
   * @param productService Servicio para operaciones CRUD de productos
   * @param route Servicio para acceder a los parámetros de la ruta actual
   * @param router Servicio para navegación programática
   * @param snackBar Servicio para mostrar notificaciones al usuario
   */
  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private route: ActivatedRoute,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    /**
     * Configuración del formulario reactivo con validaciones:
     * - name: Requerido, máximo 100 caracteres
     * - price: Requerido, valor mínimo 0.01
     * - description: Opcional, máximo 500 caracteres
     */
    this.productForm = this.fb.group({
      name: ['', [
        Validators.required, 
        Validators.maxLength(100)
      ]],
      price: ['', [
        Validators.required, 
        Validators.min(0.01)
      ]],
      description: ['', [
        Validators.maxLength(500)
      ]]
    });
  }

  /**
   * Método del ciclo de vida de Angular que se ejecuta al inicializar el componente.
   * Se suscribe a los cambios en los parámetros de la ruta para determinar si se está
   * creando un producto nuevo o editando uno existente.
   */
  ngOnInit(): void {
    // Suscripción a los cambios en los parámetros de la ruta
    this.route.paramMap.subscribe(params => {
      // Obtiene el ID del producto de los parámetros de la ruta
      const id = params.get('id');
      
      // Si hay un ID, estamos en modo edición
      if (id) {
        this.isEditMode = true;
        this.productId = +id; // Convierte el ID a número
        this.loadProduct(this.productId); // Carga los datos del producto
      }
    });
  }

  /**
   * Carga los datos de un producto existente para su edición.
   * Actualiza el formulario con los datos del producto obtenidos del servicio.
   * 
   * @param id ID numérico del producto a cargar
   * @throws Muestra una notificación y redirige a la lista de productos en caso de error
   */
  loadProduct(id: number): void {
    this.isLoading = true; // Activa el indicador de carga
    
    // Realiza la petición para obtener el producto por su ID
    this.productService.getById(id).subscribe({
      next: (product) => {
        // Si se obtuvo el producto correctamente, actualiza el formulario
        if (product) {
          this.productForm.patchValue({
            name: product.name,
            price: product.price,
            description: product.description || '' // Asegura que description no sea null
          });
        }
        this.isLoading = false; // Desactiva el indicador de carga
      },
      error: (err) => {
        // Manejo de errores
        console.error('Error al cargar el producto:', err);
        this.snackBar.open('Error al cargar el producto', 'Cerrar', { 
          duration: 5000, // Duración de la notificación: 5 segundos
          panelClass: ['error-snackbar'] // Clase CSS personalizada
        });
        this.router.navigate(['/products']); // Redirige a la lista de productos
      }
    });
  }

  /**
   * Maneja el evento de envío del formulario.
   * Valida el formulario y decide si crear un nuevo producto o actualizar uno existente.
   */
  onSubmit(): void {
    // Marca todos los campos como tocados para mostrar errores de validación
    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return; // Detiene la ejecución si el formulario no es válido
    }

    this.isLoading = true; // Activa el indicador de carga
    const productData = this.productForm.value; // Obtiene los datos del formulario

    // Determina si se está creando o actualizando un producto
    if (this.isEditMode && this.productId) {
      // Modo edición: Prepara los datos para actualización
      const updateData: UpdateProductDto = {
        id: this.productId,
        ...productData // Copia todas las propiedades del formulario
      };
      this.updateProduct(updateData);
    } else {
      // Modo creación: Usa los datos directamente
      this.createProduct(productData);
    }
  }

  /**
   * Crea un nuevo producto utilizando el servicio de productos.
   * Muestra notificaciones al usuario sobre el resultado de la operación.
   * 
   * @param productData Objeto con los datos del producto a crear
   */
  private createProduct(productData: CreateProductDto): void {
    this.productService.create(productData).subscribe({
      next: (product) => {
        if (product) {
          this.snackBar.open('Producto creado correctamente', 'Cerrar', { 
            duration: 3000,
            panelClass: ['success-snackbar']
          });
          this.isLoading = false; // Asegura detener spinner
          this.router.navigate(['/products']);
        } else {
          // Si no vino producto, consideramos flujo no exitoso
          this.isLoading = false;
        }
      },
      error: (err) => {
        console.error('Error al crear el producto:', err);
        this.snackBar.open('Error al crear el producto', 'Cerrar', { 
          duration: 5000,
          panelClass: ['error-snackbar']
        });
        this.isLoading = false;
      }
    });
  }

  /**
   * Actualiza un producto existente utilizando el servicio de productos.
   * Muestra notificaciones al usuario sobre el resultado de la operación.
   * 
   * @param updateData Objeto con los datos actualizados del producto
   */
  private updateProduct(updateData: UpdateProductDto): void {
    this.productService.update(updateData.id, updateData).subscribe({
      next: (success) => {
        if (success) {
          this.snackBar.open('Producto actualizado correctamente', 'Cerrar', { 
            duration: 3000,
            panelClass: ['success-snackbar']
          });
          this.isLoading = false;
          this.router.navigate(['/products']);
        } else {
          this.snackBar.open('No se pudo actualizar el producto', 'Cerrar', { 
            duration: 5000,
            panelClass: ['warning-snackbar']
          });
          this.isLoading = false;
        }
      },
      error: (err) => {
        console.error('Error al actualizar el producto:', err);
        this.snackBar.open('Error al actualizar el producto', 'Cerrar', { 
          duration: 5000,
          panelClass: ['error-snackbar']
        });
        this.isLoading = false;
      }
    });
  }

  /**
   * Maneja el evento de cancelación del formulario.
   * Navega de vuelta a la lista de productos sin guardar cambios.
   */
  onCancel(): void {
    // Navega a la ruta de lista de productos
    this.router.navigate(['/products']);
  }

  // =============================================
  // GETTERS PARA ACCEDER A LOS CONTROLES DEL FORMULARIO
  // =============================================
  
  /**
   * Obtiene el control del campo 'name' del formulario.
   * Útil para acceder a su estado y validaciones en la plantilla.
   */
  get name() { 
    return this.productForm.get('name'); 
  }
  
  /**
   * Obtiene el control del campo 'price' del formulario.
   * Permite acceder a su estado y validaciones en la plantilla.
   */
  get price() { 
    return this.productForm.get('price'); 
  }
  
  /**
   * Obtiene el control del campo 'description' del formulario.
   * Facilita el acceso a su estado y validaciones en la plantilla.
   */
  get description() { 
    return this.productForm.get('description'); 
  }
}
