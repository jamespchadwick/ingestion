DECLARE @MessageStatus TABLE
(
	[Id] INT,
	[Name] NVARCHAR(50)
);

INSERT INTO @MessageStatus VALUES
(1, 'Unpublished'),
(2, 'In Progress'),
(3, 'Published'),
(4, 'Failed')

SET IDENTITY_INSERT [ingestion].[MessageStatus] ON;

MERGE [ingestion].[MessageStatus] AS TARGET
USING @MessageStatus AS SOURCE
ON ([TARGET].Id = [SOURCE].Id)
WHEN NOT MATCHED BY TARGET
THEN INSERT ([Id], [Name]) VALUES ([SOURCE].[Id], [SOURCE].[Name]);

SET IDENTITY_INSERT [ingestion].[MessageStatus] OFF;