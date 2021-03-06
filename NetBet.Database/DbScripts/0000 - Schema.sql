
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bets](
	[SeasonID] [int] NOT NULL,
	[EventID] [int] NOT NULL,
	[MatchID] [int] NOT NULL,
	[PlayerID] [int] NOT NULL,
	[FighterID] [int] NOT NULL,
	[ParlayID] [uniqueidentifier] NOT NULL,
	[Stake] [decimal](18, 0) NOT NULL,
	[Result] [int] NULL,
 CONSTRAINT [PK_Bets] PRIMARY KEY CLUSTERED 
(
	[SeasonID] ASC,
	[EventID] ASC,
	[MatchID] ASC,
	[PlayerID] ASC,
	[FighterID] ASC,
    [ParlayID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Matches]    Script Date: 4/10/2019 8:01:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Matches](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[EventID] [int] NOT NULL,
	[Fighter1ID] [int] NOT NULL,
	[Fighter2ID] [int] NOT NULL,
	[Fighter1Odds] [decimal](6, 2) NOT NULL,
	[Fighter2Odds] [decimal](6, 2) NOT NULL,
	[WinnerFighterID] [int] NULL,
	[LoserFighterID] [int] NULL,
	[IsDraw] [bit] NULL,
	[DisplayOrder] [int] NOT NULL,
 CONSTRAINT [PK_Matches] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[BetsWithOdds]    Script Date: 4/10/2019 8:01:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[BetsWithOdds] AS
SELECT b.SeasonID, b.EventID, b.MatchID, b.PlayerID, b.FighterID, b.ParlayID, b.Stake, b.Result, 
     CASE WHEN b.FighterID = m.Fighter1ID THEN m.Fighter1Odds
          WHEN b.FighterID = m.Fighter2ID THEN m.Fighter2Odds
          ELSE 0.0
          END as Odds
FROM dbo.Bets as b
INNER JOIN dbo.Matches as m
  ON b.MatchID = m.ID
  AND (b.FighterID = m.Fighter1ID
    OR b.FighterID = m.Fighter2ID)
GO
/****** Object:  Table [dbo].[Players]    Script Date: 4/10/2019 8:01:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Players](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Players] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Seasons]    Script Date: 4/10/2019 8:01:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Seasons](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[StartTime] [datetime2](7) NOT NULL,
	[EndTime] [datetime2](7) NOT NULL,
	[StartingCash] [int] NOT NULL,
	[MinimumCash] [int] NOT NULL,
	[MaxParlaySize] [int] NOT NULL,
 CONSTRAINT [PK_Seasons] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SeasonPlayers]    Script Date: 4/10/2019 8:01:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SeasonPlayers](
	[SeasonID] [int] NOT NULL,
	[PlayerID] [int] NOT NULL,
	[CurrentCash] [decimal](18, 2) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[SeasonsWithPlayers]    Script Date: 4/10/2019 8:01:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[SeasonsWithPlayers] AS 
SELECT 
     s.ID as SeasonID
   , p.ID as PlayerID
   , s.[Name] as SeasonName
   , p.[Name] as PlayerName
   , s.MinimumCash as MinimumCash
   , sp.CurrentCash as CurrentCash
FROM dbo.Seasons as s
INNER JOIN dbo.SeasonPlayers as sp
  ON s.ID = sp.SeasonID
INNER JOIN dbo.Players as p
  ON p.ID = sp.PlayerID


  
GO
/****** Object:  Table [dbo].[Events]    Script Date: 4/10/2019 8:01:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Events](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SeasonID] [int] NOT NULL,
	[Name] [varchar](256) NOT NULL,
	[StartTime] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Events] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Fighters]    Script Date: 4/10/2019 8:01:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Fighters](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Image] [varbinary](max) NULL,
	[ImageLink] [varchar](5000) NULL,
 CONSTRAINT [PK_Fighters] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Bets]  WITH CHECK ADD  CONSTRAINT [FK_Bets_Events] FOREIGN KEY([EventID])
REFERENCES [dbo].[Events] ([ID])
GO
ALTER TABLE [dbo].[Bets] CHECK CONSTRAINT [FK_Bets_Events]
GO
ALTER TABLE [dbo].[Bets]  WITH CHECK ADD  CONSTRAINT [FK_Bets_Fighters] FOREIGN KEY([FighterID])
REFERENCES [dbo].[Fighters] ([ID])
GO
ALTER TABLE [dbo].[Bets] CHECK CONSTRAINT [FK_Bets_Fighters]
GO
ALTER TABLE [dbo].[Bets]  WITH CHECK ADD  CONSTRAINT [FK_Bets_Matches] FOREIGN KEY([MatchID])
REFERENCES [dbo].[Matches] ([ID])
GO
ALTER TABLE [dbo].[Bets] CHECK CONSTRAINT [FK_Bets_Matches]
GO
ALTER TABLE [dbo].[Bets]  WITH CHECK ADD  CONSTRAINT [FK_Bets_Players] FOREIGN KEY([PlayerID])
REFERENCES [dbo].[Players] ([ID])
GO
ALTER TABLE [dbo].[Bets] CHECK CONSTRAINT [FK_Bets_Players]
GO
ALTER TABLE [dbo].[Bets]  WITH CHECK ADD  CONSTRAINT [FK_Bets_Seasons] FOREIGN KEY([SeasonID])
REFERENCES [dbo].[Seasons] ([ID])
GO
ALTER TABLE [dbo].[Bets] CHECK CONSTRAINT [FK_Bets_Seasons]
GO
ALTER TABLE [dbo].[Events]  WITH CHECK ADD  CONSTRAINT [FK_Events_Seasons] FOREIGN KEY([SeasonID])
REFERENCES [dbo].[Seasons] ([ID])
GO
ALTER TABLE [dbo].[Events] CHECK CONSTRAINT [FK_Events_Seasons]
GO
ALTER TABLE [dbo].[Matches]  WITH CHECK ADD  CONSTRAINT [FK_Matches_Events] FOREIGN KEY([EventID])
REFERENCES [dbo].[Events] ([ID])
GO
ALTER TABLE [dbo].[Matches] CHECK CONSTRAINT [FK_Matches_Events]
GO
ALTER TABLE [dbo].[Matches]  WITH CHECK ADD  CONSTRAINT [FK_Matches_Fighter1] FOREIGN KEY([Fighter1ID])
REFERENCES [dbo].[Fighters] ([ID])
GO
ALTER TABLE [dbo].[Matches] CHECK CONSTRAINT [FK_Matches_Fighter1]
GO
ALTER TABLE [dbo].[Matches]  WITH CHECK ADD  CONSTRAINT [FK_Matches_Fighter2] FOREIGN KEY([Fighter2ID])
REFERENCES [dbo].[Fighters] ([ID])
GO
ALTER TABLE [dbo].[Matches] CHECK CONSTRAINT [FK_Matches_Fighter2]
GO
ALTER TABLE [dbo].[SeasonPlayers]  WITH CHECK ADD  CONSTRAINT [FK_SeasonPlayers_Players] FOREIGN KEY([PlayerID])
REFERENCES [dbo].[Players] ([ID])
GO
ALTER TABLE [dbo].[SeasonPlayers] CHECK CONSTRAINT [FK_SeasonPlayers_Players]
GO
ALTER TABLE [dbo].[SeasonPlayers]  WITH CHECK ADD  CONSTRAINT [FK_SeasonPlayers_Seasons] FOREIGN KEY([SeasonID])
REFERENCES [dbo].[Seasons] ([ID])
GO
ALTER TABLE [dbo].[SeasonPlayers] CHECK CONSTRAINT [FK_SeasonPlayers_Seasons]
GO
