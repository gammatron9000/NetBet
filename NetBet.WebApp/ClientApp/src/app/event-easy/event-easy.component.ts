import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { EventWithPrettyMatches, PrettyMatch } from "../models";

@Component({
  selector: 'app-event-easy',
  templateUrl: './event-easy.component.html',
  styleUrls: ['./event-easy.component.css']
})
export class EventEasyComponent implements OnInit {
    public events: EventWithPrettyMatches[] = [];
    public selectedEvent: EventWithPrettyMatches = new EventWithPrettyMatches;
    public seasonID: number = 0;
    
    constructor(public http: HttpClient, private route: ActivatedRoute) {
        this.seasonID = Number(this.route.snapshot.paramMap.get('seasonID'));
        this.http.get<EventWithPrettyMatches[]>('/api/event/GetUpcomingEventsFromWeb').subscribe(result => {
            this.events = result;
            console.log(this.events);
        }, error => console.error(error));
    }

    ngOnInit() { }

    test() {
        debugger;
        console.log(this.selectedEvent);
    }

    changeSelectedEvent(val: EventWithPrettyMatches) {
        console.log(val);
        debugger;
        this.selectedEvent = val;
    }
}
