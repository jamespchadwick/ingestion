CREATE TABLE [ingestion].[RequestStatus]
(
	[Id] INT IDENTITY(1, 1) NOT NULL,
	[Name] NVARCHAR(50) NOT NULL,

	CONSTRAINT [PK_Ingestion_RequestStatus] PRIMARY KEY ([Id])
)
