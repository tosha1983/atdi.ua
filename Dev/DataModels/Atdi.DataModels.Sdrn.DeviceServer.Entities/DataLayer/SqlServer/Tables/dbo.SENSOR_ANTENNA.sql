﻿CREATE TABLE [dbo].[SENSOR_ANTENNA](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SENSORID] [int] NULL,
	[CODE] [varchar](50) NULL,
	[SLEWANG] [float] NULL,
	[MANUFACTURER] [varchar](50) NULL,
	[NAME] [varchar](50) NULL,
	[TECHID] [varchar](50) NULL,
	[ANTDIR] [varchar](50) NULL,
	[HBEAMWIDTH] [float] NULL,
	[VBEAMWIDTH] [float] NULL,
	[POLARIZATION] [varchar](50) NULL,
	[USETYPE] [varchar](50) NULL,
	[CATEGORY] [varchar](50) NULL,
	[GAINTYPE] [varchar](50) NULL,
	[GAINMAX] [float] NULL,
	[LOWERFREQ] [float] NULL,
	[UPPERFREQ] [float] NULL,
	[ADDLOSS] [float] NULL,
	[XPD] [float] NULL,
	[ANTCLASS] [varchar](50) NULL,
	[REMARK] [varchar](250) NULL,
	[CUSTTXT1] [varchar](250) NULL,
	[CUSTDATA1] [varchar](250) NULL,
	[CUSTNBR1] [float] NULL,
 CONSTRAINT [XBS_SENSORANTENNA_PK] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[SENSOR_ANTENNA]  WITH CHECK ADD  CONSTRAINT [FK_XBS_SENSORANTENNA_XBS_SENSOR] FOREIGN KEY([SENSORID])
REFERENCES [dbo].[SENSOR] ([ID])
GO

ALTER TABLE [dbo].[SENSOR_ANTENNA] CHECK CONSTRAINT [FK_XBS_SENSORANTENNA_XBS_SENSOR]
GO


