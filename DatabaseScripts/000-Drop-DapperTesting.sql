IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Posts_Users]') AND parent_object_id = OBJECT_ID(N'[dbo].[Posts]'))
ALTER TABLE [dbo].[Posts] DROP CONSTRAINT [FK_Posts_Users]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_PostDetails_Posts]') AND parent_object_id = OBJECT_ID(N'[dbo].[PostDetails]'))
ALTER TABLE [dbo].[PostDetails] DROP CONSTRAINT [FK_PostDetails_Posts]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 1/1/2014 4:47:08 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
DROP TABLE [dbo].[Users]
GO
/****** Object:  Table [dbo].[Posts]    Script Date: 1/1/2014 4:47:08 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Posts]') AND type in (N'U'))
DROP TABLE [dbo].[Posts]
GO
/****** Object:  Table [dbo].[PostDetails]    Script Date: 1/1/2014 4:47:08 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PostDetails]') AND type in (N'U'))
DROP TABLE [dbo].[PostDetails]
GO
