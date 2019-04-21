module SeasonsDb

open Dapper
open DbCommon
open DbTypes

let getAllSeasons () = 
    DbContext.OpenConnection()
    let result = DbContext.Connection.Query<Season>("SELECT ID, Name, StartTime, EndTime, StartingCash, MinimumCash, MaxParlaySize FROM dbo.Seasons")
    DbContext.CloseConnection()
    result
    
let getSeasonById id = 
    let qp : QueryParamsID = { ID = id }
    DbContext.OpenConnection()
    let result = DbContext.Connection.Query<Season>("SELECT ID, Name, StartTime, EndTime, StartingCash, MinimumCash, MaxParlaySize FROM dbo.Seasons WHERE ID = @ID", qp)
    DbContext.CloseConnection()
    result
    
let insertSeason (season: Season) = 
    DbContext.OpenConnection()
    let result = DbContext.Connection.Execute("""
        INSERT INTO dbo.Seasons(Name, StartTime, EndTime, StartingCash, MinimumCash, MaxParlaySize)
        VALUES (@Name, @StartTime, @EndTime, @StartingCash, @MinimumCash, @MaxParlaySize) """, season)
    DbContext.CloseConnection()
    result

let updateSeason (season: Season) = 
    DbContext.OpenConnection()
    let result = DbContext.Connection.Execute("""
        UPDATE dbo.Seasons
        SET Name = @Name
          , StartTime = @StartTime
          , EndTime = @EndTime
          , StartingCash = @StartingCash
          , MinimumCash =@MinimumCash
          , MaxParlaySize = @MaxParlaySize)
        WHERE ID = @ID""", season)
    DbContext.CloseConnection()
    result

let deleteSeason (season: Season) =
    DbContext.OpenConnection()
    let result = DbContext.Connection.Execute("""DELETE FROM dbo.Seasons WHERE ID = @ID""", season)
    DbContext.CloseConnection()
    result
    