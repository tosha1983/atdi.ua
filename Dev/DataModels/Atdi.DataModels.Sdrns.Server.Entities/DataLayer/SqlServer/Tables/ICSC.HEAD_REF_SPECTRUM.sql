﻿SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [ICSC].[HEAD_REF_SPECTRUM](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[FILE_NAME] [nvarchar](500) NULL,
	[CREATED_DATE] [datetime] NULL,
	[CREATED_BY] [nvarchar](100) NULL,
	[COUNT_IMPORT_RECORDS] [int] NULL,
	[MIN_FREQ_MHZ] [float] NULL,
	[MAX_FREQ_MHZ] [float] NULL,
	[COUNT_SENSORS] [int] NULL,
 CONSTRAINT [HEAD_REF_SPECTRUM_PK] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO