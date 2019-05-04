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
    EventsDb.insertEvent evt |> ignore
    MatchesDb.insertMatches matches

let updateEvent (evt: Event) = 
    EventsDb.updateEvent evt

let deleteEvent eventID =
    let betsDeleted = BetsDb.deleteAllBetsForEvent eventID
    let matchesDeleted = MatchesDb.deleteAllMatchesForEvent eventID
    EventsDb.deleteEvent eventID