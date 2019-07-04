if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[RES_LEV_MEAS_ONLINE]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[RES_LEV_MEAS_ONLINE]
go
create table [ICSC].[RES_LEV_MEAS_ONLINE] (
	[ID] 	[bigint] identity (1,1) not null,
	[VALUE]	[float] null,
	[RES_MEAS_ID]	[bigint] not null,
	constraint [PK_RES_LEV_MEAS_ONLINE] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
