import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { AddReviewPayload, ReviewDto } from '../models/gig.model';

@Injectable({ providedIn: 'root' })
export class ReviewService extends ApiService {
  submitReview(payload: AddReviewPayload): Observable<ReviewDto> {
    return this.http.post<ReviewDto>(`${this.base}/reviews`, payload);
  }
}
