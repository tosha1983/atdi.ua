if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[MEAS_FREQ]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[MEAS_FREQ]
go
create table [ICSC].[MEAS_FREQ] (
	[ID] 	[bigint] identity (1,1) not null,
	[FREQ]	[numeric](22,8) null,
	[ID_MEASFREQPARAM]	[bigint] null,
	constraint [PK_MEAS_FREQ] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
