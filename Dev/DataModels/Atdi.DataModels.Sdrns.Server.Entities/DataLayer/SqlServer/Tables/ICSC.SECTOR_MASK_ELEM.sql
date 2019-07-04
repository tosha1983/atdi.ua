if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[SECTOR_MASK_ELEM]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[SECTOR_MASK_ELEM]
go
create table [ICSC].[SECTOR_MASK_ELEM] (
	[ID] 	[bigint] identity (1,1) not null,
	[LEVEL]	[float] null,
	[BW]	[float] null,
	constraint [PK_SECTOR_MASK_ELEM] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
