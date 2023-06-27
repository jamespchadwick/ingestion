CREATE TABLE [ingestion].[Message]
(
	[Id] INT IDENTITY(1, 1) NOT NULL,
	[Guid] UNIQUEIDENTIFIER NOT NULL,
	[Scope] UNIQUEIDENTIFIER NULL,
	[StatusId] INT NOT NULL,
	[Type] NVARCHAR(255) NOT NULL,
	[Payload] NVARCHAR(MAX) NOT NULL,
	[CreatedOnUtc] DATETIMEOFFSET NOT NULL,
	[TimesSent] INT NOT NULL,

	CONSTRAINT [PK_Ingestion_Message] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_Ingestion_Message_MessageStatus] FOREIGN KEY ([StatusId]) REFERENCES [ingestion].[MessageStatus]([Id])
)
GO

CREATE NONCLUSTERED INDEX [IX_Ingestion_Message_Scope_StatusId] ON [ingestion].[Message] ([Scope], [StatusId]) WITH (ONLINE = ON)
GO 
