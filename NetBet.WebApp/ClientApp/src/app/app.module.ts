import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';
import { ToastrModule } from 'ngx-toastr';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { SeasonsComponent } from './seasons/seasons.component';
import { SeasonDetailComponent } from './season-detail/season-detail.component';
import { SeasonEditComponent } from './season-edit/season-edit.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    SeasonsComponent,
    SeasonDetailComponent,
    SeasonEditComponent
  ],
    imports: [
        CommonModule,
        BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
        HttpClientModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', component: SeasonsComponent, pathMatch: 'full' },
            { path: 'counter', component: CounterComponent },
            { path: 'fetch-data', component: FetchDataComponent },
            { path: 'seasons', component: SeasonsComponent },
            { path: 'season/:id', component: SeasonDetailComponent },
            { path: 'season/edit/:id', component: SeasonEditComponent },
          ]),
        BrowserAnimationsModule,
        NgMultiSelectDropDownModule.forRoot(),
        ToastrModule.forRoot()
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
