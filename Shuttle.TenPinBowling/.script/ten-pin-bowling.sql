CREATE SCHEMA [TenPinBowling]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [TenPinBowling].[Frame](
	[GameId] [uniqueidentifier] NOT NULL,
	[Frame] [int] NOT NULL,
	[FrameRoll] [int] NOT NULL,
	[Pins] [int] NOT NULL,
	[Roll] [int] NOT NULL,
	[Score] [int] NOT NULL,
	[BonusRolls] [int] NOT NULL,
	[FrameFinished] [tinyint] NOT NULL,
	[Strike] [tinyint] NOT NULL,
	[Spare] [tinyint] NOT NULL,
	[Open] [tinyint] NOT NULL,
	[StandingPins] [int] NOT NULL,
	[GameFinished] [tinyint] NOT NULL
) ON [PRIMARY]

GO
CREATE CLUSTERED INDEX [IX_Frame] ON [TenPinBowling].[Frame]
(
	[GameId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [TenPinBowling].[FrameBonus](
	[GameId] [uniqueidentifier] NOT NULL,
	[Frame] [int] NOT NULL,
	[BonusFrame] [int] NOT NULL,
	[BonusPins] [int] NOT NULL
) ON [PRIMARY]

GO
CREATE CLUSTERED INDEX [IX_FrameBonus] ON [TenPinBowling].[FrameBonus]
(
	[GameId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [TenPinBowling].[Game](
	[Id] [uniqueidentifier] NOT NULL,
	[Bowler] [varchar](65) NOT NULL,
	[DateStarted] [datetime] NOT NULL,
 CONSTRAINT [PK_Game] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [TenPinBowling].[Frame]  WITH CHECK ADD  CONSTRAINT [FK_Frame_Game] FOREIGN KEY([GameId])
REFERENCES [TenPinBowling].[Game] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [TenPinBowling].[Frame] CHECK CONSTRAINT [FK_Frame_Game]
GO
ALTER TABLE [TenPinBowling].[FrameBonus]  WITH CHECK ADD  CONSTRAINT [FK_FrameBonus_Game] FOREIGN KEY([GameId])
REFERENCES [TenPinBowling].[Game] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [TenPinBowling].[FrameBonus] CHECK CONSTRAINT [FK_FrameBonus_Game]
GO
