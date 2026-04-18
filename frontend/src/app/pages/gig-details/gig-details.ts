import {ChangeDetectionStrategy, Component, computed, inject, OnInit, signal} from '@angular/core';
import {ActivatedRoute, Router, RouterLink} from '@angular/router';
import {DatePipe, DecimalPipe} from '@angular/common';
import {GigService} from '../../shared/services/gig.service';
import {AddReviewPayload, GigDetailDto, GigDetailPackageDto, ReviewDto} from '../../shared/models/gig.model';
import {OrderService} from '../../shared/services/order.service';
import {ChatService} from '../../shared/services/chat.service';
import {ReviewService} from '../../shared/services/review.service';
import {AuthService} from '../../shared/services/auth.service';
import {HttpErrorResponse} from '@angular/common/http';
import {LucideAngularModule, SendIcon, StarIcon} from 'lucide-angular';

type PackageTier = 'basic' | 'standard' | 'premium';

@Component({
  selector: 'app-gig-details',
  imports: [DatePipe, DecimalPipe, LucideAngularModule, RouterLink],
  templateUrl: './gig-details.html',
  styleUrl: './gig-details.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GigDetails implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly gigService = inject(GigService);
  private readonly reviewService = inject(ReviewService);
  private readonly authService = inject(AuthService);
  private readonly orderService = inject(OrderService);
  private readonly router = inject(Router);
  private readonly chatService = inject(ChatService);

  gig = signal<GigDetailDto | null>(null);
  isLoading = signal(true);
  error = signal<string | null>(null);
  isCheckingOut = signal(false);
  isStartingConversation = signal(false);

  reviewRating = signal<number>(0);
  reviewDescription = signal('');
  isSubmittingReview = signal(false);
  reviewSubmitError = signal<string | null>(null);
  reviewSubmitted = signal(false);

  selectedImageIndex = signal(0);
  selectedPackageTier = signal<PackageTier>('basic');

  selectedPackage = computed<GigDetailPackageDto | null>(() => {
    const g = this.gig();
    if (!g?.packages.length) {
      return null;
    }

    const selectedTier = this.selectedPackageTier();
    return g.packages.find((pkg) => this.normalizePackageTier(pkg.tier) === selectedTier) ?? g.packages[0];
  });

  sellerName = computed(() => {
    const g = this.gig();
    return g ? `${g.sellerFirstName} ${g.sellerLastName}` : '';
  });

  sellerOccupation = computed(() => {
    const g = this.gig();
    return g?.sellerOccupation ?? `${g?.subcategoryName ?? g?.categoryName ?? 'Freelance'} Specialist`;
  });

  sellerMeta = computed(() => {
    const g = this.gig();
    if (!g) {
      return '';
    }

    const from = g.sellerCountry ?? 'Not specified';
    const since = g.sellerMemberSinceUtc
      ? this.formatMonthYear(g.sellerMemberSinceUtc)
      : this.formatMonthYear(g.createdAtUtc);
    return `From ${from}, member since ${since}`;
  });

  sellerBio = computed(() => {
    const g = this.gig();
    return g?.description ?? '';
  });

  allImages = computed(() => {
    const g = this.gig();
    if (!g) {
      return [];
    }

    return [g.primaryPhotoUrl, ...(g.additionalPhotoUrls ?? [])].filter(Boolean);
  });

  displayedThumbnails = computed(() => {
    return this.allImages().slice(1, 4);
  });

  activeImage = computed(() => {
    const images = this.allImages();
    const idx = this.selectedImageIndex();
    return images[idx] ?? images[0] ?? '';
  });

  //TODO show this data
  ratingBreakdown = computed(() => {
    const g = this.gig();
    if (!g || !g.reviews.length) return [];
    return [5, 4, 3, 2, 1].map((star) => {
      const count = g.reviews.filter((r) => Math.round(r.rating) === star).length;
      return {star, count, pct: Math.round((count / g.reviews.length) * 100)};
    });
  });

  protected readonly currentUser = this.authService.user;
  protected readonly isAuthenticated = this.authService.isAuthenticated;

  protected readonly hasAlreadyReviewed = computed(() => {
    const user = this.currentUser();
    const g = this.gig();
    if (!user || !g) return false;
    return g.reviews.some(r => r.reviewerUserId === user.id);
  });

  protected readonly canShowReviewForm = computed(() =>
    this.isAuthenticated() && !this.hasAlreadyReviewed() && !this.reviewSubmitted()
  );

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.error.set('No gig ID provided.');
      this.isLoading.set(false);
      return;
    }
    this.gigService.getGigById(id).subscribe({
      next: (gig) => {
        this.gig.set(gig);
        this.alignSelectedTierWithApi(gig);
        this.isLoading.set(false);
      },
      error: () => {
        this.error.set('Failed to load gig. Please try again.');
        this.isLoading.set(false);
      },
    });
  }

  checkout() {
    const g = this.gig();
    const pkg = this.selectedPackage();
    if (!g || !pkg || this.isCheckingOut()) return;

    this.isCheckingOut.set(true);

    this.orderService.createCheckoutSession({ gigId: g.id, packageId: pkg.id }).subscribe({
      next: ({ sessionUrl }) => {
        window.location.href = sessionUrl;
      },
      error: () => {
        this.isCheckingOut.set(false);
        this.error.set('Failed to start checkout. Please try again.');
      },
    });
  }

  contactSeller() {
    const g = this.gig();
    if (!g || this.isStartingConversation()) return;

    this.isStartingConversation.set(true);
    this.error.set(null);

    this.chatService.startConversation({
      gigId: g.id,
      initialMessage: `Hi! I'm interested in your gig: ${g.title}`
    }).subscribe({
      next: (conversation) => {
        this.isStartingConversation.set(false);
        this.router.navigate(['/inbox', conversation.id]);
      },
      error: () => {
        this.isStartingConversation.set(false);
        this.error.set('Failed to start conversation. Please try again.');
      }
    });
  }

  submitReview(): void {
    const g = this.gig();
    if (!g || this.isSubmittingReview()) return;

    const rating = this.reviewRating();
    const description = this.reviewDescription().trim();

    if (rating < 1 || rating > 5) {
      this.reviewSubmitError.set('Please select a star rating.');
      return;
    }
    if (description.length < 10 || description.length > 2000) {
      this.reviewSubmitError.set('Description must be between 10 and 2000 characters.');
      return;
    }

    this.isSubmittingReview.set(true);
    this.reviewSubmitError.set(null);

    const payload: AddReviewPayload = { gigId: g.id, rating, description };

    this.reviewService.submitReview(payload).subscribe({
      next: (newReview: ReviewDto) => {
        this.gig.update(current => {
          if (!current) return current;
          const updatedReviews = [newReview, ...current.reviews];
          const totalReviews = updatedReviews.length;
          const averageRating = Math.round(
            (updatedReviews.reduce((s, r) => s + r.rating, 0) / totalReviews) * 10) / 10;
          return { ...current, reviews: updatedReviews, totalReviews, averageRating };
        });
        this.reviewSubmitted.set(true);
        this.isSubmittingReview.set(false);
      },
      error: (err: HttpErrorResponse) => {
        this.isSubmittingReview.set(false);
        if (err.status === 400) {
          this.reviewSubmitError.set(err.error?.message ?? 'Cannot review this gig.');
        } else if (err.status === 401) {
          this.reviewSubmitError.set('Please log in to submit a review.');
        } else {
          this.reviewSubmitError.set('Failed to submit review. Please try again.');
        }
      },
    });
  }

  protected setRating(star: number): void {
    this.reviewRating.set(star);
  }

  selectImage(index: number) {
    this.selectedImageIndex.set(index);
  }

  setSelectedPackageTier(tier: PackageTier) {
    this.selectedPackageTier.set(tier);
  }

  private alignSelectedTierWithApi(gig: GigDetailDto) {
    const availableTiers = gig.packages
      .map((pkg) => this.normalizePackageTier(pkg.tier))
      .filter((tier): tier is PackageTier => tier !== null);

    if (!availableTiers.length) {
      return;
    }

    if (!availableTiers.includes(this.selectedPackageTier())) {
      this.selectedPackageTier.set(availableTiers[0]);
    }
  }

  private normalizePackageTier(tier: string): PackageTier | null {
    const value = tier.trim().toLowerCase();
    if (value === 'basic' || value === 'standard' || value === 'premium') {
      return value;
    }
    return null;
  }

  private formatMonthYear(dateValue: string): string {
    const date = new Date(dateValue);
    if (Number.isNaN(date.getTime())) {
      return 'Unknown';
    }
    return date.toLocaleString('en-US', {month: 'short', year: 'numeric'});
  }

  protected readonly StarIcon = StarIcon;
  protected readonly SendIcon = SendIcon;
}
