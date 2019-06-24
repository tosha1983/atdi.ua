if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[BEARING]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[BEARING]
go
create table [ICSC].[BEARING] (
	[ID] 	[bigint] identity (1,1) not null,
	[RES_MEAS_STATION_ID]	[bigint] null,
	[LEVEL_DBM]	[numeric](22,8) null,
	[LEVEL_DBMKVM]	[numeric](22,8) null,
	[TIME_OF_MEASUREMENTS]	[datetime] null,
	[BW]	[numeric](22,8) null,
	[QUALITY]	[numeric](22,8) null,
	[CENTRAL_FREQUENCY]	[numeric](22,8) null,
	[BEARING]	[numeric](22,8) null,
	[AZIMUTH]	[numeric](22,8) null,
	[ASL]	[numeric](22,8) null,
	[LON]	[numeric](22,8) null,
	[LAT]	[numeric](22,8) null,
	[AGL]	[numeric](22,8) null,
	constraint [PK_BEARING] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
