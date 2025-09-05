import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, map, catchError, of } from 'rxjs';
import { environment } from '../../environments/environment';
import { Person, CreatePersonDto, UpdatePersonDto } from '../models/person';

@Injectable({
  providedIn: 'root'
})
export class PersonService {
  // Asegurarse de que no haya doble barra al final de la URL
  private apiUrl = `${environment.apiUrl.replace(/\/+$/, '')}/api/person`;
  
  // Headers para asegurar que el contenido sea JSON
  private jsonHeaders = new HttpHeaders({ 'Content-Type': 'application/json' });

  constructor(private http: HttpClient) {}

  /**
   * Obtiene todas las personas del backend
   * Maneja diferentes formatos de respuesta (array directo, {items: [...]}, {data: [...]})
   */
  getAll(): Observable<Person[]> {
    return this.http.get<any>(this.apiUrl).pipe(
      map(res => {
        console.log('API Response:', res); // Para depuración
        
        // Manejar el formato con $values
        if (res && Array.isArray(res.$values)) {
          return res.$values;
        }
        
        // Otros formatos de respuesta
        if (Array.isArray(res)) return res;  // Si es un array directo
        if (Array.isArray(res?.items)) return res.items;  // Si es {items: [...]}
        if (Array.isArray(res?.data)) return res.data;  // Si es {data: [...]}
        
        // Si es un objeto único con propiedades de Person
        if (res && typeof res === 'object' && 'id' in res) {
          return [res];
        }
        
        return [];  // Si no coincide con ningún formato conocido
      }),
      catchError(error => {
        console.error('Error en la petición:', error);
        return of([]); // Retornar array vacío en caso de error
      })
    );
  }

  // Alias para mantener compatibilidad
  getPersons(): Observable<Person[]> {
    return this.getAll();
  }

  getPerson(id: number): Observable<Person> {
    return this.http.get<Person>(`${this.apiUrl}/${id}`);
  }

  add(dto: CreatePersonDto): Observable<Person> {
    return this.http.post<Person>(
      this.apiUrl, 
      dto, 
      { headers: this.jsonHeaders }
    );
  }

  // Alias para mantener compatibilidad
  createPerson(dto: CreatePersonDto): Observable<Person> {
    return this.add(dto);
  }

  update(id: number, dto: UpdatePersonDto): Observable<Person> {
    return this.http.put<Person>(
      `${this.apiUrl}/${id}`, 
      dto, 
      { headers: this.jsonHeaders }
    );
  }

  // Alias para mantener compatibilidad
  updatePerson(id: number, dto: UpdatePersonDto): Observable<Person> {
    return this.update(id, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  // Alias para mantener compatibilidad
  deletePerson(id: number): Observable<void> {
    return this.delete(id);
  }
}
