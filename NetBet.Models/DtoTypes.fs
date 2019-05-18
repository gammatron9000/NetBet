module DtoTypes

open DbTypes
open System

type BetResult = 
    | Lose 
    | Win 
    | Push 
        override x.ToString() =
            sprintf "%i: %s" x.Code x.DisplayText
        member x.Code = 
            x |> BetResult.GetCode
        member x.DisplayText =
            x |> BetResult.GetDisplayText

        static member GetCode x =
            match x with 
            | Lose -> 0
            | Win  -> 1
            | Push -> 2
        
        static member GetDisplayText x =
            match x with 
            | Lose -> "Lose"
            | Win  -> "Win"
            | Push -> "Push"

        static member ConvertDisplayTextToType (displayText: string) =
            match displayText with 
            | x when String.Compare(x, (Lose |> BetResult.GetDisplayText), true) = 0 -> Lose
            | x when String.Compare(x, (Win  |> BetResult.GetDisplayText), true) = 0 -> Win
            | x when String.Compare(x, (Push |> BetResult.GetDisplayText), true) = 0 -> Push
            | _ -> failwithf "unknown BetResult type: %s" displayText

        static member ConvertCodeToType (code: int) =
            match code with 
            | x when x = (Lose |> BetResult.GetCode) -> Lose
            | x when x = (Win  |> BetResult.GetCode) -> Win
            | x when x = (Push |> BetResult.GetCode) -> Push
            | _ -> failwithf "Unknown BetResult code: %i" code



type EventWithMatches =
    { Event: Event
      Matches: Match[] }
      

type SeasonWithPlayers = 
    { Season: Season
      Players: SeasonPlayer[] }

type FullEvent = 
    { Event: Event
      Matches: Match[]
      Bets: PrettyBet[]
      Players: SeasonPlayer[] }

type ResolveMatchDto = 
    { SeasonID : int
      EventID  : int
      MatchID  : int
      WinnerID : Nullable<int>
      IsDraw   : Nullable<bool> }