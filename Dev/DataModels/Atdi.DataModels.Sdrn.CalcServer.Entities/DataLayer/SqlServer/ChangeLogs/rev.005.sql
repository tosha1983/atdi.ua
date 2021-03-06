﻿SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CALCRESULTS_STATION_CALIBRATION_TEMP](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[RESULT_ID] [bigint] NOT NULL,
	[CONTENT] [varbinary](max) NOT NULL,
 CONSTRAINT [PK_CALCRESULTS_STATION_CALIBRATION_TEMP] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO



ALTER TABLE [CALC].[CLIENT_CONTEXTS_ABSORPTION] ADD [HYBRID] [bit]  DEFAULT ((0))  NOT NULL
GO
