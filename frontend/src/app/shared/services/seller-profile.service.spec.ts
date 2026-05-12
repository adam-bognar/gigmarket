import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { SellerProfileService } from './seller-profile.service';
import { environment } from '../../../environments/environment';
import { CreateSellerProfilePayload, UpdateSellerProfilePayload } from '../models/seller.model';

const api = environment.apiUrl;

describe('SellerProfileService', () => {
  let service: SellerProfileService;
  let httpMock: HttpTestingController;

  const profilePayload: CreateSellerProfilePayload = {
    firstName: 'Adam',
    lastName: 'Test',
    profilePicUrl: 'avatar.jpg',
    description: 'I build fullstack web applications.',
    languageIds: ['lang-1'],
    occupation: {
      occupationName: 'Software Developer',
      occupationFromYear: 2023,
      occupationToYear: 2026,
    },
    skills: ['Angular', 'ASP.NET Core'],
    educations: null,
    certifications: null,
    personalWebsite: null,
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        SellerProfileService,
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: Router, useValue: { navigate: vi.fn() } },
      ],
    });

    service = TestBed.inject(SellerProfileService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should get selectable languages', () => {
    service.getLanguages().subscribe((result) => {
      expect(result).toEqual([{ id: 'lang-1', name: 'English' }]);
    });

    const req = httpMock.expectOne(`${api}/languages`);

    expect(req.request.method).toBe('GET');

    req.flush([{ id: 'lang-1', name: 'English' }]);
  });

  it('should create seller profile', () => {
    service.createProfile(profilePayload).subscribe((result) => {
      expect(result.id).toBe('seller-1');
    });

    const req = httpMock.expectOne(`${api}/seller/create`);

    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(profilePayload);

    req.flush({ id: 'seller-1' });
  });

  it('should get current seller profile', () => {
    service.getMyProfile().subscribe((result) => {
      expect(result.id).toBe('seller-1');
      expect(result.firstName).toBe('Adam');
    });

    const req = httpMock.expectOne(`${api}/seller/me`);

    expect(req.request.method).toBe('GET');

    req.flush({
      id: 'seller-1',
      userId: 'user-1',
      firstName: 'Adam',
      lastName: 'Test',
      description: 'Seller description',
      profileImageUrl: 'avatar.jpg',
      personalWebsite: null,
      occupation: { name: 'Software Developer', fromYear: 2023, toYear: 2026 },
      languages: [{ id: 'lang-1', name: 'English' }],
      skills: ['Angular'],
      educations: [],
      certifications: [],
      createdAtUtc: '2026-01-01T00:00:00Z',
    });
  });

  it('should update seller profile', () => {
    const updatePayload: UpdateSellerProfilePayload = {
      ...profilePayload,
      description: 'Updated seller description.',
    };

    service.updateProfile(updatePayload).subscribe();

    const req = httpMock.expectOne(`${api}/seller/update`);

    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual(updatePayload);

    req.flush(null);
  });

  it('should get public seller profile', () => {
    service.getPublicProfile('seller-1').subscribe((result) => {
      expect(result.id).toBe('seller-1');
      expect(result.totalReviews).toBe(3);
    });

    const req = httpMock.expectOne(`${api}/seller/seller-1/public`);

    expect(req.request.method).toBe('GET');

    req.flush({
      id: 'seller-1',
      userId: 'user-1',
      firstName: 'Adam',
      lastName: 'Test',
      description: 'Public seller description',
      profileImageUrl: 'avatar.jpg',
      personalWebsite: null,
      occupation: { name: 'Software Developer', fromYear: 2023, toYear: 2026 },
      languages: [],
      skills: [],
      educations: [],
      certifications: [],
      memberSinceUtc: '2026-01-01T00:00:00Z',
      averageRating: 4.5,
      totalReviews: 3,
      gigs: [],
      reviews: [],
    });
  });

  it('should request Stripe onboarding link', () => {
    service.connectStripe().subscribe((result) => {
      expect(result.onboardingUrl).toBe('https://stripe.test/onboarding');
      expect(result.status).toBe('Pending');
    });

    const req = httpMock.expectOne(`${api}/seller/connect`);

    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({});

    req.flush({ onboardingUrl: 'https://stripe.test/onboarding', status: 'Pending' });
  });

  it('should get Stripe dashboard link', () => {
    service.getStripeDashboardLink().subscribe((result) => {
      expect(result.url).toBe('https://stripe.test/dashboard');
    });

    const req = httpMock.expectOne(`${api}/seller/connect/dashboard`);

    expect(req.request.method).toBe('GET');

    req.flush({ url: 'https://stripe.test/dashboard' });
  });
});
