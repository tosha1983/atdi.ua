if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[SIGN_SYSINFO]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[SIGN_SYSINFO]
go
create table [ICSC].[SIGN_SYSINFO] (
	[ID] 	[bigint] identity (1,1) not null,
	[BANDWIDTH_HZ] [float] null,
	[STANDARD] [nvarchar](20) not null,
	[FREQ_HZ] [float] not null,
	[LEVEL_DBM] [float] null,
	[CID] [int] null,
	[MCC] [int] null,
	[MNC] [int] null,
	[BSIC] [int] null,
	[CHANNEL_NUMBER] [int] null,
	[LAC] [int] null,
	[RNC] [int] null,
	[CTOI] [float] null,
	[POWER] [float] null,
	[EMITTING_ID] [bigint] not null,
	constraint [PK_SIGN_SYSINFO] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go