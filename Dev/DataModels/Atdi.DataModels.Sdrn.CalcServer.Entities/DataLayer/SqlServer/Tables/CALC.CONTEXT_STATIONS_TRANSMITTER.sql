﻿SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO 

CREATE TABLE [CALC].[CONTEXT_STATIONS_TRANSMITTER](
	[STATION_ID] [bigint] NOT NULL,
	[FREQ_MHZ] [FLOAT] NOT NULL,
	[BW_KHZ] [FLOAT] NOT NULL,
	[LOSS_DB] [REAL] NOT NULL,
	[MAX_POWER_DBM] [REAL] NOT NULL,
	[POLARIZATION_CODE] [tinyint] NOT NULL,
	[POLARIZATION_NAME] [nvarchar](50) NOT NULL,
	[FREQS_MHZ] [varbinary](max) NULL,
 CONSTRAINT [PK_CONTEXT_STATIONS_TRANSMITTER] PRIMARY KEY CLUSTERED 
(
	[STATION_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 
GO

