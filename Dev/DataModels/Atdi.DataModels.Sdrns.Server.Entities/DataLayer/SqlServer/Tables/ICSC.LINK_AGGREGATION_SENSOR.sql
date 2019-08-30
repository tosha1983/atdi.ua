if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[LINK_AGGREGATION_SENSOR]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[LINK_AGGREGATION_SENSOR]
go
create table [ICSC].[LINK_AGGREGATION_SENSOR] (
	[ID] 	[bigint] identity (1,1) not null,
	[SENSOR_ID]	[bigint] null,
	[AGGR_SERVER_INSTANCE]	[nvarchar](150) null,
	constraint [PK_LINK_AGGREGATION_SENSOR] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
