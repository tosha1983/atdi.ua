﻿SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[CALCRESULTS_REF_SPECTRUM_BY_DRIVE_TESTS_DETAIL](
    [ID] [bigint] IDENTITY(1,1) NOT NULL,
    [ORDER_ID] [bigint]  NOT NULL,
	[RESULT_REF_SPECTRUM_ID] [bigint] NOT NULL,
	[ID_ICSM] [bigint] NOT NULL,
	[TABLE_ICSM_NAME] [nvarchar](50) NOT NULL,
	[ID_SENSOR] [bigint] NOT NULL,
	[GLOBAL_GSID] [nvarchar](50) NOT NULL,
	[FREQ_MHZ] [REAL] NOT NULL,
	[LEVEL_DBM] [REAL] NOT NULL,
	[PERCENT] [REAL] NOT NULL,
    [DATE_MEAS] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_CALCRESULTS_REF_SPECTRUM_BY_DRIVE_TESTS_DETAIL] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



