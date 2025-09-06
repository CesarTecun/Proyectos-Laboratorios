import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { provideHttpClient } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter } from '@angular/router';
import { routes } from './app/app.routes';
import { LOCALE_ID } from '@angular/core';
import { registerLocaleData } from '@angular/common';
import localeEsGT from '@angular/common/locales/es-GT';

// Registrar locale de Guatemala para pipes (currency, date, number)
registerLocaleData(localeEsGT, 'es-GT');

bootstrapApplication(AppComponent, {
  ...appConfig,
  providers: [
    ...(appConfig.providers || []),
    provideHttpClient(), provideAnimationsAsync(), 
    provideRouter(routes), provideAnimationsAsync(), provideAnimationsAsync('noop'),
    { provide: LOCALE_ID, useValue: 'es-GT' }
  ],
})
  .catch((err) => console.error(err));
