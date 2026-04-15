import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { NavigationEnd, Router, RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { filter, map } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import {ChatService} from '../../services/chat.service';
import {MessageSquare, LucideAngularModule} from 'lucide-angular';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, FormsModule, LucideAngularModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Navbar {
  //TODO implement search
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly chatService = inject(ChatService);
  protected readonly MessageSquareIcon = MessageSquare;
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

  protected readonly user = this.authService.user;
  protected readonly isAuthenticated = this.authService.isAuthenticated;
  protected readonly isSeller = computed(() => this.user()?.isSeller ?? false);
  protected readonly isDashboard = computed(() => this.currentUrl().includes('/dashboard'));
  protected readonly userInitials = computed(() => {
    const u = this.user();
    if (!u) return '';
    return u.customUsername?.charAt(0).toUpperCase() ?? '';
  });

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
    console.log('Search:', this.searchQuery());
    this.closeMobileMenu();
  }

  onLogout(): void {
    this.closeProfileMenu();
    this.closeMobileMenu();
    this.authService.logout().subscribe();
  }
}
