module Shared

open System

let denullBool (v: Boolean Nullable) = 
    if not (v.HasValue) then 
        false
    else v.Value

let denullInt (v: int Nullable) =
    if not(v.HasValue) then
        0
    else v.Value