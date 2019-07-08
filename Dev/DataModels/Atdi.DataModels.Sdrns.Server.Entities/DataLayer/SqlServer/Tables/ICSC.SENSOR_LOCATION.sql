if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[SENSOR_LOCATION]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[SENSOR_LOCATION]
go
create table [ICSC].[SENSOR_LOCATION] (
	[ID] 	[bigint] identity (1,1) not null,
	[SENSOR_ID]	[bigint] null,
	[DATA_FROM]	[datetime] null,
	[DATA_TO]	[datetime] null,
	[DATA_CREATED]	[datetime] null,
	[STATUS]	[nvarchar](25) null,
	[LON]	[float] null,
	[LAT]	[float] null,
	[ASL]	[float] null,
	constraint [PK_SENSOR_LOCATION] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
