import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DatePipe, DecimalPipe } from '@angular/common';
import { OrderService } from '../../../shared/services/order.service';
import { OrderSummaryDto } from '../../../shared/models/order.model';
import { LucideAngularModule, ClipboardList, Clock, AlertCircle } from 'lucide-angular';

@Component({
  selector: 'app-seller-orders',
  imports: [RouterLink, DatePipe, DecimalPipe, LucideAngularModule],
  templateUrl: './seller-orders.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SellerOrders implements OnInit {
  private readonly orderService = inject(OrderService);

  readonly clipboardIcon = ClipboardList;
  readonly clockIcon = Clock;
  readonly alertIcon = AlertCircle;

  orders = signal<OrderSummaryDto[]>([]);
  isLoading = signal(true);
  error = signal<string | null>(null);

  ngOnInit() {
    this.orderService.getSellerOrders().subscribe({
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
      case 'InProgress':     return 'bg-blue-100 text-blue-700';
      case 'Delivered':      return 'bg-purple-100 text-purple-700';
      case 'UnderRevision':  return 'bg-yellow-100 text-yellow-700';
      case 'Completed':      return 'bg-green-100 text-green-700';
      case 'Cancelled':      return 'bg-red-100 text-red-700';
      default:               return 'bg-surface-alt text-muted';
    }
  }

  statusLabel(status: string): string {
    switch (status) {
      case 'InProgress':    return 'In Progress';
      case 'UnderRevision': return 'Under Revision';
      default:              return status;
    }
  }

  isActionable(status: string): boolean {
    return status === 'InProgress' || status === 'UnderRevision';
  }
}
