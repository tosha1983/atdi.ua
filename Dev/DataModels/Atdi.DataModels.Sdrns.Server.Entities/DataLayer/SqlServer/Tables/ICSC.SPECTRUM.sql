if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[SPECTRUM]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[SPECTRUM]
go
create table [ICSC].[SPECTRUM] (
	[ID] 	[bigint] identity (1,1) not null,
	[STARTFREQ_MHZ]	[numeric](28,10) null,
	[STEPFREQ_KHZ]	[numeric](28,10) null,
	[T1]	[int] null,
	[T2]	[int] null,
	[MARKER_INDEX]	[int] null,
	[BW_KHZ]	[numeric](28,10) null,
	[CORRECT_ESTIM]	[int] null,
	[TRACE_COUNT]	[int] null,
	[SIGNALLEVEL_DBM]	[real] null,
	[LEVELS_DBM]	[varbinary](max) null,
	[EMITTING_ID]	[bigint] null,
	[CONTRAVENTION]	[int] null,


	constraint [PK_SPECTRUM] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
