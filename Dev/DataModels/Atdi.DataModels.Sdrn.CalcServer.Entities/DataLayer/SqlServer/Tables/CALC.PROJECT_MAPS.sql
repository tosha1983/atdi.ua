﻿SET ANSI_NULLS ON
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

