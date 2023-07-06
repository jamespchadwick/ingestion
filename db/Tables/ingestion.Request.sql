CREATE TABLE [ingestion].[Request]
(
	[Id] INT IDENTITY(1, 1) NOT NULL,
	[IdempotencyKey] NVARCHAR(255) NOT NULL,
	[StatusId] INT NOT NULL DEFAULT 1,
	[Type] NVARCHAR(255) NOT NULL,
	[Payload] NVARCHAR(max) NOT NULL,
	[TimeStamp] DATETIMEOFFSET NOT NULL,
	[TimesProcessed] INT NOT NULL DEFAULT 0,
	[IsSuccess] BIT NULL,
	[Message] NVARCHAR(max) NULL,
	[LastProcessedOnUtc] DATETIMEOFFSET NULL,

	CONSTRAINT [PK_Ingestion_Request] PRIMARY KEY ([Id]),
	CONSTRAINT [UC_Ingestion_Request_Key] UNIQUE ([IdempotencyKey]),
	CONSTRAINT [FK_Ingestion_Request_RequestStatus] FOREIGN KEY ([StatusId]) REFERENCES [ingestion].[RequestStatus]([Id])
)
