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
          PlayerID = playerID
          CurrentCash = 0M }
    DbContext.Instance.Connection.Query<Bet>(
        """SELECT SeasonID, EventID, MatchID, PlayerID, FighterID, ParlayID, Stake, Result
           FROM dbo.Bets
           WHERE SeasonID = @SeasonID
             AND PlayerID = @PlayerID""", qp)

let createBet (bet: Bet) =
    DbContext.Instance.Connection.Execute("""
        INSERT INTO dbo.Bets (SeasonID, EventID, MatchID, PlayerID, FighterID, ParlayID, Stake, Result)
        VALUES (@SeasonID, @EventID, @MatchID, @PlayerID, @FighterID, @ParlayID, @Stake, NULL)""", bet)

let resolveBet (bet: Bet) =
    DbContext.Instance.Connection.Execute("""
        UPDATE dbo.Bets
        SET Result = @Result
        WHERE SeasonID  = @SeasonID
          AND EventID   = @EventID
          AND MatchID   = @MatchID
          AND PlayerID  = @PlayerID
          AND FighterID = @FighterID""", bet)

let resolveAllBetsForMatch (m: Match) =
    if m.WinnerFighterID = Nullable() && 
       m.LoserFighterID = Nullable() &&
       m.IsDraw = Nullable() then 
        0
    else 
        let winCount = 
            DbContext.Instance.Connection.Execute("""
                UPDATE dbo.Bets
                SET Result = 1
                WHERE MatchID = @ID
                  AND FighterID = @WinnerFighterID""", m)
        let loseCount = 
            DbContext.Instance.Connection.Execute("""
                UPDATE dbo.Bets
                SET Result = 0
                WHERE MatchID = @ID
                  AND (FighterID = @LoserFighterID 
                     OR  @IsDraw = 'true')""", m)
        winCount + loseCount
