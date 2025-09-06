import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, catchError, map, of } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Item {
  id: number;
  name: string;
  description: string;
  price: number;
  stock: number;
  category: string;
}

@Injectable({
  providedIn: 'root'
})
export class ItemService {
  private apiUrl = `${environment.apiUrl}/api/item`;

  constructor(private http: HttpClient) {}

  getItems(): Observable<Item[]> {
    return this.http.get<any>(this.apiUrl).pipe(
      map((resp: any) => Array.isArray(resp) ? resp as Item[] : (resp?.$values as Item[] ?? [])),
      catchError((err) => {
        console.error('Error fetching items:', err);
        return of([] as Item[]);
      })
    );
  }

  getItem(id: number): Observable<Item | null> {
    return this.http.get<Item>(`${this.apiUrl}/${id}`).pipe(
      catchError((err) => {
        console.error('Error fetching item:', err);
        return of(null);
      })
    );
  }

  createItem(item: Omit<Item, 'id'>): Observable<Item> {
    return this.http.post<Item>(this.apiUrl, item);
  }

  updateItem(id: number, item: Partial<Item>): Observable<Item> {
    return this.http.put<Item>(`${this.apiUrl}/${id}`, item);
  }

  deleteItem(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
