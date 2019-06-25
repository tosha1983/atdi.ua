﻿CREATE TABLE [dbo].[SCAN_PARAMETER](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[STANDARD] [varchar](50) NULL,
	[FREQRELATIVE] [float] NULL,
	[LEVELDBM] [float] NULL,
	[DETECTLEVELDBM] [float] NULL,
	[MAXPERMISSBW] [float] NULL,
	[RBW] [float] NULL,
	[VBW] [float] NULL,
	[SCANBW] [float] NULL,
	[MEASTIME_SEC] [float] NULL,
	[REFLEVEL_DBM] [float] NULL,
	[DETECTTYPE] [varchar](50) NULL,
	[PREAMPLIFICATION] [int] NULL,
	[RFATTENUATION] [int] NULL,
	[ID_MEASTASK] [bigint] NULL,
 CONSTRAINT [XBS_SCANPARAMETER_PK] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[SCAN_PARAMETER]  WITH CHECK ADD  CONSTRAINT [FK_XBS_SCANPARAMETER_XBS_MEASTASK] FOREIGN KEY([ID_MEASTASK])
REFERENCES [dbo].[MEAS_TASK] ([ID])
GO

ALTER TABLE [dbo].[SCAN_PARAMETER] CHECK CONSTRAINT [FK_XBS_SCANPARAMETER_XBS_MEASTASK]
GO
