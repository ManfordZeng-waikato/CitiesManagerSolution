import { Injectable, inject } from '@angular/core';
import { City } from '../models/city';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CitiesService {
  private http = inject(HttpClient);

  private baseUrl = 'https://localhost:7188/api/v1/cities';

  getCities(): Observable<City[]> {
    return this.http.get<City[]>(this.baseUrl);
  }
}
