import { DecimalPipe } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  computed,
  inject,
  OnInit,
  signal,
} from '@angular/core';
import { RouterLink } from '@angular/router';
import { Card, type BrowseCardItem } from './card/card';
import { DEFAULT_BROWSE_FILTERS, Filter, type BrowseFilterState } from './filter/filter';
import { GigService } from '../../shared/services/gig.service';
import { GigSummaryDto } from '../../shared/models/gig.model';

const PRICE_MIN = 0;
const PRICE_MAX = 300;

function mapToCardItem(dto: GigSummaryDto): BrowseCardItem {
  return {
    id: dto.id,
    coverImageSrc: dto.primaryPhotoUrl,
    coverImageAlt: dto.title,
    sellerName: `${dto.sellerFirstName} ${dto.sellerLastName}`.trim(),
    sellerAvatarUrl: dto.sellerAvatarUrl,
    title: dto.title,
    category: dto.categoryName,
    categoryId: dto.categoryId,
    basePrice: dto.startingPrice,
    deliveryDays: dto.minDeliveryDays,
    rating: dto.averageRating,
    reviewCount: dto.totalReviews,
  };
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
  readonly minPrice = PRICE_MIN;
  readonly maxPrice = PRICE_MAX;

  readonly filteredCards = computed(() => {
    const f = this.filters();
    return this.cards().filter(card => {
      const matchesCategory = f.categoryId === null || card.categoryId === f.categoryId;
      const matchesPrice = card.basePrice >= f.minPrice && card.basePrice <= f.maxPrice;
      const matchesDelivery =
        f.deliveryTime === 'any'   ? true :
          f.deliveryTime === '24h'   ? card.deliveryDays <= 1 :
            f.deliveryTime === '3days' ? card.deliveryDays <= 3 :
              card.deliveryDays <= 7;
      const matchesRating = card.rating >= f.minRating;

      return matchesCategory && matchesPrice && matchesDelivery && matchesRating;
    });
  });

  readonly availableServicesCount = computed(() => this.filteredCards().length);

  ngOnInit(): void {
    this.gigService.getGigs().subscribe({
      next: (dtos) => {
        this.cards.set(dtos.map(mapToCardItem));
        this.isLoading.set(false);
      },
      error: () => {
        this.error.set('Failed to load gigs. Please try again later.');
        this.isLoading.set(false);
      },
    });
  }

  updateFilters(nextFilters: BrowseFilterState): void {
    this.filters.set(nextFilters);
  }
}
