module BetsDb

open Dapper
open DbCommon
open DbTypes
open System

let getBetsForMatch matchID =
    let qp : QueryParamsID = { ID = matchID }
    DbContext.Instance.Connection.Query<Bet>(
        """SELECT SeasonID, EventID, MatchID, PlayerID, FighterID, ParlayID, Stake, Result
           FROM dbo.Bets
           WHERE MatchID = @ID""", qp)

let getBetsForEvent eventID = 
    let qp : QueryParamsID = { ID = eventID }
    DbContext.Instance.Connection.Query<Bet>(
        """SELECT SeasonID, EventID, MatchID, PlayerID, FighterID, ParlayID, Stake, Result
           FROM dbo.Bets
           WHERE EventID = @ID""", qp)

let getBetsForPlayerForSeason playerID seasonID =
    let qp : QueryParamsSeasonPlayer = 
        { SeasonID = seasonID
          PlayerID = playerID }
    DbContext.Instance.Connection.Query<Bet>(
        """SELECT SeasonID, EventID, MatchID, PlayerID, FighterID, ParlayID, Stake, Result
           FROM dbo.Bets
           WHERE SeasonID = @SeasonID
             AND PlayerID = @PlayerID""", qp)

let insertBet (bet: Bet) =
    DbContext.Instance.Connection.Execute("""
        INSERT INTO dbo.Bets (SeasonID, EventID, MatchID, PlayerID, FighterID, ParlayID, Stake, Result)
        VALUES (@SeasonID, @EventID, @MatchID, @PlayerID, @FighterID, @ParlayID, @Stake, NULL)""", bet)

let updateBet (bet: Bet) =
    DbContext.Instance.Connection.Execute("""
        UPDATE dbo.Bets
        SET Result = @Result
        WHERE SeasonID  = @SeasonID
          AND EventID   = @EventID
          AND MatchID   = @MatchID
          AND PlayerID  = @PlayerID
          AND FighterID = @FighterID""", bet)
          