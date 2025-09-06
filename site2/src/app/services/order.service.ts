import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, of } from 'rxjs';
import { environment } from '../../environments/environment';

export interface OrderItemDto {
  itemId: number;
  quantity: number;
  unitPrice: number;
  itemName?: string;
}

export interface OrderDetailReadDto {
  id: number;
  itemId: number;
  itemName: string;
  quantity: number;
  unitPrice: number;
  total: number;
}

export interface OrderCreateDto {
  personId: number;
  createdBy: number; // User ID who created the order
  orderDetails: OrderItemDto[];
}

export interface OrderReadDto {
  id: number;
  number: number;
  personId: number;
  personName: string;
  createdAt: string;
  totalAmount: number;
  status?: string;
  orderDetails: OrderDetailReadDto[];
}

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private apiUrl = `${environment.apiUrl}/api/orders`;

  constructor(private http: HttpClient) {}

  /**
   * Get all orders
   */
  getOrders(): Observable<OrderReadDto[]> {
    return this.http.get<OrderReadDto[]>(this.apiUrl).pipe(
      map(response => (Array.isArray(response) ? response : response['$values'] || [])),
      catchError(() => of([]))
    );
  }

  /**
   * Get a single order by ID
   */
  getOrderById(id: number): Observable<OrderReadDto | null> {
    return this.http.get<OrderReadDto>(`${this.apiUrl}/${id}`).pipe(
      catchError(() => of(null))
    );
  }

  /**
   * Create a new order
   */
  createOrder(order: OrderCreateDto): Observable<OrderReadDto | null> {
    // Set default createdBy if not provided
    const orderToCreate = {
      ...order,
      createdBy: order.createdBy || 1 // Default user ID
    };
    
    return this.http.post<OrderReadDto>(this.apiUrl, orderToCreate).pipe(
      catchError(err => {
        console.error('Error creating order:', err);
        return of(null);
      })
    );
  }

  /**
   * Update an existing order
   */
  updateOrder(id: number, order: OrderCreateDto): Observable<boolean> {
    return this.http.put(`${this.apiUrl}/${id}`, order, { observe: 'response' }).pipe(
      map(response => response.status === 204),
      catchError(() => of(false))
    );
  }

  /**
   * Delete an order
   */
  deleteOrder(id: number): Observable<boolean> {
    return this.http.delete(`${this.apiUrl}/${id}`, { observe: 'response' }).pipe(
      map(response => response.status === 204),
      catchError(() => of(false))
    );
  }
}
