module EventsDb

open Dapper
open DbCommon
open DbTypes

let getEventByID eventID =
    let qp : QueryParamsID = { ID = eventID }
    use connection = Db.CreateConnection()
    connection.Query<Event>("""
        SELECT ID, SesaonID, Name, StartTime 
        FROM dbo.Events
        WHERE ID = @ID""", qp)

let getEventsForSeason seasonID =
    let qp : QueryParamsID = { ID = seasonID }
    use connection = Db.CreateConnection()
    connection.Query<Event>("""
        SELECT ID, SeasonID, Name, StartTime 
        FROM dbo.Events
        WHERE SeasonID = @ID""", qp)

let insertEvent (evt: Event) = 
    use connection = Db.CreateConnection()
    connection.Execute("""
        IF NOT EXISTS
        ( SELECT 1
          FROM dbo.Events
          WHERE ID = @ID )
        BEGIN
            INSERT INTO dbo.Events (SeasonID, Name, StartTime)
            VALUES(@SeasonID, @Name, @StartTime)
        END""", evt)

let deleteEvent eventID = 
    let qp : QueryParamsID = { ID = eventID }
    use connection = Db.CreateConnection()
    connection.Execute("""
        DELETE FROM dbo.Events WHERE ID = @ID""", qp)

let updateEvent (evt: Event) = 
    use connection = Db.CreateConnection()
    connection.Execute("""
        UPDATE dbo.Events
        SET Name = @Name
          , StartTime = @StartTime
        WHERE ID = @ID""", evt)
