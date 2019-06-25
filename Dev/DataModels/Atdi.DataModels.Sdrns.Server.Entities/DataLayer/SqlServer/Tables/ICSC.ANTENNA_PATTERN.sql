if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[ANTENNA_PATTERN]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[ANTENNA_PATTERN]
go
create table [ICSC].[ANTENNA_PATTERN] (
	[ID] 	[bigint] identity (1,1) not null,
	[SENSORANTENNA_ID]	[bigint] null,
	[FREQ]	[numeric](22,8) null,
	[GAIN]	[numeric](22,8) null,
	[DIAGA]	[varchar](1000) null,
	[DIAGH]	[varchar](1000) null,
	[DIAGV]	[varchar](1000) null,
	constraint [PK_ANTENNA_PATTERN] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go