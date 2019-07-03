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
	[AZIMUTH]	[float] null,
	[ELEVATION]	[float] null,
	[AGL]	[float] null,
	[IDSYSARGUS]	[nvarchar](50) null,
	[TYPESENSOR]	[nvarchar](50) null,
	[STEPMEASTIME]	[float] null,
	[RXLOSS]	[float] null,
	[OPHHFR]	[float] null,
	[OPHHTO]	[float] null,
	[OPDAYS]	[nvarchar](50) null,
	[CUSTTXT1]	[nvarchar](50) null,
	[CUSTDATA1]	[datetime] null,
	[CUSTNBR1]	[float] null,
	[DATECREATED]	[datetime] null,
	[CREATEDBY]	[nvarchar](50) null,
	[APIVERSION]	[nvarchar](10) null,
	[TECHID]	[nvarchar](150) null,
	[LASTACTIVITY]	[datetime] null,
	constraint [PK_SENSOR] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
