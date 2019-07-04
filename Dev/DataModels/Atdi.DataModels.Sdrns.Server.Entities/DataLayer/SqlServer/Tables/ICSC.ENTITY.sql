if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[ENTITY]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[ENTITY]
go
create table [ICSC].[ENTITY] (
	[ID] 	[nvarchar](250) not null,
	[NAME] 	[nvarchar](250) null,
	[DESCRIPTION] 	[nvarchar](4000) null,
	[PARENT_ID] 	[nvarchar](250) null,
	[PARENT_TYPE] 	[nvarchar](250) null,
	[CONTENT_TYPE] 	[nvarchar](250) null,
	[HASH_ALGORITM] 	[nvarchar](250) null,
	[HASH_CODE] 	[nvarchar](4000) null,
	constraint [PK_ENTITY] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
