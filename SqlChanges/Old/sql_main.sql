CREATE TABLE [dbo].[ConsumerContacts](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ConsumerId] [int] NOT NULL,
	[ContactId] [int] NOT NULL,
 CONSTRAINT [PK_ConsumerContacts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ConsumerContacts]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerContacts_Consumers] FOREIGN KEY([ConsumerId])
REFERENCES [dbo].[Consumers] ([ConsumerId])
GO

ALTER TABLE [dbo].[ConsumerContacts] CHECK CONSTRAINT [FK_ConsumerContacts_Consumers]
GO

ALTER TABLE [dbo].[ConsumerContacts]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerContacts_Contacts] FOREIGN KEY([ContactId])
REFERENCES [dbo].[Contacts] ([ContactId])
GO

ALTER TABLE [dbo].[ConsumerContacts] CHECK CONSTRAINT [FK_ConsumerContacts_Contacts]
GO


CREATE TABLE [dbo].[Agencies](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NameCompany] [nvarchar](1024) NOT NULL,
 CONSTRAINT [PK_Agencies] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[Agencies]([NameCompany]) VALUES ('Yedei Chesed, Inc');
INSERT INTO [dbo].[Agencies]([NameCompany]) VALUES ('Women''s League Community Residences, Inc.');
INSERT INTO [dbo].[Agencies]([NameCompany]) VALUES ('Special Care, Inc.');
INSERT INTO [dbo].[Agencies]([NameCompany]) VALUES ('Share of New Square, Inc.');
INSERT INTO [dbo].[Agencies]([NameCompany]) VALUES ('Rayim of Hudson Valley, Inc.');
INSERT INTO [dbo].[Agencies]([NameCompany]) VALUES ('Human Care');
INSERT INTO [dbo].[Agencies]([NameCompany]) VALUES ('Hamaspik of Kings County, Inc.');
INSERT INTO [dbo].[Agencies]([NameCompany]) VALUES ('Hamaspik of Rockland County, Inc.'); 
INSERT INTO [dbo].[Agencies]([NameCompany]) VALUES ('Ahivim, Inc.'); 
INSERT INTO [dbo].[Agencies]([NameCompany]) VALUES ('Family Empowerment'); 
GO

ALTER TABLE [dbo].[Consumers] ADD [AgencyNameId] int NULL;
GO

ALTER TABLE [dbo].[Consumers]  WITH CHECK ADD  CONSTRAINT [FK_Consumers_Agencies] FOREIGN KEY([AgencyNameId])
REFERENCES [dbo].[Agencies] ([Id])
GO

ALTER TABLE [dbo].[Consumers] CHECK CONSTRAINT [FK_Consumers_Agencies]
GO																																	
UPDATE [dbo].[Consumers] SET AgencyNameId = (SELECT Id FROM Agencies Where NameCompany = 'Yedei Chesed, Inc') Where AgencyName LIKE 'Yedei%';
UPDATE [dbo].[Consumers] SET AgencyNameId = (SELECT Id FROM Agencies  Where NameCompany = 'Women''s League Community Residences, Inc.') Where AgencyName LIKE 'Women''s League%';
UPDATE [dbo].[Consumers] SET AgencyNameId = (SELECT Id FROM Agencies Where NameCompany = 'Special Care, Inc.') Where AgencyName LIKE 'Special Care%';
UPDATE [dbo].[Consumers] SET AgencyNameId = (SELECT Id FROM Agencies Where NameCompany = 'Share of New Square, Inc.') Where AgencyName LIKE 'Share of New Square%';
UPDATE [dbo].[Consumers] SET AgencyNameId = (SELECT Id FROM Agencies Where NameCompany = 'Rayim of Hudson Valley, Inc.') Where AgencyName LIKE 'Rayim%';
UPDATE [dbo].[Consumers] SET AgencyNameId = (SELECT Id FROM Agencies Where NameCompany = 'Human Care') Where AgencyName LIKE 'Human Care%';
UPDATE [dbo].[Consumers] SET AgencyNameId = (SELECT Id FROM Agencies Where NameCompany = 'Hamaspik of Kings County, Inc.') Where AgencyName LIKE 'Hamspik of Kings%' OR AgencyName LIKE 'Hamaspik';
UPDATE [dbo].[Consumers] SET AgencyNameId = (SELECT Id FROM Agencies Where NameCompany = 'Hamaspik of Rockland County, Inc.') Where AgencyName LIKE 'Hamaspik of Rockland County' OR  AgencyName LIKE 'Hampaspik of Rockland County' OR  AgencyName LIKE 'Hampaspik' ;
UPDATE [dbo].[Consumers] SET AgencyNameId = (SELECT Id FROM Agencies Where NameCompany = 'Ahivim, Inc.') Where AgencyName LIKE 'Ahivim%';
UPDATE [dbo].[Consumers] SET AgencyNameId = (SELECT Id FROM Agencies Where NameCompany = 'Family Empowerment') Where AgencyName LIKE 'Family Empowerment';


ALTER TABLE [dbo].[Consumers] ADD [ServiceCoordinatorId] int NULL;
GO
ALTER TABLE [dbo].[Consumers]  WITH CHECK ADD  CONSTRAINT [FK_Consumers_ServiceCoordinator] FOREIGN KEY([ServiceCoordinatorId])
REFERENCES [dbo].[Contacts] ([ContactId])
GO
ALTER TABLE [dbo].[Consumers] CHECK CONSTRAINT [FK_Consumers_ServiceCoordinator]
GO


ALTER TABLE [dbo].[Contacts] ADD [AgencyNameId] int NULL;
GO

ALTER TABLE [dbo].[Contacts]  WITH CHECK ADD  CONSTRAINT [FK_Contacts_Agencies] FOREIGN KEY([AgencyNameId])
REFERENCES [dbo].[Agencies] ([Id])
GO

ALTER TABLE [dbo].[Contacts] CHECK CONSTRAINT [FK_Contacts_Agencies]
GO

--------SERVICE COORDINATOR ID REWRITE---------------

BEGIN
	DECLARE  @ContactId int, @AgencyId int, @isMidleName int,@CompanyName  nvarchar(1000),@ServiceCoordinator nvarchar(1000), @MidleName nvarchar(1000), @FirstName nvarchar(1000),@LastName nvarchar(1000), @ConsumerId int;

	DECLARE consumer_cursor CURSOR FOR 
	SELECT  [ServiceCoordinator],[ConsumerId], [AgencyNameId]
	FROM [Consumers]
	WHERE ServiceCoordinator IS NOT NULL

	OPEN consumer_cursor
	FETCH NEXT FROM consumer_cursor
	INTO @ServiceCoordinator,@ConsumerId, @AgencyId

	PRINT 'START.....'
	WHILE @@FETCH_STATUS = 0
	BEGIN		
	   SET @MidleName =NULL;
	   SET @FirstName = NULL;
	   SET @ContactId = 0;
	   SET @ServiceCoordinator = REPLACE(@ServiceCoordinator, 'Mr. ', '');
	   SET @ServiceCoordinator = REPLACE(@ServiceCoordinator, 'Mrs. ', '');	   
	   SET @LastName = @ServiceCoordinator;
	    SELECT  @isMidleName = CHARINDEX(',',@ServiceCoordinator)
		IF @isMidleName !=0
		BEGIN			
			SELECT
				@LastName = LTRIM(RTRIM(SUBSTRING(@ServiceCoordinator, 1, CHARINDEX(' ',@ServiceCoordinator) - 1))),
				@FirstName =  LTRIM(RTRIM(SUBSTRING(@ServiceCoordinator, CHARINDEX(' ',@ServiceCoordinator) + 1, 8000)))
			SET @LastName = REPLACE(@LastName, ',', '');
		END 
		ELSE	
		BEGIN
			SELECT  @isMidleName = CHARINDEX(' ',@ServiceCoordinator)
			IF @isMidleName !=0
			BEGIN			
				SELECT
					@FirstName = LTRIM(RTRIM(SUBSTRING(@ServiceCoordinator, 1, CHARINDEX(' ',@ServiceCoordinator) - 1))),
					@LastName =  LTRIM(RTRIM(SUBSTRING(@ServiceCoordinator, CHARINDEX(' ',@ServiceCoordinator) + 1, 8000)))
			END 	
		END

		SELECT  @isMidleName = CHARINDEX(' ',@LastName)
		IF @isMidleName !=0
		BEGIN
			SELECT
			@MidleName = LTRIM(RTRIM(SUBSTRING(@LastName, 1, CHARINDEX(' ',@LastName) - 1))),
			@LastName =  LTRIM(RTRIM(SUBSTRING(@LastName, CHARINDEX(' ',@LastName) + 1, 8000)))

		END 	    	
		SELECT @FirstName = UPPER(LEFT(@FirstName,1))+LOWER(SUBSTRING(@FirstName,2,LEN(@FirstName)));
		SELECT @LastName = UPPER(LEFT(@LastName,1))+LOWER(SUBSTRING(@LastName,2,LEN(@LastName)));
		SELECT @MidleName = UPPER(LEFT(@MidleName,1))+LOWER(SUBSTRING(@MidleName,2,LEN(@LastName)));

		SELECT @ContactId = ContactId FROM Contacts WHERE LastName  LIKE @LastName;
		IF @ContactId =0
		BEGIN 
			SELECT @CompanyName = NameCompany FROM Agencies WHere Id = @AgencyId;
			INSERT INTO Contacts(CategoryId,AgencyNameId,CompanyName,[FirstName],[MiddleName],[LastName],[IsServiceCoordinator]) 
				VALUES(2900, @AgencyId,@CompanyName,@FirstName,@MidleName,@LastName,1);
			SET @ContactId =@@IDENTITY;
		END	
		UPDATE [Consumers] SET ServiceCoordinatorId = @ContactId WHERE ConsumerId=@ConsumerId;

	   FETCH NEXT FROM consumer_cursor
	 INTO @ServiceCoordinator,@ConsumerId,@AgencyId
	END 
	CLOSE consumer_cursor;
	DEALLOCATE consumer_cursor;
	PRINT 'FINISH.....'
END
GO

-----------------------------------------------------


CREATE TABLE [dbo].[ConsumerEmployees](
	[ConsumerEmployeeId] [int] IDENTITY(1,1) NOT NULL,
	[ConsumerId] [int] NOT NULL,
	[ContactId] [int] NOT NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[ServiceId] [int] NULL,
 CONSTRAINT [PK_[ConsumerEmployees] PRIMARY KEY CLUSTERED 
(
	[ConsumerEmployeeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ConsumerEmployees]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerEmployees_ConsumerServices] FOREIGN KEY([ConsumerId])
REFERENCES [dbo].[Consumers] ([ConsumerId])
GO

ALTER TABLE [dbo].[ConsumerEmployees] CHECK CONSTRAINT [FK_ConsumerEmployees_ConsumerServices]
GO

ALTER TABLE [dbo].[ConsumerEmployees]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerEmployees_Contacts] FOREIGN KEY([ContactId])
REFERENCES [dbo].[Contacts] ([ContactId])
GO

ALTER TABLE [dbo].[ConsumerEmployees] CHECK CONSTRAINT [FK_ConsumerEmployees_Contacts]
GO

ALTER TABLE [dbo].[ConsumerEmployees]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerEmployees_Lists] FOREIGN KEY([ServiceId])
REFERENCES [dbo].[Lists] ([ListId])
GO

ALTER TABLE [dbo].[ConsumerEmployees] CHECK CONSTRAINT [FK_ConsumerEmployees_Lists]
GO
-----------------------------------ConsumerServices------------------

ALTER TABLE [dbo].[ConsumerServices]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerServices_UnitQuantity] FOREIGN KEY([UnitQuantities])
REFERENCES [dbo].[Lists] ([ListId])
GO

ALTER TABLE [dbo].[ConsumerServices] CHECK CONSTRAINT [FK_ConsumerServices_UnitQuantity]
GO
----------------------------------ConsumerRates

ALTER TABLE [dbo].[Consumers] ADD [Rate] decimal(18,2) NULL;
GO
ALTER TABLE [dbo].[Consumers] ADD [MaxHoursPerWeek] int NULL;
GO
ALTER TABLE [dbo].[Consumers] ADD [MaxHoursPerYear] int NULL;
GO
ALTER TABLE [dbo].[Consumers] ADD [RateNote] nvarchar(2024) NULL;
GO

----------------------------------Coordinators
ALTER TABLE [dbo].[Consumers] ADD [MSCId] int NULL;
GO
ALTER TABLE [dbo].[Consumers]  WITH CHECK ADD  CONSTRAINT [FK_Consumers_Contact_MSC] FOREIGN KEY([MSCId])
REFERENCES [dbo].[Contacts] ([ContactId])
GO

ALTER TABLE [dbo].[Consumers] CHECK CONSTRAINT [FK_Consumers_Contact_MSC]
GO
-----
ALTER TABLE [dbo].[Consumers] ADD [DHCoordinatorId] int NULL;
GO
ALTER TABLE [dbo].[Consumers]  WITH CHECK ADD  CONSTRAINT [FK_Consumers_Contact_DH] FOREIGN KEY([DHCoordinatorId])
REFERENCES [dbo].[Contacts] ([ContactId])
GO
ALTER TABLE [dbo].[Consumers] CHECK CONSTRAINT [FK_Consumers_Contact_DH]
GO
-----
ALTER TABLE [dbo].[Consumers] ADD [CHCoordinatorId] int NULL;
GO
ALTER TABLE [dbo].[Consumers]  WITH CHECK ADD  CONSTRAINT [FK_Consumers_Contact_CH] FOREIGN KEY([CHCoordinatorId])
REFERENCES [dbo].[Contacts] ([ContactId])
GO

ALTER TABLE [dbo].[Consumers] CHECK CONSTRAINT [FK_Consumers_Contact_CH]
GO



-------------------------SystemUSers


CREATE TABLE [dbo].[SystemUsers](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[AspNetUserId] [nvarchar](128) NOT NULL,
	[FirstName] [nvarchar](255) NOT NULL,
	[LastName] [nvarchar](255) NOT NULL,
	[Email] [nvarchar](1024) NOT NULL,
	[IsDeleted] [bit] NOT NULL CONSTRAINT [DF_SystemUsers_IsDeleted]  DEFAULT ((0)),
 CONSTRAINT [PK_SystemUsers] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SystemUsers]  WITH CHECK ADD  CONSTRAINT [FK_SystemUsers_AspNetUsers] FOREIGN KEY([AspNetUserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
GO

ALTER TABLE [dbo].[SystemUsers] CHECK CONSTRAINT [FK_SystemUsers_AspNetUsers]
GO


-----------------------------------Documents

CREATE TABLE [dbo].[EmployeeDocumentStatuses](
	[DocumentEmployeeStatusId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_DocumentEmployeeStatuses] PRIMARY KEY CLUSTERED 
(
	[DocumentEmployeeStatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
INSERT INTO [dbo].[EmployeeDocumentStatuses] ([Name]) VALUES ('Waiting')
GO
INSERT INTO [dbo].[EmployeeDocumentStatuses] ([Name]) VALUES ('Approved')
GO
INSERT INTO [dbo].[EmployeeDocumentStatuses] ([Name]) VALUES ('Back to employee')
GO
INSERT INTO [dbo].[EmployeeDocumentStatuses] ([Name]) VALUES ('Finished')
GO

---
CREATE TABLE [dbo].[EmployeeDocumentTypes](
	[EmployeeDocumentTypeId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](512) NOT NULL,
 CONSTRAINT [PK_EmployeeDocumentTypes] PRIMARY KEY CLUSTERED 
(
	[EmployeeDocumentTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[EmployeeDocumentTypes] ([Name]) VALUES ('Respite Documentation Record - Individual Summary')
GO

INSERT INTO [dbo].[EmployeeDocumentTypes] ([Name]) VALUES ('Community Habilitation Monthly Progress Summary')
GO

INSERT INTO [dbo].[EmployeeDocumentTypes] ([Name]) VALUES ('Monthly Community Habilitation Am-Pm Sign-in')
GO

----
CREATE TABLE [dbo].[EmployeeDocuments](
	[EmployeeDocumentId] [int] IDENTITY(1,1) NOT NULL,
	[ConsumerId] [int] NOT NULL,
	[DocumentTypeId] [int] NOT NULL,
	[DocumentStatusId] [int] NOT NULL,
	[EmployeeId] [int] NOT NULL,
	[AddedById] [int] NOT NULL,
	[UpdatedById] [int] NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateUpdated] [datetime] NULL,
	[DocumentPath] [nvarchar](2024) NULL,
 CONSTRAINT [PK_EmployeeDouments] PRIMARY KEY CLUSTERED 
(
	[EmployeeDocumentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[EmployeeDocuments]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeDocuments_Consumers] FOREIGN KEY([ConsumerId])
REFERENCES [dbo].[Consumers] ([ConsumerId])
GO

ALTER TABLE [dbo].[EmployeeDocuments] CHECK CONSTRAINT [FK_EmployeeDocuments_Consumers]
GO

ALTER TABLE [dbo].[EmployeeDocuments]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeDocuments_EmployeeDocuments] FOREIGN KEY([EmployeeDocumentId])
REFERENCES [dbo].[EmployeeDocuments] ([EmployeeDocumentId])
GO

ALTER TABLE [dbo].[EmployeeDocuments] CHECK CONSTRAINT [FK_EmployeeDocuments_EmployeeDocuments]
GO

ALTER TABLE [dbo].[EmployeeDocuments]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeDouments_Contacts] FOREIGN KEY([EmployeeId])
REFERENCES [dbo].[Contacts] ([ContactId])
GO

ALTER TABLE [dbo].[EmployeeDocuments] CHECK CONSTRAINT [FK_EmployeeDouments_Contacts]
GO

ALTER TABLE [dbo].[EmployeeDocuments]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeDouments_EmployeeDocumentStatuses] FOREIGN KEY([DocumentStatusId])
REFERENCES [dbo].[EmployeeDocumentStatuses] ([DocumentEmployeeStatusId])
GO

ALTER TABLE [dbo].[EmployeeDocuments] CHECK CONSTRAINT [FK_EmployeeDouments_EmployeeDocumentStatuses]
GO

ALTER TABLE [dbo].[EmployeeDocuments]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeDouments_EmployeeDocumentTypes] FOREIGN KEY([DocumentTypeId])
REFERENCES [dbo].[EmployeeDocumentTypes] ([EmployeeDocumentTypeId])
GO

ALTER TABLE [dbo].[EmployeeDocuments] CHECK CONSTRAINT [FK_EmployeeDouments_EmployeeDocumentTypes]
GO

ALTER TABLE [dbo].[EmployeeDocuments]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeDouments_SystemUsers_Added] FOREIGN KEY([AddedById])
REFERENCES [dbo].[SystemUsers] ([UserId])
GO

ALTER TABLE [dbo].[EmployeeDocuments] CHECK CONSTRAINT [FK_EmployeeDouments_SystemUsers_Added]
GO

ALTER TABLE [dbo].[EmployeeDocuments]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeDouments_SystemUsers_Updated] FOREIGN KEY([UpdatedById])
REFERENCES [dbo].[SystemUsers] ([UserId])
GO

ALTER TABLE [dbo].[EmployeeDocuments] CHECK CONSTRAINT [FK_EmployeeDouments_SystemUsers_Updated]
GO
---------------

CREATE TABLE [dbo].[EmployeeDocumentNotes](
	[EmployeeDocumentNoteId] [int] NOT NULL,
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

----------------------ConsumerHabPlans-------------------

CREATE TABLE [dbo].[ConsumerHabPlanDurations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_ConsumerHabPlanDurations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
---------------------
CREATE TABLE [dbo].[ConsumerHabPlanFrequencies](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_ConsumerHabPlanFrequencies] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
---------------------

CREATE TABLE [dbo].[ConsumerHabPlanStatuses](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_ConsumerHabPlanStatuses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

--------------------

CREATE TABLE [dbo].[ConsumerHabPlans](
	[ConsumerHabPlanId] [int] IDENTITY(1,1) NOT NULL,
	[ConsumerId] [int] NOT NULL,
	[HabServiceId] [int] NOT NULL,
	[CoordinatorId] [int] NOT NULL,
	[FrequencyId] [int] NOT NULL,
	[DurationId] [int] NOT NULL,
	[StatusId] [int] NOT NULL,
	[AddedById] [int] NOT NULL,
	[UpdatedById] [int] NULL,
	[Name] [nvarchar](512) NOT NULL,
	[QMRP] [nvarchar](512) NOT NULL,
	[EnrolmentDate] [datetime] NOT NULL,
	[DatePlan] [datetime] NOT NULL,
	[EffectivePlan] [datetime] NOT NULL,
	[IsAproved] [bit] NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateUpdated] [datetime] NULL,
 CONSTRAINT [PK_ConsumerHabPlans] PRIMARY KEY CLUSTERED 
(
	[ConsumerHabPlanId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ConsumerHabPlans]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerHabPlans_ConsumerHabPlanDurations] FOREIGN KEY([DurationId])
REFERENCES [dbo].[ConsumerHabPlanDurations] ([Id])
GO

ALTER TABLE [dbo].[ConsumerHabPlans] CHECK CONSTRAINT [FK_ConsumerHabPlans_ConsumerHabPlanDurations]
GO

ALTER TABLE [dbo].[ConsumerHabPlans]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerHabPlans_ConsumerHabPlanFrequencies] FOREIGN KEY([FrequencyId])
REFERENCES [dbo].[ConsumerHabPlanFrequencies] ([Id])
GO

ALTER TABLE [dbo].[ConsumerHabPlans] CHECK CONSTRAINT [FK_ConsumerHabPlans_ConsumerHabPlanFrequencies]
GO

ALTER TABLE [dbo].[ConsumerHabPlans]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerHabPlans_ConsumerHabPlanStatuses] FOREIGN KEY([StatusId])
REFERENCES [dbo].[ConsumerHabPlanStatuses] ([Id])
GO

ALTER TABLE [dbo].[ConsumerHabPlans] CHECK CONSTRAINT [FK_ConsumerHabPlans_ConsumerHabPlanStatuses]
GO

ALTER TABLE [dbo].[ConsumerHabPlans]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerHabPlans_Consumers] FOREIGN KEY([ConsumerId])
REFERENCES [dbo].[Consumers] ([ConsumerId])
GO

ALTER TABLE [dbo].[ConsumerHabPlans] CHECK CONSTRAINT [FK_ConsumerHabPlans_Consumers]
GO

ALTER TABLE [dbo].[ConsumerHabPlans]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerHabPlans_Contacts] FOREIGN KEY([CoordinatorId])
REFERENCES [dbo].[Contacts] ([ContactId])
GO

ALTER TABLE [dbo].[ConsumerHabPlans] CHECK CONSTRAINT [FK_ConsumerHabPlans_Contacts]
GO

ALTER TABLE [dbo].[ConsumerHabPlans]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerHabPlans_ServicesList] FOREIGN KEY([HabServiceId])
REFERENCES [dbo].[ServicesList] ([ServiceId])
GO

ALTER TABLE [dbo].[ConsumerHabPlans] CHECK CONSTRAINT [FK_ConsumerHabPlans_ServicesList]
GO

ALTER TABLE [dbo].[ConsumerHabPlans]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerHabPlans_SystemUsers] FOREIGN KEY([AddedById])
REFERENCES [dbo].[SystemUsers] ([UserId])
GO

ALTER TABLE [dbo].[ConsumerHabPlans] CHECK CONSTRAINT [FK_ConsumerHabPlans_SystemUsers]
GO

ALTER TABLE [dbo].[ConsumerHabPlans]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerHabPlans_SystemUsers1] FOREIGN KEY([UpdatedById])
REFERENCES [dbo].[SystemUsers] ([UserId])
GO

ALTER TABLE [dbo].[ConsumerHabPlans] CHECK CONSTRAINT [FK_ConsumerHabPlans_SystemUsers1]
GO

-------------------

INSERT INTO [dbo].[ConsumerHabPlanStatuses]
           ([Name])
     VALUES
           ('In Review')
GO
INSERT INTO [dbo].[ConsumerHabPlanStatuses]
           ([Name])
     VALUES
           ('Approved')
GO


INSERT INTO [dbo].[ConsumerHabPlanFrequencies]
           ([Name])
     VALUES
           ('Hourly')
GO
INSERT INTO [dbo].[ConsumerHabPlanFrequencies]
           ([Name])
     VALUES
           ('Daily')
GO
INSERT INTO [dbo].[ConsumerHabPlanFrequencies]
           ([Name])
     VALUES
           ('Weekly')
GO
INSERT INTO [dbo].[ConsumerHabPlanFrequencies]
           ([Name])
     VALUES
           ('Monthly')
GO

INSERT INTO [dbo].ConsumerHabPlanDurations
           ([Name])
     VALUES
           ('Ongoing')
GO

-------------------


CREATE TABLE [dbo].[ConsumerHabPlanValuedOutcomes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HabPlanId] [int] NULL,
	[ValuedOutcome] [nvarchar](2048) NULL,
 CONSTRAINT [PK_ConsumerHabPlanValuedOutcomes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ConsumerHabPlanValuedOutcomes]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerHabPlanValuedOutcomes_ConsumerHabPlans] FOREIGN KEY([HabPlanId])
REFERENCES [dbo].[ConsumerHabPlans] ([ConsumerHabPlanId])
GO

ALTER TABLE [dbo].[ConsumerHabPlanValuedOutcomes] CHECK CONSTRAINT [FK_ConsumerHabPlanValuedOutcomes_ConsumerHabPlans]
GO

----------------

CREATE TABLE [dbo].[ConsumerHabPlanVOServeActions](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HabPlanValuedOutcomeId] [int] NOT NULL,
	[ServeAndAction] [nvarchar](2048) NOT NULL,
 CONSTRAINT [PK_ConsumerHabPlanVOServeActions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ConsumerHabPlanVOServeActions]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerHabPlanVOServeActions_ConsumerHabPlanValuedOutcomes] FOREIGN KEY([HabPlanValuedOutcomeId])
REFERENCES [dbo].[ConsumerHabPlanValuedOutcomes] ([Id])
GO

ALTER TABLE [dbo].[ConsumerHabPlanVOServeActions] CHECK CONSTRAINT [FK_ConsumerHabPlanVOServeActions_ConsumerHabPlanValuedOutcomes]
GO


----------------------Logs

CREATE TABLE [dbo].[Logs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Date] [datetime] NULL,
	[Message] [nvarchar](max) NULL,
	[InnerException] [nvarchar](max) NULL,
	[StackTrace] [nvarchar](max) NULL,
	[Source] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

-----------------ContactTypes
CREATE TABLE [dbo].[ContactTypes](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](1024) NOT NULL,
 CONSTRAINT [PK_ContactTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Contacts] ADD [ContactTypeId] int NULL;
ALTER TABLE [dbo].[Contacts]  WITH CHECK ADD  CONSTRAINT [FK_Contacts_ContactTypes] FOREIGN KEY([ContactTypeId])
REFERENCES [dbo].[ContactTypes] ([Id])
GO

ALTER TABLE [dbo].[Contacts] CHECK CONSTRAINT [FK_Contacts_ContactTypes]
GO

-----------------Departments---------------------
CREATE TABLE [dbo].[Departments](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](1024) NOT NULL,
 CONSTRAINT [PK_Departments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[Contacts] ADD [DepartmentId] int NULL;
ALTER TABLE [dbo].[Contacts]  WITH CHECK ADD  CONSTRAINT [FK_Contacts_Departments] FOREIGN KEY([DepartmentId])
REFERENCES [dbo].[Departments] ([Id])
GO

ALTER TABLE [dbo].[Contacts] CHECK CONSTRAINT [FK_Contacts_Departments]
GO
---------------------
INSERT INTO [dbo].[ContactTypes] ([Id],[Name]) VALUES(1, 'CH Coordinator');
INSERT INTO [dbo].[ContactTypes] ([Id],[Name]) VALUES(2, 'DH Coordinator');
INSERT INTO [dbo].[ContactTypes] ([Id],[Name]) VALUES(3, 'Hab Coordinator');
INSERT INTO [dbo].[ContactTypes] ([Id],[Name]) VALUES(4, 'Direct care worker (DCW)');
INSERT INTO [dbo].[ContactTypes] ([Id],[Name]) VALUES(5, 'Qualified Mental Retardation Professional (QMRP)');
INSERT INTO [dbo].[ContactTypes] ([Id],[Name]) VALUES(6, 'MSC');
--------------------
INSERT INTO [dbo].[Departments] ([Id] ,[Name]) VALUES(1, 'Hab dept');
INSERT INTO [dbo].[Departments] ([Id] ,[Name]) VALUES(2, 'Intake dept');

--------------------STATES
CREATE TABLE [dbo].[States](
	[StateId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](1024) NOT NULL,
	[Code] [nvarchar](2) NOT NULL,
	[SortingPlace] [int] NOT NULL CONSTRAINT [DF_States_SortingPlace]  DEFAULT ((0)),
 CONSTRAINT [PK_States] PRIMARY KEY CLUSTERED 
(
	[StateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO



INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Alabama','AL',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Alaska','AK',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Arizona','AZ',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Arkansas','AR',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('California','CA',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Colorado','CO',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Connecticut','CT',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Delaware','DE',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Florida','FL',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Georgia','GA',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Hawaii','HI',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Idaho','ID',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Illinois','IL',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Indiana','IN',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Iowa','IA',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Kansas','KS',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Kentucky','KY',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Louisiana','LA',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Maine','ME',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Maryland','MD',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Massachusetts','MA',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Michigan','MI',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Minnesota','MN',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Mississippi','MS',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Missouri','MO',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Montana','MT',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Nebraska','NE',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Nevada','NV',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('New Hampshire','NH',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('New Jersey','NJ',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('New Mexico','NM',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('New York','NY',1);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('North Carolina','NC',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('North Dakota','ND',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Ohio','OH',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Oklahoma','OK',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Oregon','OR',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Pennsylvania','PA',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Rhode Island','RI',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('South Carolina','SC',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('South Dakota','SD',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Tennessee','TN',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Texas','TX',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Utah','UT',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Vermont','VT',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Virginia','VA',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Washington','WA',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('West Virginia','WV',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Wisconsin','WI',50);
INSERT INTO [dbo].[States]([Name],[Code],[SortingPlace]) VALUES('Wyoming','WY',50);
GO

ALTER TABLE [dbo].[Contacts] ADD [IsDeleted] bit NOT NULL  DEFAULT ((0));
GO
ALTER TABLE [dbo].[Contacts] ADD [Phone] nvarchar(100) NULL;
GO

----------------------------DocumentPrintTypes

CREATE TABLE [dbo].[DocumentPrintTypes](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](1024) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_DoumentPrintTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [dbo].[DocumentPrintTypes] (Id,[Name],[IsActive]) VALUES (1,'Respite Documentation Record - Individual Summary',0)
GO
INSERT INTO [dbo].[DocumentPrintTypes] (Id,[Name],[IsActive]) VALUES (2,'Community Habilitation Monthly Progress Summary',0)
GO
INSERT INTO [dbo].[DocumentPrintTypes] (Id,[Name],[IsActive]) VALUES (3, 'Monthly Community Habilitation Am-Pm Sign-in',1)
GO

----------------------------DocumentPrintStatuses

CREATE TABLE [dbo].[DocumentPrintStatuses](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_DocumntPrintStatuses] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO [dbo].[DocumentPrintStatuses] (Id,[Name]) VALUES (1,'Ready')
GO
INSERT INTO [dbo].[DocumentPrintStatuses] (Id,[Name]) VALUES (2,'Sending')

GO

-----------------------------ConsumerPrintDocuments

CREATE TABLE [dbo].[ConsumerPrintDocuments](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PrintDocumnentTypeId] [int] NOT NULL,
	[ContactId] [int] NOT NULL,
	[StatusId] [int] NOT NULL,
	[ConsumerId] [int] NOT NULL,
	[AddedById] [int] NOT NULL,
	[UpdatedById] [int] NULL,
	[ServiceAction1] [nvarchar](2024) NOT NULL,
	[ServiceAction2] [nvarchar](2024) NULL,
	[ServiceAction3] [nvarchar](2024) NULL,
	[ServiceAction4] [nvarchar](2024) NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateUpdated] [datetime] NULL,
	[Notes] [nvarchar](4000) NULL,
	[EffectiveDate] [date] NULL,
 CONSTRAINT [PK_ConsumerPrintDocuments] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ConsumerPrintDocuments]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerPrintDocuments__AddedBy] FOREIGN KEY([AddedById])
REFERENCES [dbo].[SystemUsers] ([UserId])
GO

ALTER TABLE [dbo].[ConsumerPrintDocuments] CHECK CONSTRAINT [FK_ConsumerPrintDocuments__AddedBy]
GO

ALTER TABLE [dbo].[ConsumerPrintDocuments]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerPrintDocuments_Consumers] FOREIGN KEY([ConsumerId])
REFERENCES [dbo].[Consumers] ([ConsumerId])
GO

ALTER TABLE [dbo].[ConsumerPrintDocuments] CHECK CONSTRAINT [FK_ConsumerPrintDocuments_Consumers]
GO

ALTER TABLE [dbo].[ConsumerPrintDocuments]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerPrintDocuments_Contacts] FOREIGN KEY([ContactId])
REFERENCES [dbo].[Contacts] ([ContactId])
GO

ALTER TABLE [dbo].[ConsumerPrintDocuments] CHECK CONSTRAINT [FK_ConsumerPrintDocuments_Contacts]
GO

ALTER TABLE [dbo].[ConsumerPrintDocuments]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerPrintDocuments_DocumentPrintStatuses] FOREIGN KEY([StatusId])
REFERENCES [dbo].[DocumentPrintStatuses] ([Id])
GO

ALTER TABLE [dbo].[ConsumerPrintDocuments] CHECK CONSTRAINT [FK_ConsumerPrintDocuments_DocumentPrintStatuses]
GO

ALTER TABLE [dbo].[ConsumerPrintDocuments]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerPrintDocuments_DocumentPrintTypes] FOREIGN KEY([PrintDocumnentTypeId])
REFERENCES [dbo].[DocumentPrintTypes] ([Id])
GO

ALTER TABLE [dbo].[ConsumerPrintDocuments] CHECK CONSTRAINT [FK_ConsumerPrintDocuments_DocumentPrintTypes]
GO

ALTER TABLE [dbo].[ConsumerPrintDocuments]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerPrintDocuments_UpdatedBy] FOREIGN KEY([UpdatedById])
REFERENCES [dbo].[SystemUsers] ([UserId])
GO

ALTER TABLE [dbo].[ConsumerPrintDocuments] CHECK CONSTRAINT [FK_ConsumerPrintDocuments_UpdatedBy]
GO


Insert Into AspNetUsers (Id,Hometown,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled, LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName) VALUES(
'ceaa5344-faa6-4f11-a5ae-520dc3a8af10',	NULL,	'admin@admin.com',	0,	'AIwsCjM+rNvYN6gMvC2wwOMFmIklyXXu2RkV34g3KZ0vk8Q1phV6VaySJga3OYXqFQ==',	'd23c03f3-7218-4bfd-953d-0c89d1830145',	NULL,	0,	0,	NULL,	1,	0,	'admin@admin.com')


INSERT INTO [SystemUsers] (AspNetUserId,FirstName,LastName,Email,IsDeleted) VALUES('ceaa5344-faa6-4f11-a5ae-520dc3a8af10',	'Admin', 'Admin',	'admin@admin.com',	0)
