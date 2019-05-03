module MatchesDb

open Dapper
open DbCommon
open DbTypes

let getMatchByID matchID = 
    let qp : QueryParamsID = { ID = matchID }
    use connection = Db.CreateConnection()
    connection.Query<Match>("""
        SELECT ID, EventID, Fighter1ID, Fighter2ID, Fighter1Odds, Fighter2Odds, WinnerFighterID, LoserFighterID, IsDraw, DisplayOrder
        FROM dbo.Matches
        WHERE ID = @ID""", qp)

let getMatchesForEvent eventID = 
    let qp : QueryParamsID = { ID = eventID }
    use connection = Db.CreateConnection()
    connection.Query<Match>("""
        SELECT ID, EventID, Fighter1ID, Fighter2ID, Fighter1Odds, Fighter2Odds, WinnerFighterID, LoserFighterID, IsDraw, DisplayOrder
        FROM dbo.Matches
        WHERE EventID = @ID""", qp)

let insertMatch (m: Match) =
    use connection = Db.CreateConnection()
    connection.Execute("""
        INSERT INTO dbo.Matches(EventID, Fighter1ID, Fighter2ID, Fighter1Odds, Fighter2Odds, WinnerFighterID, LoserFighterID, IsDraw, DisplayOrder)
        VALUES ( @EventID, @Fighter1ID, @Fighter2ID, @Fighter1Odds, @Fighter2ODds, @WinnerFighterID, @LoserFighterID, @IsDraw, @DisplayOrder )""", m)

let insertMatches (matches: Match[]) =
    let mutable allResults = 0
    for m in matches do 
        let queryResult = insertMatch m
        allResults <- allResults + queryResult
    allResults

let deleteMatch matchID = 
    let qp : QueryParamsID = { ID = matchID }
    use connection = Db.CreateConnection()
    connection.Execute("""
        DELETE FROM dbo.Matches WHERE ID = @ID""", qp)

let resolveMatch matchID winnerID isDraw =
    let qp : QueryParamsResolveMatch =
        { MatchID = matchID
          WinnerID = winnerID
          IsDraw = isDraw }
    use connection = Db.CreateConnection()
    connection.Execute("""
        UPDATE dbo.Matches
        SET WinnerFighterID = @WinnerID
          , LoserFighterID = 
            CASE WHEN @WinnerID = Fighter1ID THEN Fighter2ID
                 WHEN @WinnerID = Fighter2ID THEN Fighter1ID
                 ELSE NULL END
          , IsDraw = @IsDraw
        WHERE ID = @MatchID""", qp)

let deleteAllMatchesForEvent eventID = 
    let qp : QueryParamsID = { ID = eventID }
    use connection = Db.CreateConnection()
    connection.Execute("""
        DELETE FROM dbo.Matches
        WHERE EventID = @ID""", qp)
