import { Routes } from '@angular/router';
import { Cities } from './cities/cities';
import { Component } from '@angular/core';


@Component({
  standalone: true,
  template: '',  
})
export class Home { }


export const routes: Routes = [
  { path: 'cities', component: Cities },
  { path: '', component: Home },
];
