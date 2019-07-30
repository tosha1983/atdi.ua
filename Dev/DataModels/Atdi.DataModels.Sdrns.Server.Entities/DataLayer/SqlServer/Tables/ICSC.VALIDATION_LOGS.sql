if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[VALIDATION_LOGS]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[VALIDATION_LOGS]
go
create table [ICSC].[VALIDATION_LOGS] (
	[ID] 	[bigint] identity (1,1) not null,
	[TABLE_NAME]	[nvarchar](20) null,
	[INFO]	[nvarchar](250) null,
	[WHEN]	[datetime] null,
	[MESSAGE_ID]	[bigint] null,
	[RES_MEAS_ID]	[bigint] null,
	constraint [PK_VALIDATION_LOGS] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
