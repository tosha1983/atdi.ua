if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[SENSOR]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[SENSOR]
go
create table [ICSC].[SENSOR] (
	[ID] 	[bigint] identity (1,1) not null,
	[SENSORIDENTIFIER_ID]	[bigint] null,
	[STATUS]	[nvarchar](25) null,
	[NAME]	[nvarchar](150) null,
	[ADMINISTRATION]	[nvarchar](50) null,
	[NETWORKID]	[nvarchar](150) null,
	[REMARK]	[nvarchar](250) null,
	[BIUSEDATE]	[datetime] null,
	[EOUSEDATE]	[datetime] null,
	[AZIMUTH]	[numeric](22,8) null,
	[ELEVATION]	[numeric](22,8) null,
	[AGL]	[numeric](22,8) null,
	[IDSYSARGUS]	[nvarchar](50) null,
	[TYPESENSOR]	[nvarchar](50) null,
	[STEPMEASTIME]	[numeric](22,8) null,
	[RXLOSS]	[numeric](22,8) null,
	[OPHHFR]	[numeric](22,8) null,
	[OPHHTO]	[numeric](22,8) null,
	[OPDAYS]	[nvarchar](50) null,
	[CUSTTXT1]	[nvarchar](50) null,
	[CUSTDATA1]	[datetime] null,
	[CUSTNBR1]	[numeric](22,8) null,
	[DATECREATED]	[datetime] null,
	[CREATEDBY]	[nvarchar](50) null,
	[APIVERSION]	[nvarchar](10) null,
	[TECHID]	[nvarchar](150) null,
	[LASTACTIVITY]	[datetime] null,
	constraint [PK_SENSOR] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
