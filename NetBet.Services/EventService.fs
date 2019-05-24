module EventService

open DbTypes
open DtoTypes
open BetsDb
open SeasonPlayersDb

let getEventByID eventID = 
    EventsDb.getEventByID eventID |> Seq.exactlyOne

let getEventAndMatches eventID : EventWithPrettyMatches =
    let event = getEventByID eventID 
    let matches = MatchesDb.getPrettyMatchesForEvent eventID |> Seq.toArray
    { Event = event; Matches = matches }

let getEventsForSeason seasonID = 
    EventsDb.getEventsForSeason seasonID |> Seq.toArray

let createEvent (evt: Event) = 
    EventsDb.insertEvent evt

let createEventWithMatches (ewm: EventWithPrettyMatches) =
    let evt = ewm.Event
    let prettyMatches = ewm.Matches
    let matches = prettyMatches |> Array.map Shared.mapPrettyMatchToMatch
    let eventID = EventsDb.insertEvent evt
    let matchesWithEventID = 
        matches
        |> Array.map (fun m -> { m with EventID = eventID })
    MatchesDb.insertMatches matchesWithEventID

let getFullEvent eventID = 
    let ewm = getEventAndMatches eventID
    let evt = ewm.Event
    let matches = ewm.Matches
    let bets = getPrettyBets eventID |> Seq.toArray
    let sp = getPlayersForSeason evt.SeasonID |> Seq.toArray
    { Event = evt
      Matches = matches
      Bets = bets
      Players = sp }


let updateEvent (evt: Event) = 
    EventsDb.updateEvent evt

let deleteEvent eventID =
    BetsDb.deleteAllBetsForEvent eventID |> ignore
    MatchesDb.deleteAllMatchesForEvent eventID |> ignore
    EventsDb.deleteEvent eventID
    
let getUpcomingEventsFromWeb seasonID =
    WebScraper.CreateEventsFromScrape() 
    |> Array.map (fun x -> x |> WebScraper.mapScrapedEventToNetbetEvent seasonID)