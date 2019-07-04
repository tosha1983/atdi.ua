if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[RES_STMASKELM]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[RES_STMASKELM]
go
create table [ICSC].[RES_STMASKELM] (
	[ID] 	[bigint] identity (1,1) not null,
	[RES_STGENERAL_ID]	[bigint] not null,
	[BW]		[float] null,
	[LEVEL]		[float] null,
	constraint [PK_RES_STMASKELM] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
