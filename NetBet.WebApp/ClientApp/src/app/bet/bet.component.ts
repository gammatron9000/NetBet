import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { Player } from "../models";

@Component({
  selector: 'app-bet',
  templateUrl: './bet.component.html',
  styleUrls: ['./bet.component.css']
})
export class BetComponent implements OnInit {
    public selectedPlayer = new Player();
    public allPlayers: Player[] = [];

    constructor(private route: ActivatedRoute, public http: HttpClient, private toastr: ToastrService) {

    }

    ngOnInit() {
    }

}
