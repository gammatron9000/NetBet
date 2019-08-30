import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Season, SeasonPlayer, NbEvent, FullSeason, SeasonWinPercent } from "../models";
import { ToastrService } from 'ngx-toastr';
import { faTrophy, faStar, faFrown } from '@fortawesome/free-solid-svg-icons'

@Component({
  selector: 'app-season-detail',
  templateUrl: './season-detail.component.html',
  styleUrls: ['./season-detail.component.css']
})
export class SeasonDetailComponent implements OnInit {
    public seasonDetail = new FullSeason();
    public stats: SeasonWinPercent[] = [];
    public faTrophy = faTrophy;
    public faStar = faStar;
    public faFrown = faFrown;

    constructor(private route: ActivatedRoute, public http: HttpClient, private toastr: ToastrService) {
        this.refreshData();
    }

    ngOnInit() { }

    refreshData() {
        let id = this.route.snapshot.paramMap.get('id');
        this.http.get<FullSeason>('/api/Season/GetFullSeason/' + id).subscribe(result => {
            let sortedPlayers = result.players.sort((n1, n2) => n1.currentCash - n2.currentCash).reverse();
            result.players = sortedPlayers;
            this.seasonDetail = result;
            console.log(this.seasonDetail);
        }, error => console.error(error));

        this.http.get<SeasonWinPercent[]>('/api/bet/GetBetStatsForSeason/' + id).subscribe(result => {
            let sortedStats = result.sort((n1, n2) => n1.winPercent - n2.winPercent).reverse();
            this.stats = sortedStats;
            console.log(this.stats);
        }, error => console.error(error));
    }


    deleteEvent(e: NbEvent) {
        if (confirm(`Are you sure you want to delete ${e.name}?`)) {
            this.http.delete(`/api/event/delete/${e.id}`).subscribe(response => {
                this.toastr.success('Event deleted');
                console.log('delete success', response);
                this.refreshData();
            }, function (error) {
                this.toastr.error('error');
                console.error('error deleting event: ', error);
            });
        }
    }

    getStarColor(i) {
        if (i === 0) { return 'gold'; }
        else if (i === 1) { return 'silver'; }
        else if (i === 2) { return 'brown'; }
        else return 'black';
    }

}
