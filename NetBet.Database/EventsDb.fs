module EventsDb

open Dapper
open DbCommon
open DbTypes

let getEventByID eventID =
    let qp : QueryParamsID = { ID = eventID }
    use db = new DbConnection()
    db.connection.Query<Event>("""
        SELECT ID, SeasonID, Name, StartTime 
        FROM dbo.Events
        WHERE ID = @ID""", qp)

let getEventByName eventName = 
    let qp : QueryParamsName = { Name = eventName }
    use db = new DbConnection()
    db.connection.Query<Event>("""
        SELECT ID, SeasonID, Name, StartTime
        FROM dbo.Events
        WHERE Name = @Name""", qp)

let getEventsForSeason seasonID =
    let qp : QueryParamsID = { ID = seasonID }
    use db = new DbConnection()
    db.connection.Query<Event>("""
        SELECT ID, SeasonID, Name, StartTime 
        FROM dbo.Events
        WHERE SeasonID = @ID""", qp)

// returns the id of the event it will create
let insertOrUpdateEvent (evt: Event) = 
    use db = new DbConnection()
    db.connection.QuerySingle<int>("""
        IF NOT EXISTS
        ( SELECT 1
          FROM dbo.Events
          WHERE ID = @ID )
        BEGIN
            INSERT INTO dbo.Events (SeasonID, Name, StartTime)
            VALUES(@SeasonID, @Name, @StartTime)
            SELECT SCOPE_IDENTITY()
        END
        ELSE 
        BEGIN 
            UPDATE dbo.Events
            SET Name = @Name
              , StartTime = @StartTime
            WHERE ID = @ID
            SELECT @ID
        END
        """, evt)

let deleteEvent eventID = 
    let qp : QueryParamsID = { ID = eventID }
    use db = new DbConnection()
    db.connection.Execute("""
        DELETE FROM dbo.Events WHERE ID = @ID""", qp)
        
