﻿SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [ICSC].[AREA_LOCATION](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[LONGITUDE] [float] NULL,
	[LATITUDE] [float] NULL,
	[AREA_ID] [bigint] NULL,
 CONSTRAINT [AREA_LOCATION_PK] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
