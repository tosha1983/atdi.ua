﻿SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [ICSC].[REF_SPECTRUM](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ID_NUM] [int] NULL,
	[TABLE_NAME] [nvarchar](100) NULL,
	[TABLE_ID] [int] NULL,
	[SENSOR_ID] [bigint] NULL,
	[GLOBAL_SID] [nvarchar](100) NULL,
	[FREQ_MHZ] [float] NULL,
	[LEVEL_DBM] [float] NULL,
	[DISPERSION_LOW] [float] NULL,
	[DISPERSION_UP] [float] NULL,
	[PERCENT] [float] NULL,
	[DATE_MEAS] [datetime] NULL,
	[HEAD_REF_SPECTRUM_ID] [bigint] NULL,
	[STATUS_MEAS] [nvarchar](8) NULL,
 CONSTRAINT [REF_SPECTRUM_PK] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO