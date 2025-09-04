import { Routes } from '@angular/router';
import { PersonComponent } from './pages/person/person.component';

export const routes: Routes = [
    { path: '', redirectTo: 'home', pathMatch: 'full' },
    { path: 'persons', component: PersonComponent },
];
