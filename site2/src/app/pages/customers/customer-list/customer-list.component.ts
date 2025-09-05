import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { PersonService } from '../../../services/person.service';

@Component({
  selector: 'app-customer-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="container mx-auto p-4">
      <div class="flex justify-between items-center mb-6">
        <h1 class="text-2xl font-bold">Lista de Clientes</h1>
      </div>
      
      <div class="bg-white shadow-md rounded-lg overflow-hidden">
        <table class="min-w-full">
          <thead class="bg-gray-100">
            <tr>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">ID</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Nombre</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Correo</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Teléfono</th>
              <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Dirección</th>
            </tr>
          </thead>
          <tbody class="bg-white divide-y divide-gray-200">
            <tr *ngFor="let person of persons" class="hover:bg-gray-50">
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{{ person.id }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                {{ person.firstName }} {{ person.lastName }}
              </td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{{ person.email || '-' }}</td>
              <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{{ person.phone || '-' }}</td>
              <td class="px-6 py-4 text-sm text-gray-500">{{ person.address || '-' }}</td>
            </tr>
            <tr *ngIf="persons.length === 0">
              <td colspan="5" class="px-6 py-4 text-center text-sm text-gray-500">
                No hay clientes registrados
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  `,
  styles: []
})
export class CustomerListComponent implements OnInit {
  persons: any[] = [];
  isLoading = true;
  error = '';

  constructor(private personService: PersonService) {}

  ngOnInit(): void {
    this.loadPersons();
  }

  loadPersons(): void {
    this.isLoading = true;
    this.personService.getPersons().subscribe({
      next: (data) => {
        this.persons = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading persons:', err);
        this.error = 'Error al cargar los clientes';
        this.isLoading = false;
      }
    });
  }
}
