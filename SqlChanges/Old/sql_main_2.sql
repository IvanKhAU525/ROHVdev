--------------------------------------------------------------
ALTER TABLE [dbo].[ConsumerContactCalls] ADD [AddedById] int NULL;
GO
ALTER TABLE [dbo].[ConsumerContactCalls] ADD [UpdatedById] int NULL;
GO
ALTER TABLE [dbo].[ConsumerContactCalls] ADD [DateCreated] datetime NULL;
GO
ALTER TABLE [dbo].[ConsumerContactCalls] ADD [DateUpdated] datetime NULL;
GO

ALTER TABLE [dbo].[ConsumerContactCalls]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerContactCalls_SystemUsers] FOREIGN KEY([UpdatedById])
REFERENCES [dbo].[SystemUsers] ([UserId])
GO

ALTER TABLE [dbo].[ConsumerContactCalls] CHECK CONSTRAINT [FK_ConsumerContactCalls_SystemUsers]
GO

ALTER TABLE [dbo].[ConsumerContactCalls]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerContactCalls_SystemUsers1] FOREIGN KEY([AddedById])
REFERENCES [dbo].[SystemUsers] ([UserId])
GO

ALTER TABLE [dbo].[ConsumerContactCalls] CHECK CONSTRAINT [FK_ConsumerContactCalls_SystemUsers1]
GO


-----------------------------------

ALTER TABLE [dbo].[ConsumerEmployees] DROP CONSTRAINT [FK_ConsumerEmployees_Lists]
GO


ALTER TABLE [dbo].[ConsumerEmployees]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerEmployees_ServicesList] FOREIGN KEY([ServiceId])
REFERENCES [dbo].[ServicesList] ([ServiceId])
GO

ALTER TABLE [dbo].[ConsumerEmployees] CHECK CONSTRAINT [FK_ConsumerEmployees_ServicesList]
GO

--------------------------------------
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

INSERT INTO [dbo].[ConsumerNoteTypes] ([Id],[Name],[IsActive]) VALUES(1,'Worker',1)
GO
INSERT INTO [dbo].[ConsumerNoteTypes] ([Id],[Name],[IsActive]) VALUES(2,'Intake dept',1)
GO
INSERT INTO [dbo].[ConsumerNoteTypes] ([Id],[Name],[IsActive]) VALUES(3,'Hab dept',1)
GO
INSERT INTO [dbo].[ConsumerNoteTypes] ([Id],[Name],[IsActive]) VALUES(4,'Advocate',1)
GO
--------------------------------------


CREATE TABLE [dbo].[ConsumerNotes](
	[ConsumerNoteId] [int] IDENTITY(1,1) NOT NULL,
	[ConsumerId] [int] NOT NULL,
	[ContactId] [int] NULL,
	[TypeId] [int] NULL,
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

ALTER TABLE [dbo].[ConsumerNotes]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerNotes_ConsumerNoteTypes] FOREIGN KEY([TypeId])
REFERENCES [dbo].[ConsumerNoteTypes] ([Id])
GO

ALTER TABLE [dbo].[ConsumerNotes] CHECK CONSTRAINT [FK_ConsumerNotes_ConsumerNoteTypes]
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

-----------------------------------------------------------------------------------
ALTER TABLE [dbo].[EmployeeDocuments] ADD [DocumentContentType] nvarchar(2024) NULL;

DROP TABLE [dbo].[EmployeeDocumentNotes]
GO

CREATE TABLE [dbo].[EmployeeDocumentNotes](
	[EmployeeDocumentNoteId] [int] IDENTITY(1,1) NOT NULL,
	[DocumentId] [int] NOT NULL,
	[AddedById] [int] NOT NULL,
	[Note] [nvarchar](1024) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_EmployeeDocumentNotes] PRIMARY KEY CLUSTERED 
(
	[EmployeeDocumentNoteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[EmployeeDocumentNotes]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeDocumentNotes_EmployeeDocumentNotes] FOREIGN KEY([DocumentId])
REFERENCES [dbo].[EmployeeDocuments] ([EmployeeDocumentId])
GO

ALTER TABLE [dbo].[EmployeeDocumentNotes] CHECK CONSTRAINT [FK_EmployeeDocumentNotes_EmployeeDocumentNotes]
GO

ALTER TABLE [dbo].[EmployeeDocumentNotes]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeDocumentNotes_SystemUsers] FOREIGN KEY([AddedById])
REFERENCES [dbo].[SystemUsers] ([UserId])
GO

ALTER TABLE [dbo].[EmployeeDocumentNotes] CHECK CONSTRAINT [FK_EmployeeDocumentNotes_SystemUsers]
GO

-----------------------------


CREATE TABLE [dbo].[NotificationTypes](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](1014) NOT NULL,
	[ActionNo] [int] NOT NULL,
 CONSTRAINT [PK_NotificationTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[NotificationTypes] ([Id] ,[Name] ,[ActionNo]) VALUES (1,'Once',1)
GO
INSERT INTO [dbo].[NotificationTypes] ([Id] ,[Name] ,[ActionNo]) VALUES (2,'Every 1 week',2)
GO
INSERT INTO [dbo].[NotificationTypes] ([Id] ,[Name] ,[ActionNo]) VALUES (3,'Every 1 month',3)
GO
INSERT INTO [dbo].[NotificationTypes] ([Id] ,[Name] ,[ActionNo]) VALUES (4,'Every 6 months',4)
GO
INSERT INTO [dbo].[NotificationTypes] ([Id] ,[Name] ,[ActionNo]) VALUES (5,'Every 1 year',5)
GO

--------------------------------------

CREATE TABLE [dbo].[NotificationStatuses](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](512) NOT NULL,
	[ShortName] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_NotificationStatuses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[NotificationStatuses] ([Id] ,[Name] ,[ShortName]) VALUES(1,'Stopped','Stopped')
GO
INSERT INTO [dbo].[NotificationStatuses] ([Id] ,[Name] ,[ShortName]) VALUES(2,'Working','Working')
GO

----------------------------------------


CREATE TABLE [dbo].[ConsumerNotificationSettings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ConsumerId] [int] NOT NULL,
	[RepetingTypeId] [int] NOT NULL,
	[StatusId] [int] NOT NULL,
	[Name] [nvarchar](1024) NOT NULL,
	[Note] [nvarchar](1024) NULL,
	[DateStart] [datetime] NOT NULL,
	[AddedById] [int] NOT NULL,
	[UpdatedById] [int] NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateUpdated] [datetime] NULL,
 CONSTRAINT [PK_ConsumerNotificationSettings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ConsumerNotificationSettings]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerNotificationSettings_Consumers] FOREIGN KEY([ConsumerId])
REFERENCES [dbo].[Consumers] ([ConsumerId])
GO

ALTER TABLE [dbo].[ConsumerNotificationSettings] CHECK CONSTRAINT [FK_ConsumerNotificationSettings_Consumers]
GO

ALTER TABLE [dbo].[ConsumerNotificationSettings]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerNotificationSettings_NotificationStatuses] FOREIGN KEY([StatusId])
REFERENCES [dbo].[NotificationStatuses] ([Id])
GO

ALTER TABLE [dbo].[ConsumerNotificationSettings] CHECK CONSTRAINT [FK_ConsumerNotificationSettings_NotificationStatuses]
GO

ALTER TABLE [dbo].[ConsumerNotificationSettings]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerNotificationSettings_NotificationTypes] FOREIGN KEY([RepetingTypeId])
REFERENCES [dbo].[NotificationTypes] ([Id])
GO

ALTER TABLE [dbo].[ConsumerNotificationSettings] CHECK CONSTRAINT [FK_ConsumerNotificationSettings_NotificationTypes]
GO

ALTER TABLE [dbo].[ConsumerNotificationSettings]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerNotificationSettings_SystemUsers] FOREIGN KEY([AddedById])
REFERENCES [dbo].[SystemUsers] ([UserId])
GO

ALTER TABLE [dbo].[ConsumerNotificationSettings] CHECK CONSTRAINT [FK_ConsumerNotificationSettings_SystemUsers]
GO

ALTER TABLE [dbo].[ConsumerNotificationSettings]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerNotificationSettings_SystemUsers1] FOREIGN KEY([UpdatedById])
REFERENCES [dbo].[SystemUsers] ([UserId])
GO

ALTER TABLE [dbo].[ConsumerNotificationSettings] CHECK CONSTRAINT [FK_ConsumerNotificationSettings_SystemUsers1]
GO
---------------------------------------------
ALTER TABLE [dbo].[ConsumerPrintDocuments] DROP CONSTRAINT [FK_ConsumerPrintDocuments_DocumentPrintTypes]
GO
ALTER TABLE [ConsumerPrintDocuments] DROP COLUMN [PrintDocumnentTypeId];
GO

-------------------------


CREATE TABLE [dbo].[ConsumerNotificationRecipients](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NotificationId] [int] NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[Position] [nvarchar](100) NULL,
	[Name] [nvarchar](1024) NULL,
	[AddedById] [int] NOT NULL,
	[UpdatedById] [int] NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateUpdated] [datetime] NULL,
 CONSTRAINT [PK_ConsumerNotificationRecipients] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ConsumerNotificationRecipients]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerNotificationRecipients_ConsumerNotificationRecipients] FOREIGN KEY([AddedById])
REFERENCES [dbo].[SystemUsers] ([UserId])
GO

ALTER TABLE [dbo].[ConsumerNotificationRecipients] CHECK CONSTRAINT [FK_ConsumerNotificationRecipients_ConsumerNotificationRecipients]
GO

ALTER TABLE [dbo].[ConsumerNotificationRecipients]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerNotificationRecipients_ConsumerNotificationSettings] FOREIGN KEY([NotificationId])
REFERENCES [dbo].[ConsumerNotificationSettings] ([Id])
GO

ALTER TABLE [dbo].[ConsumerNotificationRecipients] CHECK CONSTRAINT [FK_ConsumerNotificationRecipients_ConsumerNotificationSettings]
GO

ALTER TABLE [dbo].[ConsumerNotificationRecipients]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerNotificationRecipients_SystemUsers] FOREIGN KEY([UpdatedById])
REFERENCES [dbo].[SystemUsers] ([UserId])
GO

ALTER TABLE [dbo].[ConsumerNotificationRecipients] CHECK CONSTRAINT [FK_ConsumerNotificationRecipients_SystemUsers]
GO


-------------------------
CREATE TABLE [dbo].[Advocates](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[Company] [nvarchar](255) NOT NULL,
	[Address1] [nvarchar](1024) NULL,
	[Address2] [nvarchar](1024) NULL,
	[City] [nvarchar](255) NULL,
	[State] [nvarchar](2) NULL,
	[Zip] [nvarchar](14) NULL,
	[Specialization] [nvarchar](1024) NULL,
	[Phone] [nvarchar](20) NULL,
	[Email] [nvarchar](255) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[Notes] [nvarchar](2015) NULL,
 CONSTRAINT [PK_Advocates] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

--------------------------------
ALTER TABLE [dbo].[Consumers] ADD [AdvocateId] int NULL;
GO

ALTER TABLE [dbo].[Consumers]  WITH CHECK ADD  CONSTRAINT [FK_Consumers_Advocates] FOREIGN KEY([AdvocateId])
REFERENCES [dbo].[Advocates] ([Id])
GO

ALTER TABLE [dbo].[Consumers] CHECK CONSTRAINT [FK_Consumers_Advocates]
GO
