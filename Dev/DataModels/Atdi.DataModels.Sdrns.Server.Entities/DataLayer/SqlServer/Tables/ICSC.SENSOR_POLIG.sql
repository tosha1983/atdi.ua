if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[SENSOR_POLIG]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[SENSOR_POLIG]
go
create table [ICSC].[SENSOR_POLIG] (
	[ID] 	[bigint] identity (1,1) not null,
	[SENSOR_ID]	[bigint] null,
	[LON]	[float] null,
	[LAT]	[float] null,
	constraint [PK_SENSOR_POLIG] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
