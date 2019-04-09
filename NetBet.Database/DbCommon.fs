module DbCommon

open System.Data.SqlClient
open System

let connectionString = "Server=localhost\SQLEXPRESS; Database=NetBetDb; Integrated Security=true;"

type DbContext() =
    let connection = new SqlConnection(connectionString)
    static let instance = new DbContext()
    do connection.Open() |> ignore

    member __.Connection = connection
    static member Instance = instance
    
    interface IDisposable with
        member this.Dispose() = 
            this.Connection.Close() |> ignore
    

type QueryParamsID = { ID: int }
type QueryParamsSeasonPlayer = 
    { SeasonID: int
      PlayerID: int }
type QueryParamsCurrentCash =
    { CurrentCash: decimal }
type QueryParamsName =
    { Name: string }
type QueryParamsResolveBet =
    { SeasonID: int
      EventID: int 
      MatchID: int
      FighterID: int }
type QueryParamsParlayedBets = 
    { SeasonID: int
      EventID: int
      MatchID: int
      PlayerID: int
      ParlayID: Nullable<Guid> }