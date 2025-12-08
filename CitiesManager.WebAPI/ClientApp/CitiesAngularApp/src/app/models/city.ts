export class City {
  cityId: string | null;
  cityName: string | null;

  constructor(cityID: string | null = null, cityName: string | null = null) {
    this.cityId = cityID;
    this.cityName = cityName;
  }
}
