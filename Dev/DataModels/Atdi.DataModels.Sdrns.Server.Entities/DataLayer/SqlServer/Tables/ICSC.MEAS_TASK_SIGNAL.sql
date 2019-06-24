if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[MEAS_TASK_SIGNAL]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[MEAS_TASK_SIGNAL]
go
create table [ICSC].[MEAS_TASK_SIGNAL] (
	[ID] 	[bigint] identity (1,1) not null,
	[AUTO_DIV_EMIT]	[int] null,
	[COMPARE_TRACE_JUST_REF_LEVELS]	[int] null,
	[FILTRATION_TRACE]	[int] null,
	[DIFF_MAX_MAX]	[numeric](22,8) null,
	[ALLOW_EXCESS_DB]	[numeric](22,8) null,
	[SIGN_NCOUNT]	[int] null,
	[SIGN_NCHENAL]	[int] null,
	[ID_MEASTASK]	[bigint] null,
	constraint [PK_MEAS_TASK_SIGNAL] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
