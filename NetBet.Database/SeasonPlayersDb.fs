module SeasonPlayersDb

open Dapper
open DbCommon
open DbTypes


let GetPlayersForSeason seasonID = 
    let qp : QueryParamsID = { ID = seasonID }
    DbContext.Instance.Connection.Query<SeasonWithPlayers>("""
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
          
let AddPlayerToSeason (seasonPlayer: SeasonPlayer) =
    let qp : QueryParamsSeasonPlayer = 
        { SeasonID = seasonPlayer.SeasonID
          PlayerID = seasonPlayer.PlayerID
          CurrentCash = seasonPlayer.CurrentCash }
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

let RemovePlayerFromSeason (seasonPlayer: SeasonPlayer) =
    let qp : QueryParamsSeasonPlayer = 
        { SeasonID = seasonPlayer.SeasonID
          PlayerID = seasonPlayer.PlayerID
          CurrentCash = seasonPlayer.CurrentCash}
    DbContext.Instance.Connection.Execute("""
        DELETE FROM dbo.SeasonPlayers
        WHERE SeasonID = @SeasonID
        AND PlayerID = @PlayerID""", qp)

let UpdateCurrentCash (sp: SeasonPlayer) =
    let qp : QueryParamsSeasonPlayer =
        { SeasonID = sp.SeasonID
          PlayerID = sp.PlayerID
          CurrentCash = sp.CurrentCash }
    DbContext.Instance.Connection.Execute("""
        UPDATE dbo.SeasonPlayers
        SET CurrentCash = @CurrentCash
        WHERE SeasonID = @SeasonID
          AND PlayerID = @PlayerID""", qp)