module BetService

open DbTypes
open DbTypes
open System

let getBetsForMatch matchID =
    BetsDb.getBetsForMatch matchID |> Seq.toArray

let getBetsForEvent eventID =
    BetsDb.getBetsForEvent eventID |> Seq.toArray

let getBetsForPlayerForSeason playerID seasonID =
    BetsDb.getBetsForPlayerForSeason playerID seasonID |> Seq.toArray

let createBet bet =
    BetsDb.insertBet bet

let deleteBet (bet: Bet) =
    if bet.Result = Nullable() then 
        BetsDb.deleteBet bet
    else failwithf "This bet has already been resolved and cannot be deleted"

// todo: 
// bet resolution stuff