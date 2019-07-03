if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[RES_MEAS_STATION]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[RES_MEAS_STATION]
go
create table [ICSC].[RES_MEAS_STATION] (
	[ID] 	[bigint] identity (1,1) not null,
	[GLOBAL_SID]	[nvarchar](250) null,
	[MEAS_GLOBAL_SID]	[nvarchar](250) null,
	[SECTOR_ID]	[bigint] null,
	[IDSTATION]	[bigint] null,
	[STATUS]	[nvarchar](250) null,
	[RES_MEAS_ID]	[bigint] null,
	[STANDARD]	[nvarchar](50) null,
	[STATION_ID]	[bigint] null,
	constraint [PK_RES_MEAS_STATION] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
