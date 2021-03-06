﻿CREATE TABLE [dbo].[OWNER_DATA](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[OWNER_NAME] [varchar](100) NULL,
	[OKPO] [varchar](50) NULL,
	[ZIP] [varchar](50) NULL,
	[CODE] [varchar](50) NULL,
	[ADDRES] [varchar](2000) NULL,
 CONSTRAINT [XBS_OWNERDATA_PK] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


