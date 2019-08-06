if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[RES_STGENERAL]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[RES_STGENERAL]
go
create table [ICSC].[RES_STGENERAL] (
	[ID] 	[bigint] identity (1,1) not null,
	[RES_MEAS_STATION_ID]	[bigint] null,
	[CENTRAL_FREQUENCY]		[float] null,
	[CENTRAL_FREQUENCY_MEAS]		[float] null,
	[DURATION_MEAS]		[float] null,
	[MARKER_INDEX]		[int] null,
	[T1]		[int] null,
	[T2]		[int] null,
	[TIME_START_MEAS]		[datetime] null,
	[TIME_FINISH_MEAS]		[datetime] null,
	[OFFSET_FREQUENCY]		[float] null,
	[SPECRUM_START_FREQ]		[numeric](22,8) null,
	[SPECRUM_STEPS]		[numeric](22,8) null,
	[CORRECTNESS_ESTIM]		[int] null,
	[TRACE_COUNT]		[int] null,
	[RBW]		[float] null,
	[VBW]		[float] null,
	[BW]		[float] null,
	[LEVEL_SSPECTRUM_DBM]		[varbinary](max) null,
	constraint [PK_RES_STGENERAL] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
