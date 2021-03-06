﻿SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [INFOC].[CLUTTERS_DESCS](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[MAP_ID] [bigint] NULL,
	[CREATED_DATE] [datetimeoffset](7) NOT NULL,
	[NAME] [nvarchar](450) NOT NULL,
	[NOTE] [nvarchar](max) NULL,
 CONSTRAINT [PK_CLUTTERS_DESCS] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [INFOC].[CLUTTERS_DESCS] ADD  CONSTRAINT [DF_CLUTTERS_DESCS_CREATED_DATE]  DEFAULT (sysdatetimeoffset()) FOR [CREATED_DATE]
GO
