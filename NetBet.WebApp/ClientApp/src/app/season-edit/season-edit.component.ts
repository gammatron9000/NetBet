import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Season, SeasonPlayer, NbEvent, FullSeason, Player } from "../models";

@Component({
    selector: 'app-season-edit',
    templateUrl: './season-edit.component.html',
    styleUrls: ['./season-edit.component.css']
})
export class SeasonEditComponent implements OnInit {
    public season: FullSeason;
    public allPlayers: Player[];
    public selectedPlayers: Player[]
    public dropdownSettings = {};

    constructor(private route: ActivatedRoute, public http: HttpClient) {
        this.dropdownSettings = {
            idField: 'id',
            textField: 'name',
            selectAllText: 'Select All',
            unSelectAllText: 'UnSelect All'
        };

        // get all available players
        this.http.get<Player[]>('/api/Player/Get').subscribe(result => {
            this.allPlayers = result;
            console.log('players', this.allPlayers);
        }, error => console.error('error getting players: ', error));
        

        let id = this.route.snapshot.paramMap.get('id');
        if (Number(id) === 0) { this.season = new FullSeason(); }
        else {
            this.http.get<FullSeason>('/api/Season/GetFullSeason/' + id).subscribe(result => {
                this.season = result;
                console.log(this.season);
                this.selectedPlayers = result.players.map(function (x) { return { ID: x.playerID, name: x.playerName } });
            }, error => console.error('error getting season: ', error));
        }
    }

    onSubmit() { console.log('you submitted a thing great job', this.season); }

    ngOnInit() { }
}
