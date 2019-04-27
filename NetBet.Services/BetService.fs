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

let getPrettyBetsForEvent eventID = 
    BetsDb.getPrettyBets eventID

let createBet bet =
    BetsDb.insertBet bet |> ignore
    SeasonService.removePlayerMoney bet.SeasonID bet.PlayerID bet.Stake

let deleteBet (bet: Bet) =
    if bet.Result = Nullable() then 
        BetsDb.deleteBet bet
    else failwithf "This bet has already been resolved and cannot be deleted"
    
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

let areAllBetsWinOrPush (bets: BetWithOdds[]) =
    bets |> Array.forall(fun x -> (x.Result = Nullable(BetResult.Win.Code) || x.Result = Nullable(BetResult.Push.Code)) )

let pushBetsForMatch matchID =
    BetsDb.pushBetsForMatch matchID |> ignore
    // refund all non parlay bets
    let betsForThisMatch = getBetsForMatch matchID
    for b in betsForThisMatch do
        let refund =
            if b.ParlayID = Nullable() then
                b.Stake
            else
                let parlayedBets = getParlayedBets b
                match parlayedBets |> areAllBetsWinOrPush with
                | true -> b.Stake
                | false -> 0.0M
        
        match refund with 
        | 0.0M -> ()
        | _ -> SeasonService.givePlayerMoney b.SeasonID b.PlayerID refund
    ()

let resolveBets (m: Match) (seasonID: int) = 
    BetsDb.resolveBetWinners seasonID m.EventID m.ID (denullInt(m.WinnerFighterID)) |> ignore
    BetsDb.resolveBetLosers seasonID m.EventID m.ID (denullInt(m.LoserFighterID)) |> ignore
    
    let betsForThisMatch = getBetsForMatch m.ID
    let winningBets = 
        betsForThisMatch
        |> Array.filter(fun x -> Nullable(x.FighterID) = m.WinnerFighterID && not(denullBool(m.IsDraw)) )
    let losingBets = 
        betsForThisMatch
        |> Array.filter(fun x -> Nullable(x.FighterID) = m.LoserFighterID && not(denullBool(m.IsDraw)) )
    let winnerID = 
        if m.WinnerFighterID.HasValue then 
            failwith "Error: WinnerFighterID was null when trying to resolve bet"
        else m.WinnerFighterID.Value
    let winnerOdds = 
        match winnerID with
        | x when x = m.Fighter1ID -> m.Fighter1Odds
        | x when x = m.Fighter2ID -> m.Fighter2Odds
        | _ -> failwithf "WinnerFighterID %i doesnt match either of the figthters for this Match" winnerID
    
    // mark all other parlays as lose for losers
    for b in losingBets do
        if b.ParlayID = Nullable() then ()
        else BetsDb.resolveParlayBetLose seasonID m.EventID m.ID b.PlayerID b.ParlayID |> ignore

    // award winners with CASH PRIZES
    for b in winningBets do
        let winAmount =
            if b.ParlayID = Nullable() then
                calculateBetWin b.Stake winnerOdds
            else
                let parlayedBets = getParlayedBets b
                let parlayOdds = parlayedBets |> Array.map(fun x -> x.Odds)
                match parlayedBets |> areAllBetsWinOrPush with
                | true -> calculateParlayBetWin b.Stake parlayOdds
                | false -> 0.0M
        
        match winAmount with 
        | 0.0M -> ()
        | _ -> SeasonService.givePlayerMoney b.SeasonID b.PlayerID winAmount

    ()