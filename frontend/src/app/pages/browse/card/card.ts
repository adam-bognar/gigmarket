import { DecimalPipe, NgOptimizedImage } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

export interface BrowseCardItem {
  id: string;
  coverImageSrc: string;
  coverImageAlt: string;
  sellerName: string;
  sellerAvatarUrl: string;
  title: string;
  category: string;
  basePrice: number;
  deliveryDays: number;
  rating: number;
  reviewCount: number;
}

const DEFAULT_BROWSE_CARD: BrowseCardItem = {
  id: "",
  coverImageSrc: '/images/browse/web-design.svg',
  coverImageAlt: 'Preview card for a modern website design service',
  sellerName: 'Sarah Designs',
  sellerAvatarUrl: '',
  title: 'I will design a modern responsive website for your business',
  category: 'Graphics & Design',
  basePrice: 120,
  deliveryDays: 2,
  rating: 4.9,
  reviewCount: 1200,
};

@Component({
  selector: 'app-card',
  imports: [NgOptimizedImage, DecimalPipe],
  templateUrl: './card.html',
  styleUrl: './card.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Card {
  readonly item = input(DEFAULT_BROWSE_CARD);
}
