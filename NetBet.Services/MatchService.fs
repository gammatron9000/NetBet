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
    
let resolveMatch (m: Match) =
    let transaction = DbContext.Instance.Connection.BeginTransaction()
    MatchesDb.resolveMatch m |> ignore
    let evt = EventService.getEventByID m.EventID
    let season = SeasonService.getSeasonWithPlayers evt.SeasonID

    match denullBool(m.IsDraw) with 
    | true -> BetService.pushBetsForMatch m.ID |> ignore
    | false ->
        
        ()

    transaction.Commit() |> ignore
    ()