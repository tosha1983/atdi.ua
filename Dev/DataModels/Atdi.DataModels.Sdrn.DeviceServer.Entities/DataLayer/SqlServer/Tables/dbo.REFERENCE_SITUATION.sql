﻿CREATE TABLE [dbo].[REFERENCE_SITUATION](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[SENSOR_ID] [int] NULL,
	[MEAS_TASK_ID] [bigint] NULL,
 CONSTRAINT [PK_XBS_REFSITUATION] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[REFERENCE_SITUATION]  WITH CHECK ADD  CONSTRAINT [FK_REFERENCE_SITUATION_MEAS_TASK] FOREIGN KEY([MEAS_TASK_ID])
REFERENCES [dbo].[MEAS_TASK] ([ID])
GO

ALTER TABLE [dbo].[REFERENCE_SITUATION] CHECK CONSTRAINT [FK_REFERENCE_SITUATION_MEAS_TASK]
GO


