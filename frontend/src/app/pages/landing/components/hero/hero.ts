import { ChangeDetectionStrategy, Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { LucideAngularModule, Search } from 'lucide-angular';

@Component({
  selector: 'app-hero',
  imports: [FormsModule, LucideAngularModule],
  templateUrl: './hero.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Hero {
  readonly icons = { Search };
  searchQuery = '';

  readonly popularTags = ['Website Design', 'WordPress', 'Logo Design', 'Video Editing', 'SEO'];
  readonly stats = [
    { value: '3M+', label: 'Freelancers' },
    { value: '700K+', label: 'Active clients' },
    { value: '50M+', label: 'Jobs completed' },
    { value: '4.9★', label: 'Avg. rating' },
  ];

  constructor(private router: Router) {}

  onSearch(): void {
    const q = this.searchQuery.trim();
    this.router.navigate(['/browse'], q ? { queryParams: { q } } : {});
  }

  onTagClick(tag: string): void {
    this.router.navigate(['/browse'], { queryParams: { q: tag } });
  }
}
