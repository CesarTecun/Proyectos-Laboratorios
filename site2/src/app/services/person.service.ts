import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Person {
  id: number;
  name: string;
  email: string;
  phone: string;
  address: string;
}

@Injectable({
  providedIn: 'root'
})
export class PersonService {
  private apiUrl = `${environment.apiUrl}/api/persons`;

  constructor(private http: HttpClient) {}

  getPersons(): Observable<Person[]> {
    return this.http.get<Person[]>(this.apiUrl);
  }

  getPerson(id: number): Observable<Person> {
    return this.http.get<Person>(`${this.apiUrl}/${id}`);
  }

  createPerson(person: Omit<Person, 'id'>): Observable<Person> {
    return this.http.post<Person>(this.apiUrl, person);
  }

  updatePerson(id: number, person: Partial<Person>): Observable<Person> {
    return this.http.put<Person>(`${this.apiUrl}/${id}`, person);
  }

  deletePerson(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
