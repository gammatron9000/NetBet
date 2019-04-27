module MatchService

open DbTypes
open DbCommon
open Shared

let getMatchByID matchID =
    MatchesDb.getMatchByID matchID |> Seq.exactlyOne

let getMatchesForEvent eventID =
    MatchesDb.getMatchesForEvent eventID |> Seq.toArray

let deleteMatch matchID =
    BetsDb.deleteAllBetsForMatch matchID |> ignore
    MatchesDb.deleteMatch matchID

let createMatch (m: Match) = 
    MatchesDb.insertMatch m

let createMatches (matches: Match[]) = 
    MatchesDb.insertMatches matches

let resolveMatch (m: Match) =
    use connection = Db.CreateConnection()
    use transaction = connection.BeginTransaction()
    MatchesDb.resolveMatch m |> ignore
    let evt = EventService.getEventByID m.EventID

    match denullBool(m.IsDraw) with 
    | true -> BetService.pushBetsForMatch m.ID |> ignore
    | false -> BetService.resolveBets m evt.SeasonID

    transaction.Commit() |> ignore
    ()