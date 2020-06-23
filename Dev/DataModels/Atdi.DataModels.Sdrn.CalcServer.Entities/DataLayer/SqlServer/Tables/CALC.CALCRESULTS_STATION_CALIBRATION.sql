﻿SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CALCRESULTS_STATION_CALIBRATION](
	[RESULT_ID] [bigint] NOT NULL,
	[TIME_START] [datetimeoffset](7) NULL,
	[STANDARD] [nvarchar](50) NULL,
	[AREA_NAME] [nvarchar](50) NULL,
	[NUMBER_STATION] [int] NULL,
	[NUMBER_STATION_IN_CONTOUR] [int] NULL,
	[COUNT_STATION_CS] [int] NULL,
	[COUNT_STATION_NS] [int] NULL,
	[COUNT_STATION_IT] [int] NULL,
	[COUNT_STATION_NF] [int] NULL,
	[COUNT_STATION_UN] [int] NULL,
	[COUNT_MEAS_GSID] [int] NULL,
	[COUNT_MEAS_GSID_LS] [int] NULL,
	[COUNT_MEAS_GSID_IT] [int] NULL,
	[PERCENT_COMPLETE] [int] NULL,
	[PARAMETERS_TASK_ID] [bigint] NULL,
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_CALCRESULTS_STATION_CALIBRATION] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
