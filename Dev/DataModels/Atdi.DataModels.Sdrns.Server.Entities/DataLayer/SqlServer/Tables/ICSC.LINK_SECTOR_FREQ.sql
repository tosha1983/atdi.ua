if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[LINK_SECTOR_FREQ]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[LINK_SECTOR_FREQ]
go
create table [ICSC].[LINK_SECTOR_FREQ] (
	[ID] 	[bigint] identity (1,1) not null,
	[SECTOR_FREQ_ID]	[bigint] null,
	[SECTOR_ID]	[bigint] null,
	constraint [PK_LINK_SECTOR_FREQ] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
