create table [SDRNSVR].[SENSORS] (
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
	constraint [PK_SENSOR] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]

GO

create table [ICSC].[SENSOR_ANTENNAS] (
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
go