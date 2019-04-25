module FightersDb

open Dapper
open DbCommon
open DbTypes


let getFighterByID fighterID = 
    let qp : QueryParamsID = { ID = fighterID }
    use connection = Db.CreateConnection()
    connection.Query<Fighter>("""
        SELECT ID, Name, Image, ImageLink 
        FROM dbo.Fighters
        WHERE ID = @ID""", qp)

let getAllFighters () = 
    use connection = Db.CreateConnection()
    connection.Query<Fighter>("""
        SELECT ID, Name, Image, ImageLink 
        FROM dbo.Fighters""")

let insertFighter (fighter: Fighter) =
    use connection = Db.CreateConnection()
    connection.Execute("""
        IF NOT EXISTS
        ( SELECT 1
          FROM dbo.Fighters
          WHERE ID = @ID )
        BEGIN
            INSERT INTO dbo.Fighters ( Name, Image, ImageLink )
            VALUES (@Name, @Image, @ImageLink)
        END """, fighter)

let updateFighter (fighter: Fighter) =
    use connection = Db.CreateConnection()
    connection.Execute("""
        UPDATE dbo.Fighters
        SET Name = @Name
          , Image = @Image
          , ImageLink = @ImageLink
        WHERE ID = @ID""", fighter)

let deleteFighter fighterID =
    let qp : QueryParamsID = { ID = fighterID }
    use connection = Db.CreateConnection()
    connection.Execute("""
        DELETE FROM dbo.Fighters
        WHERE ID = @ID""", qp)
