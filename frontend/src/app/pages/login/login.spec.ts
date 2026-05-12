import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { of, throwError } from 'rxjs';
import { Login } from './login';
import { AuthService } from '../../shared/services/auth.service';
import { AuthUser } from '../../shared/models/auth.model';

describe('Login', () => {
  let component: Login;
  let fixture: ComponentFixture<Login>;
  let authService: {
    login: ReturnType<typeof vi.fn>;
    register: ReturnType<typeof vi.fn>;
  };
  let router: Router;

  const user: AuthUser = {
    id: 'user-1',
    customUsername: 'adam',
    email: 'adam@test.com',
  };

  beforeEach(async () => {
    authService = {
      login: vi.fn().mockReturnValue(of(user)),
      register: vi.fn().mockReturnValue(of(user)),
    };

    await TestBed.configureTestingModule({
      imports: [Login],
      providers: [
        provideRouter([]),
        { provide: AuthService, useValue: authService },
      ],
    }).compileComponents();

    router = TestBed.inject(Router);
    vi.spyOn(router, 'navigate').mockResolvedValue(true);

    fixture = TestBed.createComponent(Login);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should start on sign in tab', () => {
    expect((component as any).activeTab()).toBe('signin');
  });

  it('should switch tab and clear previous error message', () => {
    (component as any).errorMessage.set('Previous error');

    component.setTab('join');

    expect((component as any).activeTab()).toBe('join');
    expect((component as any).errorMessage()).toBe('');
  });

  it('should toggle password visibility', () => {
    expect((component as any).showPassword()).toBe(false);

    component.togglePassword();

    expect((component as any).showPassword()).toBe(true);
  });

  it('should call login and navigate home in sign in mode', () => {
    (component as any).email.set('adam@test.com');
    (component as any).password.set('Password123!');

    component.onSubmit();

    expect(authService.login).toHaveBeenCalledWith({
      email: 'adam@test.com',
      password: 'Password123!',
    });
    expect(authService.register).not.toHaveBeenCalled();
    expect(router.navigate).toHaveBeenCalledWith(['/']);
  });

  it('should call register and navigate home in join mode', () => {
    component.setTab('join');
    (component as any).email.set('adam@test.com');
    (component as any).password.set('Password123!');
    (component as any).customUsername.set('adam');

    component.onSubmit();

    expect(authService.register).toHaveBeenCalledWith({
      customUsername: 'adam',
      email: 'adam@test.com',
      password: 'Password123!',
    });
    expect(authService.login).not.toHaveBeenCalled();
    expect(router.navigate).toHaveBeenCalledWith(['/']);
  });

  it('should show invalid credentials message on 401 login error', () => {
    authService.login.mockReturnValue(
      throwError(() => new HttpErrorResponse({ status: 401, statusText: 'Unauthorized' }))
    );

    component.onSubmit();

    expect((component as any).loading()).toBe(false);
    expect((component as any).errorMessage()).toBe(
      'Invalid credentials: Please check your email and password.'
    );
    expect(router.navigate).not.toHaveBeenCalled();
  });

  it('should show email already in use message on 409 register error', () => {
    authService.register.mockReturnValue(
      throwError(() => new HttpErrorResponse({ status: 409, statusText: 'Conflict' }))
    );
    component.setTab('join');

    component.onSubmit();

    expect((component as any).loading()).toBe(false);
    expect((component as any).errorMessage()).toBe(
      'Email already in use: Please use a different email address.'
    );
    expect(router.navigate).not.toHaveBeenCalled();
  });

  it('should show network error message when backend is unreachable', () => {
    authService.login.mockReturnValue(
      throwError(() => new HttpErrorResponse({ status: 0, statusText: 'Unknown Error' }))
    );

    component.onSubmit();

    expect((component as any).errorMessage()).toBe(
      'Network error: Please check your internet connection.'
    );
  });

  it('should show backend string error message when available', () => {
    authService.login.mockReturnValue(
      throwError(() => new HttpErrorResponse({ status: 400, error: 'Invalid request' }))
    );

    component.onSubmit();

    expect((component as any).errorMessage()).toBe('Error: Invalid request');
  });

  it('should show backend object message when available', () => {
    authService.login.mockReturnValue(
      throwError(() => new HttpErrorResponse({ status: 400, error: { message: 'Password is invalid' } }))
    );

    component.onSubmit();

    expect((component as any).errorMessage()).toBe('Error: Password is invalid');
  });
});
