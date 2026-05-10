import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  OnInit,
  PLATFORM_ID,
  signal,
} from '@angular/core';
import {isPlatformBrowser} from '@angular/common';
import {ActivatedRoute, Router, RouterLink} from '@angular/router';
import {DatePipe, DecimalPipe} from '@angular/common';
import {LucideAngularModule, ExternalLinkIcon, StarIcon, GlobeIcon, CalendarIcon, BriefcaseIcon, GraduationCapIcon, AwardIcon, MessageCircleIcon, PencilIcon} from 'lucide-angular';
import {SellerProfileService} from '../../shared/services/seller-profile.service';
import {AuthService} from '../../shared/services/auth.service';
import {ChatService} from '../../shared/services/chat.service';
import {GigSummaryDto} from '../../shared/models/gig.model';
import {Card, BrowseCardItem} from '../browse/card/card';
import {SellerPublicProfileDto} from '../../shared/models/seller.model';

function toCardItem(g: GigSummaryDto): BrowseCardItem {
  return {
    id: g.id,
    coverImageSrc: g.primaryPhotoUrl,
    coverImageAlt: g.title,
    sellerName: `${g.sellerFirstName} ${g.sellerLastName}`.trim(),
    sellerAvatarUrl: g.sellerAvatarUrl,
    sellerProfileId: g.sellerProfileId,
    title: g.title,
    category: g.categoryName,
    categoryId: g.categoryId,
    basePrice: g.startingPrice,
    deliveryDays: g.minDeliveryDays,
    rating: g.averageRating,
    reviewCount: g.totalReviews,
  };
}

@Component({
  selector: 'app-seller-profile',
  imports: [DatePipe, DecimalPipe, RouterLink, LucideAngularModule, Card],
  templateUrl: './seller-profile.html',
  styleUrl: './seller-profile.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SellerProfile implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly platformId = inject(PLATFORM_ID);
  private readonly sellerService = inject(SellerProfileService);
  private readonly authService = inject(AuthService);
  private readonly chatService = inject(ChatService);

  protected readonly StarIcon = StarIcon;
  protected readonly GlobeIcon = GlobeIcon;
  protected readonly CalendarIcon = CalendarIcon;
  protected readonly BriefcaseIcon = BriefcaseIcon;
  protected readonly GraduationCapIcon = GraduationCapIcon;
  protected readonly AwardIcon = AwardIcon;
  protected readonly MessageCircleIcon = MessageCircleIcon;
  protected readonly PencilIcon = PencilIcon;
  protected readonly ExternalLinkIcon = ExternalLinkIcon;

  readonly profile = signal<SellerPublicProfileDto | null>(null);
  readonly isLoading = signal(true);
  readonly error = signal<string | null>(null);
  readonly isStartingConversation = signal(false);

  readonly currentUser = this.authService.user;
  readonly isAuthenticated = this.authService.isAuthenticated;

  readonly fullName = computed(() => {
    const p = this.profile();
    return p ? `${p.firstName} ${p.lastName}`.trim() : '';
  });

  readonly isOwnProfile = computed(() => {
    const user = this.currentUser();
    const p = this.profile();
    return !!user && !!p && user.id === p.userId;
  });

  readonly hasActiveGigs = computed(() => (this.profile()?.gigs.length ?? 0) > 0);

  readonly gigCards = computed<BrowseCardItem[]>(() =>
    (this.profile()?.gigs ?? []).map(toCardItem)
  );

  readonly stars = [1, 2, 3, 4, 5];

  ngOnInit(): void {
    if (!isPlatformBrowser(this.platformId)) return;

    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.error.set('No seller ID provided.');
      this.isLoading.set(false);
      return;
    }

    this.sellerService.getPublicProfile(id).subscribe({
      next: (profile) => {
        this.profile.set(profile);
        this.isLoading.set(false);
      },
      error: () => {
        this.error.set('Failed to load seller profile. Please try again.');
        this.isLoading.set(false);
      },
    });
  }

  contactSeller(): void {
    const p = this.profile();
    if (!p || this.isOwnProfile() || !this.hasActiveGigs() || this.isStartingConversation()) return;

    if (!this.isAuthenticated()) {
      this.router.navigate(['/login']);
      return;
    }

    this.isStartingConversation.set(true);

    this.chatService.startConversation({
      gigId: p.gigs[0].id,
    }).subscribe({
      next: (conversation) => {
        this.isStartingConversation.set(false);
        this.router.navigate(['/inbox', conversation.id]);
      },
      error: () => {
        this.isStartingConversation.set(false);
      },
    });
  }

  formatMemberSince(dateStr: string): string {
    const date = new Date(dateStr);
    if (isNaN(date.getTime())) return 'Unknown';
    return date.toLocaleString('en-US', {month: 'long', year: 'numeric'});
  }

  starFill(star: number, rating: number): 'full' | 'half' | 'empty' {
    if (rating >= star) return 'full';
    if (rating >= star - 0.5) return 'half';
    return 'empty';
  }
}
