module EventsDb

open Dapper
open DbCommon
open DbTypes

let getEventByID eventID =
    let qp : QueryParamsID = { ID = eventID }
    DbContext.OpenConnection()
    let result = DbContext.Connection.Query<Event>("""
        SELECT ID, SesaonID, Name, StartTime 
        FROM dbo.Events
        WHERE ID = @ID""", qp)
    DbContext.CloseConnection()
    result

let getEventsForSeason seasonID =
    let qp : QueryParamsID = { ID = seasonID }
    DbContext.OpenConnection()
    let result = DbContext.Connection.Query<Event>("""
        SELECT ID, SesaonID, Name, StartTime 
        FROM dbo.Events
        WHERE SeasonID = @ID""", qp)
    DbContext.CloseConnection()
    result

let insertEvent (evt: Event) = 
    DbContext.OpenConnection()
    let result = DbContext.Connection.Execute("""
        IF NOT EXISTS
        ( SELECT 1
          FROM dbo.Events
          WHERE ID = @ID )
        BEGIN
            INSERT INTO dbo.Events (SeasonID, Name, StartTime)
            VALUES( @ID, @SeasonID, @Name, @StartTime)
        END""", evt)
    DbContext.CloseConnection()
    result

let deleteEvent eventID = 
    let qp : QueryParamsID = { ID = eventID }
    DbContext.OpenConnection()
    let result = DbContext.Connection.Execute("""
        DELETE FROM dbo.Events WHERE ID = @ID""", qp)
    DbContext.CloseConnection()
    result

let updateEvent (evt: Event) = 
    DbContext.OpenConnection()
    let result = DbContext.Connection.Execute("""
        UPDATE dbo.Events
        SET Name = @Name
          , StartTime = @StartTime
        WHERE ID = @ID""", evt)
    DbContext.CloseConnection()
    result