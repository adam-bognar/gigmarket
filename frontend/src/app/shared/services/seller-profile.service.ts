import {inject, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Router} from '@angular/router';
import {environment} from '../../../environments/environment';
import {Observable} from 'rxjs';
import {
  ConnectStripeAccountResult,
  CreateSellerProfilePayload,
  LanguageOption,
  SellerEarningsDto,
  SellerProfileFullDto,
  SellerPublicProfileDto,
  UpdateSellerProfilePayload,
} from '../models/seller.model';

@Injectable({
  providedIn: 'root',
})
export class SellerProfileService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly baseUrl = `${environment.apiUrl}`;

  getLanguages(): Observable<LanguageOption[]> {
    return this.http.get<LanguageOption[]>(`${this.baseUrl}/languages`);
  }

  uploadProfilePic(file: File): Observable<{ blobPath: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ blobPath: string }>(
      `${this.baseUrl}/files/upload/profile/me`,
      formData,
      {withCredentials: true}
    );
  }

  createProfile(payload: CreateSellerProfilePayload): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(`${this.baseUrl}/seller/create`, payload);
  }

  getMyProfile(): Observable<SellerProfileFullDto> {
    return this.http.get<SellerProfileFullDto>(`${this.baseUrl}/seller/me`);
  }

  updateProfile(payload: UpdateSellerProfilePayload): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/seller/update`, payload);
  }

  getPublicProfile(sellerProfileId: string): Observable<SellerPublicProfileDto> {
    return this.http.get<SellerPublicProfileDto>(
      `${this.baseUrl}/seller/${sellerProfileId}/public`
    );
  }

  connectStripe(): Observable<ConnectStripeAccountResult> {
    return this.http.post<ConnectStripeAccountResult>(
      `${this.baseUrl}/seller/connect`,
      {}
    );
  }

  getStripeDashboardLink(): Observable<{ url: string }> {
    return this.http.get<{ url: string }>(
      `${this.baseUrl}/seller/connect/dashboard`
    );
  }

  getEarnings(): Observable<SellerEarningsDto> {
    return this.http.get<SellerEarningsDto>(
      `${this.baseUrl}/seller/earnings`
    );
  }
}
