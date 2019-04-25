module SeasonsDb

open Dapper
open DbCommon
open DbTypes

let getAllSeasons () = 
    use connection = Db.CreateConnection()
    connection.Query<Season>("SELECT ID, Name, StartTime, EndTime, StartingCash, MinimumCash, MaxParlaySize FROM dbo.Seasons")

let getSeasonById id = 
    let qp : QueryParamsID = { ID = id }
    use connection = Db.CreateConnection()
    connection.Query<Season>("SELECT ID, Name, StartTime, EndTime, StartingCash, MinimumCash, MaxParlaySize FROM dbo.Seasons WHERE ID = @ID", qp)

let insertSeason (season: Season) = 
    use connection = Db.CreateConnection()
    connection.Execute("""
        INSERT INTO dbo.Seasons(Name, StartTime, EndTime, StartingCash, MinimumCash, MaxParlaySize)
        VALUES (@Name, @StartTime, @EndTime, @StartingCash, @MinimumCash, @MaxParlaySize) """, season)

let updateSeason (season: Season) = 
    use connection = Db.CreateConnection()
    connection.Execute("""
        UPDATE dbo.Seasons
        SET Name = @Name
          , StartTime = @StartTime
          , EndTime = @EndTime
          , StartingCash = @StartingCash
          , MinimumCash =@MinimumCash
          , MaxParlaySize = @MaxParlaySize)
        WHERE ID = @ID""", season)

let deleteSeason (season: Season) =
    use connection = Db.CreateConnection()
    connection.Execute("""DELETE FROM dbo.Seasons WHERE ID = @ID""", season)
