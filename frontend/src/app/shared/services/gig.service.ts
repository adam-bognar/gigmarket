import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {CreateGigPayload, GigDetailDto, GigDto, GigSummaryDto, UpdateGigPayload} from '../models/gig.model';
import {ApiService} from './api.service';
import {HttpParams} from '@angular/common/http';

export type GigSortBy = 'recommended' | 'price_asc' | 'price_desc' | 'rating_desc' | 'reviews_desc';

export interface GigFilterParams {
  search?: string;
  categoryId?: string;
  minPrice?: number;
  maxPrice?: number;
  deliveryTime?: string;
  minRating?: number;
  sortBy?: GigSortBy;
}

@Injectable({ providedIn: 'root' })
export class GigService extends ApiService {

  uploadGigPhoto(gigId: string, file: File): Observable<{ blobPath: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ blobPath: string }>(
      `${this.base}/files/upload/gig/${gigId}`,
      formData);
  }

  createGig(payload: CreateGigPayload): Observable<GigDto> {
    return this.http.post<GigDto>(
      `${this.base}/gigs`, payload);
  }

  updateGig(id: string, payload: UpdateGigPayload): Observable<GigDto> {
    return this.http.put<GigDto>(`${this.base}/gigs/${id}`, payload);
  }

  getGigs(filters?: GigFilterParams): Observable<GigSummaryDto[]> {
    let params = new HttpParams();
    if (filters?.search) params = params.set('search', filters.search);
    if (filters?.categoryId) params = params.set('categoryId', filters.categoryId);
    if (filters?.minPrice != null) params = params.set('minPrice', filters.minPrice);
    if (filters?.maxPrice != null) params = params.set('maxPrice', filters.maxPrice);
    if (filters?.deliveryTime) params = params.set('deliveryTime', filters.deliveryTime);
    if (filters?.minRating) params = params.set('minRating', filters.minRating);
    if (filters?.sortBy && filters.sortBy !== 'recommended') params = params.set('sortBy', filters.sortBy);
    return this.http.get<GigSummaryDto[]>(`${this.base}/gigs`, {params});
  }

  getMyGigs(): Observable<GigSummaryDto[]> {
    return this.http.get<GigSummaryDto[]>(`${this.base}/gigs/me`);
  }

  getGigById(id: string): Observable<GigDetailDto> {
    return this.http.get<GigDetailDto>(`${this.base}/gigs/${id}`);
  }

  deleteGig(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/gigs/${id}`);
  }
}
