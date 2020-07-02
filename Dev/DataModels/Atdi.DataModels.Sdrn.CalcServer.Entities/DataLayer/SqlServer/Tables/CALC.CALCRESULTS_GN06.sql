﻿SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CALCRESULTS_GN06](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[RESULT_ID] [bigint] NOT NULL,
 CONSTRAINT [PK_CALCRESULTS_GN06] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
