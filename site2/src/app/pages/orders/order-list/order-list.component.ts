import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { OrderService, OrderReadDto } from '../../../services/order.service';

@Component({
  selector: 'app-order-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './order-list.component.html',
  styles: []
})
export class OrderListComponent implements OnInit {
  orders: OrderReadDto[] = [];
  isLoading = true;
  error = '';

  constructor(private orderService: OrderService) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.isLoading = true;
    this.orderService.getOrders().subscribe(
      (data: OrderReadDto[]) => {
        console.log('Orders data received:', data);
        // Ensure data is an array before assignment
        this.orders = Array.isArray(data) ? data : [];
        this.isLoading = false;
      },
      (err: any) => {
        console.error('Error loading orders:', err);
        this.error = 'Error al cargar las órdenes';
        this.isLoading = false;
        // Initialize empty array on error to prevent template errors
        this.orders = [];
      }
    );
  }

  deleteOrder(id: number): void {
    if (confirm('¿Estás seguro de que deseas eliminar esta orden?')) {
      // TODO: Implementar eliminación de orden
      console.log('Eliminar orden:', id);
    }
  }
}
