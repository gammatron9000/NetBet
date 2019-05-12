namespace NetBet.Controllers

open System
open System.Collections.Generic
open System.Linq
open Microsoft.AspNetCore.Mvc
open DbCommon
open DbTypes

[<Route("api/[controller]")>]
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

    [<HttpGet("{name}")>]
    member __.GetByName(name: string) =
        let season = SeasonService.getSeasonByName(name)
        ActionResult<Season>(season)

    [<HttpGet>]
    member __.GetEmpty() =
        let empty = 
            { ID            = 0
              Name          = ""
              StartTime     = DateTime.Now
              EndTime       = DateTime.Now
              StartingCash  = 0
              MinimumCash   = 0
              MaxParlaySize = 0 }
        ActionResult<Season>(empty)

    [<HttpPost>]
    member __.Create([<FromBody>] season: Season) =
        SeasonService.createOrUpdateSeason season |> ignore
        
    [<HttpDelete("{id}")>]
    member __.Delete(id:int) =
        SeasonService.deleteSeason id
