module DbCommon

open System.Data.SqlClient
open System

//let connectionString = "Server=localhost\SQLEXPRESS; Database=NetBetDb; Integrated Security=true;"

type DbContext () =
    static let mutable connection = new SqlConnection("")
    static member OpenConnection()  = connection.Open() |> ignore
    static member CloseConnection() = connection.Close() |> ignore

    static member Connection 
        with get() = connection
        and set(v) = 
            connection <- v
    
    interface IDisposable with
        member __.Dispose() = 
            DbContext.Connection.Close() |> ignore
    

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
type QueryParamsParlayID =
    { ParlayID: Guid }