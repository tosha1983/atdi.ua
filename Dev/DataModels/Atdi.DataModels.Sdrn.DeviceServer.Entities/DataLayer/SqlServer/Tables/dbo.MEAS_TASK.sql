﻿CREATE TABLE [dbo].[MEAS_TASK](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[STATUS] [varchar](50) NULL,
	[TASK_ID] [varchar](50) NULL,
	[SDRN_SERVER] [varchar](50) NULL,
	[SENSOR_NAME] [varchar](50) NULL,
	[EQUIPMENT_TECH_ID] [varchar](200) NULL,
	[TIME_START] [datetime] NULL,
	[TIME_STOP] [datetime] NULL,
	[PRIORITY] [int] NULL,
	[SCAN_PER_TASK_NUMBER] [int] NULL,
	[MOB_EQUIPMENT_MEASUREMENTS] [varchar](4000) NULL,
 CONSTRAINT [XBS_MEASTASK_PK] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


