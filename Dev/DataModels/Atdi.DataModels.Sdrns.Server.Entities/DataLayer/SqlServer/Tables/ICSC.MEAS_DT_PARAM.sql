if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[MEAS_DT_PARAM]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[MEAS_DT_PARAM]
go
create table [ICSC].[MEAS_DT_PARAM] (
	[ID] 	[bigint] identity (1,1) not null,
	[TYPE_MEASUREMENTS]	[nvarchar](50) null,
	[DETECT_TYPE]	[nvarchar](50) null,
	[RF_ATTENUATION]	[numeric](22,8) null,
	[IF_ATTENUATION]	[numeric](22,8) null,
	[MEAS_TIME]	[numeric](22,8) null,
	[DEMOD]	[nvarchar](50) null,
	[PREAMPLIFICATION]	[int] null,
	[MODE]	[nvarchar](50) null,
	[RBW]	[numeric](22,8) null,
	[VBW]	[numeric](22,8) null,
	[ID_MEASTASK]	[bigint] null,
	constraint [PK_MEAS_DT_PARAM] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
