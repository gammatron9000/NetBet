module DatabaseFixture

open DbUp.Engine
open System.Data.SqlClient


let cs (dbName) =
    sprintf "Server=localhost\SQLEXPRESS; Database=%s; Integrated Security=true;" dbName

let dropDatabase dbName = 
    let masterConnectionString = cs("master")
    use connection = new SqlConnection(masterConnectionString)
    connection.Open() |> ignore
    let sql = sprintf """
        IF EXISTS(SELECT * FROM sys.databases 
            WHERE name = '%s')
        BEGIN
            ALTER DATABASE [%s] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
            DROP DATABASE [%s]
        END
        ELSE 
        BEGIN 
            CREATE DATABASE [%s]
        END""" dbName dbName dbName dbName
    
    use command = connection.CreateCommand()
    command.CommandText <- sql
    command.ExecuteNonQuery() |> ignore
    cs(dbName)

let upgradeDb dbName =
    DbUpgrader.upgrade(dbName)