import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { EventWithPrettyMatches, PrettyMatch } from "../models";
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-event-easy',
  templateUrl: './event-easy.component.html',
  styleUrls: ['./event-easy.component.css']
})
export class EventEasyComponent implements OnInit {
    public events: EventWithPrettyMatches[] = [];
    public selectedEvent: EventWithPrettyMatches = new EventWithPrettyMatches;
    public seasonID: number = 0;
    
    constructor(public http: HttpClient, private route: ActivatedRoute, private toastr: ToastrService, private router: Router) {
        this.seasonID = Number(this.route.snapshot.paramMap.get('seasonID'));
        this.http.get<EventWithPrettyMatches[]>('/api/event/GetUpcomingEventsFromWeb').subscribe(result => {
            this.events = result;
            console.log(this.events);
            if (this.events.length > 0) {
                this.selectedEvent = this.events[0];
            }
        }, error => console.error(error));
    }

    ngOnInit() { }

    onSubmit() {
        this.selectedEvent.event.seasonID = this.seasonID;
        this.http.post('/api/event/CreateOrUpdate', this.selectedEvent).subscribe(response => {
            this.router.navigate(['/season', this.seasonID]);
        }, function (error) {
            this.toastr.error('error');
            console.error('error saving event: ', error);
        });
    }
}
