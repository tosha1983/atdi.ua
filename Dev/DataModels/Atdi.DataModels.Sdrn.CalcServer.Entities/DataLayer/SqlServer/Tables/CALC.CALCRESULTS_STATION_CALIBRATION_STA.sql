﻿SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CALCRESULTS_STATION_CALIBRATION_STA](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[CALCRESULTS_STATION_CALIBRATION_ID] [bigint] NOT NULL,
	[EXTERNAL_SOURCE] [nvarchar](50) NULL,
	[EXTERNAL_CODE] [nvarchar](50) NULL,
	[LICENSE_GSID] [nvarchar](100) NULL,
	[REAL_GSID] [nvarchar](100) NULL,
	[RESULT_STATION_STATUS] [nvarchar](50) NULL,
	[MAX_CORELLATION] [float] NULL,
	[OLD_ALTITUDE_M] [int] NULL,
	[OLD_TILT_DEG] [float] NULL,
	[OLD_AZIMUTRH_DEG] [float] NULL,
	[OLD_LAT_DEG] [real] NULL,
	[OLD_LON_DEG] [real] NULL,
	[OLD_POWER_DB] [real] NULL,
	[OLD_FREQ_MHZ] [real] NULL,
	[NEW_ALTITUDE_M] [int] NULL,
	[NEW_TILT_DEG] [float] NULL,
	[NEW_AZIMUTRH_DEG] [float] NULL,
	[NEW_LAT_DEG] [real] NULL,
	[NEW_LON_DEG] [real] NULL,
	[NEW_POWER_DB] [real] NULL,
	[STATION_MONITORING_ID] [bigint] NULL,
	[FREQ_MHZ] [float] NULL,
	[STANDARD] [nvarchar](50) NULL,
	[DELTA_CORRELATION_PC] [real] NULL,
	[USED_POINTS_PC] [int] NULL,
	[COUNT_POINTS_IN_DRIVE_TEST] [int] NULL,
 CONSTRAINT [PK_CALCRESULTS_STATION_CALIBRATION_STA] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO