import {ApiService} from './api.service';
import {Observable} from 'rxjs';
import {Injectable} from '@angular/core';
import {GigCategoryDto, GigSubcategoryDto} from '../models/gig.model';

@Injectable({ providedIn: 'root' })
export class CategoriesService extends ApiService {

  getCategories(): Observable<GigCategoryDto[]> {
    return this.http.get<GigCategoryDto[]>(`${this.base}/categories`);
  }

  getSubcategories(categoryId: string): Observable<GigSubcategoryDto[]> {
    return this.http.get<GigSubcategoryDto[]>(`${this.base}/categories/${categoryId}/subcategories`);
  }
}
