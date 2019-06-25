if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[RES_STGENERAL]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[RES_STGENERAL]
go
create table [ICSC].[RES_STGENERAL] (
	[ID] 	[bigint] identity (1,1) not null,
	[RES_LOCATION_SENSOR_MEAS_STATION_ID]	[bigint] null,
	[CENTRAL_FREQUENCY]		[numeric](22,8) null,
	[CENTRAL_FREQUENCY_MEAS]		[numeric](22,8) null,
	[DURATION_MEAS]		[numeric](22,8) null,
	[MARKER_INDEX]		[int] null,
	[T1]		[int] null,
	[T2]		[int] null,
	[TIME_START_MEAS_DATE]		[datetime] null,
	[TIME_FINISH_MEAS]		[datetime] null,
	[OFFSET_FREQUENCY]		[numeric](22,8) null,
	[SPECRUM_START_FREQ]		[float](10) null,
	[SPECRUM_STEPS]		[float](10) null,
	[CORRECTNESS_ESTIM]		[int] null,
	[TRACE_COUNT]		[int] null,
	[RBW]		[numeric](22,8) null,
	[VBW]		[numeric](22,8) null,
	[BW]		[numeric](22,8) null,
	[LEVEL_SSPECTRUM_DBM]		[varbinary](max) null,
	constraint [PK_RES_STGENERAL] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
