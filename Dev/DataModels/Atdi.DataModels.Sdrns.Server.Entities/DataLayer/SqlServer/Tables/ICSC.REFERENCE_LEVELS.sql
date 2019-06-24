if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[REFERENCE_LEVELS]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[REFERENCE_LEVELS]
go
create table [ICSC].[REFERENCE_LEVELS] (
	[ID] 	[bigint] identity (1,1) not null,
	[STARTFREQ_HZ]	[numeric](22,8) null,
	[STEPFREQ_HZ]	[numeric](22,8) null,
	[REF_LEVELS]	[varbinary](max) null,
	[RESMEAS_ID]	[bigint] null,
	constraint [PK_REFERENCE_LEVELS] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
