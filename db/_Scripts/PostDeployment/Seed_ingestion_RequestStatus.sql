DECLARE @RequestStatus TABLE
(
	[Id] INT,
	[Name] NVARCHAR(50)
);

INSERT INTO @RequestStatus VALUES
(1, 'In Progress'),
(2, 'Succeeded'),
(3, 'Failed')

SET IDENTITY_INSERT [ingestion].[RequestStatus] ON;

MERGE [ingestion].[RequestStatus] AS TARGET
USING @RequestStatus AS SOURCE
ON ([TARGET].Id = [SOURCE].Id)
WHEN NOT MATCHED BY TARGET
THEN INSERT ([Id], [Name]) VALUES ([SOURCE].[Id], [SOURCE].[Name]);

SET IDENTITY_INSERT [ingestion].[RequestStatus] OFF;