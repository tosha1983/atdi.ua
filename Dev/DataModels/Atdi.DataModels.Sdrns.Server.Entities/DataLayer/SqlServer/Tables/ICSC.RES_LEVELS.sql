if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[RES_LEVELS]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[RES_LEVELS]
go
create table [ICSC].[RES_LEVELS] (
	[ID] 	[bigint] identity (1,1) not null,
	[VALUE_LVL]	[real] null,
	[STDDEV_LVL]	[float] null,
	[VMIN_LVL]	[real] null,
	[VMMAX_LVL]	[real] null,
	[LIMIT_LVL]	[float] null,
	[OCCUPANCY_LVL]	[float] null,
	[PMIN_LVL]	[float] null,
	[PMAX_LVL]	[float] null,
	[PDIFF_LVL]	[float] null,
	[FREQ_MEAS]	[real] null,
	[VALUE_SPECT]	[real] null,
	[STDDEV_SPECT]	[float] null,
	[VMIN_SPECT]	[float] null,
	[VMMAX_SPECT]	[float] null,
	[LIMIT_SPECT]	[float] null,
	[OCCUPANCY_SPECT]	[real] null,
	[RES_MEAS_ID]	[bigint] not null,
	[LEVEL_MIN_ARR] [float] null,
    [SPECTRUM_OCCUP_ARR]  [varbinary](max) null
	constraint [PK_RES_LEVELS] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
