import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';

export const ALL_CATEGORIES = 'All categories';

export const BROWSE_CATEGORY_OPTIONS = [
  'Graphics & Design',
  'Digital Marketing',
  'Writing & Translation',
  'Video & Animation',
  'Music & Audio',
  'Programming & Tech',
] as const;

export type BrowseCategory = (typeof BROWSE_CATEGORY_OPTIONS)[number];
export type BrowseCategorySelection = BrowseCategory | typeof ALL_CATEGORIES;
export type DeliveryTimeFilter = 'any' | '24h' | '3days' | '7days';
export type RatingFilter = 0 | 4.5 | 4.8 | 4.9 | 5;

export interface BrowseFilterState {
  category: BrowseCategorySelection;
  minPrice: number;
  maxPrice: number;
  deliveryTime: DeliveryTimeFilter;
  minRating: RatingFilter;
}

export const DEFAULT_BROWSE_FILTERS: BrowseFilterState = {
  category: ALL_CATEGORIES,
  minPrice: 0,
  maxPrice: 300,
  deliveryTime: 'any',
  minRating: 0,
};

const DELIVERY_TIME_OPTIONS: ReadonlyArray<{ value: DeliveryTimeFilter; label: string }> = [
  { value: '24h', label: 'Express 24H' },
  { value: '3days', label: 'Up to 3 days' },
  { value: '7days', label: 'Up to 7 days' },
  { value: 'any', label: 'Anytime' },
];

const RATING_OPTIONS: ReadonlyArray<{ value: RatingFilter; label: string }> = [
  { value: 0, label: 'All ratings' },
  { value: 4.5, label: '4.5 & up' },
  { value: 4.8, label: '4.8 & up' },
  { value: 4.9, label: '4.9 & up' },
  { value: 5, label: '5.0 only' },
];

@Component({
  selector: 'app-filter',
  imports: [DecimalPipe],
  templateUrl: './filter.html',
  host: {
    class: 'block',
  },
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Filter {
  readonly state = input<BrowseFilterState>(DEFAULT_BROWSE_FILTERS);
  readonly categoryOptions = input<readonly BrowseCategory[]>(BROWSE_CATEGORY_OPTIONS);
  readonly minAllowedPrice = input(0);
  readonly maxAllowedPrice = input(300);
  readonly stateChange = output<BrowseFilterState>();

  readonly allCategories = ALL_CATEGORIES;
  readonly deliveryTimeOptions = DELIVERY_TIME_OPTIONS;
  readonly ratingOptions = RATING_OPTIONS;

  readonly priceFillStart = computed(() => {
    const range = this.maxAllowedPrice() - this.minAllowedPrice();

    if (range <= 0) {
      return 0;
    }

    return ((this.state().minPrice - this.minAllowedPrice()) / range) * 100;
  });

  readonly priceFillWidth = computed(() => {
    const range = this.maxAllowedPrice() - this.minAllowedPrice();

    if (range <= 0) {
      return 100;
    }

    return ((this.state().maxPrice - this.state().minPrice) / range) * 100;
  });

  selectCategory(category: BrowseCategorySelection): void {
    this.emitState({
      ...this.state(),
      category,
    });
  }

  updateMinPrice(event: Event): void {
    const rawValue = this.getInputValue(event);
    const minPrice = this.clampPrice(rawValue);

    this.emitState({
      ...this.state(),
      minPrice,
      maxPrice: Math.max(minPrice, this.state().maxPrice),
    });
  }

  updateMaxPrice(event: Event): void {
    const rawValue = this.getInputValue(event);
    const maxPrice = this.clampPrice(rawValue);

    this.emitState({
      ...this.state(),
      minPrice: Math.min(this.state().minPrice, maxPrice),
      maxPrice,
    });
  }

  selectDeliveryTime(deliveryTime: DeliveryTimeFilter): void {
    this.emitState({
      ...this.state(),
      deliveryTime,
    });
  }

  selectRating(minRating: RatingFilter): void {
    this.emitState({
      ...this.state(),
      minRating,
    });
  }

  resetFilters(): void {
    this.emitState({
      category: ALL_CATEGORIES,
      minPrice: this.minAllowedPrice(),
      maxPrice: this.maxAllowedPrice(),
      deliveryTime: 'any',
      minRating: 0,
    });
  }

  private clampPrice(rawValue: string): number {
    const parsed = Number(rawValue);
    const fallback = this.minAllowedPrice();
    const safeValue = Number.isFinite(parsed) ? parsed : fallback;

    return Math.min(this.maxAllowedPrice(), Math.max(this.minAllowedPrice(), Math.round(safeValue)));
  }

  private emitState(nextState: BrowseFilterState): void {
    this.stateChange.emit(nextState);
  }

  private getInputValue(event: Event): string {
    const target = event.target;

    return target instanceof HTMLInputElement ? target.value : `${this.minAllowedPrice()}`;
  }

}
