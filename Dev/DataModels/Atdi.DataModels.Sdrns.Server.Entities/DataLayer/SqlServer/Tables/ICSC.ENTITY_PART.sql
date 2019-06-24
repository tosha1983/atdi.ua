if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[ENTITY_PART]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[ENTITY_PART]
go
create table [ICSC].[ENTITY_PART] (
	[ENTITY_ID]	[nvarchar](250) not null,
	[PART_INDEX]	[int] not null,
	[EOF]	[bit] null,
	[CONTENT]	[varbinary](max) null,
	constraint [PK_ENTITY_PART] primary key clustered ([ENTITY_ID], [PART_INDEX]) on [PRIMARY]  
) on [PRIMARY]
go
