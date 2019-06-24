if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[LINK_RES_SENSOR]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[LINK_RES_SENSOR]
go
create table [ICSC].[LINK_RES_SENSOR] (
	[ID] 	[bigint] identity (1,1) not null,
	[ID_SENSOR]	[bigint] null,
	[ID_RESMEAS_STATION]	[bigint] null,
	constraint [PK_LINK_RES_SENSOR] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
