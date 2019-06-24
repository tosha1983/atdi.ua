if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[MEAS_SUB_TASK]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[MEAS_SUB_TASK]
go
create table [ICSC].[MEAS_SUB_TASK] (
	[ID] 	[bigint] identity (1,1) not null,
	[TIME_START]	[datetime] null,
	[TIME_STOP]	[datetime] null,
	[STATUS]	[nvarchar](50) null,
	[INTERVAL]	[int] null,
	[ID_MEASTASK]	[bigint] null,
	constraint [PK_MEAS_SUB_TASK] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
