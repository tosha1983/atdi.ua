if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[MEAS_SUB_TASK_STATION]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[MEAS_SUB_TASK_STATION]
go
create table [ICSC].[MEAS_SUB_TASK_STATION] (
	[ID] 	[bigint] identity (1,1) not null,
	[STATUS]	[nvarchar](50) null,
	[COUNT]	[int] null,
	[TIME_NEXT_TASK]	[datetime] null,
	[ID_SENSOR]	[int] null,
	[ID_MEASSUBTASK]	[int] null,
	constraint [PK_MEAS_SUB_TASK_STATION] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
