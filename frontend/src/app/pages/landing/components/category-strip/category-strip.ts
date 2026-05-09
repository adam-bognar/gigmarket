import { ChangeDetectionStrategy, Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LucideAngularModule, Palette, Code2, PenLine, TrendingUp, Video, Music, Globe, Camera, LucideIconData } from 'lucide-angular';
import { CategoriesService } from '../../../../shared/services/categories.service';
import { GigCategoryDto } from '../../../../shared/models/gig.model';

interface CategoryDisplay extends GigCategoryDto {
  icon: LucideIconData | null;
  initials: string | null;
  colorClass: string;
  bgClass: string;
}

const ICON_MAP: { keyword: string; icon: LucideIconData; colorClass: string; bgClass: string }[] = [
  { keyword: 'design',     icon: Palette,    colorClass: 'text-amber-500',   bgClass: 'bg-amber-50' },
  { keyword: 'graphic',    icon: Palette,    colorClass: 'text-amber-500',   bgClass: 'bg-amber-50' },
  { keyword: 'program',    icon: Code2,      colorClass: 'text-blue-500',    bgClass: 'bg-blue-50' },
  { keyword: 'tech',       icon: Code2,      colorClass: 'text-blue-500',    bgClass: 'bg-blue-50' },
  { keyword: 'writing',    icon: PenLine,    colorClass: 'text-violet-500',  bgClass: 'bg-violet-50' },
  { keyword: 'translat',   icon: Globe,      colorClass: 'text-violet-500',  bgClass: 'bg-violet-50' },
  { keyword: 'marketing',  icon: TrendingUp, colorClass: 'text-green-500',   bgClass: 'bg-green-50' },
  { keyword: 'video',      icon: Video,      colorClass: 'text-red-500',     bgClass: 'bg-red-50' },
  { keyword: 'animation',  icon: Video,      colorClass: 'text-red-500',     bgClass: 'bg-red-50' },
  { keyword: 'music',      icon: Music,      colorClass: 'text-teal-500',    bgClass: 'bg-teal-50' },
  { keyword: 'audio',      icon: Music,      colorClass: 'text-teal-500',    bgClass: 'bg-teal-50' },
  { keyword: 'photo',      icon: Camera,     colorClass: 'text-pink-500',    bgClass: 'bg-pink-50' },
];

const FALLBACK = { icon: Globe, colorClass: 'text-primary', bgClass: 'bg-primary/10' };

const FALLBACK_STYLES = [
  { colorClass: 'text-blue-600', bgClass: 'bg-blue-50' },
  { colorClass: 'text-violet-600', bgClass: 'bg-violet-50' },
  { colorClass: 'text-emerald-600', bgClass: 'bg-emerald-50' },
  { colorClass: 'text-amber-600', bgClass: 'bg-amber-50' },
  { colorClass: 'text-pink-600', bgClass: 'bg-pink-50' },
  { colorClass: 'text-cyan-600', bgClass: 'bg-cyan-50' },
];

function getInitials(name: string): string {
  return name
    .split(' ')
    .filter(Boolean)
    .slice(0, 2)
    .map(word => word[0])
    .join('')
    .toUpperCase();
}

function getFallbackStyle(name: string) {
  const index = name.length % FALLBACK_STYLES.length;

  return {
    icon: null,
    initials: getInitials(name),
    ...FALLBACK_STYLES[index],
  };
}

function resolveIcon(name: string) {
  const lower = name.toLowerCase();
  const match = ICON_MAP.find(m => lower.includes(m.keyword));

  if (match) {
    return {
      ...match,
      initials: null,
    };
  }

  return getFallbackStyle(name);
}

@Component({
  selector: 'app-category-strip',
  imports: [RouterLink, LucideAngularModule],
  templateUrl: './category-strip.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CategoryStrip implements OnInit {
  private readonly categoriesService = inject(CategoriesService);

  readonly categories = signal<CategoryDisplay[]>([]);
  readonly isLoading = signal(true);

  ngOnInit(): void {
    this.categoriesService.getCategories().subscribe({
      next: (cats) => {
        this.categories.set(cats.map(c => ({ ...c, ...resolveIcon(c.name) })));
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false),
    });
  }
}
