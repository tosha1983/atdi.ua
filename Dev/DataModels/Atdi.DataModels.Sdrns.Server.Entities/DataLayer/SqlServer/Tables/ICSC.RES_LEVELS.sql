if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[RES_LEVELS]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[RES_LEVELS]
go
create table [ICSC].[RES_LEVELS] (
	[ID] 	[bigint] identity (1,1) not null,
	[VALUE_LVL]	[numeric](22,8) null,
	[STDDEV_LVL]	[numeric](22,8) null,
	[VMIN_LVL]	[numeric](22,8) null,
	[VMMAX_LVL]	[numeric](22,8) null,
	[LIMIT_LVL]	[numeric](22,8) null,
	[OCCUPANCY_LVL]	[numeric](22,8) null,
	[PMIN_LVL]	[numeric](22,8) null,
	[PMAX_LVL]	[numeric](22,8) null,
	[PDIFF_LVL]	[numeric](22,8) null,
	[FREQ_MEAS]	[numeric](22,8) null,
	[VALUE_SPECT]	[numeric](22,8) null,
	[STDDEV_SPECT]	[numeric](22,8) null,
	[VMIN_SPECT]	[numeric](22,8) null,
	[VMMAX_SPECT]	[numeric](22,8) null,
	[LIMIT_SPECT]	[numeric](22,8) null,
	[OCCUPANCY_SPECT]	[numeric](22,8) null,
	[RESMEAS_ID]	[bigint] null,
	constraint [PK_RES_LEVELS] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
