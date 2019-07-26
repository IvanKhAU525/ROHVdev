-- task R-18 02-07-2017 ----------

ALTER TABLE [dbo].[EmployeeDocuments] DROP
   CONSTRAINT FK_EmployeeDocuments_Consumers;

ALTER TABLE [dbo].[EmployeeDocuments] ADD
   CONSTRAINT FK_EmployeeDocuments_Consumers
      FOREIGN KEY (ConsumerId)
      REFERENCES [dbo].[Consumers] (ConsumerId)
      ON DELETE CASCADE
      ON UPDATE CASCADE;
-- task R-18 02-07-2017 ----------

-- task "add new sheets to print documents section" 01-12-2018 ----

INSERT INTO [RayimTest].[dbo].[ServiceTypes] ([Id], [Name])
Values(4 , 'Broker'),(5 , 'Self Hired');

INSERT INTO [RayimTest].[dbo].[DocumentPrintTypes] ([Id], [Name], [IsActive], [ServiceTypeId])
Values(8 , 'Broker Documentation Record - Individual Summary', 1, 4),
(9 , 'Self Hired Community Habilitation Documentation Record - Individual Summary', 1, 5), 
(10 , 'Self Hired Community Habilitation Monthly Progress Summary', 1, 5),
(11 , 'Self Hired Respite Documentation Record - Individual Summary', 1, 5);

SET IDENTITY_INSERT [RayimTest].[dbo].[SystemUsers] ON

INSERT INTO [RayimTest].[dbo].[SystemUsers] ([UserId], [AspNetUserId], [FirstName], [LastName], [Email], [IsDeleted])
Values(34 , '021cbb03-a11f-49c4-a728-7ccb6abf7c0f', 'Oleg', 'Bondarenko', 'oleg.bondarenko@itechcraft.com', 0);

SET IDENTITY_INSERT [RayimTest].[dbo].[SystemUsers] OFF
-----------------------------------------------------
-- task "Add Service type to print document "Supplemental" Group Day Habilitation Services" 21-01-2018---------- 
INSERT INTO [RayimTest].[dbo].[ServiceTypes] ([Id], [Name])
Values(6 , 'Supplemental Group Day Habilitation');

INSERT INTO [RayimTest].[dbo].[DocumentPrintTypes] ([Id], [Name], [IsActive], [ServiceTypeId])
Values(12 , 'Supplemental Group Day Habilitation Documentation Record - Individual Summary', 1, 6),
(13, 'Supplemental Group Day Habilitation Monthly Progress Summary', 1, 6);

-- task "Add Service type to print document "Supplemental" Group Day Habilitation Services" 16-02-2018---------- 
SET IDENTITY_INSERT [RayimTest].[dbo].[SystemUsers] ON

INSERT INTO [RayimTest].[dbo].[ServicesList] ( [ServiceDescription], [ServiceType] ,[MinutesInUnit])
Values('Supplemental Day Hab','Supplemental Day Hab', 15);

SET IDENTITY_INSERT [RayimTest].[dbo].[SystemUsers] OFF

--------- task #25 Add to Approved Services tab ------------
ALTER TABLE [dbo].[ConsumerServices] ADD Notes nvarchar(100)
ALTER TABLE [dbo].[ConsumerServices] ADD DateInactive datetime

--------------task #28 03-22-2018
CREATE TABLE [dbo].[ConsumerServiceCoordinators](
		[Id] int IDENTITY(1,1) PRIMARY KEY,
		[ConsumerId] int not null,
		[ContactId] int not null,
		[FromDate] datetime,
		[ToDate] datetime,
		CONSTRAINT fk_Consumer_ConsumerServiceCoordinators FOREIGN KEY (ConsumerId)  REFERENCES [dbo].[Consumers](ConsumerId)  
		ON DELETE CASCADE,
		CONSTRAINT fk_Contact_ConsumerServiceCoordinators FOREIGN KEY (ContactId)  REFERENCES [dbo].[Contacts](ContactId)  
		ON DELETE CASCADE
	);

	--------------task #29 03-26-2018
CREATE TABLE [dbo].[ConsumerAddresses](
		[Id] int IDENTITY(1,1) PRIMARY KEY,
		[ConsumerId] int not null,		
		[Address1] nvarchar(1000),
		[Address2] nvarchar(1000),
		[City] nvarchar(1000),
		[State] nvarchar(1000),
		[Zip] nvarchar(1000),
		[FromDate] datetime,
		[ToDate] datetime,
		CONSTRAINT fk_Consumer_ConsumerAddresses FOREIGN KEY (ConsumerId)  REFERENCES [dbo].[Consumers](ConsumerId)  
		ON DELETE CASCADE		
	);
	-----------------start-task #38 05-11-2018-------
	ALTER TABLE [dbo].[ConsumerServices] ALTER COLUMN Notes nvarchar(max)
		-----------------end-task #38 05-11-2018-------

		-----------------start-task #40 06-04-2018-------
	ALTER TABLE [dbo].[ConsumerEmployees] Add  MaxHoursPerWeek [int], MaxHoursPerYear [int],RateNote nvarchar(2024)
    -----------------end-task #40 06-04-2018-------
    -----------------start-task #41 06-18-2018-------
    ALTER TABLE [dbo].[Contacts] Add  FileNumber nvarchar(20)
    -----------------end-task #41 06-18-2018-------
	-----------------start-task #44 06-27-2018-------
    ALTER TABLE [dbo].[ConsumerServices] Add  CreatedByUserId int,TotalUnits decimal(18,2),
	 CONSTRAINT FK_ConsumerServices_SystemUsers
      FOREIGN KEY (CreatedByUserId)
      REFERENCES [dbo].[SystemUsers] (UserId)
      ON DELETE CASCADE
      ON UPDATE CASCADE;
    -----------------end-task #45 06-27-2018-------
	-----------------start-task #44 06-29-2018-------
	insert into [dbo].[ConsumerNoteTypes] ([Id],[Name],[IsActive]) values (3,'Pop up Note',1);
	-----------------end-task #45 06-29-2018-------
	-----------------start-task #31 07-03-2018-------
	EXEC sp_rename '[dbo].[ConsumerServices].TotalUnits', 'TotalHours', 'COLUMN';  
	-----------------end-task #31 07-03-2018-------

	-----------------start-task #49 08-10-2018-------
	ALTER TABLE [dbo].[Contacts] Add  PhoneExtension nvarchar(20), Fax nvarchar(20)
	-----------------end-task #49 08-10-2018-------

	-----------------start-task #50 09-28-2018-------
	CREATE TABLE [dbo].[ConsumerMedicaidNumbers](
		[Id] int IDENTITY(1,1) PRIMARY KEY,
		[ConsumerId] int NOT NULL,
		[MedicaidNo] nvarchar(50),
		[FromDate] datetime,
		[ToDate] datetime,
		CONSTRAINT fk_Consumer_ConsumerMedicaidNumbers FOREIGN KEY (ConsumerId) REFERENCES [dbo].[Consumers]
	);
	CREATE UNIQUE NONCLUSTERED INDEX [MedicaidNo_IX] ON [dbo].[ConsumerMedicaidNumbers] (MedicaidNo);
	-----------------end-task #50 09-28-2018-------
	
	-----------------start-task #54 08-10-2018-------
		CREATE TABLE [dbo].[EntityTypes](
		[Id] int IDENTITY(1,1) PRIMARY KEY,		
		[Name] [nvarchar](1024) NULL
	);
	Go
	insert into [dbo].[EntityTypes] ([Name]) values ('Consumer Employee');


	CREATE TABLE [dbo].[FileMetaDatas](
		[Id] int IDENTITY(1,1) PRIMARY KEY,
		[ParentEntityId] [int] NOT NULL,
		[ParentEntityTypeId] [int] NOT NULL,
		[AddedById] [int] NOT NULL,
		[UpdatedById] [int] NULL,
		[CreatedOn] [datetime] NOT NULL,
		[UpdatedOn] [datetime] NULL,
		[FilePath] [nvarchar](1024) NULL,
		[FileContentType] [nvarchar](1024) NULL,
		[FileDisplayName] [nvarchar](1024) NULL,
		CONSTRAINT fk_FileMetaDatas_EntityTypes FOREIGN KEY (ParentEntityTypeId)  REFERENCES [dbo].[EntityTypes](Id)  
		ON Update CASCADE,		
		CONSTRAINT fk_FileMetaDatas_SystemUsers_Added FOREIGN KEY (AddedById)  REFERENCES [dbo].[SystemUsers](UserId),		
		CONSTRAINT fk_FileMetaDatas_SystemUsers_Updated FOREIGN KEY (UpdatedById)  REFERENCES [dbo].[SystemUsers](UserId),		
	);
	-----------------end-task #54 08-10-2018-------

	-------------------start-task #55 08-15-2018---------------
	insert into [dbo].[AspNetRoles]
  (Id,Name) Values(1,'Admin')
  go

  insert into [dbo].[AspNetUserRoles]
  (UserId,RoleId) Values((select Id from [dbo].[AspNetUsers] where Email = 'admin@admin.com') ,(select Id from [dbo].[AspNetRoles] where Name = 'Admin'))
	----------------------end-task #55 08-15-2018-------------------
	-------------------start-task #57 08-17-2018---------------
	insert into [dbo].[AspNetRoles]
  (Id,Name) Values(2,'Manager')  
	----------------------end-task #57 08-17-2018-------------------
		-------------------start-task #59 09-08-2018---------------
	insert into [dbo].[AspNetRoles]
  (Id,Name) Values(3,'User')  
	----------------------start-task #59 09-08-2018-------------------

	-------------------start-task #60 09-11-2018---------------
	ALTER TABLE [dbo].[EmployeeDocumentTypes] Add  Color nvarchar(9)
	Go
	update [dbo].[EmployeeDocumentTypes] set Color = '#FFB233' where EmployeeDocumentTypeId = 1;
	update [dbo].[EmployeeDocumentTypes] set Color = '#A8FF33' where EmployeeDocumentTypeId = 2;
	update [dbo].[EmployeeDocumentTypes] set Color = '#33FFF0' where EmployeeDocumentTypeId = 3;
	update [dbo].[EmployeeDocumentTypes] set Color = '#FF339F' where EmployeeDocumentTypeId = 4;
	update [dbo].[EmployeeDocumentTypes] set Color = '#D133FF' where EmployeeDocumentTypeId = 5;
	----------------------end-task #60 09-11-2018-------------------
	-------------------start-task #62 13-11-2018---------------
	UPDATE [dbo].[ConsumerServices] SET TotalHours = ROUND(AnnualUnits / 4.0, 2) WHERE TotalHours IS NULL;
	----------------------end-task #62 13-11-2018-------------------
	-------------------start-task #64 13-11-2018---------------
	ALTER TABLE [dbo].[SystemUsers] ADD CanManageServices BIT NOT NULL DEFAULT 0;
		-------------------start-task #68 11-01-2019---------------
	ALTER TABLE [dbo].[ConsumerServices] ADD EditedByUserId INT NULL,
	CONSTRAINT FK_ConsumerServices_EditedByUserId FOREIGN KEY(EditedByUserId) REFERENCES [dbo].[SystemUsers](UserId);
	----------------------end-task #68 11-01-2019----------------
	--------------------start-task #69 16-01-2018---------------
	INSERT INTO EntityTypes VALUES ('Consumer Services');
	----------------------end-task #69 16-01-2018----------------
	----------------------start-task #70 16-01-2018---------------
	ALTER TABLE Contacts ADD CCO NVARCHAR(60);
	ALTER TABLE ConsumerHabPlanValuedOutcomes ADD CQLPOM NVARCHAR(110), MyGoal NVARCHAR(110);
	----------------------end-task #70 16-01-2018-------------------
-------------------start-task #71 17-01-2019---------------
	INSERT INTO EmployeeDocumentTypes (Name, Color) VALUES('Monthly Supplemental Group Day Habilitation AM-PM Sign-in', '#DE3F3F');
	INSERT INTO EmployeeDocumentTypes (Name, Color) VALUES('Supplemental Group Day Habilitation Monthly Progress Summary', '#089C39');
	----------------------end-task #71 17-01-2019-------------------
	
----------------------start-task #70 28-01-2018---------------
CREATE TABLE [dbo].[ReportType](
	[Id] INT IDENTITY(1, 1) PRIMARY KEY,
	[Name] NVARCHAR(50)
);

INSERT INTO ReportType(Name) VALUES ('ComHabPlan');

CREATE TABLE [dbo].[ReportTemplates](
	[Id] INT IDENTITY(1, 1) PRIMARY KEY,
	[ReportTypeId] INT NOT NULL,
	[StartDate] DATE NOT NULL,
	[EndDate] DATE,
	[ReportPath] NVARCHAR(100) NOT NULL,
	[Description] NVARCHAR(30),
	CONSTRAINT fk_ReportType_ReportTemplates FOREIGN KEY (ReportTypeId) REFERENCES [dbo].[ReportType](Id)
);

INSERT INTO ReportTemplates(ReportTypeId, StartDate, ReportPath, Description) VALUES (1, '2019-01-01', '~/Views/PDFViews/ComHabPlan2019.01.rdl', '2019');
----------------------end-task #70 28-01-2018-------------------
--------------------start #74  07-02-2019 --------------
ALTER TABLE [dbo].[EmployeeDocumentNotes] DROP
   CONSTRAINT [FK_EmployeeDocumentNotes_EmployeeDocumentNotes];
GO
ALTER TABLE [dbo].[EmployeeDocumentNotes]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeDocumentNotes_EmployeeDocumentNotes] FOREIGN KEY([DocumentId])
REFERENCES [dbo].[EmployeeDocuments] ([EmployeeDocumentId])
 ON DELETE CASCADE
      ON UPDATE CASCADE;
GO
--------------------end #74  07-02-2019 --------------
--------------------start #76  22-02-2019 --------------
ALTER TABLE ConsumerHabPlanValuedOutcomes ADD IsIPOP BIT NOT NULL DEFAULT 0;
ALTER TABLE ConsumerHabPlanSafeguards ADD [Action] NVARCHAR(2048);

--------------------end #76  22-02-2019 --------------
--------------------start #77  11-03-2019 --------------
INSERT INTO ReportType(Name) VALUES ('ComHabPlanReview');
INSERT INTO ReportTemplates(ReportTypeId, StartDate, ReportPath, Description) VALUES (2, '2019-01-01', '~/Views/PDFViews/6MonthReview2019.01.rdl', '2019-review');
--------------------end #77  11-03-2019 --------------
--------------------start #77  11-03-2019 --------------
ALTER TABLE [dbo].[SystemUsers] ADD EmailPassword NVARCHAR(30);
--------------------end #77  11-03-2019 --------------
--------------------start #81  19-03-2019 --------------
INSERT INTO EntityTypes VALUES ('Consumer');
--------------------start #81  19-03-2019 --------------
--------------------start #76  14-05-2019 --------------
ALTER TABLE ConsumerHabPlanSafeguards ADD IsIPOP BIT NOT NULL DEFAULT 0;
--------------------end #76  14-05-2019 --------------

--------------------start #84  21-05-2019 --------------
CREATE VIEW [dbo].[vTimeSheetData]
AS
SELECT *
FROM   [TimeSheetData].[dbo].vQryTD
ALTER TABLE [dbo].[ConsumerServices] ADD UsedHoursStartDate Date
ALTER TABLE [dbo].[ConsumerServices] ADD UsedHoursEndDate Date
ALTER TABLE [dbo].[ConsumerServices] ADD UsedHours decimal(18,2)
GO
--------------------end #84  21-05-2019 --------------


--------------------start #91  21-05-2019 --------------
declare @Id int;
select @id = max(Id) from [dbo].[ServiceTypes];
Insert into [dbo].[ServiceTypes] (Id,[Name]) Values
(@id + 1,'CONTRACTED Community Habilitation'),(@id + 2,'CONTRACTED In home Respite'),(@id + 3,'CONTRACTED Site Based Respite'),(@id + 4,'CONTRACTED Recreational Respite')
GO
--------------------end #91  21-05-2019 --------------
--------------------start #91  07-05-2019 --------------

INSERT INTO [dbo].[DocumentPrintTypes] ([Id], [Name], [IsActive], [ServiceTypeId])
Values(14 , 'CONTRACTED Community Habilitation Documentation Record - Individual Summary', 1, 7),
(15, 'CONTRACTED Community Habilitation Monthly Progress Summary', 1, 7);
--------------------end #91  07-05-2019 --------------
--------------------start #92  07-22-2019 --------------

ALTER TABLE [dbo].[FileMetaDatas] ADD Note NVARCHAR(max);
--------------------end #92  07-22-2019 --------------