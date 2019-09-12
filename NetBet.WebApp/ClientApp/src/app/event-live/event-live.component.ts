import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { HttpClient } from '@angular/common/http';
import { PrettyBet, SeasonPlayer, EventWithPrettyMatches, NbEvent, PrettyMatch } from '../models';

@Component({
  selector: 'app-event-live',
  templateUrl: './event-live.component.html',
  styleUrls: ['./event-live.component.css']
})
export class EventLiveComponent {
    public eventID: number = 0;
    public seasonID: number = 0;
    public allPlayers: SeasonPlayer[] = [];
    public allBetsForEvent: PrettyBet[] = [];
    public evnt: NbEvent = new NbEvent();
    public matches: PrettyMatch[] = [];

    constructor(private route: ActivatedRoute, public http: HttpClient, private toastr: ToastrService) {
        this.eventID = Number(this.route.snapshot.paramMap.get('eventid'));
        this.seasonID = Number(this.route.snapshot.paramMap.get('seasonid'));
        this.getEventAndMatches();
    }
    
    getBets() {
        this.http.get<PrettyBet[]>('/api/bet/GetPrettyBetsForEvent/' + this.eventID).subscribe(result => {
            this.allBetsForEvent = result;
            //let groupedBets = this.groupBy(result, bet => bet.playerName);
            
        }, error => console.error('error getting bets: ', error));
    }

    getSeasonPlayers(seasonID) {
        this.http.get<SeasonPlayer[]>('/api/Player/GetPlayersForSeason/' + seasonID).subscribe(result => {
            this.allPlayers = result;
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

    groupBy(list, keyGetter) {
        const map = new Map();
        list.forEach((item) => {
            const key = keyGetter(item);
            const collection = map.get(key);
            if (!collection) {
                map.set(key, [item]);
            } else {
                collection.push(item);
            }
        });
        return map;
    }
}
