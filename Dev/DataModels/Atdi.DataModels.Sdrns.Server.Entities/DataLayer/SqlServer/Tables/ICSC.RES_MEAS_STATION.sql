if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[RES_MEAS_STATION]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[RES_MEAS_STATION]
go
create table [ICSC].[RES_MEAS_STATION] (
	[ID] 	          [BIGINT] identity (1,1) not null,
	[GLOBAL_SID]	  [NVARCHAR](250) null,
	[MEAS_GLOBAL_SID] [NVARCHAR](250) null,
	[FREQUENCY]       [DECIMAL](28, 10) not null,
	[STATUS]	      [NVARCHAR](250) null,
	[RES_MEAS_ID]	  [BIGINT] not null,
	[STANDARD]	      [NVARCHAR](50) null,
	[CLIENT_SECTOR_CODE] [int] null,
	[CLIENT_STATION_CODE] [int] null,
	constraint [PK_RES_MEAS_STATION] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
