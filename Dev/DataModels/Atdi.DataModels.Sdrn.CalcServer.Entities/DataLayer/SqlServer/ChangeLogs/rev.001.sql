﻿SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CALCRESULTS_REF_SPECTRUM_BY_DRIVE_TESTS](
	[RESULT_ID] [bigint] NOT NULL,
	[DATE_CREATED] [datetimeoffset](7) NOT NULL,
	[PARAMETERS_TASK_ID] [bigint] NULL,
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_CALCRESULTS_REF_SPECTRUM_BY_DRIVE_TESTS] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [CALC].[CALCRESULTS_REF_SPECTRUM_BY_DRIVE_TESTS] ADD  CONSTRAINT [DF_CALCRESULTS_REF_SPECTRUM_BY_DRIVE_TESTS]  DEFAULT (sysdatetimeoffset()) FOR [DATE_CREATED]
GO



SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CALCRESULTS_REF_SPECTRUM_BY_DRIVE_TESTS_DETAIL](
    [ID] [bigint] IDENTITY(1,1) NOT NULL,
    [ORDER_ID] [bigint]  NOT NULL,
	[RESULT_REF_SPECTRUM_ID] [bigint] NOT NULL,
	[ID_ICSM] [int] NOT NULL,
	[TABLE_ICSM_NAME] [nvarchar](50) NOT NULL,
	[ID_SENSOR] [bigint] NOT NULL,
	[GLOBAL_GSID] [nvarchar](50) NOT NULL,
	[FREQ_MHZ] [REAL] NOT NULL,
	[LEVEL_DBM] [REAL] NOT NULL,
	[PERCENT] [REAL] NOT NULL,
    [DATE_MEAS] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_CALCRESULTS_REF_SPECTRUM_BY_DRIVE_TESTS_DETAIL] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO




SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[REF_SPECTRUM_BY_DRIVE_TESTS_ARGS](
 [RESULT_ID] [bigint] NOT NULL,
 [TASK_ID] [bigint] NOT NULL,
 [COMMENTS] [nvarchar](2000)  NULL,
 [POWER_THRESHOLD_DBM] [FLOAT]  NULL,
 [STATION_IDS] [varbinary](max) NULL,

 CONSTRAINT [PK_REF_SPECTRUM_BY_DRIVE_TESTS_ARGS] PRIMARY KEY CLUSTERED 
(
	[TASK_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[REF_SPECTRUM_BY_DRIVE_TESTS_ARGS_DEF](
 [RESULT_ID] [bigint] NOT NULL,
 [TASK_ID] [bigint] NOT NULL,
 [COMMENTS] [nvarchar](2000)  NULL,
 [POWER_THRESHOLD_DBM] [FLOAT]  NULL,
 [STATION_IDS] [varbinary](max) NULL,
 CONSTRAINT [PK_REF_SPECTRUM_BY_DRIVE_TESTS_ARGS_DEF] PRIMARY KEY CLUSTERED 
(
	[TASK_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [CALC].[CALCTASKS_STATION_CALIBRATION_ARGS] ADD [AREA_CONTOURS] [varbinary](max) NULL
GO

ALTER TABLE [CALC].[CALCTASKS_STATION_CALIBRATION_ARGS_DEF] ADD [AREA_CONTOURS] [varbinary](max) NULL
GO

