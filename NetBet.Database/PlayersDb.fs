module PlayersDb

open Dapper
open DbCommon
open DbTypes

let getAllPlayers() = 
    use connection = Db.CreateConnection()
    connection.Query<Player>("SELECT ID, Name FROM dbo.Players")

let getPlayerByID id = 
    let qp : QueryParamsID = { ID = id }
    use connection = Db.CreateConnection()
    connection.Query<Player>("SELECT ID, Name FROM dbo.Players WHERE ID = @ID", qp)

let insertPlayer name = 
    let qp : QueryParamsName = { Name = name }
    use connection = Db.CreateConnection()
    connection.Execute("""
        INSERT INTO dbo.Players(Name)
        VALUES (@Name)""", qp)

let updatePlayer (player: Player) = 
    use connection = Db.CreateConnection()
    connection.Execute("""
            UPDATE dbo.Players
            SET Name = @Name
            WHERE ID = @ID""", player)
    
let deletePlayer (player: Player) =
    use connection = Db.CreateConnection()
    connection.Execute("""DELETE FROM dbo.Players WHERE ID = @ID""", player)
