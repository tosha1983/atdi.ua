if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[SENSOR_ANTENNA]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[SENSOR_ANTENNA]
go
create table [ICSC].[SENSOR_ANTENNA] (
	[ID] 	[bigint] identity (1,1) not null,
	[SENSOR_ID]	[bigint] null,
	[CODE]	[nvarchar](50) null,
	[SLEWANG]	[numeric](22,8) null,
	[MANUFACTURER]	[nvarchar](50) null,
	[NAME]	[nvarchar](50) null,
	[TECHID]	[nvarchar](150) null,
	[ANTDIR]	[nvarchar](50) null,
	[HBEAMWIDTH]	[numeric](22,8) null,
	[VBEAMWIDTH]	[numeric](22,8) null,
	[POLARIZATION]	[nvarchar](50) null,
	[USETYPE]	[nvarchar](50) null,
	[CATEGORY]	[nvarchar](50) null,
	[GAINTYPE]	[nvarchar](50) null,
	[GAINMAX]	[numeric](22,8) null,
	[LOWERFREQ]	[numeric](22,8) null,
	[UPPERFREQ]	[numeric](22,8) null,
	[ADDLOSS]	[numeric](22,8) null,
	[XPD]	[numeric](22,8) null,
	[ANTCLASS]	[nvarchar](50) null,
	[REMARK]	[nvarchar](250) null,
	[CUSTTXT1]	[nvarchar](250) null,
	[CUSTDATA1]	[datetime] null,
	[CUSTNBR1]	[numeric](22,8) null,
	constraint [PK_SENSOR_ANTENNA] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
