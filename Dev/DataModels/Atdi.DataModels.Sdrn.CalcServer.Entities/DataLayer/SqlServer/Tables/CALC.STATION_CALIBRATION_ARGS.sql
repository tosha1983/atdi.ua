﻿SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [CALC].[STATION_CALIBRATION_ARGS](
	[TASK_ID] [bigint] NOT NULL,
	[CORRELATION_THRESHOLD_HARD] [float] NULL,
	[CORRELATION_THRESHOLD_WEAK] [float] NULL,
	[TRUST_OLD_RESULTS] [float] NULL,
	[USE_MEASUREMENT_SAME_GSID] [bit] NULL,
	[DISTANCE_AROUND_CONTOUR_KM] [int] NULL,
	[MIN_NUMBER_POINT_FOR_CORRELATION] [int] NULL,
	[MIN_RANGE_MEASUREMENTS_DBMKV] [float] NULL,
	[MAX_RANGE_MEASUREMENTS_DBMKV] [float] NULL,
	[CORRELATION_DISTANCE_M] [int] NULL,
	[DELTA_DB] [float] NULL,
	[MAX_ANTENNAS_PATTERN_LOSS_DB] [float] NULL,
	[DETAIL] [bit] NULL,
	[ALTITUDE_STATION] [bit] NULL,
	[SHIFT_ALTITUDE_STATION_MIN_M] [int] NULL,
	[SHIFT_ALTITUDE_STATION_MAX_M] [int] NULL,
	[SHIFT_ALTITUDE_STATION_STEP_M] [int] NULL,
	[MAX_DEVIATION_ALTITUDE_STATION_M] [int] NULL,
	[TILT_STATION] [bit] NULL,
	[SHIFT_TILT_STATION_MIN_DEG] [float] NULL,
	[SHIFT_TILT_STATION_MAX_DEG] [float] NULL,
	[SHIFT_TILT_STATION_STEP_DEG] [float] NULL,
	[MAX_DEVIATION_TILT_STATION_DEG] [float] NULL,
	[AZIMUTH_STATION] [bit] NULL,
	[SHIFT_AZIMUTH_STATION_MIN_DEG] [float] NULL,
	[SHIFT_AZIMUTH_STATION_MAX_DEG] [float] NULL,
	[SHIFT_AZIMUTH_STATION_STEP_DEG] [float] NULL,
	[MAX_DEVIATION_AZIMUTH_STATION_DEG] [float] NULL,
	[COORDINATES_STATION] [bit] NULL,
	[SHIFT_COORDINATES_STATION_M] [int] NULL,
	[SHIFT_COORDINATES_STATION_STEP_M] [int] NULL,
	[MAX_DEVIATION_COORDINATES_STATION_M] [int] NULL,
	[POWER_STATION] [bit] NULL,
	[SHIFT_POWER_STATION_MIN_DB] [float] NULL,
	[SHIFT_POWER_STATION_MAX_DB] [float] NULL,
	[SHIFT_POWER_STATION_STEP_DB] [float] NULL,
	[CASCADE_TUNING] [bit] NULL,
	[NUMBER_CASCADE] [int] NULL,
	[DETAIL_OF_CASCADE] [int] NULL,
	[METHOD] [tinyint] NULL,
	[INFOC_MEAS_RESULTS] [varbinary](max) NULL,
	[STATION_IDS] [varbinary](max) NULL,
 CONSTRAINT [PK_STATION_CALIBRATION_ARGS] PRIMARY KEY CLUSTERED 
(
	[TASK_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
