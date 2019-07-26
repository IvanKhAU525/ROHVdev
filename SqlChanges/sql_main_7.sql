CREATE TABLE [dbo].[ConsumerHabPlanSafeguards](
	[ConsumerHabPlanSafeguardId] [int] IDENTITY(1,1) NOT NULL,
	[ConsumerHabPlanId] [int] NULL,
	[Item] [nvarchar](2048) NULL,
 CONSTRAINT [PK_ConsumerHabPlanSafeguards] PRIMARY KEY CLUSTERED 
(
	[ConsumerHabPlanSafeguardId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ConsumerHabPlanSafeguards]  WITH CHECK ADD  CONSTRAINT [FK_ConsumerHabPlanSafeguards_ConsumerHabPlans] FOREIGN KEY([ConsumerHabPlanId])
REFERENCES [dbo].[ConsumerHabPlans] ([ConsumerHabPlanId])
GO

ALTER TABLE [dbo].[ConsumerHabPlanSafeguards] CHECK CONSTRAINT [FK_ConsumerHabPlanSafeguards_ConsumerHabPlans]
GO
