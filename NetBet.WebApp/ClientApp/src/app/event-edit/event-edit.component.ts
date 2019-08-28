import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { EventWithPrettyMatches, PrettyMatch } from "../models";
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-event-edit',
  templateUrl: './event-edit.component.html',
  styleUrls: ['./event-edit.component.css']
})
export class EventEditComponent implements OnInit {
    public evnt: EventWithPrettyMatches = new EventWithPrettyMatches();
    private eventID = 0;

    constructor(private route: ActivatedRoute, public http: HttpClient, private toastr: ToastrService) {
        this.eventID = Number(this.route.snapshot.paramMap.get('id'));
        this.refreshData(this.eventID);
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
        const index: number = this.evnt.matches.indexOf(m);
        if (index !== -1) {
            this.evnt.matches.splice(index, 1);
        }
        // resequence the display orders
        for (let i = 0; i < this.evnt.matches.length; i++) {
            this.evnt.matches[i].displayOrder = i;
        }
    }

    addMatch() {
        var newMatch = new PrettyMatch();
        newMatch.displayOrder = this.evnt.matches.length;
        newMatch.eventID = this.eventID;
        this.evnt.matches.push(newMatch);
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
