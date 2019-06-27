if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[RES_STLEVEL_CAR]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[RES_STLEVEL_CAR]
go
create table [ICSC].[RES_STLEVEL_CAR] (
	[ID] 	[bigint] identity (1,1) not null,
	[RES_MEAS_STATION_ID]	[bigint] null,
	[ALTITUDE]		[float] null,
	[DIFFERENCE_TIMESTAMP]		[float] null,
	[LON]		[float] null,
	[LAT]		[float] null,
	[LEVEL_DBM]		[float] null,
	[LEVEL_DBMKVM]		[float] null,
	[TIME_OF_MEASUREMENTS]		[datetime] null,
	[AGL]		[float] null,
	constraint [PK_RES_STLEVEL_CAR] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
