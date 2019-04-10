module SeasonPlayersDb

open Dapper
open DbCommon
open DbTypes


let getPlayersForSeason seasonID = 
    let qp : QueryParamsID = { ID = seasonID }
    DbContext.Instance.Connection.Query<SeasonPlayer>("""
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
          ON s.ID = sp.SeasonID""", qp)

let getSeasonPlayer seasonID playerID =
    let qp : QueryParamsSeasonPlayer = 
        { SeasonID = seasonID
          PlayerID = playerID }
    DbContext.Instance.Connection.Query<SeasonPlayer>("""
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
    DbContext.Instance.Connection.Execute("""
        IF NOT EXISTS
        ( SELECT 1
          FROM dbo.SeasonPlayers
          WHERE PlayerID = @PlayerID
          AND SeasonID = @SeasonID)
        BEGIN
            INSERT INTO dbo.SeasonPlayers (SeasonID, PlayerID, CurrentCash)
            VALUES( @SeasonID, @PlayerID, @CurrentCash )
        END """, qp)

let removePlayerFromSeason seasonID playerID =
    let qp : QueryParamsSeasonPlayer = 
        { SeasonID = seasonID
          PlayerID = playerID } 
    DbContext.Instance.Connection.Execute("""
        DELETE FROM dbo.SeasonPlayers
        WHERE SeasonID = @SeasonID
        AND PlayerID = @PlayerID""", qp)
        
let updateCurrentCash (sp: SeasonPlayer) =
    DbContext.Instance.Connection.Execute("""
        UPDATE dbo.SeasonPlayers
        SET CurrentCash = @CurrentCash
        WHERE SeasonID = @SeasonID
          AND PlayerID = @PlayerID""", sp)