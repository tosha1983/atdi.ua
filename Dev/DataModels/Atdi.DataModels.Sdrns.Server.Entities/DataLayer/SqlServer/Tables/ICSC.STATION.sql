if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[STATION]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[STATION]
go
create table [ICSC].[STATION] (
	[ID] 	[bigint] identity (1,1) not null,
	[GLOBAL_SID]	[nvarchar](50) null,
	[STATUS]	[nvarchar](50) null,
	[STANDARD]	[nvarchar](50) null,
	[ID_XBS_MEASTASK]	[bigint] null,
	[ID_STATION]	[bigint] null,
	[START_DATE]	[datetime] null,
	[END_DATE]	[datetime] null,
	[CLOSE_DATE]	[datetime] null,
	[DOZVIL_NAME]	[nvarchar](100) null,
	[ID_OWNERDATA]	[bigint] null,
	[ID_STATIONSITE]	[bigint] null,
	[ID_PERMISSION]	[bigint] null,
	constraint [PK_STATION] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go