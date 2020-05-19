﻿SET ANSI_NULLS ON
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