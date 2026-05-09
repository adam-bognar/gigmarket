import { ChangeDetectionStrategy, Component, computed, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DatePipe, DecimalPipe } from '@angular/common';
import { forkJoin } from 'rxjs';
import {
  ArrowRight,
  CircleDollarSign,
  ClipboardList,
  Clock,
  CreditCard,
  LayoutGrid,
  LucideAngularModule,
  Receipt,
  UserRound,
} from 'lucide-angular';
import { AuthService } from '../../shared/services/auth.service';
import { SellerProfileService } from '../../shared/services/seller-profile.service';
import { GigService } from '../../shared/services/gig.service';
import { OrderService } from '../../shared/services/order.service';
import { SellerEarningsDto } from '../../shared/models/seller.model';
import { GigSummaryDto } from '../../shared/models/gig.model';
import { OrderSummaryDto } from '../../shared/models/order.model';

@Component({
  selector: 'app-seller-dashboard',
  imports: [RouterLink, DatePipe, DecimalPipe, LucideAngularModule],
  templateUrl: './seller-dashboard.html',
  styleUrl: './seller-dashboard.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SellerDashboard implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly sellerService = inject(SellerProfileService);
  private readonly gigService = inject(GigService);
  private readonly orderService = inject(OrderService);

  protected readonly CreditCardIcon = CreditCard;
  protected readonly CircleDollarSignIcon = CircleDollarSign;
  protected readonly ClockIcon = Clock;
  protected readonly LayoutGridIcon = LayoutGrid;
  protected readonly ClipboardListIcon = ClipboardList;
  protected readonly ArrowRightIcon = ArrowRight;
  protected readonly UserRoundIcon = UserRound;
  protected readonly ReceiptIcon = Receipt;

  readonly user = this.authService.user;

  earnings = signal<SellerEarningsDto | null>(null);
  gigs = signal<GigSummaryDto[]>([]);
  orders = signal<OrderSummaryDto[]>([]);
  isLoading = signal(true);
  error = signal<string | null>(null);

  activeGigCount = computed(() =>
    this.gigs().filter(g => g.status.toLowerCase() === 'active').length
  );

  activeOrderCount = computed(() =>
    this.orders().filter(o => o.status === 'InProgress' || o.status === 'UnderRevision').length
  );

  recentTransactions = computed(() =>
    this.earnings()?.transactions.slice(0, 3) ?? []
  );

  stripeStatus = computed(() => this.earnings()?.stripeAccountStatus ?? 'NotConnected');
  isStripeActive = computed(() => this.stripeStatus() === 'Active');

  ngOnInit() {
    forkJoin({
      earnings: this.sellerService.getEarnings(),
      gigs: this.gigService.getMyGigs(),
      orders: this.orderService.getSellerOrders(),
    }).subscribe({
      next: ({ earnings, gigs, orders }) => {
        this.earnings.set(earnings);
        this.gigs.set(gigs);
        this.orders.set(orders);
        this.isLoading.set(false);
      },
      error: () => {
        this.error.set('Unable to load dashboard. Please try again.');
        this.isLoading.set(false);
      },
    });
  }
}
