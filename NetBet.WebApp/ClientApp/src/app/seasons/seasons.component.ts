import { Component, OnInit } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Season } from "../models";
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-seasons-component',
  templateUrl: './seasons.component.html'
})
export class SeasonsComponent {
  public seasons: Season[];

    constructor(private http: HttpClient, private toastr: ToastrService) {
        this.refreshData();
    }

    private refreshData() {
        this.http.get<Season[]>('/api/Season/GetAll').subscribe(result => {
            this.seasons = result;
        }, error => console.error(error));
    }

    deleteSeason(s: Season) {
        if (confirm(`Are you sure you want to delete ${s.name}?`)) {
            this.http.delete(`/api/season/delete/${s.id}`).subscribe(response => {
                this.toastr.success('Season deleted');
                console.log('delete success', response);
                this.refreshData();
            }, function (error) {
                this.toastr.error('error');
                console.error('error deleting season: ', error);
            });
        }
    }
    
}



