if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[LINK_MEAS_STATION]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[LINK_MEAS_STATION]
go
create table [ICSC].[LINK_MEAS_STATION] (
	[ID] 	[bigint] identity (1,1) not null,
	[ID_MEASTASK]	[bigint] null,
	[ID_STATION]	[bigint] null,
	constraint [PK_LINK_MEAS_STATION] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
