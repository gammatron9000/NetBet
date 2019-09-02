module FightersDb

open Dapper
open DbCommon
open DbTypes


let getFighterByID fighterID = 
    let qp : QueryParamsID = { ID = fighterID }
    use db = new DbConnection()
    db.connection.Query<Fighter>("""
        SELECT ID, Name, Image, ImageLink 
        FROM dbo.Fighters
        WHERE ID = @ID""", qp)

let getAllFighters () = 
    use db = new DbConnection()
    db.connection.Query<Fighter>("""
        SELECT ID, Name, Image, ImageLink 
        FROM dbo.Fighters""")

let getOrInsertFighterIDByName (name: string) =
    let qp : QueryParamsName = { Name = name }
    use db = new DbConnection()
    db.connection.QuerySingle<int>("""
        IF NOT EXISTS 
        ( SELECT 1 
          FROM dbo.Fighters 
          WHERE Name = @Name )
        BEGIN 
            INSERT INTO dbo.Fighters (Name, Image, ImageLink)
            VALUES (@Name, CAST('' as VARBINARY(max)), '')
            SELECT SCOPE_IDENTITY()
        END
        ELSE
        BEGIN
            SELECT TOP 1 ID 
            FROM dbo.Fighters
            WHERE Name = @Name
        END """, qp)

let insertFighter (fighter: Fighter) =
    use db = new DbConnection()
    db.connection.Execute("""
        IF NOT EXISTS
        ( SELECT 1
          FROM dbo.Fighters
          WHERE ID = @ID )
        BEGIN
            INSERT INTO dbo.Fighters ( Name, Image, ImageLink )
            VALUES (@Name, @Image, @ImageLink)
        END """, fighter)

let updateFighter (fighter: Fighter) =
    use db = new DbConnection()
    db.connection.Execute("""
        UPDATE dbo.Fighters
        SET Name = @Name
          , Image = @Image
          , ImageLink = @ImageLink
        WHERE ID = @ID""", fighter)

let deleteFighter fighterID =
    let qp : QueryParamsID = { ID = fighterID }
    use db = new DbConnection()
    db.connection.Execute("""
        DELETE FROM dbo.Fighters
        WHERE ID = @ID""", qp)
