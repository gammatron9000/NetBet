module MatchesDb

open Dapper
open DbCommon
open DbTypes

let getMatchByID matchID = 
    let qp : QueryParamsID = { ID = matchID }
    DbContext.Instance.Connection.Query<Match>("""
        SELECT ID, EventID, Fighter1ID, Fighter2ID, Fighter1Odds, Fighter2Odds, WinnerFighterID, LoserFighterID, IsDraw
        FROM dbo.Matches
        WHERE ID = @ID""", qp)

let getMatchesForEvent eventID = 
    let qp : QueryParamsID = { ID = eventID }
    DbContext.Instance.Connection.Query<Match>("""
        SELECT ID, EventID, Fighter1ID, Fighter2ID, Fighter1Odds, Fighter2Odds, WinnerFighterID, LoserFighterID, IsDraw
        FROM dbo.Matches
        WHERE EventID = @ID""", qp)

let insertMatches (matches: Match[]) =
    let mutable allResults = 0
    for m in matches do 
        let queryResult = DbContext.Instance.Connection.Execute("""
            INSERT INTO dbo.Matches(EventID, Fighter1ID, Fighter2ID, Fighter1Odds, Fighter2Odds, WinnerFighterID, LoserFighterID, IsDraw)
            VALUES ( @EventID, @Fighter1ID, @Fighter2ID, @Fighter1Odds, @Fighter2ODds, @WinnerFighterID, @LoserFighterID, @IsDraw )""", m)
        allResults <- allResults + queryResult
    allResults

let deleteMatch (m: Match) = 
    DbContext.Instance.Connection.Execute("""
        DELETE FROM dbo.Matches WHERE ID = @ID""", m)

let resolveMatch (m: Match) =
    DbContext.Instance.Connection.Execute("""
        UPDATE dbo.Matches
        SET WinnerFighterID = @WinnerFighterID
          , LoserFighterID = @LoserFighterID
          , IsDraw = @IsDraw
        WHERE ID = @ID""", m)