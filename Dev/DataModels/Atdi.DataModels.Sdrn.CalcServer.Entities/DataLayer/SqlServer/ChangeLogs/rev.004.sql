﻿ALTER TABLE [CALC].[CALCTASKS] ADD [NOTE] [nvarchar](250) NULL
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[COMMANDS](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[CREATED_DATE] [datetimeoffset](7) NOT NULL,
	[TYPE_CODE] [int] NOT NULL,
	[TYPE_NAME] [nvarchar](250) NOT NULL,
	[STATUS_CODE] [tinyint] NOT NULL,
	[STATUS_NAME] [nvarchar](50) NOT NULL,
	[STATUS_NOTE] [nvarchar](max) NULL,	

	[CALLER_INSTANCE] [nvarchar](250) NOT NULL,
	[CALLER_COMMAND_ID] [uniqueidentifier] NOT NULL,
	
	[START_TIME]   [datetimeoffset](7) NULL,
	[FINISH_TIME]  [datetimeoffset](7) NULL,

	[ARGS_JSON]    [nvarchar](max) NULL,	
	[RESULT_JSON]  [nvarchar](max) NULL,	
 CONSTRAINT [PK_COMMANDS] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [CALC].[COMMANDS] ADD  CONSTRAINT [DF_COMMANDS_CREATED_DATE]  DEFAULT (sysdatetimeoffset()) FOR [CREATED_DATE]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CHECKPOINTS](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[CREATED_DATE] [datetimeoffset](7) NOT NULL,
	[RESULT_ID] [bigint] NOT NULL,
	[STATUS_CODE] [tinyint] NOT NULL,
	[STATUS_NAME] [nvarchar](50) NOT NULL,
	[STATUS_NOTE] [nvarchar](max) NULL,	
    [NAME] [nvarchar](250) NOT NULL,
 CONSTRAINT [PK_CHECKPOINTS] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [CALC].[CHECKPOINTS] ADD  CONSTRAINT [DF_CHECKPOINTS_CREATED_DATE]  DEFAULT (sysdatetimeoffset()) FOR [CREATED_DATE]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CHECKPOINTS_DATA](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[CREATED_DATE] [datetimeoffset](7) NOT NULL,
	[CHECKPOINT_ID] [bigint] NOT NULL,
	[DATA_CONTEXT] [nvarchar](250) NOT NULL,
	[DATA_JSON] [nvarchar](max) NULL,	
 CONSTRAINT [PK_CHECKPOINTS_DATA] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [CALC].[CHECKPOINTS_DATA] ADD  CONSTRAINT [DF_CHECKPOINTS_DATA_CREATED_DATE]  DEFAULT (sysdatetimeoffset()) FOR [CREATED_DATE]
GO


ALTER TABLE [CALC].[CALCRESULTS_STATION_CALIBRATION_STA] ADD  [DELTA_CORRELATION_PC] [REAL]  NULL
GO

ALTER TABLE [CALC].[CALCRESULTS_STATION_CALIBRATION_STA] ADD  [USED_POINTS_PC] [int]  NULL
GO


ALTER TABLE [CALC].[CALCRESULTS_STATION_CALIBRATION_STA] ADD  [COUNT_POINTS_IN_DRIVE_TEST] [int]  NULL
GO

