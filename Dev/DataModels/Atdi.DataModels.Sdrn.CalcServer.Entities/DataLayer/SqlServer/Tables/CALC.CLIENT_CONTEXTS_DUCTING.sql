﻿SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO 

CREATE TABLE [CALC].[CLIENT_CONTEXTS_DUCTING](
	[CONTEXT_ID] [bigint]  NOT NULL,
	[AVAILABLE] [BIT] NOT NULL,
 CONSTRAINT [PK_CLIENT_CONTEXTS_DUCTING] PRIMARY KEY CLUSTERED 
(
	[CONTEXT_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
