import { Component } from '@angular/core';
import { City } from '../models/city';
import { CitiesService } from '../services/cities.service';
import { CommonModule } from '@angular/common'; 

@Component({
  selector: 'app-cities',
  imports: [CommonModule],
  templateUrl: './cities.html',
  styleUrl: './cities.css',
})
export class Cities {
  cities: City[] = [];
  constructor(private citiesService: CitiesService) { }

  ngOnInit() {
    this.cities = this.citiesService.getCities();
  }
}
