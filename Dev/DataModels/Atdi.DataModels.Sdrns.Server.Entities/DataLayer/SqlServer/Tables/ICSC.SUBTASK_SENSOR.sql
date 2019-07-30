if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[SUBTASK_SENSOR]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[SUBTASK_SENSOR]
go
create table [ICSC].[SUBTASK_SENSOR] (
	[ID] 	[bigint] identity (1,1) not null,
	[STATUS]	[nvarchar](50) null,
	[COUNT]	[int] null,
	[TIME_NEXT_TASK]	[datetime] null,
	[SENSOR_ID]	[int] null,
	[SUBTASK_ID]	[int] null,
	constraint [PK_MSUBTASK_SENSOR] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
