namespace NetBet.Controllers

open System
open System.Collections.Generic
open System.Linq
open Microsoft.AspNetCore.Mvc
open DbTypes
open DtoTypes

[<Route("api/[controller]/[action]")>]
[<ApiController>]
type EventController () =
    inherit ControllerBase()
    
    [<HttpGet>]
    member __.Get() =
        let players = SeasonService.getAllPlayers()
        ActionResult<Player[]>(players)

    [<HttpGet("{eventid}")>]
    member __.GetById(eventid: int) =
        let evt = EventService.getEventByID eventid
        ActionResult<Event>(evt)

    [<HttpGet("{seasonid}")>]
    member __.GetEventsForSeason(seasonid: int) =
        let evts = EventService.getEventsForSeason seasonid
        ActionResult<Event[]>(evts)

    [<HttpGet("{eventid}")>]
    member __.GetEventWithMatches(eventid: int) = 
        let evt = EventService.getEventAndMatches eventid
        ActionResult<EventWithMatches>(evt)

    [<HttpGet("{eventid}")>]
    member __.GetFullEvent(eventid: int) = 
        let evt = EventService.getFullEvent eventid
        ActionResult<FullEvent>(evt)
        
    [<HttpGet>]
    member __.GetEmpty() =
        let emptyevt = 
            { ID        = 0
              SeasonID  = 0
              Name      = ""
              StartTime = DateTime.Now }
        let emptyEventWithMatches = 
            { Event = emptyevt
              Matches = [| |] }
        ActionResult<EventWithMatches>(emptyEventWithMatches)
        
    [<HttpPost>]
    member __.CreateOrUpdate([<FromBody>] evt: EventWithMatches) =
        EventService.createEventWithMatches evt
        
    [<HttpDelete("{id}")>]
    member __.Delete(id:int) =
        EventService.deleteEvent id
