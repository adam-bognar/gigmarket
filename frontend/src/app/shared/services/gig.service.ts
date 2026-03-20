import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateGigPayload } from '../models/gig.model';
import {ApiService} from './api.service';

export interface GigDto {
  id: string;
  sellerProfileId: string;
  title: string;
  status: string;
  createdAtUtc: string;
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
}
