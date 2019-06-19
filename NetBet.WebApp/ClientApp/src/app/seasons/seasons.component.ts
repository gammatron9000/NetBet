import { Component, OnInit } from '@angular/core';
import { HttpClient } from "@angular/common/http";

@Component({
  selector: 'app-seasons-component',
  templateUrl: './seasons.component.html'
})
export class SeasonsComponent {
  public seasons: Season[];

  constructor(http: HttpClient) {
      http.get<Season[]>('/api/Season/GetAll').subscribe(result => {
          console.log(result);
          this.seasons = result;
      }, error => console.error(error));
  }
}


interface Season {
    StartingCash: string;
    EndTime: Date;
    StartTime: Date;
    Name: string;
    ID: number;
    MinimumCash: number;
    MaxParlaySize: number;
}
