﻿module WebScraper

open FSharp.Data
open System
open DbTypes
open DtoTypes
open System.Data.SqlTypes
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
        Fights: ScrapedFight[]
    }



let fighterLookup (fighterDict: IDictionary<string, int>) (name: string) =
    match fighterDict.TryGetValue(name) with
    | true, x  -> x
    | false, _ -> FighterService.getOrInsertFighterIDByName name

let mapScrapedEventToNetbetEvent seasonID (s: ScrapedEvent) : EventWithMatches =
    let evt = 
        { ID = 0
          SeasonID = seasonID
          Name = s.Name 
          StartTime = SqlDateTime.MinValue.Value }
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


let CreateEventsFromScrape () = 
    let doc = HtmlDocument.Load("https://www.bestfightodds.com")
    
    let eventids = 
        doc.CssSelect("div.table-div")
        |> Seq.map(fun x -> x.AttributeValue("id"))
        
    let events = 
        eventids
        |> Seq.collect(fun eid ->
            let nameSelector = sprintf "#%s > div.table-header > a" eid
            doc.CssSelect(nameSelector) |> Seq.map(fun x -> eid, x.InnerText())
        )
        |> Seq.toArray
        
    events 
    |> Array.map(fun (id, e) -> 
        let primaryBaseSelector = sprintf "#%s > div.table-inner-wrapper > div.table-scroller > table.odds-table > tbody > tr.even" id
        let primaryBase = 
            doc.CssSelect(primaryBaseSelector)
            |> Seq.toArray
        let primaryNames = 
            primaryBase 
            |> Array.map(fun x -> x.CssSelect("th a span") 
                                    |> Seq.map(fun n -> n.InnerText())
                                    |> Seq.head )
        let primaryOdds = 
            primaryBase
            |> Array.map(fun x -> x.CssSelect("td > a > span.tw > span")
                                    |> Seq.map(fun n -> n.InnerText() |> int |> convertMoneyLineToDecimalOdds)
                                    |> Seq.head )
            
        let secondaryBaseSelector = sprintf "#%s > div.table-inner-wrapper > div.table-scroller > table.odds-table > tbody > tr.odd" id
        let secondaryBase = 
            doc.CssSelect(secondaryBaseSelector)
            |> Seq.toArray
        let secondaryNames = 
            secondaryBase 
            |> Array.map(fun x -> x.CssSelect("th a span") 
                                    |> Seq.map(fun n -> n.InnerText())
                                    |> Seq.head )
        let secondaryOdds = 
            secondaryBase
            |> Array.map(fun x -> x.CssSelect("td > a > span.tw > span")
                                    |> Seq.map(fun n -> n.InnerText() |> int |> convertMoneyLineToDecimalOdds)
                                    |> Seq.head )

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
          Fights = pairedFighters }
    )



    