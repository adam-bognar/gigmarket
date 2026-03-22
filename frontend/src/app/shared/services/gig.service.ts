import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {CreateGigPayload, GigDetailDto, GigDto, GigSummaryDto} from '../models/gig.model';
import {ApiService} from './api.service';

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

  getGigs(): Observable<GigSummaryDto[]> {
    return this.http.get<GigSummaryDto[]>(`${this.base}/gigs`);
  }

  getGigById(id: string): Observable<GigDetailDto> {
    return this.http.get<GigDetailDto>(`${this.base}/gigs/${id}`);
  }
}
