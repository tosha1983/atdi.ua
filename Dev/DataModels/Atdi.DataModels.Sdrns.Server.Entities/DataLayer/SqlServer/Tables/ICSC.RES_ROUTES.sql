if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[RES_ROUTES]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[RES_ROUTES]
go
create table [ICSC].[RES_ROUTES] (
	[ID] 	[bigint] identity (1,1) not null,
	[ROUTE_ID]	[nvarchar](250) null,
	[AGL]	[float] null,
	[ASL]	[float] null,
	[START_TIME]	[datetime] null,
	[FINISH_TIME]	[datetime] null,
	[LAT]	[float] null,
	[LON]	[float] null,
	[POINT_STAY_TYPE]	[nvarchar](150) null,
	[RESMEAS_ID]	[bigint] null,
	constraint [PK_RES_ROUTES] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
