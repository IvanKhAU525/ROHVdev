INSERT INTO [dbo].[ContactTypes] (Id, [Name]) VALUES (7, 'Advocate')
GO

ALTER TABLE [dbo].[Consumers] DROP CONSTRAINT [FK_Consumers_Advocates]
GO

UPDATE  [dbo].[Consumers] SET [AdvocateId] = NULL
GO

ALTER TABLE [dbo].[Consumers]  WITH CHECK ADD  CONSTRAINT [FK_Consumers_Contact_Advocate] FOREIGN KEY([AdvocateId])
REFERENCES [dbo].[Contacts] ([ContactId])
GO

ALTER TABLE [dbo].[Consumers] CHECK CONSTRAINT [FK_Consumers_Contact_Advocate]
GO

ALTER TABLE [dbo].[Consumers] ADD [AdvocatePaperId] int NULL;
GO

ALTER TABLE [dbo].[Consumers]  WITH CHECK ADD  CONSTRAINT [FK_Consumers_Contact_AdvocatePaper] FOREIGN KEY([AdvocatePaperId])
REFERENCES [dbo].[Contacts] ([ContactId])
GO

ALTER TABLE [dbo].[Consumers] CHECK CONSTRAINT [FK_Consumers_Contact_AdvocatePaper]
GO

ALTER TABLE [dbo].[ConsumerHabReviews] DROP CONSTRAINT [FK_ConsumerHabReviews_Advocates]
GO

ALTER TABLE [dbo].[ConsumerHabReviews]
ALTER COLUMN AdvocateId int NULL
GO

UPDATE  [dbo].[ConsumerHabReviews] SET [AdvocateId] = NULL
GO

ALTER TABLE [dbo].[ConsumerHabReviews]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerHabReviews_Contacts_Advocate] FOREIGN KEY([AdvocateId])
REFERENCES [dbo].[Contacts] ([ContactId])
GO

ALTER TABLE [dbo].[ConsumerHabReviews] CHECK CONSTRAINT [FK_ConsumerHabReviews_Contacts_Advocate]
GO
--------------------------------------
ALTER TABLE [dbo].[ConsumerNotes] DROP CONSTRAINT [FK_ConsumerNotes_SystemUsers1]
GO

ALTER TABLE [dbo].[ConsumerNotes] DROP CONSTRAINT [FK_ConsumerNotes_SystemUsers]
GO

ALTER TABLE [dbo].[ConsumerNotes] DROP CONSTRAINT [FK_ConsumerNotes_Contacts]
GO

ALTER TABLE [dbo].[ConsumerNotes] DROP CONSTRAINT [FK_ConsumerNotes_Consumers]
GO

ALTER TABLE [dbo].[ConsumerNotes] DROP CONSTRAINT [FK_ConsumerNotes_ConsumerNoteTypes1]
GO

ALTER TABLE [dbo].[ConsumerNotes] DROP CONSTRAINT [FK_ConsumerNotes_ConsumerNoteTypes]
GO

/****** Object:  Table [dbo].[ConsumerNotes]    Script Date: 9/4/2016 10:37:09 AM ******/
DROP TABLE [dbo].[ConsumerNotes]
GO

CREATE TABLE [dbo].[ConsumerNoteFromTypes](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](255) NULL,
	[IsActive] [bit] NOT NULL CONSTRAINT [DF_ConsumerNoteFromTypes_IsActive]  DEFAULT ((0)),
 CONSTRAINT [PK_ConsumerNoteFromTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

DROP TABLE [dbo].[ConsumerNoteTypes]
GO

CREATE TABLE [dbo].[ConsumerNoteTypes](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](255) NULL,
	[IsActive] [bit] NOT NULL CONSTRAINT [DF_ConsumerNoteTypes_IsActive]  DEFAULT ((0)),
 CONSTRAINT [PK_ConsumerNoteTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[ConsumerNoteTypes] ([Id],[Name] ,[IsActive]) VALUES (1,'Simple',1)
INSERT INTO [dbo].[ConsumerNoteTypes] ([Id],[Name] ,[IsActive]) VALUES (2,'Status',1)
GO

INSERT INTO [dbo].[ConsumerNoteFromTypes] ([Id],[Name] ,[IsActive]) VALUES (1,'Worker',1)
INSERT INTO [dbo].[ConsumerNoteFromTypes] ([Id],[Name] ,[IsActive]) VALUES (2,'Intake dept',1)
INSERT INTO [dbo].[ConsumerNoteFromTypes] ([Id],[Name] ,[IsActive]) VALUES (3,'Hab dept',1)
INSERT INTO [dbo].[ConsumerNoteFromTypes] ([Id],[Name] ,[IsActive]) VALUES (4,'Advocate',1)
GO




CREATE TABLE [dbo].[ConsumerNotes](
	[ConsumerNoteId] [int] IDENTITY(1,1) NOT NULL,
	[ConsumerId] [int] NOT NULL,
	[ContactId] [int] NULL,
	[TypeId] [int] NULL,
	[TypeFromId] [int] NULL,
	[Date] [datetime] NULL,
	[Notes] [nvarchar](1024) NULL,
	[AddedById] [int] NOT NULL,
	[UpdatedById] [int] NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateUpdated] [datetime] NULL,
 CONSTRAINT [PK_ConsumerNotes] PRIMARY KEY CLUSTERED 
(
	[ConsumerNoteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ConsumerNotes]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerNotes_ConsumerNoteTypes] FOREIGN KEY([TypeFromId])
REFERENCES [dbo].[ConsumerNoteFromTypes] ([Id])
GO

ALTER TABLE [dbo].[ConsumerNotes] CHECK CONSTRAINT [FK_ConsumerNotes_ConsumerNoteTypes]
GO

ALTER TABLE [dbo].[ConsumerNotes]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerNotes_ConsumerNoteTypes1] FOREIGN KEY([TypeId])
REFERENCES [dbo].[ConsumerNoteTypes] ([Id])
GO

ALTER TABLE [dbo].[ConsumerNotes] CHECK CONSTRAINT [FK_ConsumerNotes_ConsumerNoteTypes1]
GO

ALTER TABLE [dbo].[ConsumerNotes]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerNotes_Consumers] FOREIGN KEY([ConsumerId])
REFERENCES [dbo].[Consumers] ([ConsumerId])
GO

ALTER TABLE [dbo].[ConsumerNotes] CHECK CONSTRAINT [FK_ConsumerNotes_Consumers]
GO

ALTER TABLE [dbo].[ConsumerNotes]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerNotes_Contacts] FOREIGN KEY([ContactId])
REFERENCES [dbo].[Contacts] ([ContactId])
GO

ALTER TABLE [dbo].[ConsumerNotes] CHECK CONSTRAINT [FK_ConsumerNotes_Contacts]
GO

ALTER TABLE [dbo].[ConsumerNotes]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerNotes_SystemUsers] FOREIGN KEY([AddedById])
REFERENCES [dbo].[SystemUsers] ([UserId])
GO

ALTER TABLE [dbo].[ConsumerNotes] CHECK CONSTRAINT [FK_ConsumerNotes_SystemUsers]
GO

ALTER TABLE [dbo].[ConsumerNotes]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerNotes_SystemUsers1] FOREIGN KEY([UpdatedById])
REFERENCES [dbo].[SystemUsers] ([UserId])
GO

ALTER TABLE [dbo].[ConsumerNotes] CHECK CONSTRAINT [FK_ConsumerNotes_SystemUsers1]
GO


ALTER TABLE [dbo].[ConsumerNotes] ADD [AditionalInformation] nvarchar(MAX) NULL;
GO

--------------------------
ALTER TABLE [dbo].[Consumers]  WITH CHECK ADD  CONSTRAINT [FK_Consumers_Lists] FOREIGN KEY([Status])
REFERENCES [dbo].[Lists] ([ListId])
GO

ALTER TABLE [dbo].[Consumers] CHECK CONSTRAINT [FK_Consumers_Lists]
GO


ALTER TABLE [dbo].[Contacts] ADD [MobilePhone] nvarchar(100) NULL;
GO


