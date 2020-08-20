﻿CREATE SCHEMA [CALC]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CALCRESULT_EVENTS](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[CREATED_DATE] [datetimeoffset](7) NOT NULL,
	[RESULT_ID] [bigint] NOT NULL,
	[LEVEL_CODE] [tinyint] NOT NULL,
	[LEVEL_NAME] [nvarchar](50) NOT NULL,
	[CONTEXT] [nvarchar](250) NULL,
	[MESSAGE] [nvarchar](max) NULL,	
	[DATA_TYPE] [nvarchar](250) NULL,
	[DATA_JSON] [nvarchar](max) NULL,	
 CONSTRAINT [PK_CALCRESULT_EVENTS] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [CALC].[CALCRESULT_EVENTS] ADD  CONSTRAINT [DF_CALCRESULT_EVENTS_CREATED_DATE]  DEFAULT (sysdatetimeoffset()) FOR [CREATED_DATE]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CALCRESULTS](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[TASK_ID] [bigint] NOT NULL,
	[STATUS_CODE] [tinyint] NOT NULL,
	[STATUS_NAME] [nvarchar](50) NOT NULL,
	[STATUS_NOTE] [nvarchar](max) NULL,	
	[CALLER_INSTANCE] [nvarchar](250) NOT NULL,
	[CALLER_RESULT_ID] [uniqueidentifier] NOT NULL,
	[CREATED_DATE] [datetimeoffset](7) NOT NULL,
	[START_TIME]   [datetimeoffset](7) NULL,
	[FINISH_TIME]  [datetimeoffset](7) NULL,
 CONSTRAINT [PK_CALCRESULTS] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [CALC].[CALCRESULTS] ADD  CONSTRAINT [DF_CALCRESULTS_CREATED_DATE]  DEFAULT (sysdatetimeoffset()) FOR [CREATED_DATE]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CALCRESULTS_GN06](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[RESULT_ID] [bigint] NOT NULL,
 CONSTRAINT [PK_CALCRESULTS_GN06] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CALCRESULTS_POINTFIELDSTRENGTH](
	[RESULT_ID] [bigint] NOT NULL,
	[FS_DBUVM] [REAL] NULL,
	[LEVEL_DBM] [REAL] NULL,
 CONSTRAINT [PK_CALCRESULTS_POINTFIELDSTRENGTH] PRIMARY KEY CLUSTERED 
(
	[RESULT_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CALCRESULTS_STATION_CALIBRATION](
	[RESULT_ID] [bigint] NOT NULL,
	[TIME_START] [datetimeoffset](7) NULL,
	[STANDARD] [nvarchar](50) NULL,
	[AREA_NAME] [nvarchar](1000) NULL,
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

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CALCRESULTS_STATION_CALIBRATION_DRIVE_TEST](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[CALCRESULTS_STATION_CALIBRATION_ID] [bigint] NOT NULL,
	[EXTERNAL_SOURCE] [nvarchar](50) NULL,
	[EXTERNAL_CODE] [nvarchar](50) NULL,
	[STATION_GSID] [nvarchar](50) NULL,
	[MEAS_GSID] [nvarchar](50) NULL,
	[RESULT_DRIVE_TEST_STATUS] [nvarchar](50) NULL,
	[MAX_PERCENT_CORELLATION] [float] NULL,
	[COUNT_POINTS_IN_DRIVE_TEST] [int] NULL,
	[DRIVE_TEST_ID] [bigint] NULL,
	[LINK_STATION_MONITORING_ID] [bigint] NULL,
	[FREQ_MHZ] [float] NULL,
	[STANDARD] [nvarchar](50) NULL,
 CONSTRAINT [PK_CALCRESULTS_STATION_CALIBRATION_DRIVE_TEST] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CALCRESULTS_STATION_CALIBRATION_STA](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[CALCRESULTS_STATION_CALIBRATION_ID] [bigint] NOT NULL,
	[EXTERNAL_SOURCE] [nvarchar](50) NULL,
	[EXTERNAL_CODE] [nvarchar](50) NULL,
	[LICENSE_GSID] [nvarchar](50) NULL,
	[REAL_GSID] [nvarchar](50) NULL,
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
	[FREQ_MHZ] [real] NULL,
	[STANDARD] [nvarchar](50) NULL,
 CONSTRAINT [PK_CALCRESULTS_STATION_CALIBRATION_STA] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CALCTASKS](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[CONTEXT_ID] [bigint] NOT NULL,
	[TYPE_CODE] [int] NOT NULL,
	[TYPE_NAME] [nvarchar](250) NOT NULL,
	[STATUS_CODE] [tinyint] NOT NULL,
	[STATUS_NAME] [nvarchar](50) NOT NULL,
	[STATUS_NOTE] [nvarchar](max) NULL,
	[OWNER_INSTANCE] [nvarchar](250) NOT NULL,
	[OWNER_TASK_ID] [uniqueidentifier] NOT NULL,
	[CREATED_DATE] [datetimeoffset](7) NOT NULL,
	[MAP_NAME] [nvarchar](250) NULL,
 CONSTRAINT [PK_CALCTASKS] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [CALC].[CALCTASKS] ADD  CONSTRAINT [DF_CALCTASKS_CREATED_DATE]  DEFAULT (sysdatetimeoffset()) FOR [CREATED_DATE]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CALCTASKS_COVERAGEPROFILES](
	[ID] [bigint] NOT NULL,
	[MODE_CODE] [tinyint] NOT NULL,
	[MODE_NAME] [nvarchar](50) NULL,
	[POINTS_CRDS_X] [varbinary](max) NULL,
	[POINTS_CRDS_Y] [varbinary](max) NULL,
	[RESULT_PATH] [nvarchar](450) NULL,
 CONSTRAINT [PK_CALCTASKS_COVERAGEPROFILES] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CALCTASKS_GN06_ARGS](
	[TASK_ID] [bigint] NOT NULL,
	[AZIMUTH_STEP_DEG] [float] NULL,
	[ADDITIONAL_CONTOURS_BY_DISTANCE] [bit] NULL,
	[INFOC_MEAS_RESULTS] [varbinary](max) NULL,
	[CONTURE_BY_FIELD_STRENGTH] [bit] NULL,
	[FIELD_STRENGTH] [varbinary](max) NULL,
	[SUBSCRIBERS_HEIGHT] [int] NULL,
	[PERCENTAGE_TIME] [real] NULL,
	[USE_EFFECTIVE_HEIGHT] [bit] NULL,
	[CALCULATION_TYPE_CODE] [tinyint] NOT NULL,
	[CALCULATION_TYPE_NAME] [nvarchar](50) NULL,
	[BROADCASTING_EXTEND] [nvarchar](max) NULL,
	[STEP_BETWEEN_BUNDARY_POINTS] [int] NULL,
 CONSTRAINT [PK_CALCTASKS_GN06_ARGS] PRIMARY KEY CLUSTERED 
(
	[TASK_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CALCTASKS_GN06_ARGS_DEF](
	[TASK_ID] [bigint] NOT NULL,
	[AZIMUTH_STEP_DEG] [float] NULL,
	[ADDITIONAL_CONTOURS_BY_DISTANCE] [bit] NULL,
	[INFOC_MEAS_RESULTS] [varbinary](max) NULL,
	[CONTURE_BY_FIELD_STRENGTH] [bit] NULL,
	[FIELD_STRENGTH] [varbinary](max) NULL,
	[SUBSCRIBERS_HEIGHT] [int] NULL,
	[PERCENTAGE_TIME] [real] NULL,
	[USE_EFFECTIVE_HEIGHT] [bit] NULL,
	[CALCULATION_TYPE_CODE] [tinyint] NOT NULL,
	[CALCULATION_TYPE_NAME] [nvarchar](50) NULL,
	[BROADCASTING_EXTEND] [nvarchar](max) NULL,
	[STEP_BETWEEN_BUNDARY_POINTS] [int] NULL,
 CONSTRAINT [PK_CALCTASKS_GN06_ARGS_DEF] PRIMARY KEY CLUSTERED 
(
	[TASK_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CALCTASKS_POINTFIELDSTRENGTH_ARGS](
	[TASK_ID] [bigint] NOT NULL,
	[STATION_ID] [bigint] NULL,
	[POINT_LON_DEC] [FLOAT] NULL,
	[POINT_LAT_DEC] [FLOAT] NULL,
	[POINT_ALT_M]   [FLOAT] NULL,
 CONSTRAINT [PK_CALCTASKS_POINTFIELDSTRENGTH_ARGS] PRIMARY KEY CLUSTERED 
(
	[TASK_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CALCTASKS_POINTFIELDSTRENGTH_ARGS_DEF](
	[TASK_ID] [bigint] NOT NULL,
	[STATION_ID] [bigint] NULL,
	[POINT_LON_DEC] [FLOAT] NULL,
	[POINT_LAT_DEC] [FLOAT] NULL,
	[POINT_ALT_M]   [FLOAT] NULL,
 CONSTRAINT [PK_CALCTASKS_POINTFIELDSTRENGTH_ARGS_DEF] PRIMARY KEY CLUSTERED 
(
	[TASK_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO




SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CALCTASKS_STATION_CALIBRATION_ARGS](
	[STANDARD] [nvarchar](50)  NULL,
	[TASK_ID] [bigint] NOT NULL,
	[CORRELATION_THRESHOLD_HARD] [float] NULL,
	[CORRELATION_THRESHOLD_WEAK] [float] NULL,
	[TRUST_OLD_RESULTS] [bit] NULL,
	[USE_MEASUREMENT_SAME_GSID] [bit] NULL,
	[DISTANCE_AROUND_CONTOUR_KM] [int] NULL,
	[MIN_NUMBER_POINT_FOR_CORRELATION] [int] NULL,
	[MIN_RANGE_MEASUREMENTS_DBMKV] [float] NULL,
	[MAX_RANGE_MEASUREMENTS_DBMKV] [float] NULL,
	[CORRELATION_DISTANCE_M] [int] NULL,
	[DELTA_DB] [float] NULL,
	[MAX_ANTENNAS_PATTERN_LOSS_DB] [float] NULL,
	[DETAIL] [bit] NULL,
	[ALTITUDE_STATION] [bit] NULL,
	[SHIFT_ALTITUDE_STATION_MIN_M] [int] NULL,
	[SHIFT_ALTITUDE_STATION_MAX_M] [int] NULL,
	[SHIFT_ALTITUDE_STATION_STEP_M] [int] NULL,
	[MAX_DEVIATION_ALTITUDE_STATION_M] [int] NULL,
	[TILT_STATION] [bit] NULL,
	[SHIFT_TILT_STATION_MIN_DEG] [float] NULL,
	[SHIFT_TILT_STATION_MAX_DEG] [float] NULL,
	[SHIFT_TILT_STATION_STEP_DEG] [float] NULL,
	[MAX_DEVIATION_TILT_STATION_DEG] [float] NULL,
	[AZIMUTH_STATION] [bit] NULL,
	[SHIFT_AZIMUTH_STATION_MIN_DEG] [float] NULL,
	[SHIFT_AZIMUTH_STATION_MAX_DEG] [float] NULL,
	[SHIFT_AZIMUTH_STATION_STEP_DEG] [float] NULL,
	[MAX_DEVIATION_AZIMUTH_STATION_DEG] [float] NULL,
	[COORDINATES_STATION] [bit] NULL,
	[SHIFT_COORDINATES_STATION_M] [int] NULL,
	[SHIFT_COORDINATES_STATION_STEP_M] [int] NULL,
	[MAX_DEVIATION_COORDINATES_STATION_M] [int] NULL,
	[POWER_STATION] [bit] NULL,
	[SHIFT_POWER_STATION_MIN_DB] [float] NULL,
	[SHIFT_POWER_STATION_MAX_DB] [float] NULL,
	[SHIFT_POWER_STATION_STEP_DB] [float] NULL,
	[CASCADE_TUNING] [bit] NULL,
	[NUMBER_CASCADE] [int] NULL,
	[DETAIL_OF_CASCADE] [int] NULL,
	[METHOD] [tinyint] NULL,
	[INFOC_MEAS_RESULTS] [varbinary](max) NULL,
	[STATION_IDS] [varbinary](max) NULL,
 CONSTRAINT [PK_CALCTASKS_STATION_CALIBRATION_ARGS] PRIMARY KEY CLUSTERED 
(
	[TASK_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CALCTASKS_STATION_CALIBRATION_ARGS_DEF](
	[STANDARD] [nvarchar](50)  NULL,
	[TASK_ID] [bigint] NOT NULL,
	[CORRELATION_THRESHOLD_HARD] [float] NULL,
	[CORRELATION_THRESHOLD_WEAK] [float] NULL,
	[TRUST_OLD_RESULTS] [bit] NULL,
	[USE_MEASUREMENT_SAME_GSID] [bit] NULL,
	[DISTANCE_AROUND_CONTOUR_KM] [int] NULL,
	[MIN_NUMBER_POINT_FOR_CORRELATION] [int] NULL,
	[MIN_RANGE_MEASUREMENTS_DBMKV] [float] NULL,
	[MAX_RANGE_MEASUREMENTS_DBMKV] [float] NULL,
	[CORRELATION_DISTANCE_M] [int] NULL,
	[DELTA_DB] [float] NULL,
	[MAX_ANTENNAS_PATTERN_LOSS_DB] [float] NULL,
	[DETAIL] [bit] NULL,
	[ALTITUDE_STATION] [bit] NULL,
	[SHIFT_ALTITUDE_STATION_MIN_M] [int] NULL,
	[SHIFT_ALTITUDE_STATION_MAX_M] [int] NULL,
	[SHIFT_ALTITUDE_STATION_STEP_M] [int] NULL,
	[MAX_DEVIATION_ALTITUDE_STATION_M] [int] NULL,
	[TILT_STATION] [bit] NULL,
	[SHIFT_TILT_STATION_MIN_DEG] [float] NULL,
	[SHIFT_TILT_STATION_MAX_DEG] [float] NULL,
	[SHIFT_TILT_STATION_STEP_DEG] [float] NULL,
	[MAX_DEVIATION_TILT_STATION_DEG] [float] NULL,
	[AZIMUTH_STATION] [bit] NULL,
	[SHIFT_AZIMUTH_STATION_MIN_DEG] [float] NULL,
	[SHIFT_AZIMUTH_STATION_MAX_DEG] [float] NULL,
	[SHIFT_AZIMUTH_STATION_STEP_DEG] [float] NULL,
	[MAX_DEVIATION_AZIMUTH_STATION_DEG] [float] NULL,
	[COORDINATES_STATION] [bit] NULL,
	[SHIFT_COORDINATES_STATION_M] [int] NULL,
	[SHIFT_COORDINATES_STATION_STEP_M] [int] NULL,
	[MAX_DEVIATION_COORDINATES_STATION_M] [int] NULL,
	[POWER_STATION] [bit] NULL,
	[SHIFT_POWER_STATION_MIN_DB] [float] NULL,
	[SHIFT_POWER_STATION_MAX_DB] [float] NULL,
	[SHIFT_POWER_STATION_STEP_DB] [float] NULL,
	[CASCADE_TUNING] [bit] NULL,
	[NUMBER_CASCADE] [int] NULL,
	[DETAIL_OF_CASCADE] [int] NULL,
	[METHOD] [tinyint] NULL,
	[INFOC_MEAS_RESULTS] [varbinary](max) NULL,
	[STATION_IDS] [varbinary](max) NULL,
 CONSTRAINT [PK_CALCTASKS_STATION_CALIBRATION_ARGS_DEF] PRIMARY KEY CLUSTERED 
(
	[TASK_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO 

CREATE TABLE [CALC].[CLIENT_CONTEXTS](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[BASE_CONTEXT_ID] [bigint] NULL,
	[PROJECT_ID] [bigint] NOT NULL,
	[CONTEXT_NAME] [nvarchar](250) NOT NULL,
	[CONTEXT_NOTE] [nvarchar](max) NULL,
	[OWNER_INSTANCE] [nvarchar](250) NOT NULL,
	[OWNER_CONTEXT_ID] [uniqueidentifier] NOT NULL,
	[CREATED_DATE] [datetimeoffset](7) NOT NULL,
	[TYPE_CODE] [tinyint] NOT NULL,
	[TYPE_NAME] [nvarchar](50) NOT NULL,
	[STATUS_CODE] [tinyint] NOT NULL,
	[STATUS_NAME] [nvarchar](50) NOT NULL,
	[STATUS_NOTE] [nvarchar](max) NULL,
	

 CONSTRAINT [PK_CLIENT_CONTEXTS] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 
GO

ALTER TABLE [CALC].[CLIENT_CONTEXTS] ADD  CONSTRAINT [DF_CLIENT_CONTEXTS_CREATED_DATE]  DEFAULT (sysdatetimeoffset()) FOR [CREATED_DATE]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO 

CREATE TABLE [CALC].[CLIENT_CONTEXTS_ABSORPTION](
	[CONTEXT_ID] [bigint]  NOT NULL,
	[MODEL_TYPE_CODE] [tinyint] NOT NULL,
	[MODEL_TYPE_NAME] [nvarchar](100) NOT NULL,
	[AVAILABLE] [BIT] NOT NULL,
 CONSTRAINT [PK_CLIENT_CONTEXTS_ABSORPTION] PRIMARY KEY CLUSTERED 
(
	[CONTEXT_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO 

CREATE TABLE [CALC].[CLIENT_CONTEXTS_ADDITIONAL](
	[CONTEXT_ID] [bigint]  NOT NULL,
	[MODEL_TYPE_CODE] [tinyint] NOT NULL,
	[MODEL_TYPE_NAME] [nvarchar](100) NOT NULL,
	[AVAILABLE] [BIT] NOT NULL,
 CONSTRAINT [PK_CLIENT_CONTEXTS_ADDITIONAL] PRIMARY KEY CLUSTERED 
(
	[CONTEXT_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO 

CREATE TABLE [CALC].[CLIENT_CONTEXTS_ATMOSPHERIC](
	[CONTEXT_ID] [bigint]  NOT NULL,
	[MODEL_TYPE_CODE] [tinyint] NOT NULL,
	[MODEL_TYPE_NAME] [nvarchar](100) NOT NULL,
	[AVAILABLE] [BIT] NOT NULL,
 CONSTRAINT [PK_CLIENT_CONTEXTS_ATMOSPHERIC] PRIMARY KEY CLUSTERED 
(
	[CONTEXT_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO 

CREATE TABLE [CALC].[CLIENT_CONTEXTS_CLUTTER](
	[CONTEXT_ID] [bigint]  NOT NULL,
	[MODEL_TYPE_CODE] [tinyint] NOT NULL,
	[MODEL_TYPE_NAME] [nvarchar](100) NOT NULL,
	[AVAILABLE] [BIT] NOT NULL,
 CONSTRAINT [PK_CLIENT_CONTEXTS_CLUTTER] PRIMARY KEY CLUSTERED 
(
	[CONTEXT_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO 

CREATE TABLE [CALC].[CLIENT_CONTEXTS_DIFFRACTION](
	[CONTEXT_ID] [bigint]  NOT NULL,
	[MODEL_TYPE_CODE] [tinyint] NOT NULL,
	[MODEL_TYPE_NAME] [nvarchar](100) NOT NULL,
	[AVAILABLE] [BIT] NOT NULL,
 CONSTRAINT [PK_CLIENT_CONTEXTS_DIFFRACTION] PRIMARY KEY CLUSTERED 
(
	[CONTEXT_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO 

CREATE TABLE [CALC].[CLIENT_CONTEXTS_DUCTING](
	[CONTEXT_ID] [bigint]  NOT NULL,
	[AVAILABLE] [BIT] NOT NULL,
 CONSTRAINT [PK_CLIENT_CONTEXTS_DUCTING] PRIMARY KEY CLUSTERED 
(
	[CONTEXT_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO 

CREATE TABLE [CALC].[CLIENT_CONTEXTS_GLOBALPARAMS](
	[CONTEXT_ID] [bigint]  NOT NULL,
	[TIME_PC] [REAL] NULL,
	[LOCATION_PC] [REAL]NULL,
	[EARTH_RADIUS_KM] [REAL] NULL,
 CONSTRAINT [PK_CLIENT_CONTEXTS_GLOBALPARAMS] PRIMARY KEY CLUSTERED 
(
	[CONTEXT_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO 

CREATE TABLE [CALC].[CLIENT_CONTEXTS_MAINBLOCK](
	[CONTEXT_ID] [bigint]  NOT NULL,
	[MODEL_TYPE_CODE] [tinyint] NOT NULL,
	[MODEL_TYPE_NAME] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_CLIENT_CONTEXTS_MAINBLOCK] PRIMARY KEY CLUSTERED 
(
	[CONTEXT_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO 

CREATE TABLE [CALC].[CLIENT_CONTEXTS_REFLECTION](
	[CONTEXT_ID] [bigint]  NOT NULL,
	[AVAILABLE] [BIT] NOT NULL,
 CONSTRAINT [PK_CLIENT_CONTEXTS_REFLECTION] PRIMARY KEY CLUSTERED 
(
	[CONTEXT_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO 

CREATE TABLE [CALC].[CLIENT_CONTEXTS_SUBPATHDIFFRACTION](
	[CONTEXT_ID] [bigint]  NOT NULL,
	[MODEL_TYPE_CODE] [tinyint] NOT NULL,
	[MODEL_TYPE_NAME] [nvarchar](100) NOT NULL,
	[AVAILABLE] [BIT] NOT NULL,
 CONSTRAINT [PK_CLIENT_CONTEXTS_SUBPATHDIFFRACTION] PRIMARY KEY CLUSTERED 
(
	[CONTEXT_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO 

CREATE TABLE [CALC].[CLIENT_CONTEXTS_TROPO](
	[CONTEXT_ID] [bigint]  NOT NULL,
	[MODEL_TYPE_CODE] [tinyint] NOT NULL,
	[MODEL_TYPE_NAME] [nvarchar](100) NOT NULL,
	[AVAILABLE] [BIT] NOT NULL,
 CONSTRAINT [PK_CLIENT_CONTEXTS_TROPO] PRIMARY KEY CLUSTERED 
(
	[CONTEXT_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CLUTTERS_DESCS](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[INFOC_DESC_ID] [bigint] NOT NULL,
	[MAP_ID] [bigint] NOT NULL,
	[CREATED_DATE] [datetimeoffset](7) NOT NULL,
	[NAME] [nvarchar](450) NOT NULL,
	[NOTE] [nvarchar](max) NULL,
 CONSTRAINT [PK_CLUTTERS_DESCS] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [CALC].[CLUTTERS_DESCS] ADD  CONSTRAINT [DF_CLUTTERS_DESCS_CREATED_DATE]  DEFAULT (sysdatetimeoffset()) FOR [CREATED_DATE]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CLUTTERS_DESCS_CLUTTERS](
	[CLUTTERS_DESC_ID] [bigint] NOT NULL,
	[CODE] [tinyint] NOT NULL,
	[NAME] [nvarchar](150) NOT NULL,
	[NOTE] [nvarchar](max) NULL,
	[HEIGHT_M] [tinyint] NOT NULL,
 CONSTRAINT [PK_CLUTTERS_DESCS_CLUTTERS] PRIMARY KEY CLUSTERED 
(
	[CLUTTERS_DESC_ID] ASC,
	[CODE] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CLUTTERS_DESCS_FREQS](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[CLUTTERS_DESC_ID] [bigint] NOT NULL,
	[FREQ_MHZ] [FLOAT] NOT NULL,
	[NOTE] [nvarchar](max) NULL
 CONSTRAINT [PK_CLUTTERS_DESCS_FREQS] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CLUTTERS_DESCS_FREQS_CLUTTERS](
	[FREQ_ID] [bigint] NOT NULL,
	[CODE] [tinyint] NOT NULL,
	[LINEAR_LOSS_DBKM] [REAL] NULL,
	[FLAT_LOSS_DB] [REAL] NULL,
	[REFLECTION_MHZ] [REAL] NULL,
	[NOTE] [nvarchar](max) NULL,
 CONSTRAINT [PK_CLUTTERS_DESCS_FREQS_CLUTTERS] PRIMARY KEY CLUSTERED 
(
	[FREQ_ID] ASC,
	[CODE] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CONTEXT_PLANNEDCALCTASK](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[CONTEXT_ID] [bigint] NOT NULL,
	[TYPE_CODE] [int] NOT NULL,
	[TYPE_NAME] [nvarchar](250) NOT NULL,
	[START_NUMBER] [int] NOT NULL,
	[MAP_NAME] [nvarchar](250) NULL,
	[NOTE] [nvarchar](max) NULL,
 CONSTRAINT [PK_CONTEXT_PLANNEDCALCTASK] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO 

CREATE TABLE [CALC].[CONTEXT_STATION_COORDINATES](
	[STATION_ID] [bigint]  NOT NULL,
	[ATDI_X] [int] NOT NULL,
	[ATDI_Y] [int] NOT NULL,
	[EPSG_X] [FLOAT] NOT NULL,
	[EPSG_Y] [FLOAT] NOT NULL,
 CONSTRAINT [PK_CONTEXT_STATION_COORDINATES] PRIMARY KEY CLUSTERED 
(
	[STATION_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO 

CREATE TABLE [CALC].[CONTEXT_STATION_PATTERNS](
	[STATION_ID] [bigint]  NOT NULL,
	[ANTENNA_PLANE] [nvarchar](1)  NOT NULL,
	[WAVE_PLANE] [nvarchar](1)  NOT NULL,
	[LOSS_DB] [varbinary](max) NOT NULL,
	[ANGLE_DEG] [varbinary](max) NOT NULL,
 CONSTRAINT [PK_CONTEXT_STATION_PATTERNS] PRIMARY KEY CLUSTERED 
(
	[STATION_ID] ASC, [ANTENNA_PLANE] ASC, [WAVE_PLANE] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]  TEXTIMAGE_ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CONTEXT_STATIONS](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[CONTEXT_ID] [bigint] NOT NULL,
	[CREATED_DATE] [datetimeoffset](7) NOT NULL,
	[NAME] [nvarchar](250) NOT NULL,
	[CALL_SIGN] [nvarchar](50) NULL,
	[STANDARD] [nvarchar](50),
	[STATE_CODE] [tinyint] NOT NULL,
	[STATE_NAME] [nvarchar](50) NOT NULL,
	[EXTERNAL_CODE] [nvarchar](50) NULL,
	[EXTERNAL_SOURCE] [nvarchar](50) NULL,
	[MODIFIED_DATE] [datetimeoffset](7) NULL,
	[LICENSE_GSID] [nvarchar](50) NULL,
	[REAL_GSID] [nvarchar](50) NULL,
	[REGION_CODE] [nvarchar](50) NULL,
 CONSTRAINT [PK_CONTEXT_STATIONS] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [CALC].[CONTEXT_STATIONS] ADD  CONSTRAINT [DF_CONTEXT_STATIONS_CREATED_DATE]  DEFAULT (sysdatetimeoffset()) FOR [CREATED_DATE]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO 

CREATE TABLE [CALC].[CONTEXT_STATIONS_ANTENNA](
	[ID] [bigint] NOT NULL,
	[GAIN_DB] [REAL] NOT NULL,
	[TILT_DEG] [FLOAT] NOT NULL,
	[AZIMUTH_DEG] [FLOAT] NOT NULL,
	[XPD_DB] [REAL] NOT NULL,
	[ITU_PATTERN_CODE] [tinyint] NOT NULL,
	[ITU_PATTERN_NAME] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_CONTEXT_STATIONS_ANTENNA] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO 

CREATE TABLE [CALC].[CONTEXT_STATIONS_RECEIVER](
	[STATION_ID] [bigint] NOT NULL,
	[FREQ_MHZ] [FLOAT] NOT NULL,
	[BW_KHZ] [FLOAT] NOT NULL,
	[LOSS_DB] [REAL] NOT NULL,
	[KTBF_DBM] [REAL] NOT NULL,
	[THRESHOLD_DBM] [REAL] NOT NULL,
	[POLARIZATION_CODE] [tinyint] NOT NULL,
	[POLARIZATION_NAME] [nvarchar](50) NOT NULL,
	[FREQS_MHZ] [varbinary](max) NULL,
 CONSTRAINT [PK_CONTEXT_STATIONS_RECEIVER] PRIMARY KEY CLUSTERED 
(
	[STATION_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO 

CREATE TABLE [CALC].[CONTEXT_STATIONS_SITE](
	[ID] [bigint] NOT NULL,
	[LON_DEC] [FLOAT] NOT NULL,
	[LAT_DEC] [FLOAT] NOT NULL,
	[ALT_M] [FLOAT] NOT NULL,
 CONSTRAINT [PK_CONTEXT_STATIONS_SITE] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO 

CREATE TABLE [CALC].[CONTEXT_STATIONS_TRANSMITTER](
	[STATION_ID] [bigint] NOT NULL,
	[FREQ_MHZ] [REAL] NOT NULL,
	[BW_KHZ] [FLOAT] NOT NULL,
	[LOSS_DB] [REAL] NOT NULL,
	[MAX_POWER_DBM] [REAL] NOT NULL,
	[POLARIZATION_CODE] [tinyint] NOT NULL,
	[POLARIZATION_NAME] [nvarchar](50) NOT NULL,
	[FREQS_MHZ] [varbinary](max) NULL,
 CONSTRAINT [PK_CONTEXT_STATIONS_TRANSMITTER] PRIMARY KEY CLUSTERED 
(
	[STATION_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[GN06_AFFECTED_ADM_RESULT](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[CALCRESULTS_GN06_ID] [bigint] NOT NULL,
	[ADM] [nvarchar](50) NULL,
	[TYPE_AFFECTED] [nvarchar](50) NULL,
	[AFFECTED_SERVICES] [nvarchar](50) NULL,
 CONSTRAINT [PK_GN06_AFFECTED_ADM_RESULT] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[GN06_ALLOTMENT_OR_ASSIGNMENT_RESULT](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[CALCRESULTS_GN06_ID] [bigint] NOT NULL,
	[ADM] [nvarchar](50) NULL,
	[TYPE_TABLE] [nvarchar](50) NULL,
	[NAME] [nvarchar](100) NULL,
	[FREQ_MHZ] [real] NULL,
	[LON_DEC] [real] NULL,
	[LAT_DEC] [real] NULL,
	[MAX_EFFECTIVE_HEIGHT_M] [int] NULL,
	[POLAR] [nvarchar](100) NULL,
	[ERPH_DBW] [float] NULL,
	[ERPV_DBW] [float] NULL,
	[ANTENNA_DIRECTIONAL] [nvarchar](50) NULL,
	[ADM_REF_ID] [nvarchar](100) NULL,
    [CONTOURS_POINTS] [nvarchar](max) NULL,
	[SOURCE] [nvarchar](50) NULL,
 CONSTRAINT [PK_GN06_ALLOTMENT_OR_ASSIGNMENT_RESULT] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[GN06_CONTOURS_RESULT](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[CALCRESULTS_GN06_ID] [bigint] NOT NULL,
	[CONTOUR_TYPE] [tinyint] NOT NULL,
	[DISTANCE] [int] NULL,
	[FS] [real] NULL,
	[AFFECTED_ADM] [nvarchar](50) NULL,
	[POINTS_COUNT] [int] NULL,
	[CONTOURS_POINTS] [nvarchar](max) NULL,
 CONSTRAINT [PK_GN06_CONTOURS_RESULT] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[PROJECT_MAP_CONTENT_SOURCES](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[CONTENT_ID] [bigint] NOT NULL,
	[INFOC_MAP_ID] [bigint] NOT NULL,
	[INFOC_MAP_NAME] [nvarchar](450) NOT NULL,
	[COVERAGE] [numeric](22, 8) NULL,
	[CRD_UPL_X] [int] NOT NULL,
	[CRD_UPL_Y] [int] NOT NULL,
	[CRD_LWR_X] [int] NOT NULL,
	[CRD_LWR_Y] [int] NOT NULL,
	[PRIORITY_CODE] [tinyint] NULL,
	[PRIORITY_NAME] [nvarchar](50) NULL,
 CONSTRAINT [PK_PROJECT_MAP_CONTENT_SOURCES] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[PROJECT_MAP_CONTENTS](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[MAP_ID] [bigint] NOT NULL,
	[CREATED_DATE] [datetimeoffset](7) NOT NULL,
	[TYPE_CODE] [tinyint] NOT NULL,
	[TYPE_NAME] [nvarchar](50) NOT NULL,
	[STEP_DATATYPE] [nvarchar](50) NOT NULL,
	[STEP_DATASIZE] [tinyint] NOT NULL,
	[SOURCE_COUNT] [int] NULL,
	[SOURCE_COVERAGE] [numeric](22, 8) NULL,
	[CONTENT_SIZE] [int] NOT NULL,
	[CONTENT_TYPE] [nvarchar](250) NOT NULL,
	[CONTENT_ENCODING] [nvarchar](250) NOT NULL,
	[CONTENT] [varbinary](max) NOT NULL,
 CONSTRAINT [PK_PROJECT_MAP_CONTENTS] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [CALC].[PROJECT_MAP_CONTENTS] ADD  CONSTRAINT [DF_PROJECT_MAP_CONTENTS_CREATED_DATE]  DEFAULT (sysdatetimeoffset()) FOR [CREATED_DATE]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[PROJECT_MAPS](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[PROJECT_ID] [bigint] NOT NULL,
	[MAP_NAME] [nvarchar](250) NOT NULL,
	[MAP_NOTE] [nvarchar](max) NULL,
	[OWNER_INSTANCE] [nvarchar](250) NOT NULL,
	[OWNER_MAP_ID] [uniqueidentifier] NOT NULL,
	[CREATED_DATE] [datetimeoffset](7) NOT NULL,
	[STATUS_CODE] [tinyint] NOT NULL,
	[STATUS_NAME] [nvarchar](50) NOT NULL,
	[STATUS_NOTE] [nvarchar](max) NULL,
	
	[SOURCE_TYPE_CODE] [tinyint] NULL,
	[SOURCE_TYPE_NAME] [nvarchar](50) NULL,

	[STEP_UNIT] [nvarchar](10) NOT NULL,
	[OWNER_AXIS_X_NUMBER] [int] NOT NULL,
	[OWNER_AXIS_X_STEP] [int] NOT NULL,
	[OWNER_AXIS_Y_NUMBER] [int] NOT NULL,
	[OWNER_AXIS_Y_STEP] [int] NOT NULL,
	[OWNER_CRD_UPL_X] [int] NOT NULL,
	[OWNER_CRD_UPL_Y] [int] NOT NULL,
	[AXIS_X_NUMBER] [int] NULL,
	[AXIS_X_STEP] [int] NULL,
	[AXIS_Y_NUMBER] [int] NULL,
	[AXIS_Y_STEP] [int] NULL,
	[CRD_UPL_X] [int] NULL,
	[CRD_UPL_Y] [int] NULL,
	[CRD_LWR_X] [int] NULL,
	[CRD_LWR_Y] [int] NULL,
 CONSTRAINT [PK_PROJECT_MAPS] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [CALC].[PROJECT_MAPS] ADD  CONSTRAINT [DF_PROJECT_MAPS_CREATED_DATE]  DEFAULT (sysdatetimeoffset()) FOR [CREATED_DATE]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[PROJECTS](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[PROJECT_NAME] [nvarchar](250) NOT NULL,
	[PROJECT_NOTE] [nvarchar](max) NULL,
	[OWNER_INSTANCE] [nvarchar](250) NOT NULL,
	[OWNER_PROJECT_ID] [uniqueidentifier] NOT NULL,
	[CREATED_DATE] [datetimeoffset](7) NOT NULL,
	[STATUS_CODE] [tinyint] NOT NULL,
	[STATUS_NAME] [nvarchar](50) NOT NULL,
	[STATUS_NOTE] [nvarchar](max) NULL,
	[PROJECTION] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_PROJECTS] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [CALC].[PROJECTS] ADD  CONSTRAINT [DF_PROJECTS_CREATED_DATE]  DEFAULT (sysdatetimeoffset()) FOR [CREATED_DATE]
GO
