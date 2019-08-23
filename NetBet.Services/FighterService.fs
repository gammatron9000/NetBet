module FighterService

open DbTypes

let getFighterByID fighterID =
    FightersDb.getFighterByID fighterID |> Seq.exactlyOne

let getAllFighters () =
    FightersDb.getAllFighters() |> Seq.toArray

let getOrInsertFighterIDByName (name: string) =
    FightersDb.getOrInsertFighterIDByName name

let getFightersIDLookupByName () = 
    getAllFighters() 
    |> Array.map(fun x -> x.Name, x.ID)
    |> dict

let getFightersNameLookupByID () =
    getAllFighters()
    |> Array.map(fun x -> x.ID, x.Name)
    |> dict

let getFightersFullLookupByName () = 
    getAllFighters()
    |> Array.map(fun x -> x.Name, x)
    |> dict

let createFighter (fighter: Fighter) =
    FightersDb.insertFighter fighter 

let updateFighter (fighter: Fighter) =
    FightersDb.updateFighter fighter

let deleteFighter fighterID =
    FightersDb.deleteFighter fighterID