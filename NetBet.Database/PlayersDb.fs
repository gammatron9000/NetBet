module PlayersDb

open Dapper
open DbCommon
open DbTypes

let getAllPlayers() = 
    use db = new DbConnection()
    db.connection.Query<Player>("SELECT ID, Name FROM dbo.Players")

let getPlayerByID id = 
    let qp : QueryParamsID = { ID = id }
    use db = new DbConnection()
    db.connection.Query<Player>("SELECT ID, Name FROM dbo.Players WHERE ID = @ID", qp)

let insertPlayer name = 
    let qp : QueryParamsName = { Name = name }
    use db = new DbConnection()
    db.connection.Execute("""
        INSERT INTO dbo.Players(Name)
        VALUES (@Name)""", qp)

let updatePlayer (player: Player) = 
    use db = new DbConnection()
    db.connection.Execute("""
            UPDATE dbo.Players
            SET Name = @Name
            WHERE ID = @ID""", player)
    
let deletePlayer playerID =
    let qp : QueryParamsID = { ID = playerID }
    use db = new DbConnection()
    db.connection.Execute("""DELETE FROM dbo.Players WHERE ID = @ID""", qp)
