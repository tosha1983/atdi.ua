if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[SENSOR_EQUIP_SENSITIVITES]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[SENSOR_EQUIP_SENSITIVITES]
go
create table [ICSC].[SENSOR_EQUIP_SENSITIVITES] (
	[ID] 	[bigint] identity (1,1) not null,
	[SENSOR_EQUIP_ID]	[bigint] null,
	[FREQ]	[numeric](22,8) null,
	[KTBF]	[numeric](22,8) null,
	[NOISEF]	[numeric](22,8) null,
	[FREQ_STABILITY]	[numeric](22,8) null,
	[ADDLOSS]	[numeric](22,8) null,
	constraint [PK_SENSOR_EQUIP_SENSITIVITES] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
