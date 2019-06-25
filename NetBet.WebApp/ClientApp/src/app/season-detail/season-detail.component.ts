import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Season, SeasonPlayer, NbEvent, FullSeason } from "../models";

@Component({
  selector: 'app-season-detail',
  templateUrl: './season-detail.component.html',
  styleUrls: ['./season-detail.component.css']
})
export class SeasonDetailComponent implements OnInit {
    public seasonDetail = new FullSeason();

    constructor(private route: ActivatedRoute, public http: HttpClient) {
        let id = this.route.snapshot.paramMap.get('id');
        this.http.get<FullSeason>('/api/Season/GetFullSeason/' + id).subscribe(result => {
            let sortedPlayers = result.players.sort((n1, n2) => n1.currentCash - n2.currentCash).reverse();
            result.players = sortedPlayers;
            this.seasonDetail = result;
            console.log(this.seasonDetail);
        }, error => console.error(error));
    }

    ngOnInit() {

    }

}
