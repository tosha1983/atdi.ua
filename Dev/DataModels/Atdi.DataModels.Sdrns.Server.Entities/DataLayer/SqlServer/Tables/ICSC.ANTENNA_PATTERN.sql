if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[ANTENNA_PATTERN]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[ANTENNA_PATTERN]
go
create table [ICSC].[ANTENNA_PATTERN] (
	[ID] 	[bigint] identity (1,1) not null,
	[SENSOR_ANTENNA_ID]	[bigint] null,
	[FREQ]	[float] null,
	[GAIN]	[float] null,
	[DIAGA]	[varchar](1000) null,
	[DIAGH]	[varchar](1000) null,
	[DIAGV]	[varchar](1000) null,
	constraint [PK_ANTENNA_PATTERN] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
