﻿CREATE TABLE [dbo].[REFERENCE_SIGNAL](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[FREQ_MHZ] [float] NULL,
	[BANDWIDTH_KHZ] [float] NULL,
	[LEVELSIGNAL_DBM] [float] NULL,
	[REFERENCE_SITUATION_ID] [bigint] NULL,
 CONSTRAINT [PK_XBS_REFSIGNALRAW] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[REFERENCE_SIGNAL]  WITH CHECK ADD  CONSTRAINT [FK_REFERENCE_SIGNAL_REFERENCE_SITUATION] FOREIGN KEY([REFERENCE_SITUATION_ID])
REFERENCES [dbo].[REFERENCE_SITUATION] ([ID])
GO

ALTER TABLE [dbo].[REFERENCE_SIGNAL] CHECK CONSTRAINT [FK_REFERENCE_SIGNAL_REFERENCE_SITUATION]
GO


