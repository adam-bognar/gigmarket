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
import {GigFilterParams, GigService, GigSortBy} from '../../shared/services/gig.service';
import { GigSummaryDto } from '../../shared/models/gig.model';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {switchMap, tap} from 'rxjs';

const PRICE_MIN = 0;
const PRICE_MAX = 1000;

export interface SortOption {
  value: GigSortBy;
  label: string;
}

export const SORT_OPTIONS: ReadonlyArray<SortOption> = [
  { value: 'recommended',  label: 'Recommended' },
  { value: 'price_asc',    label: 'Price: Low to High' },
  { value: 'price_desc',   label: 'Price: High to Low' },
  { value: 'rating_desc',  label: 'Best Rated' },
  { value: 'reviews_desc', label: 'Most Reviews' },
];

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
  readonly searchQuery = signal<string>('');
  readonly sortBy = signal<GigSortBy>('recommended');
  readonly minPrice = PRICE_MIN;
  readonly maxPrice = PRICE_MAX;
  readonly sortOptions = SORT_OPTIONS;

  readonly availableServicesCount = computed(() => this.cards().length);
  readonly categoryLabel = computed(() =>
    this.filters().categoryId ? (this.cards()[0]?.category ?? null) : null
  );

  ngOnInit(): void {
    this.route.queryParams.pipe(
      takeUntilDestroyed(this.destroyRef),
      tap((params) => {
        this.isLoading.set(true);
        this.error.set(null);
        this.searchQuery.set(params['q'] ?? '');
        this.sortBy.set((params['sortBy'] as GigSortBy) ?? 'recommended');
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
    const currentSort = this.route.snapshot.queryParams['sortBy'];
    this.router.navigate([], {
      queryParams: {
        ...(currentQ ? { q: currentQ } : {}),
        categoryId: nextFilters.categoryId ?? null,
        minPrice: nextFilters.minPrice > PRICE_MIN ? nextFilters.minPrice : null,
        maxPrice: nextFilters.maxPrice < PRICE_MAX ? nextFilters.maxPrice : null,
        deliveryTime: nextFilters.deliveryTime !== 'any' ? nextFilters.deliveryTime : null,
        minRating: nextFilters.minRating > 0 ? nextFilters.minRating : null,
        sortBy: currentSort ?? null,
      },
      replaceUrl: true,
    });
  }

  updateSort(value: string): void {
    const sortBy = value as GigSortBy;
    this.router.navigate([], {
      queryParams: {
        ...this.route.snapshot.queryParams,
        sortBy: sortBy !== 'recommended' ? sortBy : null,
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
      sortBy: (params['sortBy'] as GigSortBy) || 'recommended',
    };
  }
}
