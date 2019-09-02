export class Season {
    public startingCash: number;
    public endTime: Date;
    public startTime: Date;
    public name: string;
    public id: number;
    public minimumCash: number;
    public maxParlaySize: number;
    constructor() {
        this.startingCash = 0;
        this.endTime = new Date();
        this.startTime = new Date();
        this.name = '';
        this.id = 0;
        this.minimumCash = 0;
        this.maxParlaySize = 0;
    }
}

export class Player {
    public id: number;
    public name: string;
}

export class SeasonPlayer {
    seasonID: number;
    playerID: number;
    seasonName: string;
    playerName: string;
    minimumCash: number;
    currentCash: number;
    constructor() {
        this.seasonID = 0;
        this.playerID = 0;
        this.seasonName = '';
        this.playerName = '';
        this.minimumCash = 0;
        this.currentCash = 0;
    }
}

export class EditSeasonDto {
    season: Season;
    playerIDs: number[];
}

export class NbEvent {
    id: number;
    seasonID: number;
    name: string
    startTime: Date;
    constructor() {
        this.id = 0;
        this.seasonID = 0;
        this.name = '';
        this.startTime = new Date();
    }
}

export class FullSeason {
    public season: Season;
    public players: SeasonPlayer[];
    public events: NbEvent[]
    constructor() {
        this.events = [];
        this.players = [];
        this.season = new Season();
    }
}

export class PrettyMatch {
    public id: number;
    public eventID: number;
    public fighter1ID: number;
    public fighter2ID: number;
    public fighter1Name: string;
    public fighter2Name: string;
    public fighter1Odds: number;
    public fighter2Odds: number;
    public winnerFighterID: number;
    public loserFighterID: number;
    public isDraw: boolean;
    public displayOrder: number;
    constructor() {
        this.id = 0;
        this.eventID = 0;
        this.fighter1ID = 0;
        this.fighter2ID = 0;
        this.fighter1Name = '';
        this.fighter2Name = '';
        this.fighter1Odds = 0.0;
        this.fighter2Odds = 0.0;
        this.winnerFighterID = null;
        this.loserFighterID = null;
        this.isDraw = null;
        this.displayOrder = 0;
    }
}

export class EventWithPrettyMatches {
    public event: NbEvent;
    public matches: PrettyMatch[]; 
    constructor() {
        this.event = new NbEvent();
        this.matches = [];
    }
}

export class SeasonWinPercent {
    public playerName: string;
    public totalBets: number;
    public winningBets: number;
    public winPercent: number;
    constructor() {
        this.playerName = '';
        this.totalBets = 0;
        this.winningBets = 0;
        this.winPercent = 0.0;
    }
}


export class PrettyBet {
    public SeasonID: number;
    public EventID: number;
    public MatchID: number;
    public PlayerID: number;
    public FighterID: number;
    public ParlayID: string;
    public Stake: number;
    public Result: number;
    public Odds: number;
    public DisplayOrder: number;
    public FighterName: string;
    public ImageLink: string;
    public PlayerName: string;
    constructor() {
        this.SeasonID = 0;
        this.EventID = 0;
        this.MatchID = 0;
        this.PlayerID = 0;
        this.FighterID = 0;
        this.ParlayID = '';
        this.Stake = 0;
        this.Result = 0;
        this.Odds = 0;
        this.DisplayOrder = 0;
        this.FighterName = '';
        this.ImageLink = '';
        this.PlayerName = '';
    }
}