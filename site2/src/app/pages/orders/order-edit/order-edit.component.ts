import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Observable, startWith, map } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators, FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { OrderService, OrderCreateDto, OrderReadDto } from '../../../services/order.service';
import { ProductService } from '../../../services/product.service';
import { PersonService } from '../../../services/person.service';
import { MaterialModule } from '../../../material.module';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { Product } from '../../../models/product';

interface Customer {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
}

@Component({
  selector: 'app-order-edit',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule, 
    RouterModule, 
    FormsModule,
    MaterialModule,
    MatAutocompleteModule
  ],
  templateUrl: './order-edit.component.html',
  styles: []
})
export class OrderEditComponent implements OnInit {
  isEditMode = false;
  isSubmitting = false;
  orderId: number | null = null;
  
  orderForm: FormGroup;
  products: Product[] = [];
  isLoading = true;
  
  // Customer autocomplete
  customers: Customer[] = [];
  filteredCustomers: Observable<Customer[]>;
  customerNameControl = new FormControl('');

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private snackBar: MatSnackBar,
    private orderService: OrderService,
    private productService: ProductService,
    private personService: PersonService
  ) {
    this.orderForm = this.fb.group({
      customerName: ['', [Validators.required, Validators.minLength(3)]],
      customerEmail: ['', [Validators.required, Validators.email]],
      status: ['PENDING', Validators.required],
      items: this.fb.array([], Validators.minLength(1))
    });
  }

  ngOnInit(): void {
    this.loadProducts();
    this.loadCustomers();
    
    // Set up customer autocomplete
    this.filteredCustomers = this.customerNameControl.valueChanges.pipe(
      startWith(''),
      map(value => this._filterCustomers(value || ''))
    );
    
    // Update form when customer is selected
    this.customerNameControl.valueChanges.subscribe((value: string | Customer | null) => {
      if (value && typeof value === 'object' && 'email' in value) {
        // Solo actualizar si es un objeto Customer con email
        this.orderForm.patchValue({
          customerName: `${value.firstName || ''} ${value.lastName || ''}`.trim(),
          customerEmail: value.email || ''
        }, { emitEvent: false });
      }
    });
    
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.isEditMode = true;
        this.orderId = +id;
        this.loadOrder(this.orderId);
      } else {
        this.addOrderItem();
        this.isLoading = false;
      }
    });
  }

  get items() {
    return this.orderForm.get('items') as FormArray;
  }

  get totalAmount() {
    return this.items.controls.reduce((total, item) => {
      const quantity = item.get('quantity')?.value || 0;
      const unitPrice = item.get('unitPrice')?.value || 0;
      return total + (quantity * unitPrice);
    }, 0);
  }

  loadProducts() {
    this.productService.getAll().subscribe({
      next: (products) => {
        this.products = products;
        if (this.items.length === 0 && products.length > 0) {
          this.addOrderItem();
        }
      },
      error: (error) => {
        console.error('Error loading products:', error);
        this.snackBar.open('Error al cargar los productos', 'Cerrar', { duration: 3000 });
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  loadOrder(id: number) {
    this.isLoading = true;
    this.orderService.getOrder(id).subscribe({
      next: (order) => {
        if (order) {
          this.orderForm.patchValue({
            customerName: order.customerName,
            customerEmail: order.customerEmail,
            status: order.status
          });

          // Clear existing items
          while (this.items.length) {
            this.items.removeAt(0);
          }

          // Add order items
          order.items.forEach(item => {
            this.items.push(this.fb.group({
              itemId: [item.itemId, Validators.required],
              itemName: [item.itemName],
              quantity: [item.quantity, [Validators.required, Validators.min(1)]],
              unitPrice: [item.unitPrice, [Validators.required, Validators.min(0)]]
            }));
          });
        }
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading order:', error);
        this.snackBar.open('Error al cargar la orden', 'Cerrar', { duration: 3000 });
        this.isLoading = false;
      }
    });
  }

  private addOrderItem() {
    const defaultProduct = this.products[0];
    this.items.push(this.fb.group({
      itemId: [defaultProduct?.id || '', Validators.required],
      itemName: [defaultProduct?.name || ''],
      quantity: [1, [Validators.required, Validators.min(1)]],
      unitPrice: [defaultProduct?.price || 0, [Validators.required, Validators.min(0)]]
    }));
  }

  removeOrderItem(index: number) {
    this.items.removeAt(index);
    if (this.items.length === 0) {
      this.addOrderItem();
    }
  }

  onProductChange(index: number) {
    const itemGroup = this.items.at(index);
    const productId = itemGroup.get('itemId')?.value;
    const product = this.products.find(p => p.id === productId);
    
    if (product) {
      itemGroup.patchValue({
        itemName: product.name,
        unitPrice: product.price
      });
    }
  }

  removeItem(index: number): void {
    this.items.removeAt(index);
    if (this.items.length === 0) {
      this.addOrderItem(); // Ensure there's always at least one item
    }
  }

  addItem(): void {
    this.addOrderItem();
  }

  onSubmit() {
    if (this.orderForm.invalid) {
      this.orderForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    const orderData: OrderCreateDto = {
      customerName: this.orderForm.value.customerName,
      customerEmail: this.orderForm.value.customerEmail,
      status: this.orderForm.value.status,
      items: this.orderForm.value.items.map((item: any) => ({
        itemId: item.itemId,
        itemName: item.itemName,
        quantity: item.quantity,
        unitPrice: item.unitPrice
      }))
    };

    const request = this.isEditMode && this.orderId
      ? this.orderService.updateOrder(this.orderId, orderData)
      : this.orderService.createOrder(orderData);

    request.subscribe({
      next: (order) => {
        if (order) {
          this.snackBar.open(
            `Orden ${this.isEditMode ? 'actualizada' : 'creada'} correctamente`,
            'Cerrar',
            { duration: 3000 }
          );
          this.router.navigate(['/orders']);
        } else {
          throw new Error('No se recibió respuesta del servidor');
        }
      },
      error: (error) => {
        console.error('Error saving order:', error);
        this.snackBar.open(
          `Error al ${this.isEditMode ? 'actualizar' : 'crear'} la orden: ${error.message || 'Error desconocido'}`,
          'Cerrar',
          { duration: 5000, panelClass: ['error-snackbar'] }
        );
        this.isSubmitting = false;
      }
    });
  }

  // Display customer name in the input
  displayCustomer(customer: Customer): string {
    if (!customer) return '';
    const name = `${customer.firstName || ''} ${customer.lastName || ''}`.trim();
    return name || 'Cliente sin nombre';
  }

  // Cargar clientes desde el servicio
  private loadCustomers(): void {
    this.isLoading = true;
    this.personService.getAll().subscribe({
      next: (response: any) => {
        // Manejar diferentes formatos de respuesta
        let customers = [];
        
        // Verificar si la respuesta tiene la propiedad $values (formato de .NET)
        if (response && Array.isArray(response.$values)) {
          customers = response.$values;
        } 
        // Verificar si la respuesta es un array directo
        else if (Array.isArray(response)) {
          customers = response;
        }
        
        // Mapear los datos al formato esperado
        this.customers = customers.map((person: any) => ({
          id: person.id,
          firstName: person.firstName || person.nombre || '',
          lastName: person.lastName || person.apellido || '',
          email: person.email || person.correo || ''
        }));
        
        console.log('Clientes cargados:', this.customers);
      },
      error: (error) => {
        console.error('Error al cargar clientes:', error);
        this.snackBar.open('Error al cargar la lista de clientes', 'Cerrar', { duration: 3000 });
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  // Filtrar clientes basado en la entrada
  private _filterCustomers(value: string | Customer): Customer[] {
    if (!value) return this.customers;
    
    const filterValue = typeof value === 'string' ? value.toLowerCase() : 
      `${value.firstName || ''} ${value.lastName || ''}`.toLowerCase().trim();
    
    if (!filterValue) return this.customers;
    
    return this.customers.filter(customer => {
      const fullName = `${customer.firstName || ''} ${customer.lastName || ''}`.toLowerCase().trim();
      const email = customer.email ? customer.email.toLowerCase() : '';
      
      return fullName.includes(filterValue) || 
             email.includes(filterValue) ||
             (customer.firstName || '').toLowerCase().includes(filterValue) ||
             (customer.lastName || '').toLowerCase().includes(filterValue);
    });
  }

  // Manejar la selección de un cliente del autocompletado
  onCustomerSelected(event: any): void {
    const customer = event.option.value as Customer;
    if (customer) {
      this.orderForm.patchValue({
        customerName: `${customer.firstName || ''} ${customer.lastName || ''}`.trim(),
        customerEmail: customer.email || ''
      }, { emitEvent: false });
    }
  }

  onCancel(): void {
    this.router.navigate(['/orders']);
  }
}
