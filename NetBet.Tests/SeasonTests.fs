module SeasonTests

open System
open Xunit
open SeasonService

[<Fact>]
let ``SeasonPlayer Updates`` () =
    let existingPlayerIDs = [| 1; 2; 3; 4 |]
    let updatedPlayerIDs = [| 1; 3; 5; 6 |]
    let toRemove, toAdd = calculatePlayerRemovalsAndAdditions existingPlayerIDs updatedPlayerIDs
    Assert.Equal<_[]>([| 2; 4 |], toRemove)
    Assert.Equal<_[]>([| 5; 6 |], toAdd)
