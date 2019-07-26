ALTER TABLE [dbo].[DocumentPrintTypes] ADD [ServiceTypeId] int NULL;
GO

CREATE TABLE [dbo].[ServiceTypes](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](1024) NOT NULL,
 CONSTRAINT [PK_ServiceTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[DocumentPrintTypes]  WITH CHECK ADD  CONSTRAINT [FK_DocumentPrintTypes_ServiceTypes] FOREIGN KEY([ServiceTypeId])
REFERENCES [dbo].[ServiceTypes] ([Id])
GO

ALTER TABLE [dbo].[DocumentPrintTypes] CHECK CONSTRAINT [FK_DocumentPrintTypes_ServiceTypes]
GO

--INSERT Service types
INSERT INTO [dbo].[ServiceTypes]([Id],[Name]) VALUES(1,	'Community Habilitation');
INSERT INTO [dbo].[ServiceTypes]([Id],[Name]) VALUES(2,	'Group Day Habilitation');
INSERT INTO [dbo].[ServiceTypes]([Id],[Name]) VALUES(3,	'Respite');
GO

--INSERT Document print types
DELETE FROM [dbo].[DocumentPrintTypes];
GO
INSERT INTO [dbo].[DocumentPrintTypes] ([Id],[Name],[IsActive] ,[ServiceTypeId]) VALUES (1,'Community Habilitation Documentation Record - Individual Summary',True,1);
INSERT INTO [dbo].[DocumentPrintTypes] ([Id],[Name],[IsActive] ,[ServiceTypeId]) VALUES (2,'Community Habilitation Monthly Progress Summary',True,1);
INSERT INTO [dbo].[DocumentPrintTypes] ([Id],[Name],[IsActive] ,[ServiceTypeId]) VALUES (3,'Community Habilitation 6 Month Review',True,1);
INSERT INTO [dbo].[DocumentPrintTypes] ([Id],[Name],[IsActive] ,[ServiceTypeId]) VALUES (4,'Group Day Habilitation Documentation Record - Individual Summary',True,2);
INSERT INTO [dbo].[DocumentPrintTypes] ([Id],[Name],[IsActive] ,[ServiceTypeId]) VALUES (5,'Group Day Habilitation Monthly Progress Summary',True,2);
INSERT INTO [dbo].[DocumentPrintTypes] ([Id],[Name],[IsActive] ,[ServiceTypeId]) VALUES (6,'Group Day Habilitation 6 Month Review',True,2);
INSERT INTO [dbo].[DocumentPrintTypes] ([Id],[Name],[IsActive] ,[ServiceTypeId]) VALUES (7,'Respite Documentation Record - Individual Summary',True,3);

GO
----------------------------------------------------
ALTER TABLE [dbo].[ConsumerPrintDocuments] ADD [ServiceTypeId] int NULL;
GO

ALTER TABLE [dbo].[ConsumerPrintDocuments]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerPrintDocuments_ServiceTypes] FOREIGN KEY([ServiceTypeId])
REFERENCES [dbo].[ServiceTypes] ([Id])
GO

ALTER TABLE [dbo].[ConsumerPrintDocuments] CHECK CONSTRAINT [FK_ConsumerPrintDocuments_ServiceTypes]
GO


