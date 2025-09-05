import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface OrderItemDto {
  itemId: number;
  quantity: number;
  unitPrice: number;
}

export interface OrderCreateDto {
  personId: number;
  orderDate: string;
  orderItems: OrderItemDto[];
}

export interface OrderReadDto {
  id: number;
  personId: number;
  orderDate: string;
  totalAmount: number;
  status?: string; // Added status as an optional property
  orderItems: {
    id: number;
    itemId: number;
    itemName: string;
    quantity: number;
    unitPrice: number;
  }[];
}

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private apiUrl = `${environment.apiUrl}/api/orders`;

  constructor(private http: HttpClient) {}

  getOrders(): Observable<OrderReadDto[]> {
    return new Observable(observer => {
      this.http.get<any>(this.apiUrl).subscribe({
        next: (response) => {
          // Handle the response with $values property or direct array
          const data = response.$values || response || [];
          observer.next(Array.isArray(data) ? data : []);
          observer.complete();
        },
        error: (err) => {
          console.error('Error in OrderService:', err);
          observer.next([]);
          observer.complete();
        }
      });
    });
  }

  getOrder(id: number): Observable<OrderReadDto> {
    return this.http.get<OrderReadDto>(`${this.apiUrl}/${id}`);
  }

  createOrder(order: OrderCreateDto): Observable<OrderReadDto> {
    return this.http.post<OrderReadDto>(this.apiUrl, order);
  }

  updateOrder(id: number, order: Partial<OrderCreateDto>): Observable<OrderReadDto> {
    return this.http.put<OrderReadDto>(`${this.apiUrl}/${id}`, order);
  }

  deleteOrder(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
