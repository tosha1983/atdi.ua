﻿SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [ICSC].[HEALTH_LOG_DETAIL](
[ID] [bigint] IDENTITY(1,1) NOT NULL,
[HEALTH_ID] [bigint] NOT NULL,
[CREATED_DATE] [datetimeoffset](7) NOT NULL,
[MESSAGE] [nvarchar](250) NOT NULL,
[NOTE] [nvarchar](max) NULL,
[THREAD_ID] [int] NOT NULL,
[SOURCE] [nvarchar](450) NULL,
[SITE_TYPE_CODE] [tinyint] NOT NULL,
[SITE_TYPE_NAME] [nvarchar](50) NOT NULL,
[SITE_INSTANCE] [nvarchar](250) NOT NULL,
[SITE_HOST] [nvarchar](250) NULL,
CONSTRAINT [PK_HEALTH_LOG_DETAIL] PRIMARY KEY CLUSTERED
(
[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
