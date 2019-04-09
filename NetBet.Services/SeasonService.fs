
module SeasonService

open DbTypes

let getAllSeasons() = 
    SeasonsDb.getAllSeasons() |> Seq.toArray

let getSeasonByID(seasonID) = 
    SeasonsDb.getSeasonById(seasonID) |> Seq.exactlyOne

let createSeason (s: Season) = 
    SeasonsDb.insertSeason(s)

let updateSeason (s: Season) = 
    SeasonsDb.updateSeason(s)

let getAllPlayers () = 
    PlayersDb.getAllPlayers() |> Seq.toArray

let getPlayer playerID =
    PlayersDb.getPlayerByID playerID |> Seq.exactlyOne
    
let createPlayer name =
    PlayersDb.insertPlayer name

let changePlayerName player =
    PlayersDb.updatePlayer player
    
let getSeasonWithPlayers seasonID =
    SeasonPlayersDb.getPlayersForSeason seasonID |> Seq.toArray

let getSeasonPlayer seasonID playerID =
    SeasonPlayersDb.getSeasonPlayer seasonID playerID |> Seq.exactlyOne

let calculatePlayerRemovalsAndAdditions (existingPlayerIDs: int[]) (updatedPlayerIDs: int[]) = 
    let toRemove =
        existingPlayerIDs
        |> Array.filter (fun e -> updatedPlayerIDs |> Array.exists(fun u -> e = u ) |> not)
    let toAdd = 
        updatedPlayerIDs
        |> Array.filter(fun u -> existingPlayerIDs |> Array.exists(fun e -> u = e) |> not)
    toRemove, toAdd

let updateSeasonPlayersToDb seasonID (players: Player[]) = 
    let existingPlayerIDs = 
        getSeasonWithPlayers(seasonID)
        |> Array.map(fun x -> x.PlayerID)
    let updatedPlayerIDs = 
        players |> Array.map(fun x -> x.ID)
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
    


