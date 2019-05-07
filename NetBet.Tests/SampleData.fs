module SampleData

open System
open DbTypes
open DataGenerators
open Xunit

let seasons = 
    [| makeSeason "Season1"
       makeSeason "Season2" |]

let players = 
    [| "Dustin"
       "Jake"
       "Tony" 
       "Stephanie" |]

let fighters = 
    [| "Gabe Ruediger"
       "Jonathan Goulet"
       "Kenneth Allen"
       "Caleb Starnes"
       "Garreth McLellan"
       "Artem Lobov"
       "Cody Pfister"
       "Chris Condo"
       "Ruan Potts"
       "Charlie Brenneman"
       "Emmanuel Yarbrough"
       "Bob Sapp"
       "Dada 5000"
       "Joe Son"
       "Tiki Ghosn"
       "Elvis Sinosic"
       "Mayhem Miller"
       "Teila Tuli" |]
       
let insertSampleDataToDb () = 
    fighters |> Array.map FighterService.getOrInsertFighterIDByName |> ignore
    players  |> Array.map SeasonService.createPlayer |> ignore
    seasons  |> Array.map SeasonService.createSeason |> ignore
    let fighterMap = FighterService.getFightersIDLookupByName()
    let ruediger  = fighterMap.["Gabe Ruediger"]
    let goulet    = fighterMap.["Jonathan Goulet"]
    let allen     = fighterMap.["Kenneth Allen"]
    let starnes   = fighterMap.["Caleb Starnes"]
    let mclellan  = fighterMap.["Garreth McLellan"]
    let lobov     = fighterMap.["Artem Lobov"]
    let pfister   = fighterMap.["Cody Pfister"]
    let condo     = fighterMap.["Chris Condo"]
    let potts     = fighterMap.["Ruan Potts"]
    let brenneman = fighterMap.["Charlie Brenneman"]
    let yarbrough = fighterMap.["Emmanuel Yarbrough"]
    let sapp      = fighterMap.["Bob Sapp"]
    let dada      = fighterMap.["Dada 5000"]
    let son       = fighterMap.["Joe Son"]
    let ghosn     = fighterMap.["Tiki Ghosn"]
    let sinosic   = fighterMap.["Elvis Sinosic"]
    let miller    = fighterMap.["Mayhem Miller"]
    let tuli      = fighterMap.["Teila Tuli"]
    let playersFromDb  = SeasonService.getAllPlayers() |> Seq.toArray
    let playerMap = 
        playersFromDb
        |> Array.map(fun x -> x.Name, x.ID)
        |> Map.ofArray

    let seasonsFromDb  = SeasonService.getAllSeasons() |> Seq.toArray
    let season1ID = seasonsFromDb.[0].ID
    let season2ID = seasonsFromDb.[1].ID

    for s in seasonsFromDb do
        for p in playersFromDb do
            SeasonService.addPlayerToSeason s.ID p.ID |> ignore
            
    let s1e1 = makeEvent season1ID "Event Number One"
    let s1e2 = makeEvent season1ID "Event Number Two"
    let s2e1 = makeEvent season2ID "Season 2 Event"
    
    let s1e1Matches = 
        [| makeMatch ruediger  goulet    1.90M 2.10M   
           makeMatch allen     starnes   5.00M 1.10M   
           makeMatch mclellan  lobov     1.25M 4.10M   
           makeMatch condo     pfister   2.60M 1.41M   
           makeMatch potts     brenneman 2.33M 1.66M   
           makeMatch yarbrough sapp      4.32M 1.22M   
           makeMatch dada      son       10.0M 1.01M   
           makeMatch ghosn     sinosic   2.01M 1.91M   
           makeMatch miller    tuli      2.75M 1.35M |]

    let s1e2Matches = 
        [| makeMatch ruediger  dada      3.00M 1.50M
           makeMatch condo     mclellan  1.78M 2.10M
           makeMatch sapp      potts     1.90M 1.90M |]

    let s2e1Matches = 
        [| makeMatch sinosic   goulet    5.00M 1.10M |]

    EventService.createEventWithMatches s1e1 s1e1Matches |> ignore
    EventService.createEventWithMatches s1e2 s1e2Matches |> ignore
    EventService.createEventWithMatches s2e1 s2e1Matches |> ignore

    let season1Events = EventsDb.getEventsForSeason season1ID  |> Seq.toArray
    let season1Event1ID = season1Events.[0].ID
    let season1Event2ID = season1Events.[1].ID
    let season2Event = EventsDb.getEventsForSeason season2ID |> Seq.exactlyOne
    
    let event1Matches = MatchesDb.getMatchesForEvent season1Event1ID |> Seq.toArray
    let event2Matches = MatchesDb.getMatchesForEvent season1Event2ID |> Seq.toArray
    let event3Matches = MatchesDb.getMatchesForEvent season2Event.ID |> Seq.toArray
    
    let dustin = playerMap.["Dustin"]
    let jake   = playerMap.["Jake"]
    let tony   = playerMap.["Tony"]
    let steph  = playerMap.["Stephanie"]

    let singleBets = 
        [| makeBet season1ID season1Event1ID event1Matches.[0].ID dustin goulet   50M
           makeBet season1ID season1Event1ID event1Matches.[0].ID jake   ruediger 10M
           makeBet season1ID season1Event1ID event1Matches.[6].ID jake   son      10M
           makeBet season1ID season1Event1ID event1Matches.[8].ID jake   tuli     10M
           makeBet season2ID season2Event.ID event3Matches.[0].ID steph  goulet   100M |]
    singleBets |> Array.iter BetService.placeSingleBet
        
    let parlay1 =  // both win
        [| makeBet season1ID season1Event1ID event1Matches.[3].ID dustin pfister   20M
           makeBet season1ID season1Event1ID event1Matches.[4].ID dustin brenneman 20M |]
    let parlay2 =  // ruediger wins, condo draws
        [| makeBet season1ID season1Event2ID event2Matches.[0].ID tony ruediger 10M
           makeBet season1ID season1Event2ID event2Matches.[1].ID tony condo    10M |]
    let parlay3 =  // condo draws, ruediger wins
        [| makeBet season1ID season1Event2ID event2Matches.[1].ID tony condo    10M
           makeBet season1ID season1Event2ID event2Matches.[0].ID tony ruediger 10M |]
    let parlay4 =  // sapp draws, mclellan draws
        [| makeBet season1ID season1Event2ID event2Matches.[2].ID tony sapp     10M
           makeBet season1ID season1Event2ID event2Matches.[1].ID tony mclellan 10M |]
    let parlay5 =  // dada loses, potts draws
        [| makeBet season1ID season1Event2ID event2Matches.[0].ID tony dada  10M
           makeBet season1ID season1Event2ID event2Matches.[2].ID tony potts 10M |]
    let parlay6 =  // potts draws, dada loses
        [| makeBet season1ID season1Event2ID event2Matches.[2].ID tony potts 10M 
           makeBet season1ID season1Event2ID event2Matches.[0].ID tony dada  10M |]
    [| parlay1; parlay2; parlay3; parlay4; parlay5; parlay6 |] |> Array.iter BetService.placeParlayBet
    
    let dustinS1 = SeasonService.getSeasonPlayer season1ID dustin 
    let jakeS1   = SeasonService.getSeasonPlayer season1ID jake
    let tonyS1   = SeasonService.getSeasonPlayer season1ID tony
    let stephS2  = SeasonService.getSeasonPlayer season2ID steph
    Assert.Equal(930M, dustinS1.CurrentCash)
    Assert.Equal(970M, jakeS1.CurrentCash)
    Assert.Equal(950M, tonyS1.CurrentCash)
    Assert.Equal(900M, stephS2.CurrentCash)

    ()

