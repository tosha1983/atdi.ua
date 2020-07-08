﻿SET ANSI_NULLS ON
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