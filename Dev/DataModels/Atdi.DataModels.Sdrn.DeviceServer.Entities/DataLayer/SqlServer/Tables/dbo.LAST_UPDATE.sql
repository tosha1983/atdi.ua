﻿CREATE TABLE [dbo].[LAST_UPDATE](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[TABLE_NAME] [nvarchar](100) NULL,
	[LAST_UPDATE] [datetime] NULL,
	[STATUS] [nvarchar](100) NULL,
 CONSTRAINT [PK_XBS_LASTUPDATE] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

