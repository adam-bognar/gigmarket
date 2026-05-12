import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { AuthService } from './auth.service';
import { environment } from '../../../environments/environment';
import { AuthUser } from '../models/auth.model';

const api = `${environment.apiUrl}/auth`;

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  let router: { navigate: ReturnType<typeof vi.fn> };

  const user: AuthUser = {
    id: 'user-1',
    customUsername: 'adam',
    email: 'adam@test.com',
    isSeller: true,
  };

  beforeEach(() => {
    router = { navigate: vi.fn() };

    TestBed.configureTestingModule({
      providers: [
        AuthService,
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: Router, useValue: router },
      ],
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should log in and store the current user', () => {
    service.login({ email: 'adam@test.com', password: 'Password123!' }).subscribe((result) => {
      expect(result).toEqual(user);
    });

    const req = httpMock.expectOne(`${api}/login`);

    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ email: 'adam@test.com', password: 'Password123!' });

    req.flush(user);

    expect(service.user()).toEqual(user);
    expect(service.isAuthenticated()).toBe(true);
  });

  it('should register and store the current user', () => {
    service
      .register({ customUsername: 'adam', email: 'adam@test.com', password: 'Password123!' })
      .subscribe((result) => {
        expect(result).toEqual(user);
      });

    const req = httpMock.expectOne(`${api}/register`);

    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({
      customUsername: 'adam',
      email: 'adam@test.com',
      password: 'Password123!',
    });

    req.flush(user);

    expect(service.user()).toEqual(user);
    expect(service.isAuthenticated()).toBe(true);
  });

  it('should load the current user from the API', () => {
    service.getMe().subscribe((result) => {
      expect(result).toEqual(user);
    });

    const req = httpMock.expectOne(`${api}/me`);

    expect(req.request.method).toBe('GET');

    req.flush(user);

    expect(service.user()).toEqual(user);
  });

  it('should clear user state when getMe fails', () => {
    service.login({ email: 'adam@test.com', password: 'Password123!' }).subscribe();
    httpMock.expectOne(`${api}/login`).flush(user);

    service.getMe().subscribe((result) => {
      expect(result).toBeNull();
    });

    const req = httpMock.expectOne(`${api}/me`);
    req.flush('Unauthorized', { status: 401, statusText: 'Unauthorized' });

    expect(service.user()).toBeNull();
    expect(service.isAuthenticated()).toBe(false);
  });

  it('should update account and replace the current user state', () => {
    const updatedUser: AuthUser = { ...user, customUsername: 'new-adam' };

    service.updateAccount('new-adam').subscribe((result) => {
      expect(result).toEqual(updatedUser);
    });

    const req = httpMock.expectOne(`${api}/account`);

    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual({ customUsername: 'new-adam' });

    req.flush(updatedUser);

    expect(service.user()).toEqual(updatedUser);
  });

  it('should log out, clear the current user, and navigate to login', () => {
    service.login({ email: 'adam@test.com', password: 'Password123!' }).subscribe();
    httpMock.expectOne(`${api}/login`).flush(user);

    service.logout().subscribe();

    const req = httpMock.expectOne(`${api}/logout`);

    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({});

    req.flush(null);

    expect(service.user()).toBeNull();
    expect(service.isAuthenticated()).toBe(false);
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });
});
