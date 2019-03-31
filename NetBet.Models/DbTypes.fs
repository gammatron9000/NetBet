module DbTypes

open System

type BetResult = Lose = 0 | Win = 1

[<CLIMutable>]
type Bet = 
    {
        SeasonID  : int
        EventID   : int
        MatchID   : int
        PlayerID  : int
        FighterID : int
        ParlayID  : Guid
        Stake     : decimal
        Result    : Nullable<BetResult>
    }


[<CLIMutable>]
type Event = 
    {
        ID        : int
        SeasonID  : int
        Name      : string
        StartTime : DateTime
    }

[<CLIMutable>]
type Fighter = 
    {
        ID        : int
        Name      : string
        Image     : byte[]
        ImageLink : string
    }


[<CLIMutable>]
type Match = 
    {
        ID              : int
        EventID         : int
        Fighter1ID      : int
        Fighter2ID      : int
        Fighter1Odds    : decimal
        Fighter2Odds    : decimal
        WinnerFighterID : Nullable<int>
        LoserFighterID  : Nullable<int>
        IsDraw          : Nullable<Boolean>
    }
    
[<CLIMutable>]
type Player = 
    {
        ID   : int
        Name : string
    }

[<CLIMutable>]
type SeasonPlayer = 
    {
        SeasonID    : int
        PlayerID    : int
        CurrentCash : decimal
    }

[<CLIMutable>]
type Season = 
    {
        ID            : int
        Name          : string
        StartTime     : DateTime
        EndTime       : DateTime
        StartingCash  : int
        MinimumCash   : int
        MaxParlaySize : int
    }


// VIEWS

type SeasonWithPlayers =
    {
        SeasonID    : int
        PlayerID    : int
        SeasonName  : string
        PlayerName  : string
        MinimumCash : int
        CurrentCash : decimal
    }