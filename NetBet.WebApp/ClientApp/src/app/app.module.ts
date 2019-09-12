import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { ToastrModule } from 'ngx-toastr';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { SeasonsComponent } from './seasons/seasons.component';
import { SeasonDetailComponent } from './season-detail/season-detail.component';
import { SeasonEditComponent } from './season-edit/season-edit.component';
import { EventEditComponent } from './event-edit/event-edit.component';
import { EventEasyComponent } from './event-easy/event-easy.component';
import { EventLiveComponent } from './event-live/event-live.component';
import { BetComponent } from './bet/bet.component';
import { BetDisplayComponent } from './bet-display/bet-display.component';
import { GroupByPipe } from './group-by.pipe';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    SeasonsComponent,
    SeasonDetailComponent,
    SeasonEditComponent,
    EventEditComponent,
    EventEasyComponent,
    BetComponent,
    EventLiveComponent,
    BetDisplayComponent,
    GroupByPipe
  ],
    imports: [
        CommonModule,
        BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
        HttpClientModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', component: SeasonsComponent, pathMatch: 'full' },
            { path: 'seasons', component: SeasonsComponent },
            { path: 'season/:id', component: SeasonDetailComponent },
            { path: 'season/edit/:id', component: SeasonEditComponent },
            { path: 'event/edit/:seasonid/:eventid', component: EventEditComponent },
            { path: 'event/easyEvent/:seasonID', component: EventEasyComponent },
            { path: 'event/live/:seasonid/:eventid', component: EventLiveComponent },
            { path: 'bet/:eventid', component: BetComponent }
          ]),
        BrowserAnimationsModule,
        NgMultiSelectDropDownModule.forRoot(),
        ToastrModule.forRoot(),
        FontAwesomeModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
