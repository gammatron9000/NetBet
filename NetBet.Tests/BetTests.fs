module BetTests

open System
open Xunit
open BetService
open DbTypes
open DtoTypes

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


let internal mapBetToPrettyBet(b: Bet) =
    { SeasonID     = b.SeasonID
      EventID      = b.EventID
      MatchID      = b.MatchID
      PlayerID     = b.PlayerID
      FighterID    = b.FighterID
      ParlayID     = b.ParlayID
      Stake        = b.Stake
      Result       = b.Result
      Odds         = 0.0M
      DisplayOrder = 1
      FighterName  = ""
      ImageLink    = ""
      PlayerName   = "" }

[<Fact>]
let ``Calculate bet win percentages`` () =
    let parlay1id = Guid.NewGuid()
    let parlay2id = Guid.NewGuid()
    let parlay3id = Guid.NewGuid()
    let parlay4id = Guid.NewGuid()
    let parlay5id = Guid.NewGuid()
    let parlay6id = Guid.NewGuid()
    let bets1 = 
        [| { SeasonID = 1; EventID = 1; MatchID = 1; PlayerID = 1; FighterID = 2;  ParlayID = Guid.NewGuid(); Stake = 1M; Result = Nullable(Lose.Code) } 
           { SeasonID = 1; EventID = 1; MatchID = 4; PlayerID = 1; FighterID = 7;  ParlayID = parlay1id;      Stake = 1M; Result = Nullable(Win.Code)  }
           { SeasonID = 1; EventID = 1; MatchID = 5; PlayerID = 1; FighterID = 10; ParlayID = parlay1id;      Stake = 1M; Result = Nullable(Lose.Code) } |]
        |> Array.map mapBetToPrettyBet
    let bets2 =
        [| { SeasonID = 1; EventID = 1; MatchID = 9; PlayerID = 2; FighterID = 18; ParlayID = Guid.NewGuid(); Stake = 1M; Result = Nullable(Win.Code) }
           { SeasonID = 1; EventID = 1; MatchID = 1; PlayerID = 2; FighterID = 1;  ParlayID = Guid.NewGuid(); Stake = 1M; Result = Nullable(Win.Code) }
           { SeasonID = 1; EventID = 1; MatchID = 7; PlayerID = 2; FighterID = 14; ParlayID = Guid.NewGuid(); Stake = 1M; Result = Nullable(Push.Code) } |]
        |> Array.map mapBetToPrettyBet
    let bets3 =
        [| { SeasonID = 1; EventID = 1; MatchID = 10; PlayerID = 3; FighterID = 1;  ParlayID = parlay2id; Stake = 1M; Result = Nullable(Push.Code) } 
           { SeasonID = 1; EventID = 1; MatchID = 11; PlayerID = 3; FighterID = 8;  ParlayID = parlay2id; Stake = 1M; Result = Nullable(Win.Code) }  // W

           { SeasonID = 1; EventID = 1; MatchID = 12; PlayerID = 3; FighterID = 9;  ParlayID = parlay3id; Stake = 1M; Result = Nullable()  }
           { SeasonID = 1; EventID = 1; MatchID = 10; PlayerID = 3; FighterID = 13; ParlayID = parlay3id; Stake = 1M; Result = Nullable(Lose.Code) } // L

           { SeasonID = 1; EventID = 1; MatchID = 10; PlayerID = 3; FighterID = 1;  ParlayID = parlay4id; Stake = 1M; Result = Nullable(Lose.Code) }
           { SeasonID = 1; EventID = 1; MatchID = 11; PlayerID = 3; FighterID = 8;  ParlayID = parlay4id; Stake = 1M; Result = Nullable(Push.Code) } // L

           { SeasonID = 1; EventID = 1; MatchID = 12; PlayerID = 3; FighterID = 9;  ParlayID = parlay5id; Stake = 1M; Result = Nullable(Lose.Code) }
           { SeasonID = 1; EventID = 1; MatchID = 10; PlayerID = 3; FighterID = 13; ParlayID = parlay5id; Stake = 1M; Result = Nullable()  }         // L

           { SeasonID = 1; EventID = 1; MatchID = 11; PlayerID = 3; FighterID = 5;  ParlayID = parlay6id; Stake = 1M; Result = Nullable(Win.Code) }
           { SeasonID = 1; EventID = 1; MatchID = 12; PlayerID = 3; FighterID = 12; ParlayID = parlay6id; Stake = 1M; Result = Nullable(Win.Code) } |] // W
        |> Array.map mapBetToPrettyBet
    let r1total, r1wins, r1percent = BetService.getPercentOfWinningBets bets1
    let r2total, r2wins, r2percent = BetService.getPercentOfWinningBets bets2
    let r3total, r3wins, r3percent = BetService.getPercentOfWinningBets bets3
    Assert.Equal(2, r1total)
    Assert.Equal(2, r2total)
    Assert.Equal(5, r3total)
    Assert.Equal(0, r1wins)
    Assert.Equal(2, r2wins)
    Assert.Equal(2, r3wins)
    Assert.Equal(0M,    r1percent)
    Assert.Equal(1.0M,  r2percent)
    Assert.Equal(0.40M, r3percent)