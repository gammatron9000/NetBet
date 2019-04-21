module FightersDb

open Dapper
open DbCommon
open DbTypes


let getFighterByID fighterID = 
    let qp : QueryParamsID = { ID = fighterID }
    DbContext.OpenConnection()
    let result = DbContext.Connection.Query<Fighter>("""
        SELECT ID, Name, Image, ImageLink 
        FROM dbo.Fighters
        WHERE ID = @ID""", qp)
    DbContext.CloseConnection()
    result

let getAllFighters () = 
    DbContext.OpenConnection()
    let result = DbContext.Connection.Query<Fighter>("""
        SELECT ID, Name, Image, ImageLink 
        FROM dbo.Fighters""")
    DbContext.CloseConnection()
    result

let insertFighter (fighter: Fighter) =
    DbContext.OpenConnection()
    let result = DbContext.Connection.Execute("""
        IF NOT EXISTS
        ( SELECT 1
          FROM dbo.Fighters
          WHERE ID = @ID )
        BEGIN
            INSERT INTO dbo.Fighters ( Name, Image, ImageLink )
            VALUES (@Name, @Image, @ImageLink)
        END """, fighter)
    DbContext.CloseConnection()
    result

let updateFighter (fighter: Fighter) =
    DbContext.OpenConnection()
    let result = DbContext.Connection.Execute("""
        UPDATE dbo.Fighters
        SET Name = @Name
          , Image = @Image
          , ImageLink = @ImageLink
        WHERE ID = @ID""", fighter)
    DbContext.CloseConnection()
    result

let deleteFighter fighterID =
    let qp : QueryParamsID = { ID = fighterID }
    DbContext.OpenConnection()
    let result = DbContext.Connection.Execute("""
        DELETE FROM dbo.Fighters
        WHERE ID = @ID""", qp)
    DbContext.CloseConnection()
    result