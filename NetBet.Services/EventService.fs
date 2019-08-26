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

let getEventIDByName eventName : int = 
    let event = EventsDb.getEventByName eventName |> Seq.exactlyOne
    event.ID

let getEventsForSeason seasonID = 
    EventsDb.getEventsForSeason seasonID |> Seq.toArray
    
let createOrUpdateEventWithMatches (ewm: EventWithPrettyMatches) =
    let evt = ewm.Event
    // update fighter IDs for each match (if the names were updated, the IDs need updating too)
    let fighterdict = FighterService.getFightersIDLookupByName()
    let matchesWithUpdatedFighterIDs = 
        ewm.Matches
        |> Array.map(fun m -> 
            let f1id = WebScraper.fighterLookup fighterdict m.Fighter1Name
            let f2id = WebScraper.fighterLookup fighterdict m.Fighter2Name
            { m with Fighter1ID = f1id; Fighter2ID = f2id } )
    let matches = matchesWithUpdatedFighterIDs |> Array.map Shared.mapPrettyMatchToMatch
    let eventID = EventsDb.insertOrUpdateEvent evt
    let matchesWithEventID = 
        matches
        |> Array.map (fun m -> { m with EventID = eventID })
    let existingMatchesForEvent = MatchesDb.getMatchesForEvent eventID |> Seq.toArray
    MatchesDb.insertOrUpdateMatches matchesWithEventID existingMatchesForEvent

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
      
let deleteEvent eventID =
    BetsDb.deleteAllBetsForEvent eventID |> ignore
    MatchesDb.deleteAllMatchesForEvent eventID |> ignore
    EventsDb.deleteEvent eventID
    
let getUpcomingEventsFromWeb () =
    WebScraper.CreateEventsFromScrape() 
    |> Array.map (fun x -> x |> WebScraper.mapScrapedEventToNetbetEvent)