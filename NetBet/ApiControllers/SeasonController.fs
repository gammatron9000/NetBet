namespace NetBet.Controllers

open System
open System.Collections.Generic
open System.Linq
open Microsoft.AspNetCore.Mvc
open DbTypes
open DtoTypes

[<Route("api/[controller]/[action]")>]
[<ApiController>]
type SeasonController () =
    inherit ControllerBase()
    
    [<HttpGet>]
    member __.Get() =
        let seasons = SeasonService.getAllSeasons()
        ActionResult<Season[]>(seasons)

    [<HttpGet("{id}")>]
    member __.GetById(id:int) =
        let season = SeasonService.getSeasonByID(id)
        ActionResult<Season>(season)

    [<HttpGet>]
    member __.GetEmpty() =
        let emptySeason = 
            { ID            = 0
              Name          = ""
              StartTime     = DateTime.Now
              EndTime       = DateTime.Now
              StartingCash  = 0
              MinimumCash   = 0
              MaxParlaySize = 0 }
        let empty = 
            { Season = emptySeason
              Players = [| |] }
        ActionResult<SeasonWithPlayers>(empty)

    [<HttpPost>]
    member __.Create([<FromBody>] sp: SeasonWithPlayers) =
        SeasonService.createSeasonWithPlayers sp |> ignore
        
    [<HttpDelete("{id}")>]
    member __.Delete(id:int) =
        SeasonService.deleteSeason id
