import {ChangeDetectionStrategy, Component, computed, effect, inject, PLATFORM_ID, signal} from '@angular/core';
import { NavigationEnd, Router, RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { filter, map } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import {ChatService} from '../../services/chat.service';
import {MessageSquare, LucideAngularModule, Clock, Tag, Search, X, Bell, Menu} from 'lucide-angular';
import {CategoriesService} from '../../services/categories.service';
import {GigCategoryDto} from '../../models/gig.model';
import {isPlatformBrowser} from '@angular/common';

const RECENT_KEY = 'gm_recent_searches';
const MAX_RECENT = 5;

export interface RecentSuggestion { kind: 'recent'; label: string; }
export interface CategorySuggestion { kind: 'category'; label: string; id: string; }
export interface ActionSuggestion { kind: 'action'; label: string; }
export type Suggestion = RecentSuggestion | CategorySuggestion | ActionSuggestion;

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, FormsModule, LucideAngularModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Navbar {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly chatService = inject(ChatService);
  private readonly categoriesService = inject(CategoriesService);
  private readonly platformId = inject(PLATFORM_ID);

  protected readonly MessageSquareIcon = MessageSquare;
  protected readonly ClockIcon = Clock;
  protected readonly TagIcon = Tag;
  protected readonly SearchIcon = Search;
  protected readonly XIcon = X;
  protected readonly BellIcon = Bell;
  protected readonly MenuIcon = Menu;

  protected readonly unreadCount = this.chatService.unreadCount;

  private readonly currentUrl = toSignal(
    this.router.events.pipe(
      filter((e): e is NavigationEnd => e instanceof NavigationEnd),
      map((e) => e.urlAfterRedirects),
    ),
    { initialValue: this.router.url },
  );

  protected readonly searchQuery = signal('');
  protected readonly mobileMenuOpen = signal(false);
  protected readonly profileMenuOpen = signal(false);
  protected readonly dropdownOpen = signal(false);
  protected readonly highlightedIndex = signal(-1);

  protected readonly categories = signal<GigCategoryDto[]>([]);
  protected readonly recentSearches = signal<string[]>(this.loadRecent());

  protected readonly user = this.authService.user;
  protected readonly isAuthenticated = this.authService.isAuthenticated;
  protected readonly isSeller = computed(() => this.user()?.isSeller ?? false);
  protected readonly isDashboard = computed(() => this.currentUrl().includes('/dashboard'));
  protected readonly userInitials = computed(() => {
    const u = this.user();
    if (!u) return '';
    return u.customUsername?.charAt(0).toUpperCase() ?? '';
  });

  protected readonly recentSuggestions = computed<RecentSuggestion[]>(() => {
    const q = this.searchQuery().trim().toLowerCase();
    const recents = this.recentSearches();
    const filtered = q ? recents.filter(r => r.toLowerCase().includes(q)) : recents;
    return filtered.slice(0, MAX_RECENT).map(label => ({ kind: 'recent' as const, label }));
  });

  protected readonly categorySuggestions = computed<CategorySuggestion[]>(() => {
    const q = this.searchQuery().trim().toLowerCase();
    if (!q) return [];
    return this.categories()
      .filter(c => c.name.toLowerCase().includes(q))
      .slice(0, 4)
      .map(c => ({ kind: 'category' as const, label: c.name, id: c.id }));
  });

  protected readonly actionQuery = computed(() => this.searchQuery().trim());

  protected readonly allSuggestions = computed<Suggestion[]>(() => {
    const result: Suggestion[] = [
      ...this.recentSuggestions(),
      ...this.categorySuggestions(),
    ];
    if (this.actionQuery()) {
      result.push({ kind: 'action', label: this.actionQuery() });
    }
    return result;
  });

  protected readonly categoryOffset = computed(() => this.recentSuggestions().length);
  protected readonly actionIndex = computed(
    () => this.recentSuggestions().length + this.categorySuggestions().length,
  );

  protected readonly showDropdown = computed(
    () => this.dropdownOpen() && this.allSuggestions().length > 0,
  );

  constructor() {
    effect(() => {
      const url = this.currentUrl();
      if (url.startsWith('/browse')) {
        const tree = this.router.parseUrl(url);
        const q = (tree.queryParams['q'] as string) ?? '';
        this.searchQuery.set(q);
      }
    });

    if (isPlatformBrowser(this.platformId)) {
      this.categoriesService.getCategories().subscribe({
        next: cats => this.categories.set(cats),
      });
    }
  }

  onInputFocus(): void {
    this.highlightedIndex.set(-1);
    this.dropdownOpen.set(true);
  }

  onInputBlur(): void {
    setTimeout(() => this.dropdownOpen.set(false), 150);
  }

  onKeydown(event: KeyboardEvent): void {
    if (!this.showDropdown()) return;
    const len = this.allSuggestions().length;

    switch (event.key) {
      case 'ArrowDown':
        event.preventDefault();
        this.highlightedIndex.update(i => (i + 1) % len);
        break;
      case 'ArrowUp':
        event.preventDefault();
        this.highlightedIndex.update(i => (i - 1 + len) % len);
        break;
      case 'Escape':
        this.dropdownOpen.set(false);
        this.highlightedIndex.set(-1);
        break;
      case 'Enter': {
        const idx = this.highlightedIndex();
        if (idx >= 0) {
          event.preventDefault();
          this.selectSuggestion(this.allSuggestions()[idx]);
        }
        break;
      }
    }
  }

  selectSuggestion(s: Suggestion): void {
    this.dropdownOpen.set(false);
    this.highlightedIndex.set(-1);
    if (s.kind === 'recent' || s.kind === 'action') {
      this.searchQuery.set(s.label);
      this.navigateSearch(s.label);
    } else {
      this.navigateCategory(s.id);
    }
  }

  removeRecentSearch(label: string, event: MouseEvent): void {
    event.stopPropagation();
    event.preventDefault();
    const updated = this.recentSearches().filter(r => r !== label);
    this.recentSearches.set(updated);
    this.saveRecent(updated);
  }

  toggleMobileMenu(): void {
    this.mobileMenuOpen.update((open) => !open);
  }

  closeMobileMenu(): void {
    this.mobileMenuOpen.set(false);
  }

  toggleProfileMenu(): void {
    this.profileMenuOpen.update((open) => !open);
  }

  closeProfileMenu(): void {
    this.profileMenuOpen.set(false);
  }

  onSearch(): void {
    const q = this.searchQuery().trim();
    this.dropdownOpen.set(false);
    this.closeMobileMenu();
    if (!q) return;
    this.navigateSearch(q);
  }

  private navigateSearch(q: string): void {
    this.saveSearchTerm(q);
    const existing = this.router.url.startsWith('/browse')
      ? { ...this.router.parseUrl(this.router.url).queryParams }
      : {};
    this.router.navigate(['/browse'], {
      queryParams: { ...existing, q },
    });
  }

  private navigateCategory(categoryId: string): void {
    this.router.navigate(['/browse'], { queryParams: { categoryId } });
  }

  private loadRecent(): string[] {
    if (typeof localStorage === 'undefined') return [];
    try {
      return JSON.parse(localStorage.getItem(RECENT_KEY) ?? '[]') as string[];
    } catch {
      return [];
    }
  }

  private saveSearchTerm(term: string): void {
    const existing = this.recentSearches();
    const updated = [term, ...existing.filter(r => r !== term)].slice(0, MAX_RECENT);
    this.recentSearches.set(updated);
    this.saveRecent(updated);
  }

  private saveRecent(list: string[]): void {
    if (typeof localStorage === 'undefined') return;
    localStorage.setItem(RECENT_KEY, JSON.stringify(list));
  }

  onLogout(): void {
    this.closeProfileMenu();
    this.closeMobileMenu();
    this.authService.logout().subscribe();
  }
}
