if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[UNIT]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[UNIT]
go
create table [ICSC].[UNIT] (
	[ID] 	[bigint] identity (1,1) not null,
	[LON]	[numeric](22,8) null,
	[LAT] 	[numeric](22,8) null,
	[LEVELDBM]	[numeric](22,8) null,
	[CENTRALFREQUENCY] 	[numeric](22,8) null,
	[TIMEOFMEASUREMENTS]	[datetime] null,
	[BW] 	[numeric](22,8) null,
	[IDSTATION] [bigint] null,
	[SPECRUMSTEPS]	[float](10) null,
	[T1] [integer] null,
	[T2] [integer] null,
	[MEASGLOBALSID] 		[nvarchar](250) null,
	constraint [PK_UNIT] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go