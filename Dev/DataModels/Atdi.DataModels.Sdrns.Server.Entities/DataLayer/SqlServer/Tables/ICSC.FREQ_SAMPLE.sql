if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[FREQ_SAMPLE]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[FREQ_SAMPLE]
go
create table [ICSC].[FREQ_SAMPLE] (
	[ID] 	[bigint] identity (1,1) not null,
	[FREQ_MHZ]	[float] null,
	[LEVEL_DBM]	[float] null,
	[LEVEL_DBMKVM]	[float] null,
	[LEVEL_MIN_DBM]	[float] null,
	[LEVEL_MAX_DBM]	[float] null,
	[OCCUPATION_PT]	[float] null,
	[RES_MEAS_ID]	[bigint] null,
	constraint [PK_FREQ_SAMPLE] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
