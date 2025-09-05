import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of, throwError } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);
  public currentUser$: Observable<any> = this.currentUserSubject.asObservable();

  constructor(private router: Router) {
    // Cargar usuario del localStorage si existe
    const storedUser = localStorage.getItem('currentUser');
    if (storedUser) {
      try {
        this.currentUserSubject.next(JSON.parse(storedUser));
      } catch (e) {
        console.error('Error al cargar el usuario del localStorage', e);
        this.clearUser();
      }
    }
  }

  login(credentials: { email: string; password: string }): Observable<boolean> {
    // Simulación de autenticación
    return of({
      id: '1',
      name: 'Usuario Demo',
      email: credentials.email,
      role: 'admin'
    }).pipe(
      tap(user => {
        this.currentUserSubject.next(user);
        localStorage.setItem('currentUser', JSON.stringify(user));
      }),
      map(user => !!user),
      catchError(error => {
        console.error('Error en el login', error);
        return throwError(() => new Error('Error en la autenticación'));
      })
    );
  }

  logout(): Observable<boolean> {
    return of(true).pipe(
      tap(() => {
        this.clearUser();
        this.router.navigate(['/login']);
      })
    );
  }

  getCurrentUser(): any {
    return this.currentUserSubject.value;
  }

  isAuthenticated(): boolean {
    return !!this.currentUserSubject.value;
  }

  private clearUser(): void {
    this.currentUserSubject.next(null);
    localStorage.removeItem('currentUser');
  }
}
