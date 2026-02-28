import { NgClass } from '@angular/common';
import {Component, inject, signal} from '@angular/core';
import {FormGroup, FormsModule} from '@angular/forms';
import {Router, RouterLink} from '@angular/router';
import {HttpErrorResponse} from '@angular/common/http';
import {AuthService} from '../../shared/services/auth.service';

@Component({
  selector: 'app-login',
  imports: [FormsModule, RouterLink, NgClass],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly activeTab = signal<'signin' | 'join'>('signin');
  protected readonly email = signal('');
  protected readonly password = signal('');
  protected readonly keepSignedIn = signal(false);
  protected readonly showPassword = signal(false);
  protected readonly firstName = signal('');
  protected readonly lastName = signal('');
  protected readonly loading = signal(false);
  protected readonly errorMessage = signal('');

  setTab(tab: 'signin' | 'join'): void {
    this.activeTab.set(tab);
    this.errorMessage.set('')
  }

  togglePassword(): void {
    this.showPassword.update((show) => !show);
  }

  onSubmit(): void {
    this.errorMessage.set('');
    this.loading.set(true);

    if(this.activeTab() === 'signin') {
      this.authService
        .login({ email: this.email(), password: this.password() })
        .subscribe({
          next: () => {
            this.router.navigate(['/']);
          },
          error: (err) => this.handleError(err),
        });
    } else {
      this.authService
        .register({
          email: this.email(),
          password: this.password(),
          firstName: this.firstName(),
          lastName: this.lastName(),
        })
        .subscribe({
          next: () => {
            this.router.navigate(['/']);
          },
          error: (err) => this.handleError(err),
        });
    }
  }

  private handleError(err: HttpErrorResponse): void{

    this.loading.set(false);

    if (err.status === 0) {
      this.errorMessage.set('Network error: Please check your internet connection.');
    } else if (err.status === 401) {
      this.errorMessage.set('Invalid credentials: Please check your email and password.');
    } else if (err.status === 409) {
      this.errorMessage.set('Email already in use: Please use a different email address.');
    } else if (typeof err.error === 'string') {
      this.errorMessage.set(`Error: ${err.error}`);
    } else if (err.error?.message) {
      this.errorMessage.set(`Error: ${err.error.message}`);
    } else {
      this.errorMessage.set('An unexpected error occurred. Please try again later.');
    }
  }
}
