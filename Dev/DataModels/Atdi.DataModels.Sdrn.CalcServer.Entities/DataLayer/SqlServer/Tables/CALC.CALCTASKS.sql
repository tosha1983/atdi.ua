﻿SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CALCTASKS](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[PROJECT_ID] [bigint] NOT NULL,
	[TYPE_CODE] [int] NOT NULL,
	[STATUS_CODE] [tinyint] NOT NULL,
	[STATUS_NAME] [nvarchar](50) NOT NULL,
	[STATUS_NOTE] [nvarchar](max) NULL,
	[OWNER_INSTANCE] [nvarchar](250) NOT NULL,
	[OWNER_TASK_ID] [uniqueidentifier] NOT NULL,
	[CREATED_DATE] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_CALCTASKS] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [CALC].[CALCTASKS] ADD  CONSTRAINT [DF_CALCTASKS_CREATED_DATE]  DEFAULT (sysdatetimeoffset()) FOR [CREATED_DATE]
GO

