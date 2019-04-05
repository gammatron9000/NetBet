module FighterService

open DbTypes

let getFighterByID fighterID =
    FightersDb.getFighterByID fighterID |> Seq.exactlyOne

let getAllFighters () =
    FightersDb.getAllFighters() |> Seq.toArray

let createFighter (fighter: Fighter) =
    FightersDb.insertFighter fighter 

let updateFighter (fighter: Fighter) =
    FightersDb.updateFighter fighter

let deleteFighter fighterID =
    FightersDb.deleteFighter fighterID