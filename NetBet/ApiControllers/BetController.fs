namespace NetBet.Controllers

open System
open System.Collections.Generic
open System.Linq
open Microsoft.AspNetCore.Mvc
open DbTypes
open DtoTypes

[<Route("api/[controller]/[action]")>]
[<ApiController>]
type BetController () =
    inherit ControllerBase()
    
    [<HttpGet("matchid")>]
    member __.GetBetsForMatch matchid =
        let bets = BetService.getBetsForMatch matchid
        ActionResult<BetWithOdds[]>(bets)

    [<HttpGet("eventid")>]
    member __.GetBetsForEvent eventid =
        let bets = BetService.getBetsForEvent eventid
        ActionResult<BetWithOdds[]>(bets)
        
    [<HttpGet("eventid")>]
    member __.GetPrettyBetsForEvent eventid = 
        let bets = BetService.getPrettyBetsForEvent eventid
        ActionResult<PrettyBet[]>(bets)

    [<HttpPost>]
    member __.PlaceSingleBet([<FromBody>] b: Bet) =
        BetService.placeSingleBet b

    [<HttpPost>]
    member __.PlaceParlayBet([<FromBody>] bets: Bet[]) = 
        BetService.placeParlayBet bets
        
    [<HttpPost>]
    member __.Delete([<FromBody>] b: Bet) =
        BetService.deleteBet b

    