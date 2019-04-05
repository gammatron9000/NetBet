module MatchService

open DbTypes

let getMatchByID matchID =
    MatchesDb.getMatchByID matchID |> Seq.exactlyOne

let getMatchesForEvent eventID =
    MatchesDb.getMatchesForEvent eventID |> Seq.toArray

let resolveMatch (m: Match) =
    MatchesDb.resolveMatch m

let deleteMatch matchID =
    BetsDb.deleteAllBetsForMatch matchID |> ignore
    MatchesDb.deleteMatch matchID

let createMatch (m: Match) = 
    MatchesDb.insertMatch m

