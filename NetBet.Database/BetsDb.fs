module BetsDb

open Dapper
open DbCommon
open DbTypes

let getBetsForMatch matchID =
    let qp : QueryParamsID = { ID = matchID }
    use connection = Db.CreateConnection()
    connection.Query<BetWithOdds>("""
        SELECT SeasonID, EventID, MatchID, PlayerID, FighterID, ParlayID, Stake, Result, Odds
        FROM dbo.BetsWithOdds
        WHERE MatchID = @ID""", qp)

let getBetsForEvent eventID = 
    let qp : QueryParamsID = { ID = eventID }
    use connection = Db.CreateConnection()
    connection.Query<BetWithOdds>("""
        SELECT SeasonID, EventID, MatchID, PlayerID, FighterID, ParlayID, Stake, Result, Odds
        FROM dbo.BetsWithOdds
        WHERE EventID = @ID""", qp)

let getBetsForPlayerForSeason playerID seasonID =
    let qp : QueryParamsSeasonPlayer = 
        { SeasonID = seasonID
          PlayerID = playerID }
    use connection = Db.CreateConnection()
    connection.Query<BetWithOdds>("""
        SELECT SeasonID, EventID, MatchID, PlayerID, FighterID, ParlayID, Stake, Result, Odds
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
    use connection = Db.CreateConnection()
    connection.Query<BetWithOdds>("""
        SELECT SeasonID, EventID, MatchID, PlayerID, FighterID, ParlayID, Stake, Result
        FROM dbo.BetsWithOdds
        WHERE SeasonID = @SeasonID
          AND EventID = @EventID
          AND MatchID = @MatchID 
          AND PlayerID = @PlayerID
          AND ParlayID = @ParlayID""", qp)
          
let insertBet (bet: Bet) =
    use connection = Db.CreateConnection()
    connection.Execute("""
        INSERT INTO dbo.Bets (SeasonID, EventID, MatchID, PlayerID, FighterID, ParlayID, Stake, Result)
        VALUES (@SeasonID, @EventID, @MatchID, @PlayerID, @FighterID, @ParlayID, @Stake, NULL)""", bet)

let deleteBet (bet: Bet) = 
    use connection = Db.CreateConnection()
    connection.Execute("""
        DELETE FROM dbo.Bets
        WHERE SeasonID = @SeasonID
          AND EventID = @EventID
          AND MatchID = @MatchID
          AND PlayerID = @PlayerID
          AND FighterID = @FighterID
          AND ParlayID = @ParlayID""", bet)
    
let deleteAllBetsForEvent eventID = 
    let qp : QueryParamsID = { ID = eventID }
    use connection = Db.CreateConnection()
    connection.Execute("""
        DELETE FROM dbo.Bets
        WHERE EventID = @ID""", qp)

let deleteAllBetsForMatch matchID =
    let qp : QueryParamsID = { ID = matchID }
    use connection = Db.CreateConnection()
    connection.Execute("""
        DELETE FROM dbo.Bets
        WHERE MatchID = @ID""", qp)

let pushBetsForMatch matchID =
    let qp : QueryParamsID = { ID = matchID }
    use connection = Db.CreateConnection()
    connection.Execute("""
        UPDATE dbo.Bets
        SET Result = 2
        WHERE MatchID = @ID""", qp)

let resolveBetWinners seasonID eventID matchID winnerFighterID =
    let qp : QueryParamsResolveBet = 
        { SeasonID = seasonID
          EventID = eventID
          MatchID = matchID
          FighterID = winnerFighterID }
    use connection = Db.CreateConnection()
    connection.Execute("""
        UPDATE dbo.Bets
        SET Result = 1
        WHERE SeasonID = @SeasonID
          AND EventID = @EventID
          AND MatchID = @MatchID
          AND FighterID = @FighterID""", qp)
    
let resolveBetLosers seasonID eventID matchID loserFighterID =
    let qp : QueryParamsResolveBet = 
        { SeasonID = seasonID
          EventID = eventID
          MatchID = matchID
          FighterID = loserFighterID }
    use connection = Db.CreateConnection()
    connection.Execute("""
        UPDATE dbo.Bets
        SET Result = 0
        WHERE SeasonID = @SeasonID
          AND EventID = @EventID
          AND MatchID = @MatchID
          AND FighterID = @FighterID""", qp)
    
let resolveParlayBetLose seasonID eventID matchID playerID parlayID =
    let qp: QueryParamsParlayedBets =
        { SeasonID = seasonID
          EventID  = eventID
          MatchID  = matchID
          PlayerID = playerID
          ParlayID = parlayID }
    use connection = Db.CreateConnection()
    connection.Execute("""
        UPDATE dbo.Bets
        SET Result = 0
        WHERE SeasonID = @SeasonID
          AND EventID  = @EventID
          AND MatchID  = @MatchID
          AND PlayerID = @PlayerID
          AND ParlayID = @ParlayID
          AND Result IS NULL""", qp)
