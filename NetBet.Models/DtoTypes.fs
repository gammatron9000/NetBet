module DtoTypes

open DbTypes

type BetResult = Lose | Win | Push 
    with static member Code (r: BetResult) =
            match r with 
            | Lose -> 0
            | Win  -> 1
            | Push -> 2
            | _ -> failwithf "Unknown result: %A" r
         static member GetResultForCode (c: int) =
             match c with 
             | 0 -> Lose
             | 1 -> Win
             | 2 -> Push
             | _ -> failwithf "Unknown BetResult Code: %i" c

            // FIX SHIT
        

type EventWithMatches =
    { Event: Event
      Matches: Match[] }


type CompleteSeason =
    { Season: SeasonPlayer
      Events: EventWithMatches[] }
