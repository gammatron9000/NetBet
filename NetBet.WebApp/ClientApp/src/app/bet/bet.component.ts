import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { Player, PrettyBet, PrettyMatch, EventWithPrettyMatches, NbEvent } from "../models";
import { faPlus } from '@fortawesome/free-solid-svg-icons'

@Component({
  selector: 'app-bet',
  templateUrl: './bet.component.html',
  styleUrls: ['./bet.component.css']
})
export class BetComponent implements OnInit {
    private eventID = 0;
    public faPlus = faPlus;
    public allPlayers: Player[] = [];
    public selectedPlayer = new Player();
    public allBetsForEvent: PrettyBet[] = [];
    public matches: PrettyMatch[] = [];
    public evnt: NbEvent = new NbEvent();

    constructor(private route: ActivatedRoute, public http: HttpClient, private toastr: ToastrService) {
        this.eventID = Number(this.route.snapshot.paramMap.get('eventid'));
        this.getPlayers();
        this.getEventAndMatches();
        this.getBets();
    }

    ngOnInit() { }

    getPlayers() {
        this.http.get<Player[]>('/api/Player/Get').subscribe(result => {
            this.allPlayers = result;
            console.log('players', result);
        }, error => console.error('error getting players: ', error));
    }

    getEventAndMatches() {
        this.http.get<EventWithPrettyMatches>('/api/event/GetEventWithMatches/' + this.eventID).subscribe(result => {
            this.evnt = result.event;
            this.matches = result.matches;
            console.log('event and matches', result);
        }, error => console.error('error getting event and matches: ', error));
    }

    getBets() {
        this.http.get<PrettyBet[]>('/api/bet/GetPrettyBetsForEvent/' + this.eventID).subscribe(result => {
            this.allBetsForEvent = result;
            console.log('bets', result);
        }, error => console.error('error getting bets: ', error));
    }
    
}
