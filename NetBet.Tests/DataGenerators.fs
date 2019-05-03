module DataGenerators

open DbTypes
open System

let makeSeason name =
    { ID            = 0
      Name          = name
      StartTime     = DateTime.Now.AddMonths(-1)
      EndTime       = DateTime.Now.AddMonths(1)
      StartingCash  = 1000
      MinimumCash   = 50
      MaxParlaySize = 5 }
      
let makeSeasonPlayer seasonID playerID currentCash =
    { SeasonID    = seasonID
      PlayerID    = playerID
      CurrentCash = currentCash }

let makeEvent seasonID name =
    { ID        = 0
      SeasonID  = seasonID
      Name      = name
      StartTime = DateTime.Now.AddHours(1.0) }

let makeFighter name = 
    { ID = 0
      Name = name
      Image = [||] 
      ImageLink = "" }

let makeMatch eventID fighter1ID fighter2ID fighter1Odds fighter2Odds = 
    { ID              = 0
      EventID         = eventID
      Fighter1ID      = fighter1ID
      Fighter2ID      = fighter2ID
      Fighter1Odds    = fighter1Odds
      Fighter2Odds    = fighter2Odds
      WinnerFighterID = Nullable()
      LoserFighterID  = Nullable()
      IsDraw          = Nullable()
      DisplayOrder    = 0 }

let makeBet seasonID eventID matchID playerID fighterID stake = 
    { SeasonID  = seasonID
      EventID   = eventID
      MatchID   = matchID
      PlayerID  = playerID
      FighterID = fighterID
      ParlayID  = Guid()
      Stake     = stake
      Result    = Nullable() }