if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[LINK_ONLINE_MEAS]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[LINK_ONLINE_MEAS]
go
create table [ICSC].[LINK_ONLINE_MEAS] (
	[ID] 	[bigint] identity (1,1) not null,
	[ONLINE_MEAS_ID]	[bigint] null,
	[ONLINE_MEAS_MASTER_ID]	[bigint] null,
	constraint [PK_LINK_ONLINE_MEAS] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
