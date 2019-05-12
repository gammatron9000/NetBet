namespace NetBet.Controllers

open System
open System.Collections.Generic
open System.Linq
open Dapper
open Microsoft.AspNetCore.Mvc
open DbCommon
open DbTypes

[<Route("api/[controller]")>]
[<ApiController>]
type ValuesController () =
    inherit ControllerBase()
    
    [<HttpGet>]
    member __.Get() =
        use connection = Db.CreateConnection()
        let fighters = connection.Query<Fighter>("SELECT * FROM dbo.Fighters")
        let results = fighters |> Seq.map(fun x -> x.Name) |> Seq.toArray
        ActionResult<string[]>(results)

    [<HttpGet("{id}")>]
    member __.Get(id:int) =
        let value = "value"
        ActionResult<string>(value)

    [<HttpPost>]
    member __.Post([<FromBody>] value:string) =
        ()

    [<HttpPut("{id}")>]
    member __.Put(id:int, [<FromBody>] value:string ) =
        ()

    [<HttpDelete("{id}")>]
    member __.Delete(id:int) =
        ()
