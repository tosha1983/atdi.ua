if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[SECTOR_FREQ]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[SECTOR_FREQ]
go
create table [ICSC].[SECTOR_FREQ] (
	[ID] 	[bigint] identity (1,1) not null,
	[ID_PLAN]	[bigint] null,
	[CHANNAL_NUMBER]	[bigint] null,
	[BW]	[numeric](22,8) null,
	[ID_FREQ]	[bigint] null,
	constraint [PK_SECTOR_FREQ] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
