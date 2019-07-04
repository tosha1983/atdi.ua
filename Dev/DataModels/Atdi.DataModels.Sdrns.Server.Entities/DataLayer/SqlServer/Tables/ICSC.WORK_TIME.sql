if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[WORK_TIME]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[WORK_TIME]
go
create table [ICSC].[WORK_TIME] (
	[ID] 	[bigint] identity (1,1) not null,
	[START_EMIT] 	[datetime] null,
	[STOP_EMIT] 	[datetime] null,
	[HIT_COUNT] 	[integer] null,
	[PERCENT_AVAILABLE]	[real] null,
	[EMITTING_ID] 	[bigint] null,
	constraint [PK_WORK_TIME] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go