import { Component, OnInit, Input, OnChanges } from '@angular/core';
import { PrettyBet, BetDisplay, BetDisplayNameAndResult } from '../models';
import { faPlus, faTimes, faCheck, faHandPaper } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'bet-display',
  templateUrl: './bet-display.component.html',
  styleUrls: ['./bet-display.component.css']
})
export class BetDisplayComponent implements OnChanges {

    @Input() bets: PrettyBet[] = [];
    @Input() highlightedMatchId: number = 0;
    public displayBets: BetDisplay[] = [];
    public faTimes = faTimes;
    public faCheck = faCheck;
    public faHand = faHandPaper;
    
    ngOnChanges(changes) {
        this.populateDisplayBets(this.bets);
    }

    groupBy(list, keyGetter) {
        const map = new Map();
        list.forEach((item) => {
            const key = keyGetter(item);
            const collection = map.get(key);
            if (!collection) {
                map.set(key, [item]);
            } else {
                collection.push(item);
            }
        });
        return map;
    }

    populateDisplayBets(bets: PrettyBet[]) {
        const grouped = this.groupBy(bets, b => b.parlayID);
        let result: BetDisplay[] = [];
        let self = this; // alias 'this' to pass into mapper function
        grouped.forEach(function (value, key) {
            let display: BetDisplay = self.mapBetsToDisplayBets(value, key, self);
            result.push(display);
        });
        console.log('my bets', result);
        this.displayBets = result;
    }

    mapBetsToDisplayBets(bets: PrettyBet[], key: string, context: any) {
        let display = new BetDisplay();
        let allStakes = bets.map(b => b.stake);
        let firstStake = allStakes[0];
        let stakesMatch = allStakes.every(x => x === firstStake);
        if (!stakesMatch) { console.log('ERROR: not all stakes match in bet with parlayID ' + key); }
        display.parlayID = key;
        display.fightersAndResults =
            bets.map(b =>
                new BetDisplayNameAndResult(b.fighterName, this.mapResultCodeToString(b.result), b.matchID));
        display.totalStake = firstStake;
        display.totalOdds = context.getParlayOdds(bets);
        display.totalToWin = context.calculateExistingParlayToWin(bets, firstStake);
        return display;
    }

    getParlayOdds(bets: PrettyBet[]) {
        if (bets.length > 0) {
            // ignore push bets from this calculation
            let nonPush = bets.filter(x => this.mapResultCodeToString(x.result) !== 'PUSH');
            if (nonPush.length < 1) { return 0.0; }

            let allOdds = nonPush.map(x => x.odds);
            const reducer = (accumulator, currentValue) => accumulator * currentValue;
            return allOdds.reduce(reducer);
        }
        else return 0.0;
    }

    calculateExistingParlayToWin(bets: PrettyBet[], stake: number) {
        if (bets.length > 0) {
            // ignore push bets from this calculation
            let nonPush = bets.filter(x => this.mapResultCodeToString(x.result) !== 'PUSH');
            if (nonPush.length < 1) { return 0.0; }

            let allOdds = nonPush.map(x => x.odds);
            const reducer = (accumulator, currentValue) => accumulator * currentValue;
            let parlayOdds = allOdds.reduce(reducer);
            let result = (parlayOdds - 1.00) * stake;
            return result;
        }
        else return 0.0;
    }
    
    mapResultCodeToString(code: number) {
        switch (code) {
            case 0: return 'LOSE';
            case 1: return 'WIN';
            case 2: return 'PUSH';
            default: return 'TBD';
        }
    }

    determineFullBetResult(b: BetDisplay) {
        let results = b.fightersAndResults.map(x => x.result);
        let anyLose = results.find(x => x === 'LOSE');
        if (anyLose) { return 'LOSE'; } //if any are losses, the whole bet is a loss
        let winPushCount = results.filter(x => x === 'WIN' || x === 'PUSH');
        if (winPushCount.length === results.length) { // all bets have been resolved
            let anyWin = results.find(x => x === 'WIN');
            if (anyWin) { return 'WIN'; }
            else return 'PUSH'; // all pushes
        }
        return '';
    }

    getFullBetBgColor(b: BetDisplay) {
        let fullResult = this.determineFullBetResult(b);
        if (fullResult === 'LOSE') { return 'lightpink'; }
        if (fullResult === 'WIN') { return 'lightgreen'; }
        if (fullResult === 'PUSH') { return 'palegoldenrod'; }
        return '';
    }

    calculateResult(b: BetDisplay) {
        let fullResult = this.determineFullBetResult(b);
        if (fullResult === 'LOSE') { return 0 - b.totalStake; }
        if (fullResult === 'WIN') { return b.totalToWin; }
        if (fullResult === 'PUSH') { return 0; }
        else return 0 - b.totalStake;
    }

    calculateAllResults() {
        let total = 0;
        this.displayBets.forEach((item) => total += this.calculateResult(item));
        return total;
    }

    calculateTotalStake() {
        let total = 0
        this.displayBets.forEach((item) => total += item.totalStake);
        return total;
    }

    betShouldHighlight(b: BetDisplay) {
        //let fullResult = this.determineFullBetResult(b);
        //if (fullResult !== '') { return false; }

        let allMatches = b.fightersAndResults.map(x => x.matchID);
        if (allMatches.includes(this.highlightedMatchId)) { return true; }
        else { return false; }
    }
}
