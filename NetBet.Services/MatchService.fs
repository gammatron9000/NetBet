module MatchService

open DbTypes
open DtoTypes
open Shared

let getMatchByID matchID =
    MatchesDb.getMatchByID matchID |> Seq.exactlyOne

let getMatchesForEvent eventID =
    MatchesDb.getPrettyMatchesForEvent eventID |> Seq.toArray

let deleteMatch matchID =
    BetsDb.deleteAllBetsForMatch matchID |> ignore
    MatchesDb.deleteMatch matchID

let createMatch (m: Match) = 
    MatchesDb.insertMatch m

let createMatches (matches: Match[]) = 
    MatchesDb.insertMatches matches

let validateMatch seasonID eventID matchID = 
    let m = getMatchByID matchID
    let e = EventService.getEventByID eventID
    match m.EventID with 
    | x when x = eventID -> ()
    | _ -> failwithf "Match %i does not belong to Event %i" matchID eventID
    match e.SeasonID with 
    | x when x = seasonID -> ()
    | _ -> failwithf "Error Trying to resolve Match %i : Event %i does not belong to Season %i" matchID eventID seasonID


let resolveMatch (res: ResolveMatchDto) =
    validateMatch res.SeasonID res.EventID res.MatchID
    MatchesDb.resolveMatch res.MatchID res.WinnerID res.IsDraw |> ignore
    
    match denullBool(res.IsDraw) with 
    | true -> BetService.pushBetsForMatch res.MatchID |> ignore
    | false -> BetService.resolveBets res.SeasonID res.EventID res.MatchID
    ()