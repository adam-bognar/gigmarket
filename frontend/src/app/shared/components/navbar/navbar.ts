import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-navbar',
  imports: [RouterLink, FormsModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Navbar {
  private readonly authService = inject(AuthService);

  protected readonly searchQuery = signal('');
  protected readonly mobileMenuOpen = signal(false);
  protected readonly profileMenuOpen = signal(false);

  protected readonly user = this.authService.user;
  protected readonly isAuthenticated = this.authService.isAuthenticated;
  protected readonly isSeller = computed(() => this.user()?.isSeller ?? false);
  protected readonly userInitials = computed(() => {
    const u = this.user();
    if (!u) return '';
    const first = u.firstName?.charAt(0) ?? '';
    const last = u.lastName?.charAt(0) ?? '';
    return `${first}${last}`.toUpperCase();
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
