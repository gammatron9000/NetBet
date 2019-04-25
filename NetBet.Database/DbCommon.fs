module DbCommon

open System.Data.SqlClient
open System

type Cs() = 
    static let mutable cs = ""
    static member ConnectionString
        with get() = cs
        and set(v) = cs <- v


type Db () =
    static let mutable connection = new SqlConnection()
    static member CreateConnection() =
        connection <- new SqlConnection(Cs.ConnectionString)
        connection.Open() |> ignore
        connection
    
    interface IDisposable with
        member __.Dispose() = 
            connection.Close() |> ignore


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