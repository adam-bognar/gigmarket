import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { GigService } from './gig.service';
import { environment } from '../../../environments/environment';
import { CreateGigPayload } from '../models/gig.model';

describe('GigService', () => {
  let service: GigService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        GigService,
        provideHttpClient(),
        provideHttpClientTesting(),
      ],
    });

    service = TestBed.inject(GigService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should request gigs without filters', () => {
    service.getGigs().subscribe((result) => {
      expect(result).toEqual([]);
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/gigs`);

    expect(req.request.method).toBe('GET');

    req.flush([]);
  });

  it('should include filter params when requesting gigs', () => {
    service
      .getGigs({
        search: 'logo',
        categoryId: 'cat-1',
        minPrice: 10,
        maxPrice: 100,
        deliveryTime: '3',
        minRating: 4,
        sortBy: 'price_asc',
      })
      .subscribe();

    const req = httpMock.expectOne((request) => request.url === `${environment.apiUrl}/gigs`);

    expect(req.request.params.get('search')).toBe('logo');
    expect(req.request.params.get('categoryId')).toBe('cat-1');
    expect(req.request.params.get('minPrice')).toBe('10');
    expect(req.request.params.get('maxPrice')).toBe('100');
    expect(req.request.params.get('deliveryTime')).toBe('3');
    expect(req.request.params.get('minRating')).toBe('4');
    expect(req.request.params.get('sortBy')).toBe('price_asc');

    req.flush([]);
  });

  it('should not send sortBy when recommended sorting is selected', () => {
    service.getGigs({ sortBy: 'recommended' }).subscribe();

    const req = httpMock.expectOne((request) => request.url === `${environment.apiUrl}/gigs`);

    expect(req.request.params.has('sortBy')).toBe(false);

    req.flush([]);
  });

  it('should create gig with correct payload', () => {
    const payload: CreateGigPayload = {
      gigId: 'gig-1',
      title: 'I will build a website',
      categoryId: 'cat-1',
      subcategoryId: 'sub-1',
      tags: ['angular', 'dotnet'],
      description: 'Detailed gig description',
      packages: [
        {
          tier: 'Basic',
          name: 'Basic',
          description: 'Basic package description',
          deliveryDays: 3,
          revisions: 1,
          price: 25,
        },
      ],
      requirements: null,
      primaryPhotoUrl: 'primary.jpg',
      additionalPhotoUrls: [],
      videoUrl: null,
    };

    service.createGig(payload).subscribe((result) => {
      expect(result.id).toBe('gig-1');
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/gigs`);

    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(payload);

    req.flush({
      id: 'gig-1',
      sellerProfileId: 'seller-1',
      title: payload.title,
      status: 'Active',
      createdAtUtc: '2026-01-01T00:00:00Z',
    });
  });

  it('should delete gig by id', () => {
    service.deleteGig('gig-1').subscribe();

    const req = httpMock.expectOne(`${environment.apiUrl}/gigs/gig-1`);

    expect(req.request.method).toBe('DELETE');

    req.flush(null);
  });
});
