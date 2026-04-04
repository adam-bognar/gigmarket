import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DatePipe, DecimalPipe } from '@angular/common';
import { OrderService } from '../../shared/services/order.service';
import { OrderDto } from '../../shared/models/order.model';
import { LucideAngularModule, Package, Clock, CircleCheck } from 'lucide-angular';

@Component({
  selector: 'app-my-orders',
  imports: [RouterLink, DatePipe, DecimalPipe, LucideAngularModule],
  templateUrl: './my-orders.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MyOrders implements OnInit {
  private readonly orderService = inject(OrderService);

  readonly packageIcon = Package;
  readonly clockIcon = Clock;
  readonly circleCheckIcon = CircleCheck;

  orders = signal<OrderDto[]>([]);
  isLoading = signal(true);
  error = signal<string | null>(null);

  ngOnInit() {
    this.orderService.getMyOrders().subscribe({
      next: (orders) => {
        this.orders.set(orders);
        this.isLoading.set(false);
      },
      error: () => {
        this.error.set('Failed to load orders. Please try again.');
        this.isLoading.set(false);
      },
    });
  }

  statusClass(status: string): string {
    switch (status) {
      case 'Paid':      return 'bg-green-100 text-green-700';
      case 'Pending':   return 'bg-yellow-100 text-yellow-700';
      case 'Cancelled': return 'bg-red-100 text-red-700';
      default:          return 'bg-surface-alt text-muted';
    }
  }
}
