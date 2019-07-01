if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[SECTOR]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[SECTOR]
go
create table [ICSC].[SECTOR] (
	[ID] 	[bigint] identity (1,1) not null,
	[AGL]	[numeric](22,8) null,
	[EIRP]	[numeric](22,8) null,
	[AZIMUTH]	[numeric](22,8) null,
	[BW]	[numeric](22,8) null,
	[CLASS_EMISSION]	[nvarchar](20) null,
	[STATION_ID]	[bigint] null,
	[ID_SECTOR]	[bigint] null,
	constraint [PK_SECTOR] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
