module DbCommon

open System.Data.SqlClient
open System

type Db () =
    static let mutable connection = new SqlConnection()
    static let mutable cs = ""
    
    static member CreateConnection() =
        if String.IsNullOrWhiteSpace(cs) 
        then failwith "ERROR: ConnectionString is empty"
        else 
            connection <- new SqlConnection(cs)
            connection.Open() |> ignore
            connection

    static member ConnectionString
        with get() = cs
        and set(v) = cs <- v
    
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
      PlayerID: int
      ParlayID: Guid }
type QueryParamsParlayID =
    { ParlayID: Guid }
type QueryParamsEventID = 
    { EventID: int }
type QueryParamsResolveMatch =
    { MatchID: int
      WinnerID: Nullable<int>
      IsDraw: Nullable<bool> }

