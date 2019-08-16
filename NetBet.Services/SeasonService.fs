
module SeasonService

open DbTypes
open DtoTypes
open System
open System.Diagnostics

let getAllSeasons() = 
    SeasonsDb.getAllSeasons() |> Seq.toArray

let getSeasonByID seasonID = 
    SeasonsDb.getSeasonById(seasonID) |> Seq.exactlyOne

let getSeasonByName name = 
    SeasonsDb.getSeasonByName(name) |> Seq.exactlyOne

let createOrUpdateSeason (s: Season) = 
    match s.ID with 
    | 0 -> // create 
        let allSeasonNames = getAllSeasons() |> Array.map(fun x -> x.Name)
        if allSeasonNames |> Array.contains(s.Name)
        then failwithf "A Season with the name %s already exists" s.Name
        else SeasonsDb.insertSeason(s)
    | _ ->  // update
        SeasonsDb.updateSeason(s)
   
let getAllPlayers () = 
    PlayersDb.getAllPlayers() |> Seq.toArray

let getPlayer playerID =
    PlayersDb.getPlayerByID playerID |> Seq.exactlyOne
    
let createPlayer name =
    PlayersDb.insertPlayer name

let changePlayerName player =
    PlayersDb.updatePlayer player

let createOrUpdatePlayer (p: Player) = 
    match p.ID with 
    | 0 -> // create
        let allPlayerNames = getAllPlayers() |> Array.map(fun x -> x.Name)
        if allPlayerNames |> Array.contains(p.Name) 
        then failwithf "A Player with the name %s already exists" p.Name
        else createPlayer p.Name
    | _ -> // update
        changePlayerName p
    
let getPlayersForSeason seasonID =
    SeasonPlayersDb.getPlayersForSeason seasonID |> Seq.toArray

let getFullSeason seasonID = 
    let s = getSeasonByID seasonID
    let p = getPlayersForSeason seasonID
    let e = EventService.getEventsForSeason seasonID
    { Season = s
      Players = p
      Events = e }

let getSeasonPlayer seasonID playerID =
    SeasonPlayersDb.getSeasonPlayer seasonID playerID |> Seq.exactlyOne
    
let addPlayerToSeason seasonID playerID =
    SeasonPlayersDb.addPlayerToSeason seasonID playerID

let removePlayerFromSeason seasonID playerID =
    SeasonPlayersDb.removePlayerFromSeason seasonID playerID

let deleteSeason seasonID = 
    let eventIDs = EventService.getEventsForSeason seasonID |> Array.map (fun x -> x.ID)
    let seasonPlayerIDs = getPlayersForSeason seasonID |> Array.map (fun x -> x.PlayerID)
    eventIDs |> Array.map EventService.deleteEvent |> ignore // this delete events, bets, and matches
    seasonPlayerIDs |> Array.map (fun x -> removePlayerFromSeason seasonID x) |> ignore
    SeasonsDb.deleteSeason seasonID

let deletePlayer playerID = 
    PlayersDb.deletePlayer playerID

let getSeasonWithPlayers seasonID = 
    let season = SeasonsDb.getSeasonById seasonID |> Seq.exactlyOne
    let players = SeasonPlayersDb.getPlayersForSeason seasonID |> Seq.toArray
    { Season = season
      Players = players }

let calculatePlayerRemovalsAndAdditions (existingPlayerIDs: int[]) (updatedPlayerIDs: int[]) = 
    let toRemove =
        existingPlayerIDs
        |> Array.filter (fun e -> updatedPlayerIDs |> Array.exists(fun u -> e = u ) |> not)
    let toAdd = 
        updatedPlayerIDs
        |> Array.filter(fun u -> existingPlayerIDs |> Array.exists(fun e -> u = e) |> not)
    toRemove, toAdd

let updateSeasonPlayersToDb seasonID (updatedPlayerIDs: int[]) = 
    let existingPlayerIDs = 
        getPlayersForSeason(seasonID)
        |> Array.map(fun x -> x.PlayerID)
    let toRemove, ToAdd = calculatePlayerRemovalsAndAdditions existingPlayerIDs updatedPlayerIDs
    let removed = 
        toRemove 
        |> Array.map (fun x -> SeasonPlayersDb.removePlayerFromSeason seasonID x)
        |> Array.sum
    let added = 
        ToAdd 
        |> Array.map (fun x -> SeasonPlayersDb.addPlayerToSeason seasonID x)
        |> Array.sum
    removed + added

let createSeasonWithPlayers (s: EditSeasonDto) = 
    createOrUpdateSeason s.Season |> ignore
    s.PlayerIDs |> updateSeasonPlayersToDb s.Season.ID
    
let givePlayerMoney seasonID playerID amount =
    let seasonPlayer = getSeasonPlayer seasonID playerID
    let newCurrentCash = seasonPlayer.CurrentCash + amount
    let rounded = Math.Round(newCurrentCash, 2)
    let newPlayer = { seasonPlayer with CurrentCash = rounded }
    SeasonPlayersDb.updateCurrentCash newPlayer |> ignore

let removePlayerMoney seasonID playerID amount =
    let seasonPlayer = getSeasonPlayer seasonID playerID
    let newCurrentCash = seasonPlayer.CurrentCash - amount
    let rounded = Math.Round(newCurrentCash, 2)
    let newPlayer = { seasonPlayer with CurrentCash = rounded }
    SeasonPlayersDb.updateCurrentCash newPlayer |> ignore


