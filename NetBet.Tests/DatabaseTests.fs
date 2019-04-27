module DatabaseTests

open System
open System.Collections.Generic
open System.Linq
open Xunit
open DatabaseFixture
open DbCommon
open System.Data.SqlClient

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
    Assert.Equal(2, matchesE2.Length)
    Assert.Equal(1, matchesE3.Length)

    let getPlayersFightersStakesForEventBets eventID = 
        BetService.getPrettyBetsForEvent eventID
        |> Seq.toArray
        |> Array.map (fun x -> x.PlayerName, x.FighterName, x.Stake)
        |> Array.unzip3

    let betsE1players, betsE1Fighters, betsE1Stakes = 
        getPlayersFightersStakesForEventBets event1.ID
    let betsE2players, betsE2Fighters, betsE2Stakes = 
        getPlayersFightersStakesForEventBets event2.ID
    let betsE3players, betsE3Fighters, betsE3Stakes = 
        getPlayersFightersStakesForEventBets event3.ID
    
    Assert.Equal<_[]>([| "Dustin"; "Dustin"; "Dustin"; "Jake" |], betsE1players)
    Assert.Equal<_[]>([| "Tony"; "Tony" |], betsE2players)
    Assert.Equal<_[]>([| "Stephanie" |], betsE3players)
    Assert.Equal<_[]>([| "Jonathan Goulet"; "Cody Pfister"; "Charlie Brenneman"; "Emmanuel Yarbrough" |], betsE1Fighters)
    Assert.Equal<_[]>([| "Gabe Ruediger"; "Chris Condo" |], betsE2Fighters)
    Assert.Equal<_[]>([| "Jonathan Goulet" |], betsE3Fighters)
    Assert.Equal<_[]>([| 50M; 20M; 20M; 10M |], betsE1Stakes)
    Assert.Equal<_[]>([| 30M; 30M |], betsE2Stakes)
    Assert.Equal<_[]>([| 100M |], betsE3Stakes)
    
    
