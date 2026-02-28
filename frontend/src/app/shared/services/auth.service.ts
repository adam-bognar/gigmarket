import {HttpClient} from "@angular/common/http";
import {computed, inject, Injectable, signal} from '@angular/core';
import {Router} from '@angular/router';
import {environment} from '../../../environments/environment';
import {AuthUser, LoginRequest, RegisterRequest} from '../models/auth.model';
import {Observable, tap} from 'rxjs';

@Injectable({providedIn: 'root'})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly baseUrl = `${environment.apiUrl}/auth`;

  private readonly currentUser = signal<AuthUser | null>(null);

  readonly user = this.currentUser.asReadonly();
  readonly isAuthenticated = computed(() => this.currentUser() !== null);

  login(request: LoginRequest): Observable<AuthUser> {
    return this.http
      .post<AuthUser>(`${this.baseUrl}/login`, request)
      .pipe(tap({ next: (user) => this.currentUser.set(user) }));
  }

  register(request: RegisterRequest): Observable<AuthUser> {
    return this.http
      .post<AuthUser>(`${this.baseUrl}/register`, request)
      .pipe(tap({ next: (user) => this.currentUser.set(user) }));
  }

  getMe(): Observable<AuthUser> {
    return this.http
      .get<AuthUser>(`${this.baseUrl}/me`)
      .pipe(tap({ next: (user) => this.currentUser.set(user) }));
  }

  logout(): Observable<void> {
    return this.http
      .post<void>(`${this.baseUrl}/logout`, {})
      .pipe(
        tap({
          next: () => {
            this.currentUser.set(null);
            this.router.navigate(['/login']);
          },
        }),
      );
  }
}
