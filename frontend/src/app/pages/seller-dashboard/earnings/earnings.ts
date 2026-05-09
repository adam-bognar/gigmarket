import {ChangeDetectionStrategy, Component, computed, inject, OnInit, signal} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {DecimalPipe, DatePipe} from '@angular/common';
import {
  CircleCheck,
  CircleDollarSign,
  Clock,
  CreditCard,
  ExternalLink,
  LucideAngularModule,
  Percent,
  Receipt,
  TriangleAlert,
  X,
} from 'lucide-angular';
import {SellerProfileService} from '../../../shared/services/seller-profile.service';
import {SellerEarningsDto, StripeAccountStatus} from '../../../shared/models/seller.model';

@Component({
  selector: 'app-earnings',
  imports: [DecimalPipe, DatePipe, LucideAngularModule],
  templateUrl: './earnings.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Earnings implements OnInit {
  private readonly sellerService = inject(SellerProfileService);
  private readonly route = inject(ActivatedRoute);

  protected readonly ExternalLinkIcon = ExternalLink;
  protected readonly CircleCheckIcon = CircleCheck;
  protected readonly XIcon = X;
  protected readonly TriangleAlertIcon = TriangleAlert;
  protected readonly CreditCardIcon = CreditCard;
  protected readonly CircleDollarSignIcon = CircleDollarSign;
  protected readonly ClockIcon = Clock;
  protected readonly PercentIcon = Percent;
  protected readonly ReceiptIcon = Receipt;

  earnings = signal<SellerEarningsDto | null>(null);
  isLoading = signal(true);
  error = signal<string | null>(null);
  isConnecting = signal(false);
  connectError = signal<string | null>(null);
  stripeReturnBanner = signal<'success' | 'refresh' | null>(null);

  stripeStatus = computed<StripeAccountStatus>(
    () => this.earnings()?.stripeAccountStatus ?? 'NotConnected'
  );
  isActive = computed(() => this.stripeStatus() === 'Active');
  isPending = computed(() => this.stripeStatus() === 'Pending');
  isNotConnected = computed(() => this.stripeStatus() === 'NotConnected');

  ngOnInit() {
    const params = this.route.snapshot.queryParamMap;
    if (params.has('stripe_return')) {
      this.stripeReturnBanner.set('success');
    } else if (params.has('stripe_refresh')) {
      this.stripeReturnBanner.set('refresh');
    }

    this.loadEarnings();
  }

  loadEarnings() {
    this.isLoading.set(true);
    this.sellerService.getEarnings().subscribe({
      next: (data) => {
        this.earnings.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.error.set('Unable to load earnings. Please try again.');
        this.isLoading.set(false);
      },
    });
  }

  onConnectStripe() {
    this.isConnecting.set(true);
    this.connectError.set(null);
    this.sellerService.connectStripe().subscribe({
      next: (result) => {
        this.isConnecting.set(false);
        if (result.onboardingUrl) {
          window.location.href = result.onboardingUrl;
        } else {
          this.loadEarnings();
        }
      },
      error: () => {
        this.isConnecting.set(false);
        this.connectError.set('Failed to initiate Stripe connection. Please try again.');
      },
    });
  }

  onOpenStripeDashboard() {
    this.sellerService.getStripeDashboardLink().subscribe({
      next: (res) => window.open(res.url, '_blank'),
      error: () => this.connectError.set('Could not open Stripe dashboard. Please try again.'),
    });
  }

  dismissBanner() {
    this.stripeReturnBanner.set(null);
  }
}
