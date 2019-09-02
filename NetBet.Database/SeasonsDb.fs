module SeasonsDb

open Dapper
open DbCommon
open DbTypes

let getAllSeasons () = 
    use db = new DbConnection()
    db.connection.Query<Season>("SELECT ID, Name, StartTime, EndTime, StartingCash, MinimumCash, MaxParlaySize FROM dbo.Seasons")

let getSeasonById id = 
    let qp : QueryParamsID = { ID = id }
    use db = new DbConnection()
    db.connection.Query<Season>("SELECT ID, Name, StartTime, EndTime, StartingCash, MinimumCash, MaxParlaySize FROM dbo.Seasons WHERE ID = @ID", qp)

let getSeasonByName name = 
    let qp: QueryParamsName = { Name = name }
    use db = new DbConnection()
    db.connection.Query<Season>("SELECT ID, Name, StartTime, EndTime, StartingCash, MinimumCash, MaxParlaySize FROM dbo.Seasons WHERE [Name] = @Name", qp)

let insertSeason (season: Season) = 
    use db = new DbConnection()
    db.connection.Execute("""
        INSERT INTO dbo.Seasons(Name, StartTime, EndTime, StartingCash, MinimumCash, MaxParlaySize)
        VALUES (@Name, @StartTime, @EndTime, @StartingCash, @MinimumCash, @MaxParlaySize) """, season)

let updateSeason (season: Season) = 
    use db = new DbConnection()
    db.connection.Execute("""
        UPDATE dbo.Seasons
        SET Name = @Name
          , StartTime = @StartTime
          , EndTime = @EndTime
          , StartingCash = @StartingCash
          , MinimumCash = @MinimumCash
          , MaxParlaySize = @MaxParlaySize
        WHERE ID = @ID""", season)

let deleteSeason (seasonID: int) =
    let qp : QueryParamsID = { ID = seasonID }
    use db = new DbConnection()
    db.connection.Execute("""DELETE FROM dbo.Seasons WHERE ID = @ID""", qp)
