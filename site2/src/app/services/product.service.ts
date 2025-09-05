import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, catchError, map, of } from 'rxjs';
import { environment } from '../../environments/environment';
import { Product, CreateProductDto, UpdateProductDto } from '../models/product';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = `${environment.apiUrl.replace(/\/+$/, '')}/api/product`;
  
  private jsonHeaders = new HttpHeaders({ 'Content-Type': 'application/json' });

  constructor(private http: HttpClient) {}

  /**
   * Obtiene todos los productos
   */
  getAll(): Observable<Product[]> {
    return this.http.get<any>(this.apiUrl).pipe(
      map(response => {
        console.log('Respuesta cruda de la API:', response);
        let products: any[] = [];
        
        if (Array.isArray(response)) {
          products = response;
        } else if (response && Array.isArray(response.$values)) {
          products = response.$values;
        }
        
        console.log('Productos mapeados:', products.map(p => ({
          id: p.id,
          name: p.name,
          price: p.price,
          description: p.description,
          createdAt: p.createdAt,
          updatedAt: p.updatedAt
        })));
        
        return products;
      }),
      catchError(error => {
        console.error('Error al obtener los productos:', error);
        return of([]);
      })
    );
  }

  /**
   * Obtiene un producto por su ID
   */
  getById(id: number): Observable<Product | null> {
    return this.http.get<any>(`${this.apiUrl}/${id}`).pipe(
      catchError(error => {
        console.error(`Error al obtener el producto con ID ${id}:`, error);
        return of(null);
      })
    );
  }

  /**
   * Crea un nuevo producto
   */
  create(product: CreateProductDto): Observable<Product | null> {
    return this.http.post<Product>(this.apiUrl, product, { headers: this.jsonHeaders }).pipe(
      catchError(error => {
        console.error('Error al crear el producto:', error);
        return of(null);
      })
    );
  }

  /**
   * Actualiza un producto existente
   */
  update(id: number, product: UpdateProductDto): Observable<boolean> {
    return this.http.put(`${this.apiUrl}/${id}`, product, { 
      headers: this.jsonHeaders,
      observe: 'response' 
    }).pipe(
      map(response => response.status === 204),
      catchError(error => {
        console.error(`Error al actualizar el producto con ID ${id}:`, error);
        return of(false);
      })
    );
  }

  /**
   * Elimina un producto por su ID
   */
  delete(id: number): Observable<boolean> {
    return this.http.delete(`${this.apiUrl}/${id}`, { 
      observe: 'response' 
    }).pipe(
      map(response => response.status === 204),
      catchError(error => {
        console.error(`Error al eliminar el producto con ID ${id}:`, error);
        return of(false);
      })
    );
  }
}
