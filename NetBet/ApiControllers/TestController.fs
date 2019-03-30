namespace NetBet.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Data.SqlClient
open Dapper
open Microsoft.AspNetCore.Mvc

type TestDbResult = 
    {
        testcol: string
    }

[<Route("api/[controller]")>]
[<ApiController>]
type ValuesController () =
    inherit ControllerBase()

    let connectionString = "Server=localhost\SQLEXPRESS; Database=test; Integrated Security=true;"

    [<HttpGet>]
    member __.Get() =
        let values = [|"value1"; "value2"|]
        use connection = new SqlConnection(connectionString)
        connection.Open() |> ignore
        let results = 
                connection.Query<TestDbResult>("SELECT testcol FROM dbo.TestTable") 
                |> Seq.map(fun x -> x.testcol)
                |> Seq.toArray
        connection.Close() |> ignore

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
