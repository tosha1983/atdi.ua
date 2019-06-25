if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[REFERENCE_SIGNAL]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[REFERENCE_SIGNAL]
go
create table [ICSC].[REFERENCE_SIGNAL] (
	[ID] 	[bigint] identity (1,1) not null,
	[FREQ_MHZ]	[numeric](22,8) null,
	[BANDWIDTH_KHZ]	[numeric](22,8) null,
	[LEVELSIGNAL_DBM]	[numeric](22,8) null,
	[REFSITUATION_ID]	[bigint] null,
	[ICSC_ID]	[bigint] null,
	[ICSC_TABLE]	[nvarchar](50) null,
	[LOSS_DB]	[varbinary](max) null,
	[FREQ_KHZ]	[varbinary](max) null,
	constraint [PK_REFERENCE_SIGNAL] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go