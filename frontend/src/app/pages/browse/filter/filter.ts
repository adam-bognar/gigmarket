import { DecimalPipe } from '@angular/common';
import {ChangeDetectionStrategy, Component, inject, input, OnInit, output, signal} from '@angular/core';
import {GigCategoryDto} from '../../../shared/models/gig.model';
import {CategoriesService} from '../../../shared/services/categories.service';

export type DeliveryTimeFilter = 'any' | '24h' | '3days' | '7days';
export type RatingFilter = 0 | 4.5 | 4.8 | 4.9 | 5;

export interface BrowseFilterState {
  categoryId: string | null;
  minPrice: number;
  maxPrice: number;
  deliveryTime: DeliveryTimeFilter;
  minRating: RatingFilter;
}

export const DEFAULT_BROWSE_FILTERS: BrowseFilterState = {
  categoryId: null,
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
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Filter implements OnInit {
  //TODO adjust max price
  private readonly categoriesService = inject(CategoriesService);

  readonly state = input<BrowseFilterState>(DEFAULT_BROWSE_FILTERS);
  readonly categories = signal<GigCategoryDto[]>([]);
  readonly minAllowedPrice = input(0);
  readonly maxAllowedPrice = input(300);
  readonly stateChange = output<BrowseFilterState>();

  readonly deliveryTimeOptions = DELIVERY_TIME_OPTIONS;
  readonly ratingOptions = RATING_OPTIONS;

  ngOnInit() {
    this.categoriesService.getCategories().subscribe(cats => this.categories.set(cats));
  }

  selectCategory(categoryId: string | null): void {
    this.emitState({ ...this.state(), categoryId });
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
      categoryId: null,
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
