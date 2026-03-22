import { DecimalPipe } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  OnInit,
  signal,
} from '@angular/core';
import { Card, type BrowseCardItem } from './card/card';
import {
  ALL_CATEGORIES,
  BROWSE_CATEGORY_OPTIONS,
  DEFAULT_BROWSE_FILTERS,
  Filter,
  type BrowseFilterState,
} from './filter/filter';
import {GigService} from '../../shared/services/gig.service';
import {GigSummaryDto} from '../../shared/models/gig.model';
import {RouterLink} from '@angular/router';

const PRICE_MIN = 0;
const PRICE_MAX = 300;


function mapToCardItem(dto: GigSummaryDto): BrowseCardItem {
  const mapped = {
    id: dto.id,
    coverImageSrc: dto.primaryPhotoUrl,
    coverImageAlt: dto.title,
    sellerName: `${dto.sellerFirstName} ${dto.sellerLastName}`.trim(),
    sellerAvatarUrl: dto.sellerAvatarUrl,
    title: dto.title,
    category: dto.categoryName,
    basePrice: dto.startingPrice,
    deliveryDays: dto.minDeliveryDays,
    rating: dto.averageRating,
    reviewCount: dto.totalReviews,
  };

  console.log('[mapToCardItem] input:', dto, '→ output:', mapped);
  return mapped;
}

@Component({
  selector: 'app-browse',
  imports: [Card, DecimalPipe, Filter, RouterLink],
  templateUrl: './browse.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Browse implements OnInit {
  private readonly gigService = inject(GigService);

  readonly cards = signal<BrowseCardItem[]>([]);
  readonly isLoading = signal(true);
  readonly error = signal<string | null>(null);

  readonly filters = signal<BrowseFilterState>(DEFAULT_BROWSE_FILTERS);
  readonly categoryOptions = BROWSE_CATEGORY_OPTIONS;
  readonly minPrice = PRICE_MIN;
  readonly maxPrice = PRICE_MAX;

  readonly filteredCards = computed(() => {
    const activeFilters = this.filters();

    return this.cards().filter((card) => {
      const matchesCategory =
        activeFilters.category === ALL_CATEGORIES ||
        card.category === activeFilters.category;
      const matchesPrice =
        card.basePrice >= activeFilters.minPrice &&
        card.basePrice <= activeFilters.maxPrice;
      const matchesDelivery =
        activeFilters.deliveryTime === 'any'
          ? true
          : activeFilters.deliveryTime === '24h'
            ? card.deliveryDays <= 1
            : activeFilters.deliveryTime === '3days'
              ? card.deliveryDays <= 3
              : card.deliveryDays <= 7;
      const matchesRating = card.rating >= activeFilters.minRating;

      return matchesCategory && matchesPrice && matchesDelivery && matchesRating;
    });
  });

  readonly availableServicesCount = computed(() => this.filteredCards().length);

  ngOnInit(): void {
    console.log('[Browse] ngOnInit — fetching gigs');

    this.gigService.getGigs().subscribe({
      next: (dtos) => {
        console.log('[Browse] raw DTOs from API:', dtos);

        const mapped = dtos.map(mapToCardItem);
        console.log('[Browse] mapped BrowseCardItems:', mapped);

        this.cards.set(mapped);
        this.isLoading.set(false);

        console.log('[Browse] isLoading:', this.isLoading());
        console.log('[Browse] cards count:', this.cards().length);
        console.log('[Browse] filteredCards count:', this.filteredCards().length);
      },
      error: (err) => {
        console.error('[Browse] API error:', err);
        this.error.set('Failed to load gigs. Please try again later.');
        this.isLoading.set(false);
      },
    });
  }

  updateFilters(nextFilters: BrowseFilterState): void {
    this.filters.set(nextFilters);
  }
}
