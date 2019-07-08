if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[RES_LOCATION_SENSOR_MEAS]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[RES_LOCATION_SENSOR_MEAS]
go
create table [ICSC].[RES_LOCATION_SENSOR_MEAS] (
	[ID] 	[bigint] identity (1,1) not null,
	[LON]	[float] null,
	[LAT]	[float] null,
	[ASL]	[float] null,
	[AGL]	[float] null,
	[RES_MEAS_ID]	[bigint] not null,
	constraint [PK_RES_LOCATION_SENSOR_MEAS] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
