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
    fighters |> Array.map FightersDb.insertFighter |> ignore
    players  |> Array.map PlayersDb.insertPlayer |> ignore
    seasons  |> Array.map SeasonsDb.insertSeason |> ignore

    let fightersFromDb = FightersDb.getAllFighters() |> Seq.toArray
    let playersFromDb  = PlayersDb.getAllPlayers() |> Seq.toArray
    let seasonsFromDb  = SeasonsDb.getAllSeasons() |> Seq.toArray

    ()

