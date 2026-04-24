import { DecimalPipe } from '@angular/common';
import {
  ChangeDetectionStrategy,
  Component,
  computed, DestroyRef,
  inject,
  OnInit,
  signal,
} from '@angular/core';
import {ActivatedRoute, Params, Router, RouterLink} from '@angular/router';
import { Card, type BrowseCardItem } from './card/card';
import {
  DEFAULT_BROWSE_FILTERS,
  Filter,
  type BrowseFilterState,
  DeliveryTimeFilter,
  RatingFilter
} from './filter/filter';
import {GigFilterParams, GigService} from '../../shared/services/gig.service';
import { GigSummaryDto } from '../../shared/models/gig.model';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {switchMap, tap} from 'rxjs';

const PRICE_MIN = 0;
const PRICE_MAX = 1000;

function mapToCardItem(dto: GigSummaryDto): BrowseCardItem {
  return {
    id: dto.id,
    coverImageSrc: dto.primaryPhotoUrl,
    coverImageAlt: dto.title,
    sellerName: `${dto.sellerFirstName} ${dto.sellerLastName}`.trim(),
    sellerAvatarUrl: dto.sellerAvatarUrl,
    sellerProfileId: dto.sellerProfileId,
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
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  readonly cards = signal<BrowseCardItem[]>([]);
  readonly isLoading = signal(true);
  readonly error = signal<string | null>(null);
  readonly filters = signal<BrowseFilterState>(DEFAULT_BROWSE_FILTERS);
  readonly minPrice = PRICE_MIN;
  readonly maxPrice = PRICE_MAX;

  readonly availableServicesCount = computed(() => this.cards().length);

  ngOnInit(): void {
    this.route.queryParams.pipe(
      takeUntilDestroyed(this.destroyRef),
      tap(() => {
        this.isLoading.set(true);
        this.error.set(null);
      }),
      switchMap(params => {
        this.filters.set(this.paramsToFilters(params));
        return this.gigService.getGigs(this.paramsToApiFilters(params));
      }),
    ).subscribe({
      next: dtos => {
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
    const currentQ = this.route.snapshot.queryParams['q'];
    this.router.navigate([], {
      queryParams: {
        ...(currentQ ? { q: currentQ } : {}),
        categoryId: nextFilters.categoryId ?? null,
        minPrice: nextFilters.minPrice > PRICE_MIN ? nextFilters.minPrice : null,
        maxPrice: nextFilters.maxPrice < PRICE_MAX ? nextFilters.maxPrice : null,
        deliveryTime: nextFilters.deliveryTime !== 'any' ? nextFilters.deliveryTime : null,
        minRating: nextFilters.minRating > 0 ? nextFilters.minRating : null,
      },
      replaceUrl: true,
    });
  }

  private paramsToFilters(params: Params): BrowseFilterState {
    return {
      categoryId: params['categoryId'] ?? null,
      minPrice: params['minPrice'] != null ? Number(params['minPrice']) : PRICE_MIN,
      maxPrice: params['maxPrice'] != null ? Number(params['maxPrice']) : PRICE_MAX,
      deliveryTime: (params['deliveryTime'] as DeliveryTimeFilter) ?? 'any',
      minRating: params['minRating'] != null ? Number(params['minRating']) as RatingFilter : 0,
    };
  }

  private paramsToApiFilters(params: Params): GigFilterParams {
    return {
      search: params['q'] || undefined,
      categoryId: params['categoryId'] || undefined,
      minPrice: params['minPrice'] != null ? Number(params['minPrice']) : undefined,
      maxPrice: params['maxPrice'] != null ? Number(params['maxPrice']) : undefined,
      deliveryTime: params['deliveryTime'] || undefined,
      minRating: params['minRating'] != null ? Number(params['minRating']) : undefined,
    };
  }
}
