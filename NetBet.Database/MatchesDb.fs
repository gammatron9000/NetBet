module MatchesDb

open Dapper
open DbCommon
open DbTypes
open DtoTypes

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

let getPrettyMatchesForEvent eventID = 
    let qp : QueryParamsID = { ID = eventID }
    use connection = Db.CreateConnection()
    connection.Query<PrettyMatch>("""
        SELECT m.ID, m.EventID, m.Fighter1ID, f1.Name as Fighter1Name, m.Fighter2ID, f2.Name as Fighter2Name, m.Fighter1Odds, m.Fighter2Odds, m.WinnerFighterID, m.LoserFighterID, m.IsDraw, m.DisplayOrder
        FROM dbo.Matches as m
        INNER JOIN dbo.Fighters as f1
          ON m.Fighter1ID = f1.ID
        INNER JOIN dbo.Fighters as f2
          ON m.Fighter2ID = f2.ID
        WHERE m.EventID = @ID """, qp)

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
