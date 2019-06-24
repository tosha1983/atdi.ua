if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[RES_LOCATION_SENSOR_MEAS]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[RES_LOCATION_SENSOR_MEAS]
go
create table [ICSC].[RES_LOCATION_SENSOR_MEAS] (
	[ID] 	[bigint] identity (1,1) not null,
	[LON]	[numeric](22,8) null,
	[LAT]	[numeric](22,8) null,
	[ASL]	[numeric](22,8) null,
	[AGL]	[numeric](22,8) null,
	[RESMEAS_ID]	[bigint] null,
	constraint [PK_RES_LOCATION_SENSOR_MEAS] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
