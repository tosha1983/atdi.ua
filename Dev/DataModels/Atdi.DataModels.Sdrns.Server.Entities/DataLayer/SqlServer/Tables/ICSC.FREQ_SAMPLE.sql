if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[FREQ_SAMPLE]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[FREQ_SAMPLE]
go
create table [ICSC].[FREQ_SAMPLE] (
	[ID] 	[bigint] identity (1,1) not null,
	[FREQ_MHZ]	[float](10) null,
	[LEVEL_DBM]	[float](10) null,
	[LEVEL_DBMKVM]	[float](10) null,
	[LEVEL_MIN_DBM]	[float](10) null,
	[LEVEL_MAX_DBM]	[float](10) null,
	[OCCUPATION_PT]	[float](10) null,
	[RESMEAS_ID]	[bigint] null,
	constraint [PK_FREQ_SAMPLE] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
