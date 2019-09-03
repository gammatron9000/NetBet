import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { Player, PrettyBet, PrettyMatch, EventWithPrettyMatches, NbEvent, SeasonPlayer } from "../models";
import { faPlus } from '@fortawesome/free-solid-svg-icons'

@Component({
  selector: 'app-bet',
  templateUrl: './bet.component.html',
  styleUrls: ['./bet.component.css']
})
export class BetComponent implements OnInit {
    private eventID = 0;
    public faPlus = faPlus;
    public allPlayers: SeasonPlayer[] = [];
    public selectedPlayer = new SeasonPlayer();
    public allBetsForEvent: PrettyBet[] = [];
    public matches: PrettyMatch[] = [];
    public evnt: NbEvent = new NbEvent();
    public betslip: PrettyBet[] = [];
    public isParlay = false;

    constructor(private route: ActivatedRoute, public http: HttpClient, private toastr: ToastrService) {
        this.eventID = Number(this.route.snapshot.paramMap.get('eventid'));
        this.getEventAndMatches();
        this.getBets();
    }

    ngOnInit() { }

    getSeasonPlayers(seasonID) {
        this.http.get<SeasonPlayer[]>('/api/Player/GetPlayersForSeason/' + seasonID).subscribe(result => {
            this.allPlayers = result;
            console.log('players', result);
        }, error => console.error('error getting players: ', error));
    }
    
    getEventAndMatches() {
        this.http.get<EventWithPrettyMatches>('/api/event/GetEventWithMatches/' + this.eventID).subscribe(result => {
            this.evnt = result.event;
            let sortedMatches = result.matches.sort((m1, m2) => m1.displayOrder - m2.displayOrder);
            result.matches = sortedMatches;
            this.matches = result.matches;
            console.log('event and matches', result);
            this.getSeasonPlayers(this.evnt.seasonID);
        }, error => console.error('error getting event and matches: ', error));
    }

    getBets() {
        this.http.get<PrettyBet[]>('/api/bet/GetPrettyBetsForEvent/' + this.eventID).subscribe(result => {
            this.allBetsForEvent = result;
            console.log('bets', result);
        }, error => console.error('error getting bets: ', error));
    }

    addBetToSlip(fighterID, fighterName, odds, m: PrettyMatch) {
        let b = new PrettyBet();
        b.displayOrder = m.displayOrder;
        b.eventID = this.eventID;
        b.fighterID = fighterID
        b.fighterName = fighterName
        b.matchID = m.id;
        b.odds = odds;
        b.playerID = this.selectedPlayer.playerID;
        b.seasonID = this.evnt.seasonID;
        b.stake = 0;
        this.betslip.push(b);
    }
    
    addBetToSlipFighter1(m: PrettyMatch) {
        this.addBetToSlip(m.fighter1ID, m.fighter1Name, m.fighter1Odds, m);
    }

    addBetToSlipFighter2(m: PrettyMatch) {
        this.addBetToSlip(m.fighter2ID, m.fighter2Name, m.fighter2Odds, m);
    }
    
}
