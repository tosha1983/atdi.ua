if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[UNIT]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[UNIT]
go
create table [ICSC].[UNIT] (
	[ID] 	[bigint] identity (1,1) not null,
	[LON]	[float] null,
	[LAT] 	[float] null,
	[LEVELDBM]	[float] null,
	[CENTRALFREQUENCY] 	[float] null,
	[TIMEOFMEASUREMENTS]	[datetime] null,
	[BW] 	[float] null,
	[IDSTATION] [bigint] null,
	[SPECRUMSTEPS]	[float] null,
	[T1] [integer] null,
	[T2] [integer] null,
	[MEASGLOBALSID] 		[nvarchar](250) null,
	constraint [PK_UNIT] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go