import { HttpClient } from '@angular/common/http';
import { Injectable, Inject } from '@angular/core';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  private readonly endpoint = 'http://localhost:5062/hola';

  constructor(
    @Inject(HttpClient) private http: HttpClient
  ) {}

  /**
   * Envía un mensaje al servidor
   * @param message El mensaje a enviar
   * @returns Observable con la respuesta del servidor
   */
  sendMessage(message: string): Observable<any> {
    const payload = { message };
    return this.http.post(this.endpoint, payload);
  }
  
  /**
   * Muestra un mensaje de error al usuario
   * @param message El mensaje de error a mostrar
   * @param duration Duración en milisegundos que se mostrará el mensaje (opcional)
   */
  showError(message: string, duration: number = 5000): void {
    // En una implementación real, esto podría usar un componente de notificación
    console.error('Error:', message);
    
    // Mostrar alerta nativa como respaldo
    if (typeof window !== 'undefined' && window.alert) {
      window.alert(`Error: ${message}`);
    }
  }
  
  /**
   * Muestra un mensaje de éxito al usuario
   * @param message El mensaje de éxito a mostrar
   * @param duration Duración en milisegundos que se mostrará el mensaje (opcional)
   */
  showSuccess(message: string, duration: number = 3000): void {
    console.log('Éxito:', message);
    
    // Mostrar alerta nativa como respaldo
    if (typeof window !== 'undefined' && window.alert) {
      window.alert(`Éxito: ${message}`);
    }
  }
}
