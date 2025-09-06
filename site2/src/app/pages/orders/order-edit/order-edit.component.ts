import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Observable, startWith, map } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators, FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { OrderService, OrderCreateDto, OrderReadDto, BackendOrderCreateDto } from '../../../services/order.service';
import { PersonService } from '../../../services/person.service';
import { ProductService } from '../../../services/product.service';
import { Product } from '../../../models/product';

import { MaterialModule } from '../../../material.module';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatAutocompleteModule } from '@angular/material/autocomplete';

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
  // Track selected personId aligned with backend
  selectedPersonId: number | null = null;
  // Product search
  productFilter = new FormControl('');
  filteredProducts!: Observable<Product[]>;
  clearProductFilter() { this.productFilter.setValue(''); }

  private normalize(text: string): string {
    return (text || '')
      .toString()
      .toLowerCase()
      .normalize('NFD')
      .replace(/\p{Diacritic}+/gu, '')
      .trim();
  }

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private snackBar: MatSnackBar,
    private orderService: OrderService,
    private personService: PersonService,
    private productService: ProductService
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
        this.selectedPersonId = value.id;
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
      next: (products: Product[]) => {
        this.products = products;
        if (this.items.length === 0 && products.length > 0) {
          this.addOrderItem();
        }
        // Setup product filter stream
        this.filteredProducts = this.productFilter.valueChanges.pipe(
          startWith(''),
          map(term => {
            const t = this.normalize(term as string);
            if (!t) return this.products;
            const num = Number(t);
            return this.products.filter(p => {
              const name = this.normalize(p.name || '');
              const desc = this.normalize(p.description || '');
              const idMatch = !Number.isNaN(num) && (p.id?.toString().includes(num.toString()));
              return name.includes(t) || desc.includes(t) || idMatch;
            });
          })
        );
      },
      error: (error: any) => {
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
      next: (order: OrderReadDto | null) => {
        if (order) {
          this.orderForm.patchValue({
            customerName: order.customerName,
            customerEmail: order.customerEmail,
            status: order.status
          });
          this.selectedPersonId = order.personId ?? null;

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
      error: (error: any) => {
        console.error('Error loading order:', error);
        this.snackBar.open('Error al cargar la orden', 'Cerrar', { duration: 3000 });
        this.isLoading = false;
      }
    });
  }

  private addOrderItem() {
    const defaultItem = this.products[0];
    this.items.push(this.fb.group({
      itemId: [defaultItem?.id || '', Validators.required],
      itemName: [defaultItem?.name || ''],
      quantity: [1, [Validators.required, Validators.min(1)]],
      unitPrice: [defaultItem?.price || 0, [Validators.required, Validators.min(0)]]
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
    const it = this.products.find(p => p.id === productId);
    
    if (it) {
      itemGroup.patchValue({
        itemName: it.name,
        unitPrice: it.price
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

    // Validar que exista un cliente seleccionado (personId)
    if (!this.selectedPersonId || this.selectedPersonId <= 0) {
      this.snackBar.open('Seleccione un cliente válido', 'Cerrar', { duration: 3000, panelClass: ['error-snackbar'] });
      return;
    }

    // Validar que exista al menos un detalle con itemId y quantity válidos
    const rawItems = (this.orderForm.value.items || []) as any[];
    if (!rawItems.length) {
      this.snackBar.open('Agregue al menos un producto a la orden', 'Cerrar', { duration: 3000, panelClass: ['error-snackbar'] });
      return;
    }
    const invalid = rawItems.some(it => !it?.itemId || Number(it.quantity) <= 0);
    if (invalid) {
      this.snackBar.open('Revise los productos: cantidad debe ser mayor a 0 y el producto es obligatorio', 'Cerrar', { duration: 4000, panelClass: ['error-snackbar'] });
      return;
    }

    this.isSubmitting = true;
    const dto: BackendOrderCreateDto = {
      personId: this.selectedPersonId ?? 1, // TODO: enforce selection in UI
      createdBy: 1,
      orderDetails: rawItems.map((item: any) => ({
        itemId: Number(item.itemId),
        quantity: Number(item.quantity)
      }))
    };

    // Debug: inspeccionar payload antes de enviar
    console.log('Create/Update Order DTO =>', dto);

    const request = (this.isEditMode && this.orderId)
      ? this.orderService.updateOrderV2(this.orderId, dto)
      : this.orderService.createOrderV2(dto);

    request.subscribe({
      next: (order: OrderReadDto | null) => {
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
      error: (error: any) => {
        console.error('Error saving order:', error);
        const details = (error?.error && (error.error.errors || error.error.title || error.error.detail)) ? JSON.stringify(error.error) : (error.message || 'Error desconocido');
        this.snackBar.open(
          `Error al ${this.isEditMode ? 'actualizar' : 'crear'} la orden: ${details}`,
          'Cerrar',
          { duration: 6000, panelClass: ['error-snackbar'] }
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

        // Auto-seleccionar el primer cliente si no hay uno seleccionado aún (solo en modo creación)
        if (!this.isEditMode && !this.selectedPersonId && this.customers.length > 0) {
          const first = this.customers[0];
          this.selectedPersonId = first.id;
          this.orderForm.patchValue({
            customerName: `${first.firstName || ''} ${first.lastName || ''}`.trim(),
            customerEmail: first.email || ''
          }, { emitEvent: false });
        }
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
      this.selectedPersonId = customer.id;
    }
  }

  onCancel(): void {
    this.router.navigate(['/orders']);
  }
}
