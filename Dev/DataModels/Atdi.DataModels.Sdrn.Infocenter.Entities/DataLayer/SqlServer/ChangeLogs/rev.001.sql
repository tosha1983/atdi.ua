create table [SDRNSVR].[SENSOR] (
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