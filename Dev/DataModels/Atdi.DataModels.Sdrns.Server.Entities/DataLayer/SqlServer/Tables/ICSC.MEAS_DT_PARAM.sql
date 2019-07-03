if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[MEAS_DT_PARAM]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[MEAS_DT_PARAM]
go
create table [ICSC].[MEAS_DT_PARAM] (
	[ID] 	[bigint] identity (1,1) not null,
	[TYPE_MEASUREMENTS]	[nvarchar](50) null,
	[DETECT_TYPE]	[nvarchar](50) null,
	[RF_ATTENUATION]	[float] null,
	[IF_ATTENUATION]	[float] null,
	[MEAS_TIME]	[float] null,
	[DEMOD]	[nvarchar](50) null,
	[PREAMPLIFICATION]	[int] null,
	[MODE]	[nvarchar](50) null,
	[RBW]	[float] null,
	[VBW]	[float] null,
	[MEAS_TASK_ID]	[bigint] null,
	constraint [PK_MEAS_DT_PARAM] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
