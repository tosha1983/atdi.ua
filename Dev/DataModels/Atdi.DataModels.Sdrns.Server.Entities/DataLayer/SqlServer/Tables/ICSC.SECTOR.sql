if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[SECTOR]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[SECTOR]
go
create table [ICSC].[SECTOR] (
	[ID] 	[bigint] identity (1,1) not null,
	[AGL]	[float] null,
	[EIRP]	[float] null,
	[AZIMUTH]	[float] null,
	[BW]	[float] null,
	[CLASS_EMISSION]	[nvarchar](20) null,
	[STATION_ID]	[bigint] null,
	[CLIENT_SECTOR_CODE]	[bigint] null,
	constraint [PK_SECTOR] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
