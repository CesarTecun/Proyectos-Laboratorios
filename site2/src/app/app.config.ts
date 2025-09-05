import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideAnimations } from '@angular/platform-browser/animations';
import { routes } from './app.routes';
import { provideClientHydration } from '@angular/platform-browser';
import { provideHttpClient, withFetch, HttpClient } from '@angular/common/http';
import { provideNativeDateAdapter } from '@angular/material/core';

// Import services
import { MessageService } from './services/message.service';

// Create factory function for MessageService
const provideMessageService = () => ({
  provide: MessageService,
  useFactory: (http: HttpClient) => new MessageService(http),
  deps: [HttpClient]
});

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes), 
    provideClientHydration(), 
    provideAnimations(),
    provideHttpClient(withFetch()),
    provideNativeDateAdapter(),
    provideMessageService()
  ]
};
