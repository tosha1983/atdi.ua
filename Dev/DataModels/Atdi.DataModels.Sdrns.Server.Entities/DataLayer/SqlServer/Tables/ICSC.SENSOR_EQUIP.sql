if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[SENSOR_EQUIP]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[SENSOR_EQUIP]
go
create table [ICSC].[SENSOR_EQUIP] (
	[ID] 	[bigint] identity (1,1) not null,
	[SENSOR_ID]	[bigint] null,
	[CODE]	[nvarchar](50) null,
	[MANUFACTURER]	[nvarchar](50) null,
	[NAME]	[nvarchar](50) null,
	[FAMILY]	[nvarchar](50) null,
	[TECHID]	[nvarchar](200) null,
	[VERSION]	[nvarchar](50) null,
	[LOWER_FREQ]	[numeric](22,8) null,
	[UPPER_FREQ]	[numeric](22,8) null,
	[RBW_MIN]	[numeric](22,8) null,
	[RBW_MAX]	[numeric](22,8) null,
	[VBW_MIN]	[numeric](22,8) null,
	[VBW_MAX]	[numeric](22,8) null,
	[MOBILITY]	[bit] null,
	[FFT_POINT_MAX]	[numeric](22,8) null,
	[REF_LEVEL_DBM]	[numeric](22,8) null,
	[OPERATION_MODE]	[nvarchar](50) null,
	[TYPE]	[nvarchar](50) null,
	[EQUIP_CLASS]	[nvarchar](50) null,
	[TUNING_STEP]	[numeric](22,8) null,
	[USE_TYPE]	[nvarchar](50) null,
	[CATEGORY]	[nvarchar](50) null,
	[REMARK]	[nvarchar](250) null,
	[CUSTTXT1]	[nvarchar](250) null,
	[CUSTDATA1]	[datetime] null,
	[CUSTNBR1]	[numeric](22,8) null,
	constraint [PK_SENSOR_EQUIP] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
