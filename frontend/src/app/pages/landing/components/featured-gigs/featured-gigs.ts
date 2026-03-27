import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LucideAngularModule, Star, Clock } from 'lucide-angular';
import { DecimalPipe } from '@angular/common';
import { GigService } from '../../../../shared/services/gig.service';
import { GigSummaryDto } from '../../../../shared/models/gig.model';

const FEATURED_COUNT = 4;

@Component({
  selector: 'app-featured-gigs',
  imports: [RouterLink, LucideAngularModule, DecimalPipe],
  templateUrl: './featured-gigs.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FeaturedGigs implements OnInit {
  private readonly gigService = inject(GigService);

  readonly icons = { Star, Clock };
  readonly gigs = signal<GigSummaryDto[]>([]);
  readonly isLoading = signal(true);

  ngOnInit(): void {
    this.gigService.getGigs().subscribe({
      next: (all) => {
        this.gigs.set(all.slice(0, FEATURED_COUNT));
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false),
    });
  }

  sellerName(gig: GigSummaryDto): string {
    return `${gig.sellerFirstName} ${gig.sellerLastName}`.trim();
  }

  sellerInitial(gig: GigSummaryDto): string {
    return (gig.sellerFirstName?.[0] ?? '?').toUpperCase();
  }
}
