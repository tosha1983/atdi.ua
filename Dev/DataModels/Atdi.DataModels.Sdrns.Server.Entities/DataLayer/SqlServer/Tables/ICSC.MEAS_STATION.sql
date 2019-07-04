if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[MEAS_STATION]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[MEAS_STATION]
go
create table [ICSC].[MEAS_STATION] (
	[ID] 	[bigint] identity (1,1) not null,
	[STATION_TYPE]	[nvarchar](50) null,
	[CLIENT_STATION_CODE]	[bigint] null,
	[MEAS_TASK_ID]	[bigint] null,
	constraint [PK_MEAS_STATION] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
