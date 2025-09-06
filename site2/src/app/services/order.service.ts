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
  customerName: string;
  customerEmail: string;
  items: OrderItemDto[];
  status?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface OrderReadDto {
  id: number;
  number: number;
  customerName: string;
  customerEmail: string;
  items: OrderItemDto[];
  status: string;
  createdAt: string;
  updatedAt?: string;
  totalAmount: number;
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
   * Get order by ID
   */
  getOrder(id: number): Observable<OrderReadDto | null> {
    if (!id) return of(null);
    return this.http.get<OrderReadDto>(`${this.apiUrl}/${id}`).pipe(
      catchError(() => of(null))
    );
  }

  /**
   * Create a new order
   */
  createOrder(order: OrderCreateDto): Observable<OrderReadDto | null> {
    return this.http.post<OrderReadDto>(this.apiUrl, order).pipe(
      catchError((error) => {
        console.error('Error creating order:', error);
        return of(null);
      })
    );
  }

  /**
   * Update an existing order
   */
  updateOrder(id: number, order: Partial<OrderCreateDto>): Observable<OrderReadDto | null> {
    return this.http.put<OrderReadDto>(`${this.apiUrl}/${id}`, order).pipe(
      catchError((error) => {
        console.error('Error updating order:', error);
        return of(null);
      })
    );
  }

  /**
   * Update order status
   */
  updateStatus(id: number, status: string): Observable<OrderReadDto | null> {
    return this.http.patch<OrderReadDto>(`${this.apiUrl}/${id}/status`, { status }).pipe(
      catchError((error) => {
        console.error('Error updating order status:', error);
        return of(null);
      })
    );
  }

  /**
   * Delete an order
   */
  deleteOrder(id: number): Observable<boolean> {
    return this.http.delete<boolean>(`${this.apiUrl}/${id}`).pipe(
      map(() => true),
      catchError(() => of(false))
    );
  }
}
