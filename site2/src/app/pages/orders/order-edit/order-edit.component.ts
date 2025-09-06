import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators, FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { OrderService, OrderCreateDto, OrderReadDto, OrderItemDto } from '../../../services/order.service';
import { PersonService } from '../../../services/person.service';
import { ItemService } from '../../../services/item.service';
import { MaterialModule } from '../../../material.module';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-order-edit',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule, 
    RouterModule, 
    FormsModule,
    MaterialModule
  ],
  templateUrl: './order-edit.component.html',
  styles: []
})
export class OrderEditComponent implements OnInit {
  isEditMode = false;
  isSubmitting = false;
  orderId: number | null = null;
  
  orderForm: FormGroup;
  persons: any[] = [];
  items: any[] = [];

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private snackBar: MatSnackBar,
    @Inject(OrderService) private orderService: OrderService,
    @Inject(PersonService) private personService: PersonService,
    @Inject(ItemService) private itemService: ItemService
  ) {
    const today = new Date().toISOString().split('T')[0];
    this.orderForm = this.fb.group({
      personId: ['', Validators.required],
      orderDate: [today, Validators.required],
      orderItems: this.fb.array([])
    });
  }

  ngOnInit(): void {
    this.loadPersons();
    this.loadItems();
    
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEditMode = true;
      this.orderId = +id;
      this.loadOrder(this.orderId);
    } else {
      // Agregar un producto por defecto en modo creación
      this.addProduct();
    }
  }

  get orderDetails() {
    return this.orderForm.get('orderItems') as FormArray;
  }

  loadPersons(): void {
    this.personService.getPersons().subscribe(persons => {
      this.persons = persons;
    });
  }

  loadItems(): void {
    this.itemService.getItems().subscribe(items => {
      this.items = items;
    });
  }

  loadOrder(id: number): void {
    if (this.orderId === null) return;
    
    this.orderService.getOrderById(this.orderId).subscribe({
      next: (order: OrderReadDto | null) => {
        if (!order) {
          console.error('No se encontró la orden con ID:', this.orderId);
          return;
        }
        
        // Asegurarse de que orderDetails sea un array
        const orderDetails = order.orderDetails || [];
        
        this.orderForm.patchValue({
          personId: order.personId,
          orderDate: order.createdAt ? order.createdAt.toString().split('T')[0] : new Date().toISOString().split('T')[0]
        });

        // Limpiar items existentes
        const orderItems = this.orderForm.get('orderItems') as FormArray;
        while (orderItems.length) {
          orderItems.removeAt(0);
        }

        // Agregar items de la orden cargada
        orderDetails.forEach((item) => {
          orderItems.push(this.fb.group({
            itemId: [item.itemId, [Validators.required, Validators.min(1)]],
            quantity: [item.quantity, [Validators.required, Validators.min(1)]]
          }));
        });
      },
      error: (error: any) => {
        console.error('Error al cargar la orden:', error);
        alert('Error al cargar la orden: ' + (error.error?.message || error.message || 'Error desconocido'));
        this.router.navigate(['/orders']);
      }
    });
  }

  // Alias para compatibilidad con la plantilla
  get orderItems(): FormArray {
    return this.orderDetails as FormArray;
  }

  addItem(): void {
    const orderItems = this.orderForm.get('orderItems') as FormArray;
    orderItems.push(this.fb.group({
      itemId: ['', [Validators.required, Validators.min(1)]],
      quantity: [1, [Validators.required, Validators.min(1)]]
    }));
  }

  removeItem(index: number): void {
    const orderItems = this.orderForm.get('orderItems') as FormArray;
    orderItems.removeAt(index);
  }

  private addProduct(): void {
    this.orderDetails.push(this.fb.group({
      itemId: ['', Validators.required],
      quantity: [1, [Validators.required, Validators.min(1)]]
    }));
  }

  removeProduct(index: number): void {
    this.orderDetails.removeAt(index);
  }

  onSubmit(): void {
    if (this.orderForm.invalid) {
      return;
    }

    this.isSubmitting = true;
    
    const formValue = this.orderForm.value;
    
    const orderData: OrderCreateDto = {
      personId: formValue.personId,
      createdBy: 1, // ID del usuario actual (deberías obtenerlo del servicio de autenticación)
      orderDetails: formValue.orderItems.map((item: any) => ({
        itemId: item.itemId,
        quantity: item.quantity,
        unitPrice: this.items.find(i => i.id === item.itemId)?.price || 0
      }))
    };

    if (this.isEditMode && this.orderId) {
      this.orderService.updateOrder(this.orderId, orderData).subscribe({
        next: (success: boolean) => {
          if (success) {
            this.snackBar.open(
              'Orden actualizada correctamente',
              'Cerrar',
              { duration: 3000 }
            );
            this.router.navigate(['/orders']);
          } else {
            this.snackBar.open(
              'No se pudo actualizar la orden',
              'Cerrar',
              { duration: 5000, panelClass: ['error-snackbar'] }
            );
            this.isSubmitting = false;
          }
        },
        error: (err: any) => {
          console.error('Error al actualizar la orden:', err);
          this.snackBar.open(
            'Error al actualizar la orden: ' + (err.error?.message || err.message || 'Error desconocido'),
            'Cerrar',
            { duration: 5000, panelClass: ['error-snackbar'] }
          );
          this.isSubmitting = false;
        }
      });
    } else {
      this.orderService.createOrder(orderData).subscribe({
        next: (order: OrderReadDto | null) => {
          if (order) {
            this.snackBar.open(
              'Orden creada correctamente',
              'Cerrar',
              { duration: 3000 }
            );
            this.router.navigate(['/orders']);
          } else {
            this.snackBar.open(
              'No se pudo crear la orden',
              'Cerrar',
              { duration: 5000, panelClass: ['error-snackbar'] }
            );
            this.isSubmitting = false;
          }
        },
        error: (err: any) => {
          console.error('Error al crear la orden:', err);
          this.snackBar.open(
            'Error al crear la orden: ' + (err.error?.message || err.message || 'Error desconocido'),
            'Cerrar',
            { duration: 5000, panelClass: ['error-snackbar'] }
          );
          this.isSubmitting = false;
        }
      });
    }
  }

  calculateTotal(): number {
    let total = 0;
    this.orderDetails.controls.forEach(control => {
      const item = this.items.find(i => i.id === control.value.itemId);
      if (item) {
        total += item.price * control.value.quantity;
      }
    });
    return total;
  }

  onCancel(): void {
    this.router.navigate(['/orders']);
  }
}
