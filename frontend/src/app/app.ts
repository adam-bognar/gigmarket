import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { Navbar } from './shared/components/navbar/navbar';
import {AuthService} from './shared/services/auth.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Navbar],
  templateUrl: './app.html',
  styleUrl: './app.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class App {
  private readonly router = inject(Router);
  private authService = inject(AuthService);

  ngOnInit(): void {
    this.authService.getMe().subscribe();
  }

  get showNavbar(): boolean {
    return !this.router.url.startsWith('/login');
  }
}
