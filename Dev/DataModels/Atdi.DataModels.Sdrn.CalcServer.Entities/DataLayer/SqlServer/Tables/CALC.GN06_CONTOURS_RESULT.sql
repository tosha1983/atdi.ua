﻿SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[GN06_CONTOURS_RESULT](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[CALCRESULTS_GN06_ID] [bigint] NOT NULL,
	[CONTOUR_TYPE] [tinyint] NOT NULL,
	[DISTANCE] [int] NULL,
	[FS] [int] NULL,
	[AFFECTED_ADM] [nvarchar](50) NULL,
	[POINTS_COUNT] [int] NULL,
	[CONTOURS_POINTS] [nvarchar](max) NULL,
 CONSTRAINT [PK_GN06_CONTOURS_RESULT] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

