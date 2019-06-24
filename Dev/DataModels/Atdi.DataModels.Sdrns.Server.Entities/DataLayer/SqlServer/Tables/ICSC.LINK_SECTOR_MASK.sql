if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[LINK_SECTOR_MASK]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[LINK_SECTOR_MASK]
go
create table [ICSC].[LINK_SECTOR_MASK] (
	[ID] 	[bigint] identity (1,1) not null,
	[ID_SECTORMASKELEM]	[bigint] null,
	[ID_SECTOR]	[bigint] null,
	constraint [PK_LINK_SECTOR_MASK] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
