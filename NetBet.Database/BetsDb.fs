﻿module BetsDb

open Dapper
open DbCommon
open DbTypes

let getBetsForMatch matchID =
    let qp : QueryParamsID = { ID = matchID }
    DbContext.Instance.Connection.Query<BetWithOdds>(
        """SELECT SeasonID, EventID, MatchID, PlayerID, FighterID, ParlayID, Stake, Result, Odds
           FROM dbo.BetsWithOdds
           WHERE MatchID = @ID""", qp)

let getBetsForEvent eventID = 
    let qp : QueryParamsID = { ID = eventID }
    DbContext.Instance.Connection.Query<BetWithOdds>(
        """SELECT SeasonID, EventID, MatchID, PlayerID, FighterID, ParlayID, Stake, Result, Odds
           FROM dbo.BetsWithOdds
           WHERE EventID = @ID""", qp)

let getBetsForPlayerForSeason playerID seasonID =
    let qp : QueryParamsSeasonPlayer = 
        { SeasonID = seasonID
          PlayerID = playerID }
    DbContext.Instance.Connection.Query<BetWithOdds>(
        """SELECT SeasonID, EventID, MatchID, PlayerID, FighterID, ParlayID, Stake, Result, Odds
           FROM dbo.BetsWithOdds
           WHERE SeasonID = @SeasonID
             AND PlayerID = @PlayerID""", qp)

let getParlayedBets (b: BetWithOdds) = 
    let qp : QueryParamsParlayedBets = 
        { SeasonID = b.SeasonID
          EventID = b.EventID
          MatchID = b.MatchID
          PlayerID = b.PlayerID
          ParlayID = b.ParlayID }
    DbContext.Instance.Connection.Query<BetWithOdds>("""
        SELECT SeasonID, EventID, MatchID, PlayerID, FighterID, ParlayID, Stake, Result
        FROM dbo.BetsWithOdds
        WHERE SeasonID = @SeasonID
          AND EventID = @EventID
          AND MatchID = @MatchID 
          AND PlayerID = @PlayerID
          AND ParlayID = @ParlayID""", qp)
          
let insertBet (bet: Bet) =
    DbContext.Instance.Connection.Execute("""
        INSERT INTO dbo.Bets (SeasonID, EventID, MatchID, PlayerID, FighterID, ParlayID, Stake, Result)
        VALUES (@SeasonID, @EventID, @MatchID, @PlayerID, @FighterID, @ParlayID, @Stake, NULL)""", bet)
        
let deleteBet (bet: Bet) = 
    DbContext.Instance.Connection.Execute("""
        DELETE FROM dbo.Bets
        WHERE SeasonID = @SeasonID
          AND EventID = @EventID
          AND MatchID = @MatchID
          AND PlayerID = @PlayerID
          AND FighterID = @FighterID
          AND ParlayID = @ParlayID""", bet)

let deleteAllBetsForEvent eventID = 
    let qp : QueryParamsID = { ID = eventID }
    DbContext.Instance.Connection.Execute("""
        DELETE FROM dbo.Bets
        WHERE EventID = @ID""", qp)

let deleteAllBetsForMatch matchID =
    let qp : QueryParamsID = { ID = matchID }
    DbContext.Instance.Connection.Execute("""
        DELETE FROM dbo.Bets
        WHERE MatchID = @ID""", qp)

let pushBetsForMatch matchID =
    let qp : QueryParamsID = { ID = matchID }
    DbContext.Instance.Connection.Execute("""
        UPDATE dbo.Bets
        SET Result = 2
        WHERE MatchID = @ID""", qp)
        
let resolveBetWinners seasonID eventID matchID winnerFighterID =
    let qp : QueryParamsResolveBet = 
        { SeasonID = seasonID
          EventID = eventID
          MatchID = matchID
          FighterID = winnerFighterID }
    DbContext.Instance.Connection.Execute("""
        UPDATE dbo.Bets
        SET Result = 1
        WHERE SeasonID = @SeasonID
          AND EventID = @EventID
          AND MatchID = @MatchID
          AND FighterID = @FighterID""", qp)

let resolveBetLosers seasonID eventID matchID winnerFighterID =
    let qp : QueryParamsResolveBet = 
        { SeasonID = seasonID
          EventID = eventID
          MatchID = matchID
          FighterID = winnerFighterID }
    DbContext.Instance.Connection.Execute("""
        UPDATE dbo.Bets
        SET Result = 0
        WHERE SeasonID = @SeasonID
          AND EventID = @EventID
          AND MatchID = @MatchID
          AND FighterID = @FighterID""", qp)

          