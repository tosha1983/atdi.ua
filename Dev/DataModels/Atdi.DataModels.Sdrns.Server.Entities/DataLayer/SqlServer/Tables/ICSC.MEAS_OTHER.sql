if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[MEAS_OTHER]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[MEAS_OTHER]
go
create table [ICSC].[MEAS_OTHER] (
	[ID] 	[bigint] identity (1,1) not null,
	[TYPE_SPECTRUM_OCCUPATION]	[nvarchar](50) null,
	[LEVEL_MIN_OCCUP]	[float] null,
	[NCHENAL]	[int] null,
	[MEAS_TASK_ID]	[bigint] null,
	[SUPP_MULTY_LEVEL] [bit] null
	constraint [PK_MEAS_OTHER] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
