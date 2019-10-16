import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { HttpClient } from '@angular/common/http';
import { PrettyBet, SeasonPlayer, EventWithPrettyMatches, NbEvent, PrettyMatch, ResolveMatchDto } from '../models';
import { faStar } from '@fortawesome/free-solid-svg-icons';

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
    public faStar = faStar;
    public highlightedMatchId: number = 0;

    constructor(private route: ActivatedRoute, public http: HttpClient, private toastr: ToastrService) {
        this.eventID = Number(this.route.snapshot.paramMap.get('eventid'));
        this.seasonID = Number(this.route.snapshot.paramMap.get('seasonid'));
        this.refreshData();
    }
    
    getBets() {
        this.http.get<PrettyBet[]>('/api/bet/GetPrettyBetsForEvent/' + this.eventID).subscribe(result => {
            this.allBetsForEvent = result;
        }, error => console.error('error getting bets: ', error));
    }

    getSeasonPlayers(seasonID) {
        this.http.get<SeasonPlayer[]>('/api/Player/GetPlayersForSeason/' + seasonID).subscribe(result => {
            this.allPlayers = result;
            console.log('players', result);
            this.getBets();
        }, error => console.error('error getting players: ', error));
    }

    refreshData() {
        this.http.get<EventWithPrettyMatches>('/api/event/GetEventWithMatches/' + this.eventID).subscribe(result => {
            this.evnt = result.event;
            let sortedMatches = result.matches.sort((m1, m2) => m1.displayOrder - m2.displayOrder);
            result.matches = sortedMatches;
            this.matches = result.matches;
            console.log('event and matches', result);
            this.getSeasonPlayers(this.evnt.seasonID);
        }, error => console.error('error getting event and matches: ', error));
    }

    isFightResolved(m: PrettyMatch) {
        return m.isDraw === true || (m.winnerFighterID !== null && m.loserFighterID !== null);
    }

    getTextForResult(m: PrettyMatch, fighterID: number) {
        if (!this.isFightResolved(m)) { return 'TBD'; }
        if (m.isDraw) { return 'Push'; }
        if (fighterID === m.winnerFighterID) { return 'Win'; }
        if (fighterID === m.loserFighterID) { return 'Lose'; }
        return 'ERROR';
    }


    resolveDraw(m: PrettyMatch) {
        let dto = new ResolveMatchDto();
        dto.seasonID = this.seasonID;
        dto.eventID = this.eventID;
        dto.matchID = m.id;
        dto.winnerID = null;
        dto.isDraw = true;
        if (confirm(`Are you sure ${m.fighter1Name} vs ${m.fighter2Name} is a draw?`)) {
            this.resolveMatch(dto);
        }
    }

    resolveWinner(m: PrettyMatch, winnerID: number, winnerName: string) {
        let dto = new ResolveMatchDto();
        dto.seasonID = this.seasonID;
        dto.eventID = this.eventID;
        dto.matchID = m.id;
        dto.winnerID = winnerID;
        dto.isDraw = false;
        if (confirm(`Are you sure ${winnerName} is the winner?`)) {
            this.resolveMatch(dto);
        }
    }

    resolveMatch(dto: ResolveMatchDto) {
        this.http.post('/api/match/ResolveMatch', dto).subscribe(response => {
            this.refreshData();
        }, function (error) {
            this.toastr.error('error');
            console.error('error resolving match: ', error);
        });
    }

    setHighlightedMatch(m: PrettyMatch) {
        if (m.id === this.highlightedMatchId)
        { this.highlightedMatchId = 0; }
        else { this.highlightedMatchId = m.id; }
    }
}
