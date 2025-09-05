import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Person, CreatePersonDto, UpdatePersonDto } from '../models/person';

@Injectable({
  providedIn: 'root'
})
export class PersonService {
  private apiUrl = `${environment.apiUrl}/api/persons`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Person[]> {
    return this.http.get<Person[]>(this.apiUrl);
  }

  // Alias for backward compatibility
  getPersons(): Observable<Person[]> {
    return this.getAll();
  }

  getPerson(id: number): Observable<Person> {
    return this.http.get<Person>(`${this.apiUrl}/${id}`);
  }

  add(dto: CreatePersonDto): Observable<Person> {
    return this.http.post<Person>(this.apiUrl, dto);
  }

  // Alias for backward compatibility
  createPerson(dto: CreatePersonDto): Observable<Person> {
    return this.add(dto);
  }

  update(id: number, dto: UpdatePersonDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto);
  }

  // Alias for backward compatibility
  updatePerson(id: number, dto: UpdatePersonDto): Observable<Person> {
    return this.http.put<Person>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  // Alias for backward compatibility
  deletePerson(id: number): Observable<void> {
    return this.delete(id);
  }
}
