import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { EventWithPrettyMatches, PrettyMatch, NbEvent } from "../models";
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-event-edit',
  templateUrl: './event-edit.component.html',
  styleUrls: ['./event-edit.component.css']
})
export class EventEditComponent implements OnInit {
    public evnt: EventWithPrettyMatches = new EventWithPrettyMatches();

    constructor(private route: ActivatedRoute, public http: HttpClient, private toastr: ToastrService) {
        this.refreshData();
    }

    ngOnInit() { }

    refreshData() {
        let id = this.route.snapshot.paramMap.get('id');
        this.http.get<EventWithPrettyMatches>('/api/event/getEventWithMatches/' + id).subscribe(result => {
            let sortedMatches = result.matches.sort((m1, m2) => m1.displayOrder - m2.displayOrder);
            result.matches = sortedMatches;
            this.evnt = result;
            console.log(this.evnt);
        }, error => console.error(error));
    }

    deleteMatch(m: PrettyMatch) {
        console.log('delete match clicked');
    }

}
