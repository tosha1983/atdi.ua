if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[EMITTING]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[EMITTING]
go
create table [ICSC].[EMITTING] (
	[ID] 	[bigint] identity (1,1) not null,
	[STARTFREQ_MHZ]	[numeric](22,8) null,
	[STOPFREQ_MHZ]	[numeric](22,8) null,
	[CURRPOWER_DBM]	[numeric](22,8) null,
	[REFLEVEL_DBM]	[numeric](22,8) null,
	[MEANDEVFROM_REF]	[numeric](22,8) null,
	[TRIGGERDEVFROM_REF]	[numeric](22,8) null,
	[ROLL_OFF_ACTOR]	[numeric](22,8) null,
	[STANDARD_BW]	[numeric](22,8) null,
	[RES_MEAS_ID]	[int] null,
	[LEVELS_DISTRIBUTION_LVL]	[varbinary](max) null,
	[LEVELS_DISTRIBUTION_COUNT]	[varbinary](max) null,
	[SENSOR_ID]	[bigint] null,
	[STATION_ID]	[bigint] null,
	[STATION_TABLE_NAME]	[nvarchar](50) null,
	[LOSS_DB]	[varbinary](max) null,
	[FREQ_KHZ]	[varbinary](max) null,
	constraint [PK_EMITTING] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
