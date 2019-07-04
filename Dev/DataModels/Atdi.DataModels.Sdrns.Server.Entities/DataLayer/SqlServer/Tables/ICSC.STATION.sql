if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[STATION]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[STATION]
go
create table [ICSC].[STATION] (
	[ID] 	[bigint] identity (1,1) not null,
	[GLOBAL_SID]	[nvarchar](50) null,
	[STATUS]	[nvarchar](50) null,
	[STANDARD]	[nvarchar](50) null,
	[MEAS_TASK]	[bigint] null,
	[CLIENT_STATION_CODE]	[bigint] null,
	[START_DATE]	[datetime] null,
	[END_DATE]	[datetime] null,
	[CLOSE_DATE]	[datetime] null,
	[DOZVIL_NAME]	[nvarchar](100) null,
	[OWNER_DATA_ID]	[bigint] null,
	[STATION_SITE_ID]	[bigint] null,
	[CLIENT_PERMISSION_CODE]	[int] null,
	constraint [PK_STATION] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go