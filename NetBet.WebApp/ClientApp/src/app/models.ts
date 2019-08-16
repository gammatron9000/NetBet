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
    public ID: number;
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

export class CreateSeasonDto {
    season: Season;
    players: Player[];
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
