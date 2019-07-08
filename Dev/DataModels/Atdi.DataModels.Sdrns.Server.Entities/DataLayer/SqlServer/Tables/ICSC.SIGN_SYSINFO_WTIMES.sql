if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[SIGN_SYSINFO_WTIMES]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[SIGN_SYSINFO_WTIMES]
go
create table [ICSC].[SIGN_SYSINFO_WTIMES] (
	[ID] 	[bigint] identity (1,1) not null,
	[START_EMIT] [datetime] not null,
	[STOP_EMIT] [datetime] not null,
	[HIT_COUNT] [int] not null,
	[PERCENT_AVAILABLE] [real] null,
	[SYSINFO_ID] [bigint] not null,
	constraint [PK_SIGN_SYSINFO_WTIMES] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go