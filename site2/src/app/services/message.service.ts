import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MessageService {

  private readonly endpoint = 'http://localhost:5062/hola';

  constructor(private http: HttpClient) {}

  sendMessage(message: string): Observable<any> {
    let payload = {
      message: message
    }
    return this.http.post(this.endpoint, payload);
  }
  
}
