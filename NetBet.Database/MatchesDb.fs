module MatchesDb

open Dapper
open DbCommon
open DbTypes

let getMatchByID matchID = 
    let qp : QueryParamsID = { ID = matchID }
    DbContext.OpenConnection()
    let result = DbContext.Connection.Query<Match>("""
        SELECT ID, EventID, Fighter1ID, Fighter2ID, Fighter1Odds, Fighter2Odds, WinnerFighterID, LoserFighterID, IsDraw, DisplayOrder
        FROM dbo.Matches
        WHERE ID = @ID""", qp)
    DbContext.CloseConnection()
    result

let getMatchesForEvent eventID = 
    let qp : QueryParamsID = { ID = eventID }
    DbContext.OpenConnection()
    let result = DbContext.Connection.Query<Match>("""
        SELECT ID, EventID, Fighter1ID, Fighter2ID, Fighter1Odds, Fighter2Odds, WinnerFighterID, LoserFighterID, IsDraw, DisplayOrder
        FROM dbo.Matches
        WHERE EventID = @ID""", qp)
    DbContext.CloseConnection()
    result

let insertMatch (m: Match) =
    DbContext.OpenConnection()
    let result = DbContext.Connection.Execute("""
        INSERT INTO dbo.Matches(EventID, Fighter1ID, Fighter2ID, Fighter1Odds, Fighter2Odds, WinnerFighterID, LoserFighterID, IsDraw, DisplayOrder)
        VALUES ( @EventID, @Fighter1ID, @Fighter2ID, @Fighter1Odds, @Fighter2ODds, @WinnerFighterID, @LoserFighterID, @IsDraw, @DisplayOrder )""", m)
    DbContext.CloseConnection()
    result

let insertMatches (matches: Match[]) =
    let mutable allResults = 0
    for m in matches do 
        let queryResult = insertMatch m
        allResults <- allResults + queryResult
    allResults

let deleteMatch matchID = 
    let qp : QueryParamsID = { ID = matchID }
    DbContext.OpenConnection()
    let result = DbContext.Connection.Execute("""
        DELETE FROM dbo.Matches WHERE ID = @ID""", qp)
    DbContext.CloseConnection()
    result

let resolveMatch (m: Match) =
    DbContext.OpenConnection()
    let result = DbContext.Connection.Execute("""
        UPDATE dbo.Matches
        SET WinnerFighterID = @WinnerFighterID
          , LoserFighterID = @LoserFighterID
          , IsDraw = @IsDraw
        WHERE ID = @ID""", m)
    DbContext.CloseConnection()
    result

let deleteAllMatchesForEvent eventID = 
    let qp : QueryParamsID = { ID = eventID }
    DbContext.OpenConnection()
    let result = DbContext.Connection.Execute("""
        DELETE FROM dbo.Matches
        WHERE EventID = @ID""", qp)
    DbContext.CloseConnection()
    result