import { DecimalPipe, NgOptimizedImage } from '@angular/common';
import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import {LucideAngularModule, StarIcon} from 'lucide-angular';

export interface BrowseCardItem {
  id: string;
  coverImageSrc: string;
  coverImageAlt: string;
  sellerName: string;
  sellerAvatarUrl: string;
  sellerProfileId: string;
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
  imports: [NgOptimizedImage, DecimalPipe, RouterLink, LucideAngularModule],
  templateUrl: './card.html',
  styleUrl: './card.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Card {
  readonly item = input.required<BrowseCardItem>();
  protected readonly StarIcon = StarIcon;
}
