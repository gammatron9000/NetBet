module EventService

open DbTypes
open DtoTypes

let getEventByID eventID = 
    EventsDb.getEventByID eventID |> Seq.exactlyOne

let getEventAndMatches eventID : EventWithMatches =
    let event = getEventByID eventID 
    let matches = MatchesDb.getMatchesForEvent eventID |> Seq.toArray
    { Event = event; Matches = matches }

let getEventsForSeason seasonID = 
    EventsDb.getEventsForSeason seasonID |> Seq.toArray

let createEvent (evt: Event) = 
    EventsDb.insertEvent evt

let createEventWithMatches (evt: Event) (matches: Match[]) =
    let eventID = EventsDb.insertEvent evt
    let matchesWithEventID = 
        matches
        |> Array.map (fun m -> { m with EventID = eventID })
    MatchesDb.insertMatches matchesWithEventID

let updateEvent (evt: Event) = 
    EventsDb.updateEvent evt

let deleteEvent eventID =
    BetsDb.deleteAllBetsForEvent eventID |> ignore
    MatchesDb.deleteAllMatchesForEvent eventID |> ignore
    EventsDb.deleteEvent eventID