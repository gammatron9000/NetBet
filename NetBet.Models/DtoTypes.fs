module DtoTypes

open DbTypes

type BetResult = Lose = 0 | Win = 1 | Push = 2

type EventWithMatches =
    { Event: Event
      Matches: Match[] }


type CompleteSeason =
    { Season: SeasonWithPlayers
      Events: EventWithMatches[] }
