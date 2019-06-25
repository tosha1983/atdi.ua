if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[RES_STLEVEL_CAR]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[RES_STLEVEL_CAR]
go
create table [ICSC].[RES_STLEVEL_CAR] (
	[ID] 	[bigint] identity (1,1) not null,
	[RES_LOCATION_SENSOR_MEAS_STATION_ID]	[bigint] null,
	[ALTITUDE]		[numeric](22,8) null,
	[DIFFERENCE_TIMESTAMP]		[numeric](22,8) null,
	[LON]		[numeric](22,8) null,
	[LAT]		[numeric](22,8) null,
	[LEVEL_DBM]		[numeric](22,8) null,
	[LEVEL_DBMKVM]		[numeric](22,8) null,
	[TIME_OF_MEASUREMENTS]		[datetime] null,
	[AGL]		[numeric](22,8) null,
	constraint [PK_RES_STLEVEL_CAR] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
