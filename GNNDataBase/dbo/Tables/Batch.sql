﻿CREATE TABLE [dbo].[Batch]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(32) NOT NULL,
	[Description] NVARCHAR(MAX) NULL,
	[Generation] INT NOT NULL DEFAULT 1,
	[CreationDate] DATETIME2 NOT NULL DEFAULT getutcdate(),
)
