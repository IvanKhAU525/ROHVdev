CREATE TABLE [dbo].[ConsumerHabReviews](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ConsumerId] [int] NOT NULL,
	[ServiceId] [int] NOT NULL,
	[ContactId] [int] NULL,
	[AdvocateId] [int] NULL,
	[MSCId] [int] NULL,
	[CHCoordinatorId] [int] NULL,
	[DHCoordinatorId] [int] NULL,
	[Parents] [nvarchar](4000) NULL,
	[Others] [nvarchar](4000) NULL,
	[AddedById] [int] NOT NULL,
	[UpdatedById] [int] NULL,
	[DateCreated] [datetime] NOT NULL,
	[DateUpdated] [datetime] NULL,
	[Notes] [nvarchar](4000) NULL,
 CONSTRAINT [PK_ConsumerHabReviews] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ConsumerHabReviews]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerHabReviews_Advocates] FOREIGN KEY([AdvocateId])
REFERENCES [dbo].[Advocates] ([Id])
GO

ALTER TABLE [dbo].[ConsumerHabReviews] CHECK CONSTRAINT [FK_ConsumerHabReviews_Advocates]
GO

ALTER TABLE [dbo].[ConsumerHabReviews]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerHabReviews_Consumers] FOREIGN KEY([ConsumerId])
REFERENCES [dbo].[Consumers] ([ConsumerId])
GO

ALTER TABLE [dbo].[ConsumerHabReviews] CHECK CONSTRAINT [FK_ConsumerHabReviews_Consumers]
GO

ALTER TABLE [dbo].[ConsumerHabReviews]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerHabReviews_Contacts] FOREIGN KEY([MSCId])
REFERENCES [dbo].[Contacts] ([ContactId])
GO

ALTER TABLE [dbo].[ConsumerHabReviews] CHECK CONSTRAINT [FK_ConsumerHabReviews_Contacts]
GO

ALTER TABLE [dbo].[ConsumerHabReviews]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerHabReviews_Contacts1] FOREIGN KEY([DHCoordinatorId])
REFERENCES [dbo].[Contacts] ([ContactId])
GO

ALTER TABLE [dbo].[ConsumerHabReviews] CHECK CONSTRAINT [FK_ConsumerHabReviews_Contacts1]
GO

ALTER TABLE [dbo].[ConsumerHabReviews]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerHabReviews_Contacts2] FOREIGN KEY([ContactId])
REFERENCES [dbo].[Contacts] ([ContactId])
GO

ALTER TABLE [dbo].[ConsumerHabReviews] CHECK CONSTRAINT [FK_ConsumerHabReviews_Contacts2]
GO

ALTER TABLE [dbo].[ConsumerHabReviews]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerHabReviews_Contacts3] FOREIGN KEY([CHCoordinatorId])
REFERENCES [dbo].[Contacts] ([ContactId])
GO

ALTER TABLE [dbo].[ConsumerHabReviews] CHECK CONSTRAINT [FK_ConsumerHabReviews_Contacts3]
GO

ALTER TABLE [dbo].[ConsumerHabReviews]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerHabReviews_ServiceTypes] FOREIGN KEY([ServiceId])
REFERENCES [dbo].[ServiceTypes] ([Id])
GO

ALTER TABLE [dbo].[ConsumerHabReviews] CHECK CONSTRAINT [FK_ConsumerHabReviews_ServiceTypes]
GO

ALTER TABLE [dbo].[ConsumerHabReviews]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerHabReviews_SystemUsers] FOREIGN KEY([UpdatedById])
REFERENCES [dbo].[SystemUsers] ([UserId])
GO

ALTER TABLE [dbo].[ConsumerHabReviews] CHECK CONSTRAINT [FK_ConsumerHabReviews_SystemUsers]
GO

ALTER TABLE [dbo].[ConsumerHabReviews]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerHabReviews_SystemUsers1] FOREIGN KEY([AddedById])
REFERENCES [dbo].[SystemUsers] ([UserId])
GO

ALTER TABLE [dbo].[ConsumerHabReviews] CHECK CONSTRAINT [FK_ConsumerHabReviews_SystemUsers1]
GO


