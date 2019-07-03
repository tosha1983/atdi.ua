if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[MEAS_FREQ_PARAM]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[MEAS_FREQ_PARAM]
go
create table [ICSC].[MEAS_FREQ_PARAM] (
	[ID] 	[bigint] identity (1,1) not null,
	[MODE]	[nvarchar](50) null,
	[STEP]	[float] null,
	[RGL]	[float] null,
	[RGU]	[float] null,
	[MEAS_TASK_ID]	[bigint] null,
	constraint [PK_MEAS_FREQ_PARAM] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
