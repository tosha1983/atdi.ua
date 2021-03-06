﻿SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [INFOC].[INTEGRATION_OBJECTS](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[DATA_SOURCE] [nvarchar](150) NOT NULL,
	[OBJECT_NAME] [nvarchar](150) NOT NULL,
	[CREATED_DATE] [datetimeoffset](7) NOT NULL,
	[SYNC_KEY_CONTENT] [varbinary](max) NULL,
	[SYNC_KEY_TYPE] [nvarchar](250) NULL,
	[SYNC_KEY_NOTE] [nvarchar](max) NULL,
	[LAST_SYNC_TIME] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_INTEGRATION_OBJECTS] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [INFOC].[INTEGRATION_OBJECTS] ADD  CONSTRAINT [DF_INTEGRATION_OBJECTS_CREATED_DATE]  DEFAULT (sysdatetimeoffset()) FOR [CREATED_DATE]
GO
