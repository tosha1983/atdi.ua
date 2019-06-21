﻿
CREATE TABLE [dbo].[SENSOR_EQUIP_SENSITIVITES](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[SENSOREQUIP_ID] [bigint] NULL,
	[FREQ] [float] NULL,
	[KTBF] [float] NULL,
	[NOISEF] [float] NULL,
	[FREQSTABILITY] [float] NULL,
	[ADDLOSS] [float] NULL,
 CONSTRAINT [XBS_SENSOREQUIPSENS_PK] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SENSOR_EQUIP_SENSITIVITES]  WITH NOCHECK ADD  CONSTRAINT [FK_SENSOREQUIP_ID] FOREIGN KEY([SENSOREQUIP_ID])
REFERENCES [dbo].[SENSOR_EQUIPMENT] ([ID])
GO

ALTER TABLE [dbo].[SENSOR_EQUIP_SENSITIVITES] NOCHECK CONSTRAINT [FK_SENSOREQUIP_ID]
GO

ALTER TABLE [dbo].[SENSOR_EQUIP_SENSITIVITES]  WITH CHECK ADD  CONSTRAINT [FK_XBS_SENSOREQUIPSENS_XBS_SENSOREQUIP] FOREIGN KEY([SENSOREQUIP_ID])
REFERENCES [dbo].[SENSOR_EQUIPMENT] ([ID])
GO

ALTER TABLE [dbo].[SENSOR_EQUIP_SENSITIVITES] CHECK CONSTRAINT [FK_XBS_SENSOREQUIPSENS_XBS_SENSOREQUIP]
GO


