module PlayersDb

open Dapper
open DbCommon
open DbTypes

let getAllPlayers() = 
    DbContext.OpenConnection()
    let result = DbContext.Connection.Query<Player>("SELECT ID, Name FROM dbo.Players")
    DbContext.CloseConnection()
    result

let getPlayerByID id = 
    let qp : QueryParamsID = { ID = id }
    DbContext.OpenConnection()
    let result = DbContext.Connection.Query<Player>("SELECT ID, Name FROM dbo.Players WHERE ID = @ID", qp)
    DbContext.CloseConnection()
    result

let insertPlayer name = 
    let qp : QueryParamsName = { Name = name }
    DbContext.OpenConnection()
    let result = DbContext.Connection.Execute("""
        INSERT INTO dbo.Players(Name)
        VALUES (@Name)""", qp)
    DbContext.CloseConnection()
    result

let updatePlayer (player: Player) = 
    DbContext.OpenConnection()
    let result = DbContext.Connection.Execute("""
            UPDATE dbo.Players
            SET Name = @Name
            WHERE ID = @ID""", player)
    DbContext.CloseConnection()
    result

let deletePlayer (player: Player) =
    DbContext.OpenConnection()
    let result = DbContext.Connection.Execute("""DELETE FROM dbo.Players WHERE ID = @ID""", player)
    DbContext.CloseConnection()
    result
