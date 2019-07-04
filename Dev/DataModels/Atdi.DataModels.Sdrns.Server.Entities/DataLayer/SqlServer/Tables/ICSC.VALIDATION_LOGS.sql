if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[VALIDATION_LOGS]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[VALIDATION_LOGS]
go
create table [ICSC].[VALIDATION_LOGS] (
	[ID] 	[bigint] identity (1,1) not null,
	[EVENT]	[nvarchar](20) null,
	[TABLE_NAME]	[nvarchar](20) null,
	[LCOUNT]	[int] null,
	[INFO]	[nvarchar](250) null,
	[WHO]	[nvarchar](50) null,
	[WHEN]	[datetime] null,
	[ID_MEASTASK]	[bigint] null,
	constraint [PK_VALIDATION_LOGS] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
