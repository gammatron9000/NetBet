module WebScraper

open FSharp.Data
open System
open DbTypes
open DtoTypes
open System.Collections.Generic

type ScrapedFight =
    {
        Fighter1Name: string
        Fighter1Odds: decimal
        Fighter2Name: string
        Fighter2Odds: decimal
        FightOrder: int
    }
type ScrapedEvent = 
    {
        EventID: string
        Name: string
        StartDate: DateTime
        Fights: ScrapedFight[]
    }



let fighterLookup (fighterDict: IDictionary<string, int>) (name: string) =
    match fighterDict.TryGetValue(name) with
    | true, x  -> x
    | false, _ -> FighterService.getOrInsertFighterIDByName name

let mapScrapedEventToNetbetEvent (s: ScrapedEvent) : EventWithPrettyMatches =
    let evt = 
        { ID = 0
          SeasonID = 0
          Name = s.Name 
          StartTime = s.StartDate }
    let fighterdict = FighterService.getFightersIDLookupByName()
    
    let matches = 
        s.Fights
        |> Array.map(fun f -> 
            let f1id = fighterLookup fighterdict f.Fighter1Name
            let f2id = fighterLookup fighterdict f.Fighter2Name
            {   ID              = 0
                EventID         = 0
                Fighter1ID      = f1id
                Fighter2ID      = f2id
                Fighter1Name    = f.Fighter1Name
                Fighter2Name    = f.Fighter2Name
                Fighter1Odds    = f.Fighter1Odds
                Fighter2Odds    = f.Fighter2Odds
                WinnerFighterID = Nullable()
                LoserFighterID  = Nullable()
                IsDraw          = Nullable()
                DisplayOrder    = f.FightOrder })
    { Event = evt
      Matches = matches }


//To convert moneyline odds to decimal,
//  If the moneyline is positive, divide by 100 and add 1. 
//  If it is negative, divide 100 by the moneyline amount (without the minus sign) and add 1.
let convertMoneyLineToDecimalOdds ml = 
    let dec = ml |> decimal
    if dec > 0M then 
        let raw = (dec / 100M) + 1.0M
        Math.Round(raw, 2)
    else
        let raw = (100M / (dec |> abs)) + 1.0M
        Math.Round(raw, 2)

let getFighterNameFromHtmlNode (h: HtmlNode) = 
    h.CssSelect("th a")
    |> Seq.map(fun x -> x.InnerText())
    |> Seq.head

let getOddsFromHtmlNode (h: HtmlNode) =
    h.CssSelect("td > a > span.tw > span")
    |> Seq.map(fun n -> 
        let it = n.InnerText() 
        if String.IsNullOrWhiteSpace(it) 
        then 0.0M
        else it |> int |> convertMoneyLineToDecimalOdds)
    |> Seq.head 

// dates come in as "June 2nd". Need to strip the last two characters off and parse.
// straight parse will always use current year. If you give it Jan 3rd and it's december 2018, you need to bump the year to 2019
let parseDate (d: string) =
    let strLen = d.Length
    let dateWithoutDaySuffix = d.[..strLen - 3]
    let parsedDate = DateTime.Parse(dateWithoutDaySuffix)
    if parsedDate < DateTime.Now 
    then parsedDate.AddYears(1)
    else parsedDate

let getNamesAndOddsFromSelector (doc: HtmlDocument) (selector: string) = 
    let nodes = 
        doc.CssSelect(selector)
        |> Seq.toArray
    nodes 
    |> Array.choose(fun h -> 
        let name = getFighterNameFromHtmlNode h
        match name with 
        | n when String.Equals(n, "event props", StringComparison.OrdinalIgnoreCase) -> None
        | n -> 
            let odds = getOddsFromHtmlNode h
            Some (n, odds)
    )
    |> Array.unzip

let CreateEventsFromScrape () = 
    let doc = HtmlDocument.Load("https://www.bestfightodds.com")
    
    let eventids = 
        doc.CssSelect("div.table-div")
        |> Seq.map(fun x -> x.AttributeValue("id"))
        
    let events = 
        eventids
        |> Seq.map(fun eid ->
            let nameSelector = sprintf "#%s > div.table-header > a" eid
            let dateSelector = sprintf "#%s > div.table-header > span.table-header-date" eid
            let name = doc.CssSelect(nameSelector) |> Seq.map(fun x -> x.InnerText()) |> Seq.head
            let dateStr = doc.CssSelect(dateSelector) |> Seq.map(fun x -> x.InnerText())
            let date =
                match dateStr |> Seq.length with
                | 0 -> DateTime.Now
                | _ -> dateStr |> Seq.head |> parseDate
            (eid, name, date)
        )
        |> Seq.toArray
        
    events 
    |> Array.map(fun (id, e, date) -> 
        let primarySelector = sprintf "#%s > div.table-inner-wrapper > div.table-scroller > table.odds-table > tbody > tr.even" id
        let primaryNames, primaryOdds = getNamesAndOddsFromSelector doc primarySelector
            
        let secondarySelector = sprintf "#%s > div.table-inner-wrapper > div.table-scroller > table.odds-table > tbody > tr.odd" id
        let secondaryNames, secondaryOdds = getNamesAndOddsFromSelector doc secondarySelector

        let pairedFighters =
            primaryNames
            |> Seq.mapi (fun i x -> 
                { Fighter1Name = x
                  Fighter1Odds = primaryOdds.[i]
                  Fighter2Name = secondaryNames.[i]
                  Fighter2Odds = secondaryOdds.[i]
                  FightOrder   = i })
            |> Seq.toArray

        { EventID = id
          Name = e
          StartDate = date
          Fights = pairedFighters }
    )



    
