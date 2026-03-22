import {
  ChangeDetectionStrategy,
  Component,
  computed,
  input,
  signal,
} from '@angular/core';
import { DecimalPipe, DatePipe } from '@angular/common'
import {ReviewDto} from '../../../shared/models/gig.model';

export interface RatingBreakdownItem {
  star: number;
  count: number;
  pct: number;
}

@Component({
  selector: 'app-gig-reviews',
  imports: [],
  templateUrl: './review.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppGigReviews {
  reviews = input.required<ReviewDto[]>();
  averageRating = input.required<number>();
  totalReviews = input.required<number>();
  ratingBreakdown = input.required<RatingBreakdownItem[]>();

  visibleCount = signal(4);

  visibleReviews = computed(() => this.reviews().slice(0, this.visibleCount()));
  hasMore = computed(() => this.visibleCount() < this.reviews().length);

  loadMore() {
    this.visibleCount.update((n) => n + 4);
  }

  getStars(rating: number): number[] {
    return Array.from({ length: 5 }, (_, i) => i + 1);
  }

  isFilledStar(star: number, rating: number): boolean {
    return star <= Math.round(rating);
  }
}
