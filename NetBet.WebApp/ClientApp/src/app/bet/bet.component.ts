import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { Player, PrettyBet, PrettyMatch, EventWithPrettyMatches, NbEvent, SeasonPlayer } from "../models";
import { faPlus, faTimes } from '@fortawesome/free-solid-svg-icons'
import { makeStateKey } from '@angular/platform-browser';

@Component({
  selector: 'app-bet',
  templateUrl: './bet.component.html',
  styleUrls: ['./bet.component.css']
})
export class BetComponent implements OnInit {
    private eventID = 0;
    public faPlus = faPlus;
    public faTimes = faTimes;
    public allPlayers: SeasonPlayer[] = [];
    public selectedPlayer = new SeasonPlayer();
    public allBetsForEvent: PrettyBet[] = [];
    public matches: PrettyMatch[] = [];
    public evnt: NbEvent = new NbEvent();
    public betslip: PrettyBet[] = [];
    public currentBetsForPlayer: PrettyBet[] = [];
    public isParlay = false;
    public parlayStake = 0.0;

    constructor(private route: ActivatedRoute, public http: HttpClient, private toastr: ToastrService) {
        this.eventID = Number(this.route.snapshot.paramMap.get('eventid'));
        this.getEventAndMatches();
    }

    ngOnInit() { }

    getBets() {
        this.http.get<PrettyBet[]>('/api/bet/GetPrettyBetsForEvent/' + this.eventID).subscribe(result => {
            this.allBetsForEvent = result;
            console.log('bets', result);
            let currentPlayer = this.selectedPlayer; // makes this visible inside the following filter function
            this.currentBetsForPlayer = this.allBetsForEvent.filter(function (bet) {
                return bet.playerID === currentPlayer.playerID;
            });
        }, error => console.error('error getting bets: ', error));
    }

    getSeasonPlayers(seasonID) {
        this.http.get<SeasonPlayer[]>('/api/Player/GetPlayersForSeason/' + seasonID).subscribe(result => {
            this.allPlayers = result;
            if (this.allPlayers.length > 0) {
                this.selectedPlayer = this.allPlayers[0];
            }
            console.log('players', result);
            this.getBets();
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

    selectedPlayerChanged() {
        this.isParlay = false;
        this.parlayStake = 0.0;
        this.betslip = [];
        this.currentBetsForPlayer = [];
        let newPlayer = this.selectedPlayer;
        this.currentBetsForPlayer = this.allBetsForEvent.filter(function (bet) {
            return bet.playerID === newPlayer.playerID;
        });
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

    deleteBetFromSlip(b: PrettyBet) {
        let index = this.betslip.indexOf(b);
        this.betslip.splice(index, 1);
    }

    calculateToWin(b: PrettyBet) {
        return (b.odds - 1.0) * b.stake;
    }

    getParlayOdds() {
        if (this.betslip.length > 0) {

            let allOdds = this.betslip.map(x => x.odds);
            const reducer = (accumulator, currentValue) => accumulator * currentValue;
            return allOdds.reduce(reducer);
        }
        else return 0.0;
    }

    calculateParlayToWin() {
        let parlayOdds = this.getParlayOdds();
        return (parlayOdds - 1.00) * this.parlayStake;
    }

    calculateTotalStake() {
        if (this.betslip.length > 0) {
            let allStakes = this.betslip.map(x => x.stake);
            const reducer = (accumulator, currentValue) => accumulator + currentValue;
            return allStakes.reduce(reducer);
        }
        else return 0.0;
    }

    calculateTotalToWin() {
        if (this.betslip.length > 0) {
            let allToWins = this.betslip.map(x => this.calculateToWin(x));
            const reducer = (accumulator, currentValue) => accumulator + currentValue;
            return allToWins.reduce(reducer);
        }
        else return 0.0;
    }

    placeBets() {
        let totalStake = this.isParlay ? this.parlayStake : this.calculateTotalStake();
        if (totalStake > this.selectedPlayer.currentCash) {
            this.toastr.error('')
        }
    }
}
