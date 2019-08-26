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
        let id = this.route.snapshot.paramMap.get('id');
        this.refreshData(id);
    }

    ngOnInit() { }

    refreshData(id) {
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

    onSubmit() {
        this.http.post('/api/event/CreateOrUpdate', this.evnt).subscribe(response => {
            this.toastr.success('Event saved');
            console.log('save success', response);
            if (this.evnt.event.id === 0) {
                this.http.get<number>('/api/event/GetEventIDByName/' + this.evnt.event.name).subscribe(result => {
                this.refreshData(result);
                });
            }
        }, function (error) {
            this.toastr.error('error');
            console.error('error saving event: ', error);
        });

    }

}
