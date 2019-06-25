if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[STATION_SITE]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[STATION_SITE]
go
create table [ICSC].[STATION_SITE] (
	[ID] 	[bigint] identity (1,1) not null,
	[LON]	[numeric](22,8) null,
	[LAT]	[numeric](22,8) null,
	[ADDRES]	[nvarchar](2000) null,
	[REGION]	[nvarchar](50) null,
	constraint [PK_STATION_SITE] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go