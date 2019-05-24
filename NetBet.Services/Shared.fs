﻿module Shared

open System
open DbTypes
open DtoTypes

let denullBool (v: Boolean Nullable) = 
    if not (v.HasValue) then 
        false
    else v.Value

let denullInt (v: int Nullable) =
    if not(v.HasValue) then
        0
    else v.Value

let mapPrettyMatchToMatch (pm: PrettyMatch) : Match = 
    { ID              = pm.ID
      EventID         = pm.EventID
      Fighter1ID      = pm.Fighter1ID
      Fighter2ID      = pm.Fighter2ID
      Fighter1Odds    = pm.Fighter1Odds
      Fighter2Odds    = pm.Fighter2Odds
      WinnerFighterID = pm.WinnerFighterID
      LoserFighterID  = pm.LoserFighterID
      IsDraw          = pm.IsDraw
      DisplayOrder    = pm.DisplayOrder }