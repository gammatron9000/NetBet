module BetTests

open System
open Xunit
open BetService

[<Fact>]
let ``Calculate single bet winnings`` () = 
    let test1 = calculateBetWin 50M 1.50M
    let test2 = calculateBetWin 47M 2.92M
    let test3 = calculateBetWin 3811M 1.03M
    Assert.Equal(75M,     test1)
    Assert.Equal(137.24M,  test2)
    Assert.Equal(3925.33M, test3)


[<Fact>]
let ``Calculate parlay bet winnings`` () = 
    let test1 = calculateParlayBetWin 50M  [| 1.05M; 1.50M |]
    let test2 = calculateParlayBetWin 1M   [| 1.50M; 1.50M; 1.50M; 1.50M |]
    let test3 = calculateParlayBetWin 37M  [| 1.00M; 1.00M; 1.00M; 1.00M |]
    let test4 = calculateParlayBetWin 987M [| 5.00M; 5.00M |]
    Assert.Equal(78.75M, test1)
    Assert.Equal(5.06M,  test2)
    Assert.Equal(37.00M,  test3)
    Assert.Equal(24675.00M, test4)