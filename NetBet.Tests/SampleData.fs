module SampleData

open System
open DbTypes
open DataGenerators

let seasons = 
    [| makeSeason "Season1"
       makeSeason "Season2" |]

let players = 
    [| makePlayer "Dustin"
       makePlayer "Jake"
       makePlayer "Tony" 
       makePlayer "Stephanie" |]

let fighters = 
    [| makeFighter "Gabe Ruediger"
       makeFighter "Jonathan Goulet"
       makeFighter "Kenneth Allen"
       makeFighter "Caleb Starnes"
       makeFighter "Garreth McLellan"
       makeFighter "Artem Lobov"
       makeFighter "Cody Pfister"
       makeFighter "Chris Condo"
       makeFighter "Ruan Potts"
       makeFighter "Charlie Brenneman"
       makeFighter "Emmanuel Yarbrough"
       makeFighter "Bob Sapp"
       makeFighter "Dada 5000"
       makeFighter "Joe Son"
       makeFighter "Tiki Ghosn"
       makeFighter "Elvis Sinosic"
       makeFighter "Mayhem Miller"
       makeFighter "Teila Tuli" |]

// TODO : make events and matches and bets (from IDs)