if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[BEARING]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[BEARING]
go
create table [ICSC].[BEARING] (
	[ID] 	[bigint] identity (1,1) not null,
	[RES_MEAS_STATION_ID]	[bigint] null,
	[LEVEL_DBM]	[float] null,
	[LEVEL_DBMKVM]	[float] null,
	[TIME_OF_MEASUREMENTS]	[datetime] null,
	[BW]	[float] null,
	[QUALITY]	[float] null,
	[CENTRAL_FREQUENCY]	[float] null,
	[BEARING]	[float] null,
	[AZIMUTH]	[float] null,
	[ASL]	[float] null,
	[LON]	[float] null,
	[LAT]	[float] null,
	[AGL]	[float] null,
	constraint [PK_BEARING] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
