﻿create table [SDRNSVR].[SENSORS] (
	[ID] 	[bigint]  not null,
	[SENSORIDENTIFIER_ID]	[bigint] null,
	[STATUS]	[nvarchar](25) null,
	[NAME]	[nvarchar](150) null,
	[BIUSEDATE]	[datetime] null,
	[EOUSEDATE]	[datetime] null,
	[AZIMUTH]	[float] null,
	[ELEVATION]	[float] null,
	[AGL]	[float] null,
	[RXLOSS]	[float] null,
	[TECHID]	[nvarchar](150) null,
	constraint [PK_SENSORS] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]

GO

create table [SDRNSVR].[SENSOR_ANTENNAS] (
	[ID] 	[bigint] not null,
	[SENSOR_ID]	[bigint] null,
	[CODE]	[nvarchar](50) null,
	[MANUFACTURER]	[nvarchar](50) null,
	[NAME]	[nvarchar](50) null,
	[TECHID]	[nvarchar](150) null,
	[ANTDIR]	[nvarchar](50) null,
	[HBEAMWIDTH]	[float] null,
	[VBEAMWIDTH]	[float] null,
	[POLARIZATION]	[nvarchar](50) null,
	[GAINTYPE]	[nvarchar](50) null,
	[GAINMAX]	[float] null,
	[LOWERFREQ]	[float] null,
	[UPPERFREQ]	[float] null,
	[ADDLOSS]	[float] null,
	[XPD]	[float] null,
	constraint [PK_SENSOR_ANTENNAS] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
GO

create table [SDRNSVR].[SENSOR_ANTENNA_PATTERNS] (
	[ID] 	[bigint] not null,
	[SENSOR_ANTENNA_ID]	[bigint] null,
	[FREQ]	[float] null,
	[GAIN]	[float] null,
	[DIAGA]	[varchar](1000) null,
	[DIAGH]	[varchar](1000) null,
	[DIAGV]	[varchar](1000) null,
	constraint [PK_SENSOR_ANTENNA_PATTERNS] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
GO

create table [SDRNSVR].[SENSOR_EQUIP] (
	[ID] 	[bigint] not null,
	[SENSOR_ID]	[bigint] null,
	[CODE]	[nvarchar](50) null,
	[MANUFACTURER]	[nvarchar](50) null,
	[NAME]	[nvarchar](50) null,
	[TECHID]	[nvarchar](200) null,
	[LOWER_FREQ]	[float] null,
	[UPPER_FREQ]	[float] null,
	constraint [PK_SENSOR_EQUIP] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
GO

create table [SDRNSVR].[SENSOR_LOCATIONS] (
	[ID] 	[bigint] not null,
	[SENSOR_ID]	[bigint] null,
	[DATA_FROM]	[datetime] null,
	[DATA_TO]	[datetime] null,
	[DATA_CREATED]	[datetime] null,
	[STATUS]	[nvarchar](25) null,
	[LON]	[float] null,
	[LAT]	[float] null,
	[ASL]	[float] null,
	constraint [PK_SENSOR_LOCATIONS] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
GO

create table [SDRNSVR].[SENSOR_EQUIP_SENSITIVITES] (
	[ID] 	[bigint] not null,
	[SENSOR_EQUIP_ID]	[bigint] null,
	[FREQ]	[float] null,
	[KTBF]	[float] null,
	[NOISEF]	[float] null,
	[FREQ_STABILITY]	[float] null,
	[ADDLOSS]	[float] null,
	constraint [PK_SENSOR_EQUIP_SENSITIVITES] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go