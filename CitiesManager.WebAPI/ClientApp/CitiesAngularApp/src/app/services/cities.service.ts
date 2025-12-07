import { Injectable } from '@angular/core';
import { City } from '../models/city';

@Injectable({
  providedIn: 'root',
})
export class CitiesService {
  cities : City[] = [];
  constructor() {
    this.cities = [new City("101", 'New York'), new City("102", 'Los Angeles'), new City("103", 'Chicago')];
  }

  public getCities(): City[] { return this.cities; }
}
