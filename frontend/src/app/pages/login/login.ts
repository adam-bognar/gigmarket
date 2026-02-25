import { NgClass } from '@angular/common';
import { Component, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [FormsModule, RouterLink, NgClass],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  protected readonly activeTab = signal<'signin' | 'join'>('signin');
  protected readonly email = signal('');
  protected readonly password = signal('');
  protected readonly keepSignedIn = signal(false);
  protected readonly showPassword = signal(false);
  protected readonly firstName = signal('');
  protected readonly lastName = signal('');

  setTab(tab: 'signin' | 'join'): void {
    this.activeTab.set(tab);
  }

  togglePassword(): void {
    this.showPassword.update((show) => !show);
  }

  onSubmit() : void {
        console.log('Submit:', { tab: this.activeTab(), email: this.email() });

  }
}
