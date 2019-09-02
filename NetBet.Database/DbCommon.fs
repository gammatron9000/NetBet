module DbCommon

open System.Data.SqlClient
open System

type Db () =
    static let mutable cs = ""
    static member ConnectionString
        with get() = cs
        and set(v) = cs <- v

type DbConnection () = 
    let conn = new SqlConnection(Db.ConnectionString)
    do conn.Open() |> ignore
    member __.connection = conn
    member __.connectionString = Db.ConnectionString
    member __.openConnection () = 
        conn.Open() |> ignore
    member __.closeConnection () = 
        conn.Close() |> ignore
    interface IDisposable with
        member __.Dispose() = 
            conn.Close() |> ignore


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

