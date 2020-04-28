﻿SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [INFOC].[MAP_SECTORS](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[MAP_ID] [bigint] NOT NULL,
	[SECTOR_NAME] [nvarchar](250) NOT NULL,
	[CREATED_DATE] [datetimeoffset](7) NOT NULL,
	[AXIS_X_INDEX] [int] NOT NULL,
	[AXIS_Y_INDEX] [int] NOT NULL,
	[AXIS_X_NUMBER] [int] NOT NULL,
	[AXIS_Y_NUMBER] [int] NOT NULL,
	[CRD_UPL_X] [int] NOT NULL,
	[CRD_UPL_Y] [int] NOT NULL,
	[CRD_UPR_X] [int] NOT NULL,
	[CRD_UPR_Y] [int] NOT NULL,
	[CRD_LWL_X] [int] NOT NULL,
	[CRD_LWL_Y] [int] NOT NULL,
	[CRD_LWR_X] [int] NOT NULL,
	[CRD_LWR_Y] [int] NOT NULL,
	[CONTENT_SIZE] [int] NOT NULL,
	[CONTENT_TYPE] [nvarchar](250) NOT NULL,
	[CONTENT_ENCODING] [nvarchar](250) NOT NULL,
	[CONTENT] [varbinary](max) NOT NULL,
 CONSTRAINT [PK_MAP_SECTORS] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [INFOC].[MAP_SECTORS] ADD  CONSTRAINT [DF_MAP_SECTORS_CREATED_DATE]  DEFAULT (sysdatetimeoffset()) FOR [CREATED_DATE]
GO
