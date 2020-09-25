﻿ALTER TABLE [CALC].[CALCTASKS_STATION_CALIBRATION_ARGS] ADD  [AREAS] [nvarchar](1000)  NULL
GO

ALTER TABLE [CALC].[CALCTASKS_STATION_CALIBRATION_ARGS_DEF] ADD  [AREAS] [nvarchar](1000)  NULL
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