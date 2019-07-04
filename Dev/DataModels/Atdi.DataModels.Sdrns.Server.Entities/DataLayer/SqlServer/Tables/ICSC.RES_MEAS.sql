if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[RES_MEAS]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[RES_MEAS]
go
create table [ICSC].[RES_MEAS] (
	[ID] 	[bigint] identity (1,1) not null,
	[MEAS_SUBTASK_STATION_ID]	[bigint] null,
	[ANTVAL]	[float] null,
	[TIME_MEAS]	[datetime] null,
	[DATA_RANK]	[int] null,
	[N]	[int] null,
	[STATUS]	[nvarchar](50) null,
	[MEAS_SDR_RESULT_SID]	[nvarchar](50) null,
	[TYPE_MEASUREMENTS]	[nvarchar](450) null,
	[SYNCHRONIZED]	[bit] null,
	[START_TIME]	[datetime] null,
	[STOP_TIME]	[datetime] null,
	[SCANS_NUMBER]	[int] null,
	constraint [PK_RES_MEAS] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
