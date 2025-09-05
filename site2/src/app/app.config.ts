import { ApplicationConfig, Injectable } from '@angular/core';
import { provideRouter, Router } from '@angular/router';
import { provideAnimations } from '@angular/platform-browser/animations';

import { routes } from './app.routes';
import { provideClientHydration } from '@angular/platform-browser';
import { provideHttpClient, withFetch, HttpClient } from '@angular/common/http';
import { provideNativeDateAdapter } from '@angular/material/core';

// Import services
import { AuthService } from './services/auth.service';
import { MessageService } from './services/message.service';

// Create factory functions for services
const provideAuthService = () => ({
  provide: AuthService,
  useFactory: (http: HttpClient, router: Router) => new AuthService(router),
  deps: [HttpClient, Router]
});

const provideMessageService = () => ({
  provide: MessageService,
  useFactory: (http: HttpClient) => new MessageService(http),
  deps: [HttpClient]
});

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes), 
    provideClientHydration(), 
    provideHttpClient(withFetch()),
    provideAnimations(),
    provideAuthService(),
    provideMessageService(),
    provideNativeDateAdapter()
  ]
};
