if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[REFERENCE_SITUATION]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[REFERENCE_SITUATION]
go
create table [ICSC].[REFERENCE_SITUATION] (
	[ID] 	[bigint] identity (1,1) not null,
	[SENSOR_ID]	[bigint] null,
	[MEAS_TASK_ID]	[bigint] null,
	constraint [PK_REFERENCE_SITUATION] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
