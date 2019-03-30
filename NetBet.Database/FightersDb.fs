module FightersDb

open Dapper
open DbCommon
open DbTypes


let getFighterByID fighterID = 
    let qp : QueryParamsID = { ID = fighterID }
    DbContext.Instance.Connection.Query<Fighter>("""
        SELECT ID, Name, Image, ImageLink 
        FROM dbo.Fighters
        WHERE ID = @ID""", qp)
    |> Seq.exactlyOne

let getAllFighters () = 
    DbContext.Instance.Connection.Query<Fighter>("""
        SELECT ID, Name, Image, ImageLink 
        FROM dbo.Fighters""")

let createFighter (fighter: Fighter) =
    DbContext.Instance.Connection.Execute("""
        IF NOT EXISTS
        ( SELECT 1
          FROM dbo.Fighters
          WHERE ID = @ID )
        BEGIN
            INSERT INTO dbo.Fighters ( Name, Image, ImageLink )
            VALUES (@Name, @Image, @ImageLink)
        END """, fighter)

let updateFighter (fighter: Fighter) =
    DbContext.Instance.Connection.Execute("""
        UPDATE dbo.Fighters
        SET Name = @Name
          , Image = @Image
          , ImageLink = @ImageLink
        WHERE ID = @ID""", fighter)