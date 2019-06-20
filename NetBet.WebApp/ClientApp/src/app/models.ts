export interface Season {
    startingCash: string;
    endTime: Date;
    startTime: Date;
    name: string;
    id: number;
    minimumCash: number;
    maxParlaySize: number;
}

export interface SeasonPlayer {
    seasonID: number;
    playerID: number;
    seasonName: string;
    playerName: string;
    minimumCash: number;
    currentCash: number;
}

export interface NbEvent {
    id: number;
    seasonID: number;
    name: string
    startTime: Date;
}


export interface FullSeason {
    season: Season;
    players: SeasonPlayer[];
    events: NbEvent[]
}