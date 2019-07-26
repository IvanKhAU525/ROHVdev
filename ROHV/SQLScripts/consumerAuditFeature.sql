CREATE TABLE [dbo].[Audits](
		[Id] int IDENTITY(1,1) PRIMARY KEY,
		[ServiceId] int not null,		
		[AuditDate] datetime,		
		CONSTRAINT fk_Audits_ServicesList FOREIGN KEY (ServiceId)  REFERENCES [dbo].[ServicesList](ServiceId)  
		ON DELETE CASCADE		
	);

	GO

	CREATE TABLE [dbo].[ConsumerAudits](		
		[AuditId] int not null,		
		[ConsumerId] int not null,		
		primary key (AuditId, ConsumerId),
		CONSTRAINT fk_ConsumerAudits_Audits FOREIGN KEY (AuditId)  REFERENCES [dbo].[Audits](Id)  
		ON DELETE CASCADE,
		CONSTRAINT fk_ConsumerAudits_Consumer FOREIGN KEY (ConsumerId)  REFERENCES [dbo].[Consumers](ConsumerId)  
		ON DELETE CASCADE
	);