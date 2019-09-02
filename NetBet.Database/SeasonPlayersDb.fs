module SeasonPlayersDb

open Dapper
open DbCommon
open DbTypes


let getPlayersForSeason seasonID = 
    let qp : QueryParamsID = { ID = seasonID }
    use db = new DbConnection()
    db.connection.Query<SeasonPlayer>("""
        SELECT sp.SeasonID
             , sp.PlayerID
             , s.Name as SeasonName
             , p.Name as PlayerName
             , s.MinimumCash
             , sp.CurrentCash
        FROM dbo.SeasonPlayers as sp
        INNER JOIN dbo.Players as p
          ON p.ID = sp.PlayerID
        INNER JOIN dbo.Seasons as s
          ON s.ID = sp.SeasonID
        WHERE sp.SeasonID = @ID""", qp)
    
let getSeasonPlayer seasonID playerID =
    let qp : QueryParamsSeasonPlayer = 
        { SeasonID = seasonID
          PlayerID = playerID }
    use db = new DbConnection()
    db.connection.Query<SeasonPlayer>("""
        SELECT sp.SeasonID
             , sp.PlayerID
             , s.Name as SeasonName
             , p.Name as PlayerName
             , s.MinimumCash
             , sp.CurrentCash
        FROM dbo.SeasonPlayers as sp
        INNER JOIN dbo.Players as p
          ON p.ID = sp.PlayerID
        INNER JOIN dbo.Seasons as s
          ON s.ID = sp.SeasonID
        WHERE sp.SeasonID = @SeasonID
          AND sp.PlayerID = @PlayerID""", qp)

let addPlayerToSeason seasonID playerID =
    let qp : QueryParamsSeasonPlayer = 
        { SeasonID = seasonID
          PlayerID = playerID }
    use db = new DbConnection()
    db.connection.Execute("""
        IF NOT EXISTS
        ( SELECT 1
          FROM dbo.SeasonPlayers
          WHERE PlayerID = @PlayerID
          AND SeasonID = @SeasonID)
        BEGIN
            ;WITH season as 
            ( SELECT ID, StartingCash FROM Seasons where ID = @SeasonID )
            INSERT INTO SeasonPlayers (SeasonID, PlayerID, CurrentCash)
            SELECT @SeasonID, @PlayerID, StartingCash FROM season

        END """, qp)

let removePlayerFromSeason seasonID playerID =
    let qp : QueryParamsSeasonPlayer = 
        { SeasonID = seasonID
          PlayerID = playerID } 
    use db = new DbConnection()
    db.connection.Execute("""
        DELETE FROM dbo.SeasonPlayers
        WHERE SeasonID = @SeasonID
        AND PlayerID = @PlayerID""", qp)

let updateCurrentCash (sp: SeasonPlayer) =
    use db = new DbConnection()
    db.connection.Execute("""
        UPDATE dbo.SeasonPlayers
        SET CurrentCash = @CurrentCash
        WHERE SeasonID = @SeasonID
          AND PlayerID = @PlayerID""", sp)
    