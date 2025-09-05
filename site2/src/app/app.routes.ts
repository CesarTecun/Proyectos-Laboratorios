import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'orders', pathMatch: 'full' },
  { 
    path: 'orders',
    children: [
      { path: '', redirectTo: 'list', pathMatch: 'full' },
      { 
        path: 'list', 
        loadComponent: () => import('./pages/orders/order-list/order-list.component')
          .then(m => m.OrderListComponent) 
      },
      { 
        path: 'new', 
        loadComponent: () => import('./pages/orders/order-edit/order-edit.component')
          .then(m => m.OrderEditComponent) 
      },
      { 
        path: 'edit/:id', 
        loadComponent: () => import('./pages/orders/order-edit/order-edit.component')
          .then(m => m.OrderEditComponent) 
      }
    ]
  },
  { 
    path: 'products',
    children: [
      { path: '', redirectTo: 'list', pathMatch: 'full' },
      { 
        path: 'list', 
        loadComponent: () => import('./pages/products/product-list/product-list.component')
          .then(m => m.ProductListComponent) 
      }
    ]
  },
  { 
    path: 'customers',
    children: [
      { path: '', redirectTo: 'list', pathMatch: 'full' },
      { 
        path: 'list', 
        loadComponent: () => import('./pages/customers/customer-list/customer-list.component')
          .then(m => m.CustomerListComponent) 
      }
    ]
  },
  { 
    path: 'person',
    children: [
      { path: '', redirectTo: 'list', pathMatch: 'full' },
      { 
        path: 'list', 
        loadComponent: () => import('./pages/person/person.component')
          .then(m => m.PersonComponent) 
      }
    ]
  },
  { path: '**', redirectTo: 'orders' }
];
