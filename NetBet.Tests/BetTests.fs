module BetTests

open System
open Xunit
open BetService

[<Fact>]
let ``Calculate single bet winnings`` () = 
    let test1 = calculateBetWin 50M 1.50M
    let test2 = calculateBetWin 47M 2.92M
    let test3 = calculateBetWin 3811M 1.03M
    Assert.Equal(25M,     test1)
    Assert.Equal(90.24M,  test2)
    Assert.Equal(114.33M, test3)


[<Fact>]
let ``Calculate parlay bet winnings`` () = 
    let test1 = calculateParlayBetWin 50M  [| 1.05M; 1.50M |]
    let test2 = calculateParlayBetWin 1M   [| 1.50M; 1.50M; 1.50M; 1.50M |]
    let test3 = calculateParlayBetWin 37M  [| 1.00M; 1.00M; 1.00M; 1.00M |]
    let test4 = calculateParlayBetWin 987M [| 5.00M; 5.00M |]
    Assert.Equal(28.75M, test1)
    Assert.Equal(4.06M,  test2)
    Assert.Equal(0.00M,  test3)
    Assert.Equal(23688.00M, test4)