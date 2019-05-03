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

let resolveMatch eventID matchID winnerID isDraw =
    MatchesDb.resolveMatch matchID winnerID isDraw |> ignore
    let m = getMatchByID matchID
    let evt = EventService.getEventByID eventID

    match denullBool(isDraw) with 
    | true -> BetService.pushBetsForMatch m.ID |> ignore
    | false -> BetService.resolveBets m evt.SeasonID
    ()