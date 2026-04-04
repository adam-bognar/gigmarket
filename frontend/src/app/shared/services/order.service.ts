import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { CheckoutRequest, CheckoutResponse, OrderDto } from '../models/order.model';

@Injectable({ providedIn: 'root' })
export class OrderService extends ApiService {

  createCheckoutSession(payload: CheckoutRequest): Observable<CheckoutResponse> {
    return this.http.post<CheckoutResponse>(`${this.base}/orders/checkout`, payload);
  }

  getMyOrders(): Observable<OrderDto[]> {
    return this.http.get<OrderDto[]>(`${this.base}/orders`);
  }
}
