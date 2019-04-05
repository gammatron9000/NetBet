module EventsDb

open Dapper
open DbCommon
open DbTypes

let getEventByID eventID =
    let qp : QueryParamsID = { ID = eventID }
    DbContext.Instance.Connection.Query<Event>("""
        SELECT ID, SesaonID, Name, StartTime 
        FROM dbo.Events
        WHERE ID = @ID""", qp)

let getEventsForSeason seasonID =
    let qp : QueryParamsID = { ID = seasonID }
    DbContext.Instance.Connection.Query<Event>("""
        SELECT ID, SesaonID, Name, StartTime 
        FROM dbo.Events
        WHERE SeasonID = @ID""", qp)

let insertEvent (evt: Event) = 
    DbContext.Instance.Connection.Execute("""
        IF NOT EXISTS
        ( SELECT 1
          FROM dbo.Events
          WHERE ID = @ID )
        BEGIN
            INSERT INTO dbo.Events (SeasonID, Name, StartTime)
            VALUES( @ID, @SeasonID, @Name, @StartTime)
        END""", evt)

let deleteEvent eventID = 
    let qp : QueryParamsID = { ID = eventID }
    DbContext.Instance.Connection.Execute("""
        DELETE FROM dbo.Events WHERE ID = @ID""", qp)

let updateEvent (evt: Event) = 
    DbContext.Instance.Connection.Execute("""
        UPDATE dbo.Events
        SET Name = @Name
          , StartTime = @StartTime
        WHERE ID = @ID""", evt)