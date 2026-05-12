import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { OrderService } from './order.service';
import { environment } from '../../../environments/environment';

const api = environment.apiUrl;

describe('OrderService', () => {
  let service: OrderService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [OrderService, provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(OrderService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should create a checkout session', () => {
    const payload = { gigId: 'gig-1', packageId: 'package-1' };

    service.createCheckoutSession(payload).subscribe((result) => {
      expect(result.sessionUrl).toBe('https://stripe.test/session');
    });

    const req = httpMock.expectOne(`${api}/orders/checkout`);

    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(payload);

    req.flush({ sessionUrl: 'https://stripe.test/session' });
  });

  it('should get buyer orders', () => {
    service.getMyOrders().subscribe((result) => {
      expect(result.length).toBe(1);
      expect(result[0].id).toBe('order-1');
    });

    const req = httpMock.expectOne(`${api}/orders`);

    expect(req.request.method).toBe('GET');

    req.flush([
      {
        id: 'order-1',
        gigId: 'gig-1',
        gigTitle: 'Test gig',
        gigPrimaryPhotoUrl: 'photo.jpg',
        packageId: 'package-1',
        packageName: 'Basic',
        packageTier: 'Basic',
        deliveryDays: 3,
        totalPrice: 25,
        status: 'InProgress',
        createdAtUtc: '2026-01-01T00:00:00Z',
        paidAtUtc: null,
      },
    ]);
  });

  it('should get seller orders', () => {
    service.getSellerOrders().subscribe();

    const req = httpMock.expectOne(`${api}/orders/seller`);

    expect(req.request.method).toBe('GET');

    req.flush([]);
  });

  it('should get order by id', () => {
    service.getOrderById('order-1').subscribe((result) => {
      expect(result.id).toBe('order-1');
      expect(result.status).toBe('Delivered');
    });

    const req = httpMock.expectOne(`${api}/orders/order-1`);

    expect(req.request.method).toBe('GET');

    req.flush({
      id: 'order-1',
      gigId: 'gig-1',
      gigTitle: 'Test gig',
      gigPrimaryPhotoUrl: 'photo.jpg',
      packageId: 'package-1',
      packageName: 'Basic',
      packageTier: 'Basic',
      deliveryDays: 3,
      revisionsAllowed: 2,
      revisionsUsed: 0,
      totalPrice: 25,
      status: 'Delivered',
      createdAtUtc: '2026-01-01T00:00:00Z',
      paidAtUtc: null,
      deadlineUtc: null,
      buyerUserId: 'buyer-1',
      buyerUsername: 'buyer',
      sellerUserId: 'seller-user-1',
      sellerProfileId: 'seller-1',
      sellerFirstName: 'Seller',
      sellerLastName: 'Test',
      sellerAvatarUrl: 'avatar.jpg',
      deliveries: [],
      revisionRequests: [],
    });
  });

  it('should deliver an order', () => {
    const payload = { message: 'Here is the final delivery.', fileUrls: ['file.pdf'] };

    service.deliver('order-1', payload).subscribe();

    const req = httpMock.expectOne(`${api}/orders/order-1/deliver`);

    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(payload);

    req.flush(null);
  });

  it('should request a revision', () => {
    const payload = { message: 'Please change the color.' };

    service.requestRevision('order-1', payload).subscribe();

    const req = httpMock.expectOne(`${api}/orders/order-1/request-revision`);

    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(payload);

    req.flush(null);
  });
});
