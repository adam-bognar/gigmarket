import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import {
  DeliverOrderPayload,
  OrderDetailDto,
  OrderSummaryDto,
  RequestRevisionPayload,
} from '../models/order.model';

@Injectable({ providedIn: 'root' })
export class OrderService extends ApiService {

  getMyOrders(): Observable<OrderSummaryDto[]> {
    return this.http.get<OrderSummaryDto[]>(`${this.base}/orders`);
  }

  getOrderById(id: string): Observable<OrderDetailDto> {
    return this.http.get<OrderDetailDto>(`${this.base}/orders/${id}`);
  }

  deliver(orderId: string, payload: DeliverOrderPayload): Observable<void> {
    return this.http.post<void>(`${this.base}/orders/${orderId}/deliver`, payload);
  }

  requestRevision(orderId: string, payload: RequestRevisionPayload): Observable<void> {
    return this.http.post<void>(`${this.base}/orders/${orderId}/request-revision`, payload);
  }

  acceptDelivery(orderId: string): Observable<void> {
    return this.http.post<void>(`${this.base}/orders/${orderId}/accept`, {});
  }

  uploadDeliveryFile(orderId: string, file: File): Observable<{ url: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ url: string }>(
      `${this.base}/files/upload/order/${orderId}/delivery`,
      formData
    );
  }
}
