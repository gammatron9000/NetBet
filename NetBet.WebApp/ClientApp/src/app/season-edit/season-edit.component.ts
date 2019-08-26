import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { HttpClient, HttpHeaders, HttpRequest } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { Season, SeasonPlayer, NbEvent, FullSeason, Player, EditSeasonDto } from "../models";

@Component({
    selector: 'app-season-edit',
    templateUrl: './season-edit.component.html',
    styleUrls: ['./season-edit.component.css']
})
export class SeasonEditComponent implements OnInit {
    public season: FullSeason = new FullSeason();
    public allPlayers: Player[];
    public selectedPlayers: Player[]
    public dropdownSettings = {};

    constructor(private route: ActivatedRoute, private router: Router, private http: HttpClient, private toastr: ToastrService) {
        this.dropdownSettings = {
            idField: 'id',
            textField: 'name',
            selectAllText: 'Select All',
            unSelectAllText: 'UnSelect All'
        };
        
        // get all available players
        this.http.get<Player[]>('/api/Player/Get').subscribe(result => {
            this.allPlayers = result;
        }, error => console.error('error getting players: ', error));
        
        let id = this.route.snapshot.paramMap.get('id');
        if (Number(id) === 0) { this.season = new FullSeason(); }
        else {
            this.http.get<FullSeason>('/api/Season/GetFullSeason/' + id).subscribe(result => {
                this.season = result;
                console.log(this.season);
                this.selectedPlayers = result.players.map(function (x) { return { id: x.playerID, name: x.playerName } });
            }, error => console.error('error getting season: ', error));
        }
    }

    private mapPlayersToPlayerIDs(players: any[]) {
        return players.map(function (x) { return x.id });
    }
    
    onSubmit() {
        var dto = new EditSeasonDto();
        dto.season = this.season.season;
        dto.playerIDs = this.mapPlayersToPlayerIDs(this.selectedPlayers);
        this.http.post('/api/season/Edit', dto).subscribe(response => {
            this.toastr.success('Season saved');
            console.log('save success', response);
            if (this.season.season.id === 0) {
                this.http.get<number>('/api/Season/GetSeasonIdByName/' + this.season.season.name).subscribe(result => {
                    this.refreshPage(result);
                });
            }
        }, function (error) {
            this.toastr.error('error');
            console.error('error saving season: ', error);
            });
    }

    refreshPage(id: number) {
        this.router.navigate([ '/season/edit', id ]);
    }

    ngOnInit() { }
}
