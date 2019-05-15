namespace NetBet.Controllers

open System
open System.Collections.Generic
open System.Linq
open Microsoft.AspNetCore.Mvc
open DbTypes
open DtoTypes

[<Route("api/[controller]")>]
[<ApiController>]
type PlayerController () =
    inherit ControllerBase()
    
    [<HttpGet>]
    member __.Get() =
        let players = SeasonService.getAllPlayers()
        ActionResult<Player[]>(players)

    [<HttpGet("{seasonid}")>]
    member __.GetPlayersForSeason(seasonid: int) =
        let sp = SeasonService.getPlayersForSeason seasonid
        ActionResult<SeasonPlayer[]>(sp)
        
    [<HttpGet>]
    member __.GetEmpty() =
        let emptyPlayer = 
            { ID            = 0
              Name          = ""}
        ActionResult<Player>(emptyPlayer)
        
    [<HttpPost>]
    member __.CreateOrUpdate([<FromBody>] p: Player) =
        SeasonService.createOrUpdatePlayer p |> ignore
        
    [<HttpDelete("{id}")>]
    member __.Delete(id:int) =
        SeasonService.deletePlayer id
