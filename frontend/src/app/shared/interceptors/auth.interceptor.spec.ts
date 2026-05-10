import { TestBed } from '@angular/core/testing';
import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { authInterceptor } from './auth.interceptor';
import { environment } from '../../../environments/environment';

describe('authInterceptor', () => {
  let http: HttpClient;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([authInterceptor])),
        provideHttpClientTesting(),
      ],
    });

    http = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should add withCredentials for API requests', () => {
    http.get(`${environment.apiUrl}/gigs`).subscribe();

    const req = httpMock.expectOne(`${environment.apiUrl}/gigs`);

    expect(req.request.withCredentials).toBe(true);

    req.flush([]);
  });

  it('should not add withCredentials for external requests', () => {
    http.get('https://example.com/test').subscribe();

    const req = httpMock.expectOne('https://example.com/test');

    expect(req.request.withCredentials).toBe(false);

    req.flush({});
  });
});
