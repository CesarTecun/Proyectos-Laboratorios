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
    this.orderService.getOrder(this.orderId).subscribe(order => {
      this.orderForm.patchValue({
        personId: order.personId
      });

      // Clear existing order items
      const orderItems = this.orderForm.get('orderItems') as FormArray;
      while (orderItems.length) {
        orderItems.removeAt(0);
      }

      // Add order items from the loaded order
      order.orderItems.forEach(item => {
        orderItems.push(this.fb.group({
          itemId: [item.itemId, Validators.required],
          quantity: [item.quantity, [Validators.required, Validators.min(1)]]
        }));
      });
    });
  }

  // Alias for template compatibility
  get orderItems() {
    return this.orderDetails;
  }

  addItem(): void {
    this.addProduct();
  }

  removeItem(index: number): void {
    this.orderDetails.removeAt(index);
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
      orderDate: formValue.orderDate,
      orderItems: formValue.orderItems.map((item: any) => ({
        itemId: item.itemId,
        quantity: item.quantity,
        unitPrice: this.items.find(i => i.id === item.itemId)?.price || 0
      }))
    };

    const operation = this.isEditMode && this.orderId
      ? this.orderService.updateOrder(this.orderId, orderData)
      : this.orderService.createOrder(orderData);

    operation.subscribe({
      next: () => {
        this.snackBar.open(
          `Orden ${this.isEditMode ? 'actualizada' : 'creada'} exitosamente`,
          'Cerrar',
          { duration: 3000 }
        );
        this.router.navigate(['/orders']);
      },
      error: (error) => {
        console.error('Error al guardar la orden', error);
        this.snackBar.open(
          'Error al guardar la orden. Por favor, intente nuevamente.',
          'Cerrar',
          { duration: 5000, panelClass: ['error-snackbar'] }
        );
        this.isSubmitting = false;
      }
    });
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
