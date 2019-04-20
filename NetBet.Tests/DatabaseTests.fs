module DatabaseTests

open System
open System.Collections.Generic
open System.Linq
open Xunit
open DatabaseFixture

let dbName = "NetBetDbTest"
let connectionString = dropDatabase(dbName)
let dbUpgradeResult = upgradeDb(connectionString)

[<Fact>]
let dbUpgradeTest() =
    Assert.Equal(true, dbUpgradeResult.Successful)
    Assert.Equal(1, dbUpgradeResult.Scripts.Count())


