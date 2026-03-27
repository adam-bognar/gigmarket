import { DecimalPipe, NgOptimizedImage } from '@angular/common';
import { ChangeDetectionStrategy, Component, input } from '@angular/core';

export interface BrowseCardItem {
  id: string;
  coverImageSrc: string;
  coverImageAlt: string;
  sellerName: string;
  sellerAvatarUrl: string;
  title: string;
  category: string;
  categoryId: string;
  basePrice: number;
  deliveryDays: number;
  rating: number;
  reviewCount: number;
}

@Component({
  selector: 'app-card',
  imports: [NgOptimizedImage, DecimalPipe],
  templateUrl: './card.html',
  styleUrl: './card.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Card {
  readonly item = input.required<BrowseCardItem>();
}
