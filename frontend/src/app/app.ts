import {ChangeDetectionStrategy, Component, effect, inject, OnInit} from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { Navbar } from './shared/components/navbar/navbar';
import {AuthService} from './shared/services/auth.service';
import {ChatService} from './shared/services/chat.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Navbar],
  templateUrl: './app.html',
  styleUrl: './app.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class App implements OnInit {
  private readonly router = inject(Router);
  private authService = inject(AuthService);
  private readonly chatService = inject(ChatService);

  constructor() {
    effect(() => {
      if (this.authService.isAuthenticated()) {
        this.chatService.startConnection();
        this.chatService.loadUnreadCount().subscribe();
      } else {
        this.chatService.stopConnection();
      }
    });
  }

  ngOnInit(): void {
    this.authService.getMe().subscribe();
  }

  get showNavbar(): boolean {
    return !this.router.url.startsWith('/login') && !this.router.url.startsWith('/become');
  }
}
