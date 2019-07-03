if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[MEAS_TASK]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[MEAS_TASK]
go
create table [ICSC].[MEAS_TASK] (
	[ID] 	[bigint] identity (1,1) not null,
	[STATUS]	[nvarchar](50) null,
	[ORDER_ID]	[int] null,
	[TYPE]	[nvarchar](50) null,
	[NAME]	[nvarchar](100) null,
	[EXECUTION_MODE]	[nvarchar](50) null,
	[TASK]	[nvarchar](50) null,
	[PRIO]	[int] null,
	[RESULT_TYPE]	[nvarchar](50) null,
	[MAX_TIME_BS]	[int] null,
	[DATE_CREATED]	[datetime] null,
	[CREATED_BY]	[nvarchar](50) null,
	[ID_START]	[nvarchar](50) null,
	[PER_START]	[datetime] null,
	[PER_STOP]	[datetime] null,
	[TIME_START]	[datetime] null,
	[TIME_STOP]	[datetime] null,
	[DAYS]	[nvarchar](250) null,
	[PER_INTERVAL]	[float] null,
	constraint [PK_MEAS_TASK] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
