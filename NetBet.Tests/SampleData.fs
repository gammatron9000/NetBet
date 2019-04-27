module SampleData

open System
open DbTypes
open DataGenerators
open DbCommon

let seasons = 
    [| makeSeason "Season1"
       makeSeason "Season2" |]

let players = 
    [| "Dustin"
       "Jake"
       "Tony" 
       "Stephanie" |]

let fighters = 
    [| makeFighter "Gabe Ruediger"
       makeFighter "Jonathan Goulet"
       makeFighter "Kenneth Allen"
       makeFighter "Caleb Starnes"
       makeFighter "Garreth McLellan"
       makeFighter "Artem Lobov"
       makeFighter "Cody Pfister"
       makeFighter "Chris Condo"
       makeFighter "Ruan Potts"
       makeFighter "Charlie Brenneman"
       makeFighter "Emmanuel Yarbrough"
       makeFighter "Bob Sapp"
       makeFighter "Dada 5000"
       makeFighter "Joe Son"
       makeFighter "Tiki Ghosn"
       makeFighter "Elvis Sinosic"
       makeFighter "Mayhem Miller"
       makeFighter "Teila Tuli" |]



let insertSampleDataToDb () = 
    fighters |> Array.map FighterService.createFighter |> ignore
    players  |> Array.map SeasonService.createPlayer |> ignore
    seasons  |> Array.map SeasonService.createSeason |> ignore

    let fightersFromDb = FighterService.getAllFighters() |> Seq.toArray
    let playersFromDb  = SeasonService.getAllPlayers() |> Seq.toArray
    let seasonsFromDb  = SeasonService.getAllSeasons() |> Seq.toArray
    let season1ID = seasonsFromDb.[0].ID
    let season2ID = seasonsFromDb.[1].ID

    for s in seasonsFromDb do
        for p in playersFromDb do
            SeasonService.addPlayerToSeason s.ID p.ID |> ignore

    let events = 
        [| makeEvent season1ID "Event Number One"
           makeEvent season1ID "Event Number Two"
           makeEvent season2ID "Season 2 Event" |]
    events |> Array.iter (fun x -> x |> EventService.createEvent |> ignore)
    let season1Events = EventsDb.getEventsForSeason season1ID  |> Seq.toArray
    let season1Event1ID = season1Events.[0].ID
    let season1Event2ID = season1Events.[1].ID
    let season2Event = EventsDb.getEventsForSeason season2ID |> Seq.exactlyOne
    
    let matches = 
        [| makeMatch season1Event1ID fightersFromDb.[0].ID  fightersFromDb.[1].ID  1.90M 2.10M
           makeMatch season1Event1ID fightersFromDb.[2].ID  fightersFromDb.[3].ID  5.00M 1.10M
           makeMatch season1Event1ID fightersFromDb.[4].ID  fightersFromDb.[5].ID  1.25M 4.10M
           makeMatch season1Event1ID fightersFromDb.[6].ID  fightersFromDb.[7].ID  2.60M 1.41M
           makeMatch season1Event1ID fightersFromDb.[8].ID  fightersFromDb.[9].ID  2.33M 1.66M
           makeMatch season1Event1ID fightersFromDb.[10].ID fightersFromDb.[11].ID 4.32M 1.22M
           makeMatch season1Event1ID fightersFromDb.[12].ID fightersFromDb.[13].ID 10.0M 1.01M
           makeMatch season1Event1ID fightersFromDb.[14].ID fightersFromDb.[15].ID 2.01M 1.91M
           makeMatch season1Event1ID fightersFromDb.[16].ID fightersFromDb.[17].ID 2.75M 1.35M
           makeMatch season1Event2ID fightersFromDb.[0].ID  fightersFromDb.[12].ID 3.00M 1.50M
           makeMatch season1Event2ID fightersFromDb.[7].ID  fightersFromDb.[4].ID  1.78M 2.10M
           makeMatch season2Event.ID fightersFromDb.[15].ID fightersFromDb.[1].ID  5.00M 1.10M |]
    matches |> MatchService.createMatches |> ignore
    let event1Matches = MatchesDb.getMatchesForEvent season1Event1ID |> Seq.toArray
    let event2Matches = MatchesDb.getMatchesForEvent season1Event2ID |> Seq.toArray
    let event3Matches = MatchesDb.getMatchesForEvent season2Event.ID |> Seq.toArray
    
    // makeBet seasonID eventID matchID playerID fighterID parlayID stake
    let parlay1id = Guid.NewGuid() |> Nullable
    let parlay2id = Guid.NewGuid() |> Nullable
    let bets = 
        [| makeBet season1ID season1Event1ID event1Matches.[0].ID playersFromDb.[0].ID fightersFromDb.[1].ID  (Nullable()) 50M
           makeBet season1ID season1Event1ID event1Matches.[3].ID playersFromDb.[0].ID fightersFromDb.[6].ID  parlay1id 20M
           makeBet season1ID season1Event1ID event1Matches.[4].ID playersFromDb.[0].ID fightersFromDb.[9].ID  parlay1id 20M
           makeBet season1ID season1Event1ID event1Matches.[5].ID playersFromDb.[1].ID fightersFromDb.[10].ID (Nullable()) 10M
           makeBet season1ID season1Event2ID event2Matches.[0].ID playersFromDb.[2].ID fightersFromDb.[0].ID  parlay2id 30M
           makeBet season1ID season1Event2ID event2Matches.[1].ID playersFromDb.[2].ID fightersFromDb.[7].ID  parlay2id 30M
           makeBet season2ID season2Event.ID event3Matches.[0].ID playersFromDb.[3].ID fightersFromDb.[1].ID  (Nullable()) 100M |]
    bets |> Array.iter BetService.createBet
    

    ()

