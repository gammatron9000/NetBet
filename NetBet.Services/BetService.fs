module BetService

open DbTypes
open System
open Shared
open DtoTypes

let getBetsForMatch matchID =
    BetsDb.getBetsForMatch matchID |> Seq.toArray

let getBetsForEvent eventID =
    BetsDb.getBetsForEvent eventID |> Seq.toArray

let getBetsForPlayerForSeason playerID seasonID =
    BetsDb.getBetsForPlayerForSeason playerID seasonID |> Seq.toArray

let getParlayedBets (b: BetWithOdds) = 
    BetsDb.getParlayedBets b |> Seq.toArray

let createBet bet =
    BetsDb.insertBet bet
    // subtract from player current cash

let deleteBet (bet: Bet) =
    if bet.Result = Nullable() then 
        BetsDb.deleteBet bet
    else failwithf "This bet has already been resolved and cannot be deleted"

let pushBetsForMatch matchID =
    BetsDb.pushBetsForMatch matchID

let calculateBetWin (stake: decimal) (odds: decimal) = 
    let raw = (stake * (odds - 1.0M))
    Math.Round(raw, 2)

let calculateParlayBetWin (stake: decimal) (odds: decimal[]) = 
    let parlayOdds = odds |> Array.reduce (*) // multiply all odds together
    calculateBetWin stake parlayOdds

let isBetWinner (b: BetWithOdds) =
    if b.Result.HasValue then 
        if b.Result.Value = BetResult.Win.Code then 
            true
        else false
    else false

let resolveBets (m: Match) (s: Season) (players: SeasonPlayer[]) = 
    let betsForThisMatch = getBetsForMatch m.ID
    let winningBets = 
        betsForThisMatch
        |> Array.filter(fun x -> Nullable(x.FighterID) = m.WinnerFighterID && not(denullBool(m.IsDraw)) )
    let winnerID = 
        if m.WinnerFighterID.HasValue then 
            failwith "Error: WinnerFighterID was null when trying to resolve bet"
        else m.WinnerFighterID.Value
    let winnerOdds = 
        match winnerID with
        | x when x = m.Fighter1ID -> m.Fighter1Odds
        | x when x = m.Fighter2ID -> m.Fighter2Odds
        | _ -> failwithf "WinnerFighterID %i doesnt match either of the figthters for this Match" winnerID
    
    // resolve bets
    BetsDb.resolveBetWinners s.ID m.EventID m.ID (denullInt(m.WinnerFighterID)) |> ignore
    BetsDb.resolveBetLosers s.ID m.EventID m.ID (denullInt(m.LoserFighterID)) |> ignore

    // mark all other parlays as lose for losers

    // award winners with CASH PRIZES
    for b in winningBets do
        let player = SeasonService.getSeasonPlayer b.SeasonID b.PlayerID

        let winAmount =
            if b.ParlayID = Nullable() then
                calculateBetWin b.Stake winnerOdds
            else
                let parlayedBets = getParlayedBets b
                let parlayOdds = parlayedBets |> Array.map(fun x -> x.Odds)
                match parlayedBets |> Array.forall(fun x -> x.Result = Nullable(BetResult.Win.Code)) with
                | true -> calculateParlayBetWin b.Stake parlayOdds
                | false -> 0.0M
        ()

    ()