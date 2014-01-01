SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PostDetails]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PostDetails](
	[Id] [int] IDENTITY(1000,1) NOT NULL,
	[PostId] [int] NOT NULL,
	[SequenceNumber] [int] NOT NULL,
	[Text] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_PostDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Posts]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Posts](
	[Id] [int] IDENTITY(1000,1) NOT NULL,
	[OwnerId] [int] NOT NULL,
	[Title] [nvarchar](200) NOT NULL,
	[Slug] [varchar](50) NOT NULL,
	[PostedDate] [datetime] NOT NULL,
	[EditedDate] [datetime] NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_Posts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Users](
	[Id] [int] IDENTITY(1000,1) NOT NULL,
	[DisplayName] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](200) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[Active] [bit] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
END
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PostDetails]') AND name = N'IX_PostDetails_By_Post_And_Sequence')
CREATE UNIQUE NONCLUSTERED INDEX [IX_PostDetails_By_Post_And_Sequence] ON [dbo].[PostDetails]
(
	[PostId] ASC,
	[SequenceNumber] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
SET ANSI_PADDING ON

GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Posts]') AND name = N'IX_Posts_By_Slug')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Posts_By_Slug] ON [dbo].[Posts]
(
	[Slug] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
SET ANSI_PADDING ON

GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = N'IX_Users_By_DisplayName')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_By_DisplayName] ON [dbo].[Users]
(
	[DisplayName] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
SET ANSI_PADDING ON

GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = N'IX_Users_By_Email')
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_By_Email] ON [dbo].[Users]
(
	[Email] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PostDetails_Posts]') AND parent_object_id = OBJECT_ID(N'[dbo].[PostDetails]'))
ALTER TABLE [dbo].[PostDetails]  WITH CHECK ADD  CONSTRAINT [FK_PostDetails_Posts] FOREIGN KEY([PostId])
REFERENCES [dbo].[Posts] ([Id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PostDetails_Posts]') AND parent_object_id = OBJECT_ID(N'[dbo].[PostDetails]'))
ALTER TABLE [dbo].[PostDetails] CHECK CONSTRAINT [FK_PostDetails_Posts]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Posts_Users]') AND parent_object_id = OBJECT_ID(N'[dbo].[Posts]'))
ALTER TABLE [dbo].[Posts]  WITH CHECK ADD  CONSTRAINT [FK_Posts_Users] FOREIGN KEY([OwnerId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Posts_Users]') AND parent_object_id = OBJECT_ID(N'[dbo].[Posts]'))
ALTER TABLE [dbo].[Posts] CHECK CONSTRAINT [FK_Posts_Users]
GO
