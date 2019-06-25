if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[MEAS_OTHER]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[MEAS_OTHER]
go
create table [ICSC].[MEAS_OTHER] (
	[ID] 	[bigint] identity (1,1) not null,
	[SW_NUMBER]	[int] null,
	[TYPE_SPECTRUM_SCAN]	[nvarchar](50) null,
	[TYPE_SPECTRUM_OCCUPATION]	[nvarchar](50) null,
	[LEVEL_MIN_OCCUP]	[numeric](22,8) null,
	[NCHENAL]	[int] null,
	[ID_MEASTASK]	[bigint] null,
	constraint [PK_MEAS_OTHER] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go