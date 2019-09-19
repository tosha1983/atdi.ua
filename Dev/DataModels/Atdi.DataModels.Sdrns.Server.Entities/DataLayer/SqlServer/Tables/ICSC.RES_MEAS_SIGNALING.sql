if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[RES_MEAS_SIGNALING]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[RES_MEAS_SIGNALING]
go
create table [ICSC].[RES_MEAS_SIGNALING] (
	[ID] 	          [BIGINT] identity (1,1) not null,
	[ISSEND]		  [BIT] null,
	[RES_MEAS_ID]	  [BIGINT] not null,
	constraint [PK_RES_MEAS_SIGNALING] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
