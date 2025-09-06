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
  personId?: number;
}

// Backend-aligned DTOs
export interface BackendOrderDetailCreateDto {
  itemId: number;
  quantity: number;
}

export interface BackendOrderCreateDto {
  personId: number;
  createdBy: number;
  orderDetails: BackendOrderDetailCreateDto[];
}

export interface BackendOrderDetailReadDto {
  id: number;
  itemId: number;
  itemName: string;
  quantity: number;
  price: number;
  total: number;
}

export interface BackendOrderReadDto {
  id: number;
  personId: number;
  personName: string;
  number: number;
  createdAt: string;
  orderDetails: BackendOrderDetailReadDto[] | { $values: BackendOrderDetailReadDto[] };
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
    return this.http.get<any>(this.apiUrl).pipe(
      map((response: any) => {
        const arr: BackendOrderReadDto[] = Array.isArray(response) ? response : (response?.$values ?? []);
        return arr.map((o) => this.mapBackendToFront(o));
      }),
      catchError(() => of([]))
    );
  }

  /**
   * Get order by ID
   */
  getOrder(id: number): Observable<OrderReadDto | null> {
    if (!id) return of(null);
    return this.http.get<BackendOrderReadDto>(`${this.apiUrl}/${id}`).pipe(
      map((o) => this.mapBackendToFront(o)),
      catchError(() => of(null))
    );
  }

  /**
   * Create a new order
   */
  createOrderV2(dto: BackendOrderCreateDto): Observable<OrderReadDto | null> {
    return this.http.post<BackendOrderReadDto>(this.apiUrl, dto).pipe(
      map((o) => this.mapBackendToFront(o))
    );
  }

  /**
   * Update an existing order
   */
  updateOrderV2(id: number, dto: BackendOrderCreateDto): Observable<OrderReadDto | null> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto).pipe(
      map(() => ({ id, number: 0, customerName: '', customerEmail: '', items: [], status: 'UPDATED', createdAt: new Date().toISOString(), totalAmount: 0 } as OrderReadDto))
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

  // Map backend read dto to front read dto expected by current UI
  private mapBackendToFront(o: BackendOrderReadDto): OrderReadDto {
    const details = Array.isArray((o as any).orderDetails)
      ? (o as any).orderDetails as BackendOrderDetailReadDto[]
      : ((o as any).orderDetails?.$values as BackendOrderDetailReadDto[] | undefined) ?? [];

    const items: OrderItemDto[] = details.map(d => ({
      itemId: d.itemId,
      itemName: d.itemName,
      quantity: d.quantity,
      unitPrice: Number(d.price)
    }));

    const totalAmount = details.reduce((sum, d) => sum + Number(d.total), 0);

    return {
      id: o.id,
      number: o.number,
      personId: (o as any).personId,
      customerName: o.personName,
      customerEmail: '',
      items,
      status: 'OK',
      createdAt: o.createdAt,
      updatedAt: undefined,
      totalAmount
    };
  }
}
