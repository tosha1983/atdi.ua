if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[SUBTASK]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[SUBTASK]
go
create table [ICSC].[SUBTASK] (
	[ID] 	[bigint] identity (1,1) not null,
	[TIME_START]	[datetime] null,
	[TIME_STOP]	[datetime] null,
	[STATUS]	[nvarchar](50) null,
	[INTERVAL]	[int] null,
	[MEAS_TASK_ID]	[bigint] null,
	constraint [PK_SUBTASK] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
