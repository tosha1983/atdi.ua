if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[LINK_SUBTASK_SENSOR_MASTER]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[LINK_SUBTASK_SENSOR_MASTER]
go
create table [ICSC].[LINK_SUBTASK_SENSOR_MASTER] (
	[ID] 	[bigint] identity (1,1) not null,
	[SUBTASK_SENSOR_ID]	[bigint] null,
	[SUBTASK_SENSOR_MASTER_ID]	[bigint] null,
	constraint [PK_LINK_SUBTASK_SENSOR_MASTER] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
