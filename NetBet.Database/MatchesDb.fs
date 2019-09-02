module MatchesDb

open Dapper
open DbCommon
open DbTypes
open DtoTypes

let getMatchByID matchID = 
    let qp : QueryParamsID = { ID = matchID }
    use db = new DbConnection()
    db.connection.Query<Match>("""
        SELECT ID, EventID, Fighter1ID, Fighter2ID, Fighter1Odds, Fighter2Odds, WinnerFighterID, LoserFighterID, IsDraw, DisplayOrder
        FROM dbo.Matches
        WHERE ID = @ID""", qp)

let getMatchesForEvent eventID = 
    let qp : QueryParamsID = { ID = eventID }
    use db = new DbConnection()
    db.connection.Query<Match>("""
        SELECT ID, EventID, Fighter1ID, Fighter2ID, Fighter1Odds, Fighter2Odds, WinnerFighterID, LoserFighterID, IsDraw, DisplayOrder
        FROM dbo.Matches
        WHERE EventID = @ID""", qp)

let getPrettyMatchesForEvent eventID = 
    let qp : QueryParamsID = { ID = eventID }
    use db = new DbConnection()
    db.connection.Query<PrettyMatch>("""
        SELECT m.ID, m.EventID, m.Fighter1ID, f1.Name as Fighter1Name, m.Fighter2ID, f2.Name as Fighter2Name, m.Fighter1Odds, m.Fighter2Odds, m.WinnerFighterID, m.LoserFighterID, m.IsDraw, m.DisplayOrder
        FROM dbo.Matches as m
        INNER JOIN dbo.Fighters as f1
          ON m.Fighter1ID = f1.ID
        INNER JOIN dbo.Fighters as f2
          ON m.Fighter2ID = f2.ID
        WHERE m.EventID = @ID """, qp)

let insertMatch (m: Match) =
    use db = new DbConnection()
    db.connection.Execute("""
        INSERT INTO dbo.Matches(EventID, Fighter1ID, Fighter2ID, Fighter1Odds, Fighter2Odds, WinnerFighterID, LoserFighterID, IsDraw, DisplayOrder)
        VALUES ( @EventID, @Fighter1ID, @Fighter2ID, @Fighter1Odds, @Fighter2ODds, @WinnerFighterID, @LoserFighterID, @IsDraw, @DisplayOrder )""", m)
        |> ignore

let deleteMatch matchID = 
    let qp : QueryParamsID = { ID = matchID }
    use db = new DbConnection()
    db.connection.Execute("""
        DELETE FROM dbo.Matches WHERE ID = @ID""", qp)
        |> ignore
        
let updateMatch (m: Match) =
    use db = new DbConnection()
    db.connection.Execute("""
        UPDATE dbo.Matches
        SET Fighter1ID = @Fighter1ID, Fighter2ID = @Fighter2ID, Fighter1Odds = @Fighter1Odds, Fighter2Odds = @Fighter2Odds, WinnerFighterID = @WinnerFighterID, LoserFighterID = @LoserFighterID, IsDraw = @IsDraw, DisplayOrder = @DisplayOrder
        WHERE ID = @ID""", m)
    |> ignore

let insertOrUpdateMatches (newMatches: Match[]) (existingMatches: Match[]) = 
    // remove deleted matches
    let newMatchIDs = newMatches |> Array.map (fun x -> x.ID) 
    let toRemoveIDs = 
        existingMatches 
        |> Array.map (fun x -> x.ID)
        |> Array.filter (fun e -> newMatchIDs |> Array.exists(fun u -> e = u ) |> not)
    toRemoveIDs |> Array.iter deleteMatch
    
    // add new matches 
    let toAdd = newMatches |> Array.filter(fun x -> x.ID = 0)
    toAdd |> Array.iter insertMatch

    // update existing matches
    let toUpdateIDs = 
        newMatches
        |> Array.map(fun x -> x.ID)
        |> Array.filter (fun e -> toRemoveIDs |> Array.exists(fun u -> e = u) |> not) // not being removed
        |> Array.filter (fun e -> e = 0 |> not) // not new
    newMatches 
        |> Array.filter(fun m -> toUpdateIDs |> Array.exists(fun i -> i = m.ID))
        |> Array.iter updateMatch
    ()

let resolveMatch matchID winnerID isDraw =
    let qp : QueryParamsResolveMatch =
        { MatchID = matchID
          WinnerID = winnerID
          IsDraw = isDraw }
    use db = new DbConnection()
    db.connection.Execute("""
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
    use db = new DbConnection()
    db.connection.Execute("""
        DELETE FROM dbo.Matches
        WHERE EventID = @ID""", qp)
