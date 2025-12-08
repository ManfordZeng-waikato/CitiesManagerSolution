import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { City } from '../models/city';
import { CitiesService } from '../services/cities.service';

@Component({
  selector: 'app-cities',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './cities.html',
  styleUrls: ['./cities.css'],
})
export class Cities implements OnInit {

  cities: City[] = [];
  isLoading = false;
  errorMessage: string | null = null;


  constructor(
    private citiesService: CitiesService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.loadCities();
  }

  private loadCities(): void {
    this.isLoading = true;

    this.citiesService.getCities().subscribe({
      next: (response: City[]) => {
      
        this.cities = response;
        this.isLoading = false;

      },
      error: (error: any) => {
        console.error(error);
        this.errorMessage = 'Failed to load cities';
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      complete: () => { }
    });
  }
}
