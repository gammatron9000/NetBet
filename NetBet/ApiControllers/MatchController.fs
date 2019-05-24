namespace NetBet.Controllers

open System
open System.Collections.Generic
open System.Linq
open Microsoft.AspNetCore.Mvc
open DbTypes
open DtoTypes
open MatchService

[<Route("api/[controller]/[action]")>]
[<ApiController>]
type MatchController () =
    inherit ControllerBase()
    
    [<HttpGet("{matchid}")>]
    member __.GetById(matchid: int) =
        let m = MatchService.getMatchByID matchid
        ActionResult<Match>(m)

    [<HttpGet("{eventid}")>]
    member __.GetMatchesForEvent(eventid: int) =
        let m = MatchService.getMatchesForEvent eventid
        ActionResult<PrettyMatch[]>(m)
        
    [<HttpGet>]
    member __.GetEmpty() =
        let emptymatch = 
            { ID              = 0
              EventID         = 0
              Fighter1ID      = 0
              Fighter2ID      = 0
              Fighter1Odds    = 0.0M
              Fighter2Odds    = 0.0M
              WinnerFighterID = Nullable()
              LoserFighterID  = Nullable()
              IsDraw          = Nullable()
              DisplayOrder    = 0 } 
        ActionResult<Match>(emptymatch)
    
    [<HttpPost>]
    member __.ResolveMatch([<FromBody>] resolve: ResolveMatchDto) = 
        MatchService.resolveMatch resolve
        EventService.getFullEvent resolve.EventID // send back the updated full event

    [<HttpDelete("{id}")>]
    member __.Delete(id:int) =
        MatchService.deleteMatch id
