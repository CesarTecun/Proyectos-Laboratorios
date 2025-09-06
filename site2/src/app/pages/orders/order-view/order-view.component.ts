import { CommonModule } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { OrderService, OrderReadDto, OrderItemDto } from '../../../services/order.service';

@Component({
  selector: 'app-order-view',
  standalone: true,
  imports: [CommonModule, RouterModule, MatIconModule, MatButtonModule, MatProgressSpinnerModule],
  templateUrl: './order-view.component.html'
})
export class OrderViewComponent implements OnInit {
  isLoading = signal<boolean>(true);
  error = signal<string>('');
  order = signal<OrderReadDto | null>(null);

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private orderService: OrderService
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!id) {
      this.error.set('ID de orden inválido');
      this.isLoading.set(false);
      return;
    }
    this.orderService.getOrder(id).subscribe({
      next: (o) => {
        if (!o) {
          this.error.set('No se encontró la orden');
        } else {
          this.order.set(o);
        }
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Error loading order', err);
        this.error.set('Error al cargar la orden');
        this.isLoading.set(false);
      }
    });
  }

  statusLabel(status?: string): string {
    const s = (status || '').toUpperCase();
    if (s === 'PENDING') return 'Pendiente';
    if (s === 'COMPLETED') return 'Completada';
    if (s === 'CANCELLED') return 'Cancelada';
    return status || 'Pendiente';
  }

  normalizeStatus(s?: string): 'PENDING' | 'COMPLETED' | 'CANCELLED' | '' {
    const v = (s ?? '').trim().toUpperCase();
    if (v === 'PENDIENTE' || v === 'PENDING') return 'PENDING';
    if (v === 'COMPLETED' || v === 'COMPLETADA') return 'COMPLETED';
    if (v === 'CANCELLED' || v === 'CANCELED' || v === 'CANCELADA') return 'CANCELLED';
    return '';
  }

  getOrderTotal(): number {
    const o = this.order();
    if (!o) return 0;
    const items: OrderItemDto[] = o.items || [];
    const sum: number = items.reduce((acc: number, it: OrderItemDto) => acc + Number(it.unitPrice) * Number(it.quantity), 0);
    return Number.isFinite(sum) ? sum : 0;
  }
}
