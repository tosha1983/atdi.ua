﻿SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO 

CREATE TABLE [CALC].[CONTEXT_STATIONS](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[CONTEXT_ID] [bigint] NOT NULL,
	[CREATED_DATE] [datetimeoffset](7) NOT NULL,
	[NAME] [nvarchar](250) NOT NULL,
	[CALL_SIGN] [nvarchar](50) NOT NULL,
	[STATE_CODE] [tinyint] NOT NULL,
	[STATE_NAME] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_CONTEXT_STATIONS] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 
GO

ALTER TABLE [CALC].[CONTEXT_STATIONS] ADD  CONSTRAINT [DF_CONTEXT_STATIONS_CREATED_DATE]  DEFAULT (sysdatetimeoffset()) FOR [CREATED_DATE]
GO