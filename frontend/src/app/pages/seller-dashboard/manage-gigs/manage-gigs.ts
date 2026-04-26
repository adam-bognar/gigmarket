import {ChangeDetectionStrategy, Component, computed, inject, OnInit, signal} from '@angular/core';
import {DecimalPipe} from '@angular/common';
import {Router} from '@angular/router';
import {CheckCircle, Eye, LucideAngularModule, Star} from 'lucide-angular';
import {GigService} from '../../../shared/services/gig.service';
import {GigSummaryDto} from '../../../shared/models/gig.model';

type GigFilter = 'all' | 'active';

@Component({
  selector: 'app-manage-gigs',
  imports: [DecimalPipe, LucideAngularModule],
  templateUrl: './manage-gigs.html',
  styleUrl: './manage-gigs.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ManageGigs implements OnInit {
  private readonly gigService = inject(GigService);
  private readonly router = inject(Router);

  protected readonly CheckCircleIcon = CheckCircle;
  protected readonly EyeIcon = Eye;
  protected readonly StarIcon = Star;

  gigs = signal<GigSummaryDto[]>([]);
  selectedFilter = signal<GigFilter>('all');
  isLoading = signal(true);
  error = signal<string | null>(null);

  filteredGigs = computed(() => {
    const list = this.gigs();
    if (this.selectedFilter() === 'active') {
      return list.filter((gig) => gig.status.toLowerCase() === 'active');
    }
    return list;
  });

  activeGigCount = computed(() => this.filteredGigs().length);

  totalImpressions = computed(() => {
    return this.gigs().reduce((sum, gig) => sum + gig.totalReviews, 0);
  });

  ngOnInit() {
    this.gigService.getMyGigs().subscribe({
      next: (gigs) => {
        this.gigs.set(gigs);
        this.isLoading.set(false);
      },
      error: () => {
        this.error.set('Unable to load gigs right now. Please try again.');
        this.isLoading.set(false);
      },
    });
  }

  setFilter(filter: GigFilter) {
    this.selectedFilter.set(filter);
  }

  onCreateGig() {
    this.router.navigate(['/create-gig']);
  }

  onEditGig(gigId: string) {
    this.router.navigate(['/create-gig', gigId]);
  }

  onDeleteGig(gigId: string) {
    const current = this.gigs();
    this.gigService.deleteGig(gigId).subscribe({
      next: () => {
        this.gigs.update((items) => items.filter((gig) => gig.id !== gigId));
      },
      error: () => {
        this.gigs.set(current);
        this.error.set('Failed to delete gig. Please try again.');
      },
    });
  }

  isActiveStatus(status: string) {
    return status.toLowerCase() === 'active';
  }

}
