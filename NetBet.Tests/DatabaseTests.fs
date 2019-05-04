﻿module DatabaseTests

open System
open System.Collections.Generic
open System.Linq
open Xunit
open DatabaseFixture
open DbCommon
open SampleData

let dbName = "NetBetDbTest"
let connectionString = dropDatabase(dbName)
let dbUpgradeResult = upgradeDb(connectionString)
Db.ConnectionString <- connectionString
SampleData.insertSampleDataToDb() |> ignore

[<Fact>]
let dbUpgradeTest() =
    Assert.Equal(true, dbUpgradeResult.Successful)
    Assert.Equal(2, dbUpgradeResult.Scripts.Count())

[<Fact>]
let checkDbSetup() = 
    Assert.Equal(connectionString, Db.ConnectionString) // force eval all the lazy crap
    let allSeasons = SeasonService.getAllSeasons()
    let allPlayers = SeasonService.getAllPlayers()
    let allFighters = FighterService.getAllFighters()
    
    Assert.Equal(2, allSeasons.Length)
    Assert.Equal(4, allPlayers.Length)
    Assert.Equal(18, allFighters.Length)

    let season1 = allSeasons |> Seq.head
    let season2 = allSeasons |> Seq.tail |> Seq.exactlyOne

    let eventsS1 = EventService.getEventsForSeason season1.ID
    let eventsS2 = EventService.getEventsForSeason season2.ID
    Assert.Equal(2, eventsS1.Length)
    Assert.Equal(1, eventsS2.Length)

    let event1 = eventsS1 |> Seq.head
    let event2 = eventsS1 |> Seq.tail |> Seq.exactlyOne
    let event3 = eventsS2 |> Seq.exactlyOne

    let matchesE1 = MatchService.getMatchesForEvent event1.ID
    let matchesE2 = MatchService.getMatchesForEvent event2.ID
    let matchesE3 = MatchService.getMatchesForEvent event3.ID
    Assert.Equal(9, matchesE1.Length)
    Assert.Equal(3, matchesE2.Length)
    Assert.Equal(1, matchesE3.Length)

    let getPlayersFightersStakesForEventBets eventID = 
        let p, f, s =
            BetService.getPrettyBetsForEvent eventID
            |> Seq.toArray
            |> Array.map (fun x -> x.PlayerName, x.FighterName, x.Stake)
            |> Array.unzip3
        p |> Array.sort, f |> Array.sort, s |> Array.sort

    let betsE1players, betsE1Fighters, betsE1Stakes = 
        getPlayersFightersStakesForEventBets event1.ID
    let betsE2players, betsE2Fighters, betsE2Stakes = 
        getPlayersFightersStakesForEventBets event2.ID
    let betsE3players, betsE3Fighters, betsE3Stakes = 
        getPlayersFightersStakesForEventBets event3.ID
    
    Assert.Equal<_[]>([| "Dustin"; "Dustin"; "Dustin"; "Jake"; "Jake"; "Jake" |] |> Array.sort, betsE1players)
    Assert.Equal<_[]>([| "Tony"; "Tony"; "Tony"; "Tony"; "Tony"; "Tony"; "Tony"; "Tony"; "Tony"; "Tony" |] |> Array.sort, betsE2players)
    Assert.Equal<_[]>([| "Stephanie" |], betsE3players)
    Assert.Equal<_[]>([| "Jonathan Goulet"; "Cody Pfister"; "Charlie Brenneman"; "Gabe Ruediger"; "Joe Son"; "Teila Tuli" |] |> Array.sort, betsE1Fighters)
    Assert.Equal<_[]>([| "Bob Sapp"; "Chris Condo"; "Chris Condo"; "Dada 5000"; "Dada 5000";
                         "Gabe Ruediger"; "Gabe Ruediger"; "Garreth McLellan"; "Ruan Potts"; "Ruan Potts" |] |> Array.sort, betsE2Fighters)
    Assert.Equal<_[]>([| "Jonathan Goulet" |], betsE3Fighters)
    Assert.Equal<_[]>([| 50M; 20M; 20M; 10M; 10M; 10M |] |> Array.sort, betsE1Stakes)
    Assert.Equal<_[]>([| 10M; 10M; 10M; 10M; 10M; 10M; 10M; 10M; 10M; 10M |] |> Array.sort, betsE2Stakes)
    Assert.Equal<_[]>([| 100M |], betsE3Stakes)


[<Fact>]
let testResolveBets() = 
    Assert.Equal(connectionString, Db.ConnectionString) // force eval all the lazy crap
    let season1 = SeasonService.getSeasonByName "Season1" |> Seq.exactlyOne
    let season2 = SeasonService.getSeasonByName "Season2" |> Seq.exactlyOne
    let eventsS1 = EventService.getEventsForSeason season1.ID
    let eventsS2 = EventService.getEventsForSeason season2.ID
    let event1S1 = eventsS1 |> Seq.head
    let event2S1 = eventsS1 |> Seq.tail |> Seq.exactlyOne
    let event1S2 = eventsS2 |> Seq.head
    let matchesS1E1 = MatchService.getMatchesForEvent event1S1.ID
    let matchesS2E1 = MatchService.getMatchesForEvent event1S2.ID
    let fighterMap = getFighterMap()

    // all 'fighter2' in event1 are winners 
    let e1winners = matchesS1E1 |> Array.map(fun x -> x.ID, x.Fighter2ID)
    for mID, f in e1winners do
        MatchService.resolveMatch season1.ID event1S1.ID mID (Nullable(f)) (Nullable(false))
        
    // test push parlays (event2) 
    let matchesS1E2 = MatchService.getMatchesForEvent event2S1.ID
    // m1 = ruediger vs dada = ruediger wins
    // m2 = condo vs mclellan = draw
    // m3 = sapp vs potts = draw
    let ruediger = fighterMap.["Gabe Ruediger"]
    MatchService.resolveMatch season1.ID event2S1.ID matchesS1E2.[0].ID (Nullable(ruediger)) (Nullable(false))
    MatchService.resolveMatch season1.ID event2S1.ID matchesS1E2.[1].ID (Nullable())         (Nullable(true))
    MatchService.resolveMatch season1.ID event2S1.ID matchesS1E2.[2].ID (Nullable())         (Nullable(true))

    // test other season resolve
    let s2e1winners = matchesS2E1 |> Array.map(fun x -> x.ID, x.Fighter2ID)
    for m, f in s2e1winners do
        MatchService.resolveMatch season2.ID event1S2.ID m (Nullable(f)) (Nullable(false))
        
    let players = SeasonService.getSeasonWithPlayers season1.ID
    let dustin = players |> Array.filter(fun x -> x.PlayerName = "Dustin") |> Array.exactlyOne
    let jake   = players |> Array.filter(fun x -> x.PlayerName = "Jake")   |> Array.exactlyOne
    let tony   = players |> Array.filter(fun x -> x.PlayerName = "Tony")   |> Array.exactlyOne
    let s2players = SeasonService.getSeasonWithPlayers season2.ID
    let steph  = s2players |> Array.filter(fun x -> x.PlayerName = "Stephanie") |> Array.exactlyOne
    
    // jake bets
    // ruediger (lose) -10
    // son 10 @ 1.01 -> 10.10
    // tuli 10 @ 1.35 -> 13.50
    // $970 + 20 (stake) + 0.10 + 3.50 = $993.60
    Assert.Equal(993.60M, jake.CurrentCash)

    //dustin bets
    //goulet 50 @ 2.10 -> 105
    //pfist + brenneman
    //20 @ (1.41 * 1.66) = 2.34 -> 46.81
    // $930 + 70 (stake) + 55 + 26.81 = $1081.81
    Assert.Equal(1081.81M, dustin.CurrentCash)

    
    // tony bets
    // parlay2 = push $10
    // parlay3 = push $10
    // parlay4 = push $10
    // parlay5 = lose $10
    // parlay6 = lose $10
    // 950 + 30 (refunds for push) = $980
    Assert.Equal(980M, tony.CurrentCash)

    // steph bets
    // goulet 100 @ 1.10 -> 10
    // 900 + 100 (stake) + 10 = $1010
    Assert.Equal(1010M, steph.CurrentCash)
    
    ()