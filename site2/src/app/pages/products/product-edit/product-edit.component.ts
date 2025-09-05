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
  productForm: FormGroup;
  isEditMode = false;
  productId: number | null = null;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private route: ActivatedRoute,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.productForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      price: ['', [Validators.required, Validators.min(0.01)]]
    });
  }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.isEditMode = true;
        this.productId = +id;
        this.loadProduct(this.productId);
      }
    });
  }

  loadProduct(id: number): void {
    this.isLoading = true;
    this.productService.getById(id).subscribe({
      next: (product) => {
        if (product) {
          this.productForm.patchValue({
            name: product.name,
            price: product.price
          });
        }
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading product:', err);
        this.snackBar.open('Error al cargar el producto', 'Cerrar', { duration: 5000 });
        this.router.navigate(['/products']);
      }
    });
  }

  onSubmit(): void {
    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    const productData = this.productForm.value;

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

  private createProduct(productData: CreateProductDto): void {
    this.productService.create(productData).subscribe({
      next: (product) => {
        if (product) {
          this.snackBar.open('Producto creado correctamente', 'Cerrar', { duration: 3000 });
          this.router.navigate(['/products']);
        }
      },
      error: (err) => {
        console.error('Error creating product:', err);
        this.snackBar.open('Error al crear el producto', 'Cerrar', { duration: 5000 });
        this.isLoading = false;
      }
    });
  }

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
        console.error('Error updating product:', err);
        this.snackBar.open('Error al actualizar el producto', 'Cerrar', { duration: 5000 });
        this.isLoading = false;
      }
    });
  }

  onCancel(): void {
    this.router.navigate(['/products']);
  }

  // Helper methods for form validation
  get name() { return this.productForm.get('name'); }
  get price() { return this.productForm.get('price'); }
}
