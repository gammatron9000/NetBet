

CREATE FUNCTION getPrettyBetsForSeason(@SeasonID INT)
RETURNS TABLE
AS 
RETURN 
    SELECT b.*
         , CASE WHEN b.FighterID = m.Fighter1ID THEN m.Fighter1Odds
                WHEN b.FighterID = m.Fighter2ID THEN m.Fighter2Odds
                ELSE 0.0 END as Odds
         , m.DisplayOrder
         , f.[Name] as FighterName
         , f.ImageLink
         , p.[Name] as PlayerName
    FROM dbo.Bets as b
    INNER JOIN dbo.Matches as m
      ON b.MatchID = m.ID
    INNER JOIN dbo.Fighters as f
      ON b.FighterID = f.ID
    INNER JOIN dbo.Players as p
      ON b.PlayerID = p.ID
    WHERE b.SeasonID = @SeasonID
