if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[MEAS_TASK_SIGNAL]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[MEAS_TASK_SIGNAL]
go
create table [ICSC].[MEAS_TASK_SIGNAL] (
	[ID] 	[bigint] identity (1,1) not null,
	[AUTO_DIV_EMIT]	[bit] null,
	[COMPARE_TRACE_JUST_REF_LEVELS]	[bit] null,
	[FILTRATION_TRACE]	[bit] null,
	[DIFF_MAX_MAX]	[float] null,
	[ALLOW_EXCESS_DB]	[float] null,
	[SIGN_NCOUNT]	[int] null,
	[SIGN_NCHENAL]	[int] null,
	[CORELLATION_ANALIZE]	[bit] null,
	[CHECK_FREQ_CH]	[bit] null,
	[ANALIZE_BY_CH]	[bit] null,
	[ANALIZE_SYSINFO_CH]	[bit] null,
	[MEAS_BW_EMISSION]	[bit] null,
	[CORRELATION_FACTOR]	[float] null,
	[STANDARD]	[nvarchar](50) null,
	[TRIGGER_LEVEL_DBM_HZ]	[float] null,
	[NUMBER_POINT_FOR]	[int] null,
	[WINDOW_BW]	[float] null,
	[DIFF_LEVEL_FOR_BW]	[float] null,
	[NDBLEVEL_DB]	[float] null,
	[NUM_IGNORED_POINTS]	[int] null,
	[MIN_EXCESS_NOSE_LVL]	[float] null,
	[TIME_BETWEEN_WORKTIMES]	[int] null,
	[TYPE_JOIN_SPECTRUM]	[int] null,
	[CROSSING_BW_PERCENT_GOOD]	[float] null,
	[CROSSING_BW_PERCENT_BAD]	[float] null,
	[MAX_FREQ_DEVIATION]	[float] null,
	[MIN_POINT_DETAIL_BW]	[int] null,
	[CHECK_LEVEL_CHANNEL]	[bit] null,
	[COLLECT_EMISSION_INSTRUM_ESTIM] [bit] null,
	[MEAS_TASK_ID]	[bigint] null,
	constraint [PK_MEAS_TASK_SIGNAL] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
