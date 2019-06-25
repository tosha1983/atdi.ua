﻿SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [ICSC].[AMQP_MESSAGES](
[ID] [bigint] IDENTITY(1,1) NOT NULL,
[STATUS_CODE] [tinyint] NOT NULL,
[STATUS_NOTE] [nvarchar](max) NULL,
[CREATED_DATE] [datetimeoffset](7) NOT NULL,
[THREAD_ID] [int] NOT NULL,
[PROCESSED_SDATE] [datetimeoffset](7) NULL,
[PROCESSED_FDATE] [datetimeoffset](7) NULL,
[PROP_EXCHANGE] [nvarchar](250) NULL,
[PROP_ROUTING_KEY] [nvarchar](250) NULL,
[PROP_DELIVERY_TAG] [nvarchar](250) NULL,
[PROP_CONSUMER_TAG] [nvarchar](250) NULL,
[PROP_APP_ID] [nvarchar](250) NULL,
[PROP_TYPE] [nvarchar](250) NULL,
[PROP_TIMESTAMP] [bigint] NULL,
[PROP_MESSAGE_ID] [nvarchar](250) NULL,
[PROP_CORRELATION_ID] [nvarchar](250) NULL,
[PROP_CONTENT_ENCODING] [nvarchar](250) NULL,
[PROP_CONTENT_TYPE] [nvarchar](250) NULL,
[HEADER_CREATED] [nvarchar](250) NULL,
[HEADER_SDRNSERVER] [nvarchar](250) NULL,
[HEADER_SENSORNAME] [nvarchar](250) NULL,
[HEADER_SENSORTECHID] [nvarchar](250) NULL,
[BODY_CONTENT_TYPE] [nvarchar](250) NULL,
[BODY_CONTENT_ENCODING] [nvarchar](250) NULL,
[BODY_CONTENT] [varbinary](max) NULL,
CONSTRAINT [PK_AMQP_MESSAGES] PRIMARY KEY CLUSTERED
(
[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO