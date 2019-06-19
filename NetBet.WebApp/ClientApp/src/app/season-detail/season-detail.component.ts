import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';

@Component({
  selector: 'app-season-detail',
  templateUrl: './season-detail.component.html',
  styleUrls: ['./season-detail.component.css']
})
export class SeasonDetailComponent implements OnInit {
    public seasonDetail: any;

    constructor(
        private route: ActivatedRoute,
    ) {
        
    }

    ngOnInit() {
        let id = this.route.snapshot.paramMap.get('id');
        console.log(id);
    }

}
