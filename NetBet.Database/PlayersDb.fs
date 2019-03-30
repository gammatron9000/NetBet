module PlayersDb

open Dapper
open DbCommon
open DbTypes

let getAllPlayers() = 
    DbContext.Instance.Connection.Query<Player>("SELECT ID, Name FROM dbo.Players")

let getPlayerByID id = 
    let qp : QueryParamsID = { ID = id }
    DbContext.Instance.Connection.Query<Player>("SELECT ID, Name FROM dbo.Players WHERE ID = @ID", qp)
    |> Seq.exactlyOne

let createPlayer (player: Player) = 
    DbContext.Instance.Connection.Execute(
        """INSERT INTO dbo.Players(Name)
           VALUES (@Name)""", player)

let updatePlayer (player: Player) = 
    DbContext.Instance.Connection.Execute("""
            UPDATE dbo.Players
            SET Name = @Name
            WHERE ID = @ID""", player)

let deletePlayer (player: Player) =
    DbContext.Instance.Connection.Execute("""DELETE FROM dbo.Players WHERE ID = @ID""", player)
