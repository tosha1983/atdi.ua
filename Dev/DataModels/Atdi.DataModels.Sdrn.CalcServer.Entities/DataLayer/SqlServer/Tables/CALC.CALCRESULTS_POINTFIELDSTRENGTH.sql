﻿SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CALCRESULTS_POINTFIELDSTRENGTH](
	[ID] [bigint] NOT NULL,
	[FS_DBUVM] [REAL] NULL,
	[LEVEL_DBM] [REAL] NULL,
 CONSTRAINT [PK_CALCRESULTS_POINTFIELDSTRENGTH] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


