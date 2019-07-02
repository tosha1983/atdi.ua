if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[MEAS_LOCATION_PARAM]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[MEAS_LOCATION_PARAM]
go
create table [ICSC].[MEAS_LOCATION_PARAM] (
	[ID] 	[bigint] identity (1,1) not null,
	[LON]	[numeric](22,8) null,
	[LAT]	[numeric](22,8) null,
	[ASL]	[numeric](22,8) null,
	[MAXDIST]	[numeric](22,8) null,
	[MEAS_TASK_ID]	[bigint] null,
	constraint [PK_MEAS_LOCATION_PARAM] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
