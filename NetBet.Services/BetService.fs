module BetService

open DbTypes
open System
open Shared
open DtoTypes
open FightersDb

let getBetsForMatch matchID =
    BetsDb.getBetsForMatch matchID |> Seq.toArray

let getBetsForEvent eventID =
    BetsDb.getBetsForEvent eventID |> Seq.toArray

let getBetsForPlayerForSeason playerID seasonID =
    BetsDb.getBetsForPlayerForSeason playerID seasonID |> Seq.toArray

let getParlayedBets (b: BetWithOdds) = 
    BetsDb.getParlayedBets b |> Seq.toArray

let getPrettyBetsForEvent eventID = 
    BetsDb.getPrettyBets eventID |> Seq.toArray

let internal validateBet (b: Bet) =
    // verify player is in this season
    let sp = SeasonService.getSeasonPlayer b.SeasonID b.PlayerID
    
    // verify player has enough money
    let moneyAfterBet = sp.CurrentCash - b.Stake
    match moneyAfterBet with
    | x when x >= 0M -> ()
    | _ -> failwith "player does not have enough money to make this bet"

    // verify fighter is in this match
    let m = MatchesDb.getMatchByID b.MatchID |> Seq.exactlyOne
    match b.FighterID with 
    | x when x = m.Fighter1ID || x = m.Fighter2ID -> ()
    | _ -> failwithf "fighterid %i is not in match %i" b.FighterID b.MatchID


let placeSingleBet (bet: Bet) =
    bet |> validateBet
    let parlayID = Guid.NewGuid()
    let betWithParlayID = { bet with ParlayID = parlayID }
    BetsDb.insertBet betWithParlayID |> ignore
    SeasonService.removePlayerMoney bet.SeasonID bet.PlayerID bet.Stake

let placeParlayBet (bets: Bet[]) = 
    let parlayID = Guid.NewGuid()
    let stake    = bets |> Array.map (fun x -> x.Stake)    |> Array.distinct |> Array.exactlyOne
    let seasonID = bets |> Array.map (fun x -> x.SeasonID) |> Array.distinct |> Array.exactlyOne
    let playerID = bets |> Array.map (fun x -> x.PlayerID) |> Array.distinct |> Array.exactlyOne

    for b in bets do
        b |> validateBet
        let betWithParlayID = { b with ParlayID = parlayID }
        BetsDb.insertBet betWithParlayID |> ignore
    SeasonService.removePlayerMoney seasonID playerID stake

let deleteBet (bet: Bet) =
    if bet.Result = Nullable() then 
        SeasonService.givePlayerMoney bet.SeasonID bet.PlayerID bet.Stake // give the player a refund
        BetsDb.deleteBet bet
    else failwithf "This bet has already been resolved and cannot be deleted"
    
let calculateBetWin (stake: decimal) (odds: decimal) = 
    let raw = stake * odds
    Math.Round(raw, 2)

let calculateParlayBetWin (stake: decimal) (odds: decimal[]) = 
    let parlayOdds = odds |> Array.reduce (*) // multiply all odds together
    match parlayOdds with 
    | 0M -> failwith "danger: attempting to calculate parlay win and odds is 0"
    | _  -> calculateBetWin stake parlayOdds

let isBetWinner (b: BetWithOdds) =
    if b.Result.HasValue then 
        if b.Result.Value = BetResult.Win.Code then 
            true
        else false
    else false

let areAllBetsWin (bets: BetWithOdds[]) = 
    bets |> Array.forall(fun x -> x.Result = (Nullable(BetResult.Win.Code)))
let areAllBetsWinOrPush (bets: BetWithOdds[]) =
    bets |> Array.forall(fun x -> (x.Result = Nullable(BetResult.Win.Code) || x.Result = Nullable(BetResult.Push.Code)) )
let areAnyBetsLose (bets: BetWithOdds[]) = 
    bets |> Array.exists(fun x -> x.Result = (Nullable(BetResult.Lose.Code)))
let areAllBetsResolved (bets: BetWithOdds[]) =
    bets |> Array.forall(fun x -> 
        x.Result = Nullable(BetResult.Win.Code) || 
        x.Result = Nullable(BetResult.Push.Code) ||
        x.Result = Nullable(BetResult.Lose.Code))

let pushBetsForMatch matchID =
    BetsDb.pushBetsForMatch matchID |> ignore
    // refund all non parlay bets
    let betsForThisMatch = getBetsForMatch matchID
    for b in betsForThisMatch do
        let refund =
            let parlayedBets = getParlayedBets b
            if areAllBetsResolved parlayedBets then
                if areAnyBetsLose parlayedBets then
                    None
                else if areAllBetsWinOrPush parlayedBets then
                    Some(b.Stake)
                else None
            else None
        
        match refund with 
        | None -> ()
        | Some x -> SeasonService.givePlayerMoney b.SeasonID b.PlayerID x
    ()

let resolveBets (seasonID: int) (eventID: int) (matchID: int) = 
    let m = MatchesDb.getMatchByID matchID |> Seq.exactlyOne
    BetsDb.resolveBetWinners seasonID eventID matchID (denullInt(m.WinnerFighterID)) |> ignore
    BetsDb.resolveBetLosers seasonID eventID matchID (denullInt(m.LoserFighterID)) |> ignore
    
    let betsForThisMatch = getBetsForMatch m.ID
    for b in betsForThisMatch do
        let parlayedBets = getParlayedBets b
        let winAmount = 
            // all bets in parlay must be resolved before any money is given out
            if areAllBetsResolved parlayedBets then
                // lose = nothing
                if areAnyBetsLose parlayedBets then
                    None
                // all winners = gimmedacassshhh
                else if areAllBetsWin parlayedBets then
                    let parlayOdds = parlayedBets |> Array.map(fun x -> x.Odds)
                    calculateParlayBetWin b.Stake parlayOdds |> Some
                // if there were any pushes and everything is win, you get your money back
                else if areAllBetsWinOrPush parlayedBets then
                    Some(b.Stake)
                else None
            else None

        match winAmount with 
        | None   -> ()
        | Some x -> SeasonService.givePlayerMoney b.SeasonID b.PlayerID x
    ()


let getPercentOfWinningBets (bets: PrettyBet[]) =
    let parlayGroups = bets |> Array.groupBy(fun x -> x.ParlayID)
    // remove bets where ALL of the results are push
    let allPushes = 
        parlayGroups
        |> Array.filter(fun (_, parlayBets) -> 
            parlayBets |> Array.forall(fun x -> x.Result = Nullable(Push.Code))) 
        |> Array.map(fun (guid, _) -> guid)
    let allNonPushGroups = 
        parlayGroups
        |> Array.filter(fun (guid, bets) -> allPushes |> Array.contains(guid) |> not)
    let winCount =
        allNonPushGroups
        |> Array.filter(fun (_, parlayBets) -> 
            parlayBets 
            |> Array.forall(fun x -> x.Result = Nullable(Win.Code) || x.Result = Nullable(Push.Code)))
        |> Array.length 
        |> decimal
    winCount / (allNonPushGroups.Length |> decimal)


let getBetStatsForSeason (seasonID: int) = 
    BetsDb.getAllBetsForSeason seasonID 
    |> Seq.toArray 
    |> Array.groupBy(fun x -> x.PlayerName)
    |> Array.map(fun (player, bets) -> 
        { PlayerName = player
          WinPercent = bets |> getPercentOfWinningBets })
    