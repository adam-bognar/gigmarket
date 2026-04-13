import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { DatePipe, DecimalPipe } from '@angular/common';
import { OrderService } from '../../shared/services/order.service';
import { OrderDto } from '../../shared/models/order.model';
import { LucideAngularModule, CircleCheck, Package, Clock } from 'lucide-angular';

@Component({
  selector: 'app-order-success',
  imports: [RouterLink, DatePipe, DecimalPipe, LucideAngularModule],
  templateUrl: './order-success.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OrderSuccess implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly orderService = inject(OrderService);

  readonly circleCheckIcon = CircleCheck;
  readonly packageIcon = Package;
  readonly clockIcon = Clock;

  order = signal<OrderDto | null>(null);
  isLoading = signal(true);
  error = signal<string | null>(null);

  //TODO handle correct order
  ngOnInit() {
    this.orderService.getMyOrders().subscribe({
      next: (orders) => {
        this.order.set(orders[0] ?? null);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
      },
    });
  }
}
