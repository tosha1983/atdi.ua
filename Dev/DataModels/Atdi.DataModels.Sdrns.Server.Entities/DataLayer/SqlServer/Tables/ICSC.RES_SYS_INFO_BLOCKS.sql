if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[RES_SYS_INFO_BLOCKS]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[RES_SYS_INFO_BLOCKS]
go
create table [ICSC].[RES_SYS_INFO_BLOCKS] (
	[ID] 	[bigint] identity (1,1) not null,
	[DATA]	[varbinary](max) null,
	[TYPE]	[nvarchar](50) null,
	[RES_SYS_INFO_ID]	[bigint] null,
	constraint [PK_RES_SYS_INFO_BLOCKS] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
