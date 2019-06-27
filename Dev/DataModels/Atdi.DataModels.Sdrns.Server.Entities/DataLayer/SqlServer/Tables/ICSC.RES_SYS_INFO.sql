if exists (select * from dbo.sysobjects where id = object_id(N'[ICSC].[RES_SYS_INFO]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
	drop table [ICSC].[RES_SYS_INFO]
go
create table [ICSC].[RES_SYS_INFO] (
	[ID] 	[bigint] identity (1,1) not null,
	[BANDWIDTH]		[float] null,
	[BASEID]	[int] null,
	[BSIC]	[int] null,
	[CHANNELNUMBER]	[int] null,
	[CID]	[int] null,
	[CODE]	[float] null,
	[CTOI]	[float] null,
	[ECI]	[int] null,
	[ENODEBID]	[int] null,
	[FREQ]	[float] null,
	[ICIO]	[float] null,
	[INBAND_POWER]	[float] null,
	[ISCP]	[float] null,
	[LAC]	[int] null,
	[AGL]	[float] null,
	[ASL]	[float] null,
	[LON]	[float] null,
	[LAT]	[float] null,
	[MCC]	[int] null,
	[MNC]	[int] null,
	[NID]	[int] null,
	[PCI]	[int] null,
	[PN]	[int] null,
	[POWER]	[float] null,
	[PTOTAL]	[float] null,
	[RNC]	[int] null,
	[RSCP]	[float] null,
	[RSRP]	[float] null,
	[RSRQ]	[float] null,
	[SC]	[int] null,
	[SID]	[int] null,
	[TAC]	[int] null,
	[TYPECDMAEVDO]	[nvarchar](250) null,
	[UCID]	[int] null,
	[RES_STGENERAL_ID]	[bigint] null,
	constraint [PK_RES_SYS_INFO] primary key clustered ([ID]) on [PRIMARY]  
) on [PRIMARY]
go
