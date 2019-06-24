if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[OWNER_DATA]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[OWNER_DATA]
go
create table [ICSC].[OWNER_DATA] (
	[ID] 	[bigint] identity (1,1) not null,
	[OWNER_NAME]	[varchar](100) null,
	[OKPO]	[varchar](50) null,
	[ZIP]	[varchar](50) null,
	[CODE]	[varchar](50) null,
	[ADDRES]	[varchar](2000) null,
	[ID_STATIONDATFORM]	[bigint] null,
	constraint [PK_OWNER_DATA] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
