using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.AppUnits.Sdrn.DeviceServer.Adapters.KTN6841A
{
    public sealed class AgSalLib
    {
        private const String AGSAL_DLL_NAME_X64 = "agSal_x64.dll";
        private const String AGSAL_DLL_NAME_Win32 = "agSal.dll";

        //////// BEGIN agSal.h ////////

        public const int MAX_GEOGRAPHIC_DATUM = 64;
        public const int MAX_SENSOR_NAME = 64;
        public const int MAX_SENSOR_HOSTNAME = 64;
        public const int MAX_APPLICATION_NAME = 64;
        public const int MAX_ERROR_STRING = 64;
        public const int MAX_SENSORS = 512;
        public const int MAX_EVENT_MSG_LEN = 81;
        public const int MAX_UNIT = 32;

        public const int MAX_FILENAME = 256;
        public const int MAX_SESSION_ID = 256;
        public const int MAX_COMMENT = 256;
        public const int MAX_SENSORS_PER_GROUP = 100;
        public const int MAX_GEOLOCATION_SAMPLES = 32768;
        public const int MIN_GEOLOCATION_SAMPLES = 256;
        public const int MAX_SAMPLES_PER_TRANSFER = 32768;

        public const int MAX_FREQEXT_ACTIVATION = 100;
        public const int MAX_FREQEXT_SERIAL = 100;
        public const int MAX_FREQEXT_IP_STRING = 100;

        public enum MiscellaneousConstants
        {
            SAL_DEFAULT_QUEUED_MSGS = 10,   /**< Default number of data messages that will be queued */
            SAL_MIN_LOCATION_IMAGE_PIXELS = 20,   /**< Minimum number of pixels for the height or width of the location image */
            SAL_MAX_LOCATION_IMAGE_PIXELS = 1000, /**< Maximum number of pixels for the height or width of the location image */
            SAL_LOCATION_SPECTRUM_POINTS = 401,  /**< Number of points in the location measurement spectra */
            SAL_LOCATION_CORRELATION_POINTS = 401,  /**< Number of points in the location measurement correlation */

            SAL_MIN_AUDIO_SAMPLES = 64,  /**< Minimum blocksize when using demodulation */
        };

        public enum SensorMode
        {
            None = 0,   /**< No Measurement             */
            Tdoa = 3,   /**< TDOA measurement           */
            Lookback = 4,   /**< TDOA measurement           */
            Default = 100, /**< E3238s or IQ measurement   */
            Error = -1   /**< Error mode                 */
        };

        public enum FreqExtPairStatus
        {
            SAL_FREQEXT_PAIR_FAIL = -1, /**< The sensor could not connect and pair with the specified frequency extender. */
            SAL_FREQEXT_NOT_PAIRED = 0, /**< No frequency extender serial number was given, so the sensor did not try to pair with a frequency extender. */
            SAL_FREQEXT_PAIR_SUCCESS = 1, /**< The sensor successfully connected to and paired with the specified frequency extender. */
        };

        public enum FreqExtUpconvertBandPresent
        {
            SAL_FREQEXT_UPCONVERT_BAND_UNKNOWN = -1,  //Not paired with a frequency extender. Cannot determine if upconvert band is present or not.
            SAL_FREQEXT_UPCONVERT_BAND_NOT_PRESENT = 0,  //The frequency extender does not have the upconvert band option.
            SAL_FREQEXT_UPCONVERT_BAND_PRESENT = 1,  //The frequency extender has the upconvert band option.
        };

        //agSal.h - enum salSTATE_EVENT defined in public unsafe struct IqDataHeader
        //agSal.h - enum salCHANGE defined in public unsafe struct IqDataHeader
        //agSal.h - enum salGPS_INDICATOR defined in public struct Gps

        public enum RfStatusBits
        {
            salLO_UNLOCK = 0x01,  /**< Local oscillator became unlocked */
            salRF_TEMP_HIGH = 0x2,   /**< RF board temperature is high */
            salRF_SETUP_ERROR = 0x4,   /**< error setting up RF hardware */
            salCORRECTIONS_DISABLED = 0x8,   /**< corrections are disabled     */
        };

        public enum TimeSyncAlarmBits
        {
            ClockSet = 1,
            TimeQuestionable = 2,
        };

        public enum DataType
        {
            None = -1,
            Complex32 = 0,
            Complex16,
            COMPLEX_FLOAT32,   /**< 32 bit float complex pairs */
            REAL_INT8,         /**< 8 bit integer real data */
            REAL_INT8_ALAW,    /**< 8 bit integer real data with A-law encoding*/
            REAL_INT8_ULAW,    /**< 8 bit integer real data with A-law encoding*/
            REAL_INT16,        /**< 16 bit integer real data */
            REAL_FLOAT32,      /**< 32 bit float real data */
            REAL_FLOAT32_DBM,  /**< 32 bit float real data in units of dBm */
            NumDataType
        };

        public enum AntennaType
        {
            TestSignal = -4,  /**<  Connect input to internal comb generator (NOTE: due to the high signal level of the internal signal,
                                             the comb generator may cause detectable radiation from an antenna connected to a sensor input) */
            Auto = -3,  /**<  Select antenna as configured by the SMS */
            Unknown = -2,  /**<  Unknown antenna type */
            Termination = -1,  /**< Sensor internal 50 ohm termination */
            Antenna_1 = 0,  /**< Sensor Antenna 1 input */
            Antenna_2 = 1,  /**< Sensor Antenna 2 input */
            Antenna_TestSignal2 = 2,  /**< Connect input to internal comb generator */
            Antenna_Terminated2 = 3,  /**< Sensor internal 50 ohm termination */
            Antenna_Auto2 = 4   /**< Select antenna as configured by the SMS */
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct AntennaAuto
        {
            public double startFreq;        /**< Start frequency for this frequency band */
            public double stopFreq;         /**< Stop frequency for this frequency band */
            public double attenuation;      /**< attenuation for this frequency band */
            public double mixerLevel;       /**< mixerLevel for this frequency band */
            public Int32 preamp;            /**< preamp for this frequency band. Used only in the 750 to 1800 MHz range */
            public AntennaType antenna;     /**< antenna for this frequency band */
        };

        public enum IqMode
        {
            Stream,     /**< Stream continuous data at specified sample rate */
            Block,      /**< Capture specified number of samples */
            NumIqMode
        };

        public enum DataProtocol
        {
            UDP = 0,    /**< Use User Datagram Protocol (stream mode only, packets may be lost or delivered out-of-sequence) */
            TCP = 1,    /**< Use Transmission Control Protocol (stream or block mode) */
            NumProtocol
        };

        public enum Service
        {
            None,        /**< Undefined mode */
            IQ,          /**< IQ data (may be streaming or block mode, see salMode) */
            Diagnostics, /**< Run diagnostic tests */
            NumService
        };

        public enum Localization
        {
            English
        };

        //Same as agSal.h enum salIqCommand.
        public enum MaxBufferAction
        {
            Flush,                /**< Discard data in sensor FIFO and continue acquiring data */
            StopAcquistion        /**< Stop acquiring data, but keep sending data until FIFO is empty */
        };

        public enum salDecimation
        {
            salDECIMATION_NONE,     /**< No decimation supported */
            salDECIMATION_BY_2,     /**< Decimation by two supported */
            salDECIMATION_BY_5,     /**< Decimation by five supported */
            salDECIMATION_VARIABLE, /**< Arbitrary decimation supported */
            salDECIMATION_UNKNOWN   /**< Decimation capability unknown */
        }

        public enum salIeee1588State // Caution: only valid when 1588 is in operation
        {
            PTP_POWERUP = 0,
            PTP_INITIALIZING = 1,
            PTP_FAULTY = 2,
            PTP_DISABLED = 3,
            PTP_LISTENING = 4,
            PTP_PRE_MASTER = 5,
            PTP_MASTER = 6,
            PTP_PASSIVE = 7,
            PTP_UNCALIBRATED = 8,
            PTP_SLAVE = 9
        };

        public enum salTimeAlarms
        {
            TIME_INIT_PROBLEM = 0x01,
            TIME_QUESTIONABLE = 0x02,
            TIME_GPS_ANTENNA = 0x04,
            TIME_GPS_SATELLITE = 0x08,
            TIME_DAC_CAL_ERROR = 0x10,
            TIME_GPS_DRIFT = 0x20,
            TIME_DAC_QUESTIONABLE = 0x40
        };

        public enum salSpectrumAnalyzerAlarms
        {
            SA_INIT_PROBLEM = 0x01
        };

        public enum salSystemAlarms
        {
            SYSTEM_INIT_PROBLEM = 0x001,
            SYSTEM_LEARN_ENV_IN_OPERATION = 0x002,
            SYSTEM_LEARN_ENV_RESULTS_READY = 0x004,
            SYSTEM_PORTAL_COMMUNICATION_PROBLEM = 0x008,
            SYSTEM_FPGA_IMAGE_PROBLEM = 0x010,
            SYSTEM_INCOMPATIBLE_SMS = 0x020,
            SYSTEM_DAC_CAL_IN_OPERATION = 0x040,
            SYSTEM_ADC_DCM_UNLOCK_PROBLEM = 0x080,
            SYSTEM_SDRAM_DCM_UNLOCK_PROBLEM = 0x100,
            SYSTEM_1588_DCM_UNLOCK_PROBLEM = 0x200
        };

        public enum salIntegrityAlarms
        {
            INTEGRITY_FREQ_QUESTIONABLE = 0x001,
            INTEGRITY_TEMP_QUESTIONABLE = 0x002,
            INTEGRITY_TEMPERATURE_SHUTOFF = 0x004,
            INTEGRITY_POWER_DOWN_CONDITION = 0x008,
            INTEGRITY_CALIBRATION_PROBLEM = 0x010,
            INTEGRITY_TAMPER_SWITCH = 0x020,
            INTEGRITY_WATCHDOG_SWITCH = 0x040,
            INTEGRITY_GPS_TX_SWITCH = 0x080,
            INTEGRITY_CPU_OVERLOAD = 0x100
        };

        public enum GPSOperatingState
        {
            GPSOperatingState_unknown = -1,
            GPSOperatingState_timeSyncDisabled = 0,  /**< Time sync disabled  */
            GPSOperatingState_timeSyncEnabled = 1   /**< Time sync enabled   */
        };

        public enum GPSTimingMode
        {
            salGPSTimingMode_uknown = -1,
            salGPSTimingMode_static = 0,            /**< Static (stationary) */
            salGPSTimingMode_mobile = 1             /**< Mobile              */
        };


        public enum SensorAttribute
        {
            COMPLEX_SAMPLE_RATE_MAX,     /**< returns salFloat64 (Hertz) */

            DECIMATION_MAX,              /**< returns salInt32 */
            DECIMATION_TYPE,             /**< returns salDecimation enum */

            FREQ_SPAN_FULL,             /**< returns valid analog freq span at full span salFloat64 (Hertz) */
            FREQ_SPAN_DECIMATING,       /**< returns (sample rate)/(valid freq span) when decimating */

            MEASURABLE_FREQ_MIN,        /**< returns salFloat64 Hertz */
            MEASURABLE_FREQ_MAX,        /**< returns salFloat64 Hertz */

            CENTER_FREQ_MIN,            /**< returns salFloat64 Hertz */
            CENTER_FREQ_MAX,            /**< returns salFloat64 Hertz */
            CENTER_FREQ_RESOLUTION,     /**< returns salFloat64 Hertz */

            RESAMPLER_CAPABILITY,       /**< returns salInt32; value is 1 if resampling is supported, 0 if not */

            ATTENUATION_MIN,            /**< returns salFloat64 */
            ATTENUATION_MAX,            /**< returns salFloat64 */
            ATTENUATION_STEP,           /**< returns salFloat64 */

            PREAMPLIFIER_CAPABILITY,    /**< returns salInt32 */

            IQ_CHANNELS_MAX,           /**< returns salInt32 */

            IQ_SAMPLES_MIN,            /**< returns salInt32 */
            IQ_SAMPLES_MAX_16BIT,      /**< returns salInt32 */
            IQ_SAMPLES_MAX_32BIT,      /**< returns salInt32 */

            SAMPLES_PER_XFER_MAX_TCP, /**< returns salInt32 */
            SAMPLES_PER_XFER_MAX_UDP, /**< returns salInt32 */

            MODEL_NUMBER,               /**< returns (char *) */
            SERIAL_NUMBER,              /**< returns (char *) */

            SENSOR_NAME,               /**< returns character array terminated by NULL; value is the SMS name of the sensor */
            SENSOR_HOSTNAME,            /**< returns character array terminated by NULL; value is the hostname of the sensor */
            DATE,                     /** < returns character array terminated by NULL; value is sensor time and date (e.g. Wed Jul 15 16:14:20 UTC 2009) */
            FFT_POINTS_MIN,           /**< returns salInt32; returns the minimum size of FFT that can be requested from the sensor */
            FFT_POINTS_MAX,           /**< returns salInt32; returns the maximum size of FFT that can be requested from the sensor */
            ATTRIBUTE_DMA_HW,      /**< returns salInt32; if non-zero, this sensor has DMA hardware, which allows higher data transfer rates */

            ATTRIBUTE_LO_ADJ_MODE,         /** [in|out] sets/gets current sensor LO adjustment mode */
            ATTRIBUTE_TIME_SYNC_MODE,      /** [out]  returns salInt32; value is one of the ::salTimeSync enumerated values indicating the current time sysc source */
            ATTRIBUTE_TUNER_FIFO_BYTES,    /** [out] get the size in bytes of the sensor's high speed FIFO (uint64) */
            ATTRIBUTE_DMA_BUFFER_BYTES,    /** [out] get the size in bytes of the sensor's DMA buffer (uint64) */

            ATTRIBUTE_IEEE1588_STATE,      /**< [out] gets current IEEE1588 state (int32, see salIeee1588State) */
            ATTRIBUTE_IEEE1588_DOMAIN,     /**< [in|out] sets/gets current IEEE1588 domain (uint32) */
            ATTRIBUTE_IEEE1588_PRIORITY1,  /**< [in|out] sets/gets current IEEE1588 priority1 (uint32) */
            ATTRIBUTE_IEEE1588_PRIORITY2,  /**< [in|out] sets/gets current IEEE1588 priority2 (uint32) */

            ATTRIBUTE_SENSOR_VARIANCE,     /**< [out] gets current "overall" sensor time-sync variance (double, sec^2) */
            ATTRIBUTE_SENSOR_OFFSET,       /**< [out] gets current "overall" sensor time-sync "offset from master" (double, sec) */
            ATTRIBUTE_IEEE1588_VARIANCE,   /**< [out] gets current sensor IEEE1588 variance (double, sec^2) */
            ATTRIBUTE_IEEE1588_OFFSET,     /**< [out] gets current sensor IEEE1588 "offset from master" (double, sec) */
            ATTRIBUTE_GPS_VARIANCE,        /**< [out] gets current sensor GPS variance (double, sec^2) */
            ATTRIBUTE_GPS_OFFSET,          /**< [out] gets current sensor GPS "offset from GPS module" (double, sec) */
            ATTRIBUTE_FPGA_VARIANCE,       /**< [out] gets current sensor FPGA variance (double, sec^2) */
            ATTRIBUTE_FPGA_OFFSET,         /**< [out] gets current sensor FPGA "offset from PHY or GPS module" (double, sec) */

            ATTRIBUTE_CPU_1MIN,            /**< [out] gets 1 minute CPU load (float) */
            ATTRIBUTE_CPU_5MIN,            /**< [out] gets 5 minute CPU load (float) */
            ATTRIBUTE_CPU_10MIN,           /**< [out] gets 10 minute CPU load (float) */

            ATTRIBUTE_VARIANCE_ALARM_THRESHOLD,  /**< [in|out] sets/gets current variance alarm threshold (double, sec^2) */
            ATTRIBUTE_OFFSET_ALARM_THRESHOLD,    /**< [in|out] sets/gets current time offset alarm threshold (double, sec) */
            ATTRIBUTE_CPU_ALARM_THRESHOLD,       /**< [in|out] sets/gets CPU alarm threshold (float, 0 to 1) */

            ATTRIBUTE_RF_TEMPERATURE,			/**< [out] RF Board temperature (double, deg C) */
            ATTRIBUTE_DIG_TEMPERATURE,			/**< [out] Digital Board temperature (double, deg C) */

            ATTRIBUTE_UP_TIME,					/**< [out] Up-time (float, sec) */
            ATTRIBUTE_IDLE_TIME,				/**< [out] Idle-time (float, sec) */

            ATTRIBUTE_TIME_ALARMS,				/**< [out] Time alarm bitfield (unit32, see salTimeAlarms) */
            ATTRIBUTE_SA_ALARMS,				/**< [out] Spectrum Analyzer alarm bitfield (unit32, see salSpectrumAnalyzerAlarms) */
            ATTRIBUTE_SYS_ALARMS,				/**< [out] System alarm bitfield (unit32, see salSystemAlarms) */
            ATTRIBUTE_INTEG_ALARMS,             /**< [out] Integrity alarm bitfield (unit32, see salIntegrityAlarms) */

            ATTRIBUTE_RAM_TOTAL_KIB,             /**< [out] get the total RAM installed on the sensor (salUInt64, KiB (1024 bytes)) */
            ATTRIBUTE_RAM_FREE_KIB,              /**< [out] get the free RAM on the sensor (salUInt64, KiB (1024 bytes)) */
            ATTRIBUTE_JFFS2_FLASH_TOTAL_KIB,     /**< [out] get the total flash space mounted on the /jffs2 (salUInt64, KiB (1024 bytes)) */
            ATTRIBUTE_JFFS2_FLASH_FREE_KIB,      /**< [out] get the free flash space mounted on the /jffs2 (salUInt64, KiB (1024 bytes)) */
            ATTRIBUTE_DATA_FLASH_TOTAL_KIB,      /**< [out] get the total flash space mounted on the /data. 0 if none is mounted on /data. (salUInt64, KiB (1024 bytes)) */
            ATTRIBUTE_DATA_FLASH_FREE_KIB,       /**< [out] get the free flash space mounted on the /data. 0 if none is mounted on /data. (salUInt64, KiB (1024 bytes)) */

            ATTRIBUTE_GPS_OPERATING_STATE,		/**< [out] gets the GPS opearting state. See GPSOperatingState enumerated values */
            ATTRIBUTE_GPS_TIMING_MODE,          /**< [out] gets the GPS timing mode. See GPSTimingMode enumerated values */

            NUM_ATTRIBUTES
        };

        public enum IqAttribute
        {
            DELAY_MAX_SECONDS,      /**< returns salFloat64; value is maximum amount of data (in seconds) that will be buffered in the sensor before the data is discarded */
            DELAY_SECONDS,
            LAST_SEQUENCE_NUMBER,   /**< returns shInt32 */
            STATE,                  /**< returns shIqStates */
            USER_STREAM_ID,         /**< returns shInt32 */

            XFER_SAMPLES_MAX,       /**< returns salInt32 */
            XFER_BYTES_MAX,         /**< returns salInt32 */

            NUM_SAMPLES,            /**< returns salInt32  */
            SAMPLE_RATE,            /**< returns salFloat64 */
            CENTER_FREQUENCY,       /**< returns salFloat64 */
            ATTENUATION,            /**< returns salFloat64 */
            PREAMP,                 /**< returns salInt32  */
            ANTENNA,                /**< returns salInt32 */
            IQ_MODE,                /**< returns salInt32  */
            DATA_TYPE,              /**< returns salInt32  */

            NUM_ATTRIBUTES
        };

        public enum SensorEventType
        {
            Shutdown,           /**< The sensor powered off */
            Disconnected,       /**< The connection to the sensor was terminated by another application */
            CommunicationError, /**< The sensor failed to send or read a message */
            Unknown,            /**< An unknown event occurred */
        };

        public enum SalError
        {
            SAL_ERR_NONE = 0,	                    /**< No Error */
            SAL_ERR_NOT_IMPLEMENTED = -1,	        /**< This functionality is not implemented yet. */
            SAL_ERR_UNKNOWN = -2,	                /**< Error of unspecified type */
            SAL_ERR_BUSY = -3,	                    /**< The system is busy */
            SAL_ERR_TRUNCATED = -4,	                /**< Unspecified error */
            SAL_ERR_ABORTED = -5,	                /**< The measurement was aborted */
            SAL_ERR_RPC_NORESULT = -6,	            /**< The server accepted the call but returned no result */
            SAL_ERR_RPC_FAIL = -7,	                /**< The RPC call to the server failed completely */
            SAL_ERR_PARAM = -8,	                    /**< Incorrect parameter in call. */
            SAL_ERR_MEAS_IN_PROGRESS = -9,	        /**< Another measurement is currently in progress */
            SAL_ERR_NO_RESULT = -10,	            /**< No result was returned */
            SAL_ERR_SENSOR_NAME_EXISTS = -11,	    /**< The sensor name specified already exists */
            SAL_ERR_INVALID_CAL_FILE = -12,      	/**< The calibration file has an invalid format */
            SAL_ERR_NO_SUCH_ANTENNAPATH = -13,   	/**< The antenna path specified does not exist */
            SAL_ERR_INVALID_SENSOR_NAME = -14,	    /**< The sensor name specified does not exist */
            SAL_ERR_INVALID_MEASUREMENT_ID = -15,	/**< The given measurement ID is not valid */
            SAL_ERR_INVALID_REQUEST = -16,	        /**< Internal system error */
            SAL_ERR_MISSING_MAP_PARAMETERS = -17,	/**< You need to specify map coordinates */
            SAL_ERR_TOO_LATE = -18,	                /**< The measurement arrived at the sensor too late */
            SAL_ERR_HTTP_TRANSPORT = -19,	        /**< An HTTP error occurred when trying to talk to the sensors */
            SAL_ERR_NO_SENSORS = -20,	            /**< No sensors available for measurement */
            SAL_ERR_NOT_ENOUGH_TIMESERIES = -21,	/**< Not enough timeseries im measurement */
            SAL_ERR_NATIVE = -22,	                /**< Error in native code */
            SAL_ERR_BAD_SENSOR_LOCATION = -23,	    /**< Invalid sensor location */
            SAL_ERR_DATA_CHANNEL_OPEN = -24,        /**< Data Channel already open */
            SAL_ERR_DATA_CHANNEL_NOT_OPEN = -25,    /**< Data Channel not open */

            SAL_ERR_SOCKET_ERROR = -26,			    /**< Socket error */
            SAL_ERR_SENSOR_NOT_CONNECTED = -27,     /**< Sensor not connected */
            SAL_ERR_NO_DATA_AVAILABLE = -28,        /**< No data available */
            SAL_ERR_NO_SMS = -29,                   /**< NO SMS at that address */
            SAL_ERR_BUFFER_TOO_SMALL = -30,         /**< User data buffer too small for data > */
            SAL_ERR_DIAGNOSTIC = -31,				/**< Some Diagnostic Error */
            SAL_ERR_QUEUE_EMPTY = -32,              /**< No more msgs in the Error Queue */
            SAL_ERR_WRONG_MODE = -33,               /**< Sensor is in the wrong measurement mode */
            SAL_ERR_MEMORY = -34,                   /**< Could not allocate memory */
            SAL_ERR_INVALID_HANDLE = -35,           /**< User supplied handle was invalid */
            SAL_ERR_SENSOR_CONNECT = -36,           /**< Attempt to connect to sensor failed */
            SAL_ERR_SMS_NO_TOKEN = -37,             /**< SMS refused to issue token */
            SAL_ERR_COMMAND_FAILED = -38,           /**< Sensor command failed */
            SAL_ERR_NO_LOCATE_HISTORY = -39,        /**< Could not get locate result history */
            SAL_ERR_TIMEOUT = -40,                  /**< Measurement timed out */
            SAL_ERR_IMAGE_SIZE = -41,               /**< Requested location image size too big */
            SAL_ERR_INVALID_ANTENNA = -42,          /**< Requested antenna type not valid */
            SAL_ERR_STRING_TOO_LONG = -43,          /**< Input string too long */
            SAL_ERR_INVALID_TIMEOUT = -44,          /**< Requested timeout value not valid */
            SAL_ERR_INVALID_SENSOR_INDEX = -45,     /**< Sensor index not valid */
            SAL_ERR_INVALID_TRIGGER_TYPE = -46,     /**< Requested trigger type not valid */
            SAL_ERR_INVALID_DOPPLER_COMP = -47,     /**< Requested doppler compensation not valid */
            SAL_ERR_NUM_SENSORS = -48,              /**< Maximum number of sensors already added to group */
            SAL_ERR_EMPTY_GROUP = -49,              /**< Operation not valid on empty sensor group */
            SAL_ERR_HANDLE_IN_USE = -50,            /**< Handle can not be closed because it is in use */

            SAL_ERR_DATA_TYPE = -52,                /**< Requested salDataType not valid for measurement */
            SAL_ERR_SENSOR_SERVER = -53,            /**< Sensor measurement server communications error */
            SAL_ERR_TIME_NOT_IN_STREAM = -54,       /**< Request for time data that is not in sensor memory */
            SAL_ERR_FREQ_NOT_IN_STREAM = -55,       /**< Requested frequency is outside of current tuner range */
            SAL_ERR_NOT_IN_LOOKBACK = -56,          /**< Measurement requires sensor in lookback mode */
            SAL_ERR_AUTHORIZATION = -57,            /**< Error authorizing current application and user on the sensor */
            SAL_ERR_TUNER_LOCK = -58,               /**< Could not obtain a lock on tuner resource */
            SAL_ERR_FFT_LOCK = -59,                 /**< Could not obtain a lock on FFT resource */
            SAL_ERR_LOCK_FAILED = -60,              /**< Could not obtain a lock on requested resource */
            SAL_ERR_SENSOR_DATA_END = -61,          /**< RF Sensor data stream terminated unexpectedly */
            SAL_ERR_INVALID_SPAN = -62,             /**< Requested measurement span is not valid */
            SAL_ERR_INVALID_ALGORITHM = -63,        /**< Requested geolocation algorithm is not available */
            SAL_ERR_LICENSE = -64,                  /**< License error */
            SAL_ERR_LIST_END = -65,                 /**< End of list reached */
            SAL_ERR_MEAS_FAILED = -66,              /**< The measurement failed of timed out with no results */
            SAL_ERR_EMBEDDED = -67,                 /**< Function not supported in embedded apps. */
            SAL_ERR_SMS_EXCEPTION = -68,            /**< Exception in SMS processing */
            SAL_SDRAM_OVERFLOW = -69,               /**< SDRAM overflow in sensor */
            SAL_NO_DMA_BUFFER = -70,				/**< NO free DMA Buffers in sensor */
            SAL_DMA_FIFO_UNDERFLOW = -71,           /**< DMA FIFO Underflow in sensor */
            SAL_FFT_SETUP_ERROR = -72,              /**< FFT Setup Error */
            SAL_TRIGGER_TIMEOUT = -73,				/**< Measurement trigger timeout in sensor */
            SAL_NO_STREAM_DATA = -74,				/**< Measurement stream problem in sensor */
            SAL_DATA_AVAIL_TIMEOUT = -75,			/**< Measurement data available timeout in sensor */

            SAL_TUNER_NOT_STREAMING = -76,          /**< Tuner not streaming in sensor */
            SAL_SERVICE_EXISTS = -77,               /**< Service already exists. (See salAppAddService()) */
            SAL_ERR_NUM = -77                       /** this should ALWAYS EQUAL the last valid error message */
        };

        public enum IqState
        {
            None,
            Init,		       /**< initial state (idle)      */
            Stopped,	       /**< stopped (idle)            */
            Streaming,		   /**< streaming, real time      */
            StreamingTransfer, /**< acquistion stopped, but there is still data in sensor memory */
            WaitingForTrigger, /**< waiting for the trigger   */
            BlockAcquisition,  /**< acquiring a block of data */
            BlockTransfer,     /**< transfering a block       */
            NumIqState
        };

        //Same as agSal.h enum salMaxBufferAction.
        public enum IqCommand
        {
            FlushBuffer,
            StopAcquisition
        };

        public enum TriggerType
        {
            AbsoluteTime = 3,
            RelativeTime = 0,  /**< trigger after a specified time has elapsed */
            RelativeLevel = 1,  /**< trigger when signal exceeds a threshold by a specified amount */
            AbsoluteLevel = 2,  /**< trigger when signal exceeds specified level */
        };

        // FIXME: need event handler mechanism for Alloy
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct SensorEvent
        {
            public SensorEventType type;
            public UInt32 time;       // sec utc
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
            public string message;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
            public string name;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IqParameters
        {
            public UInt32 numSamples;               /**< Number of points (in)*/
            public double sampleRate;               /**< Sample rate in Hz (in)*/
            public double centerFrequency;          /**< Center frequency in Hz (in)*/
            public double attenuation;              /**< IF attenuation in dB (in)*/
            public UInt32 preamp;                   /**< pre-amp. Used only in the 750 to 1800 MHz range */
            public AntennaType antenna;             /**< which antenna */
            public IqMode iqMode;                   /**< IQ Streaming or IQ Block Mode */
            public DataType dataType;               /**< complex or real */
            public double maxBufferSeconds;         /**< Size (in bytes) of the Node RAM buffer for IQ block mode */
            public MaxBufferAction maxBufferAction; /**< For real-time streaming, what to do if maxBufferSeconds is exceeded */
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct Location
        {
            public double latitude;         /**< In fractional degrees, southern latitudes are negative numbers */
            public double longitude;        /**< In fractional degrees, western longitudes are negative numbers */
            public double elevation;        /**< In meters  */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_GEOGRAPHIC_DATUM)]
            public string geographicDatum; /**<  */
            public double x_offset;         /**< in meters  */
            public double y_offset;         /**< in meters  */
            public double z_offset;         /**< in meters  */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_UNIT)]
            public string xyz_unit;
            public UInt32 latitudeType;
            public UInt32 longitudeType;
            public double rotation;         /**< In degrees, counter-clockwise from Longitude */

            public override string ToString()
            {
                return latitude.ToString("0.000000000") + ", " + longitude.ToString("0.000000000");
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct SensorStatus
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_NAME)]
            public string name;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_HOSTNAME)]
            public string hostName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_HOSTNAME)]
            public string userHostName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_APPLICATION_NAME)]
            public string userApplicationName;
            public UInt32 lastHeardFrom;
            public Location location;
            public Int16 currentMode;
            public Int16 timeSyncAlarms;
            public Int16 systemAlarms;
            public Int16 integrityAlarms;

            public enum TimeSyncBits
            {
                ClockNotSet = 1,
                TimeQuestionable = 2,
            };

            public enum SystemAlarmBits
            {
                OperationSuspended = 1,
                LearnEnvironmentInProgress = 2,
                LearnEnvironmentFailed = 4,
                SmsCommunicationQuestionable = 8,
                FpgaUsingBackupImage = 16,
            };

            public enum IntegrityBits
            {
                FrequencyQuestionable = 1,
                TemperatureQuestionable = 2,
                TemperatureShutdownPending = 4,
                RfPoweredDown = 8,
                CalibrationQuestionable = 16,
                CaseOpen = 32,
                WatchDogDisabled = 64,
                GpxTxDisabled = 128,
            };

            public bool IsFrequencyQuestionable
            {
                get { return (integrityAlarms & (int)IntegrityBits.FrequencyQuestionable) != 0; }
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct SensorStatus2
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_NAME)]
            public string name;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_HOSTNAME)]
            public string hostName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_HOSTNAME)]
            public string ipAddress;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_HOSTNAME)]
            public string userHostName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_APPLICATION_NAME)]
            public string userApplicationName;
            public UInt32 lastHeardFrom;
            public Location location;
            public Int16 currentMode;
            public Int16 timeSyncAlarms;
            public Int16 systemAlarms;
            public Int16 integrityAlarms;

            public enum TimeSyncBits
            {
                ClockNotSet = 1,
                TimeQuestionable = 2,
            };

            public enum SystemAlarmBits
            {
                OperationSuspended = 1,
                LearnEnvironmentInProgress = 2,
                LearnEnvironmentFailed = 4,
                SmsCommunicationQuestionable = 8,
                FpgaUsingBackupImage = 16,
            };

            public enum IntegrityBits
            {
                FrequencyQuestionable = 1,
                TemperatureQuestionable = 2,
                TemperatureShutdownPending = 4,
                RfPoweredDown = 8,
                CalibrationQuestionable = 16,
                CaseOpen = 32,
                WatchDogDisabled = 64,
                GpxTxDisabled = 128,
            };

            public bool IsFrequencyQuestionable
            {
                get { return (integrityAlarms & (int)IntegrityBits.FrequencyQuestionable) != 0; }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Gps
        {
            public UInt32 version;                    // GPS descriptor version 
            public UInt32 reserved1;                  // Reserved for future use
            public UInt32 reserved2;                  // Reserved for future use
            public UInt32 valid;                      // bitmap indicates which GPS values are valid
            public UInt32 changeIndicator;            // bitmap indicates which GPS values have changed

            public double latitude;                   // Sensor latitude in degrees; -90 (South) to +90 (North)
            public double longitude;                  // Sensor longitude in degrees; -180 (West) to +180 (East)
            public double altitude;                   // Sensor altitude in meters 
            public double speed;                      // Sensor speed in meters/second over ground
            public double heading;                    // Sensor orientation with respect to true North in decimal degrees 
            public double trackAngle;                 // Sensor direction of travel with respect to true North in decimal degrees
            public double magneticVariation;          // Magnetic North variation from true North in decimal degrees; -180 (West) to +180 (East)

            //Same as agSal.h enum salGPS_INDICATOR.
            public const int GPS_INDICATOR_LATITUDE = 0x01;
            public const int GPS_INDICATOR_LONGITUDE = 0x02;
            public const int GPS_INDICATOR_ALTITUDE = 0x04;
            public const int GPS_INDICATOR_SPEED = 0x08;
            public const int GPS_INDICATOR_HEADING = 0x10;
            public const int GPS_INDICATOR_TRACK_ANGLE = 0x20;
            public const int GPS_INDICATOR_MAGNETIC_VARIATION = 0x40;

            public bool IsLatitudeValid
            {
                get { return (valid & GPS_INDICATOR_LATITUDE) != 0; }
            }
            public bool IsLongitudeValid
            {
                get { return (valid & GPS_INDICATOR_LONGITUDE) != 0; }
            }
            public bool IsAltitudeValid
            {
                get { return (valid & GPS_INDICATOR_ALTITUDE) != 0; }
            }
            public bool IsSpeedValid
            {
                get { return (valid & GPS_INDICATOR_SPEED) != 0; }
            }
            public bool IsHeadingValid
            {
                get { return (valid & GPS_INDICATOR_HEADING) != 0; }
            }
            public bool IsTrackAngleValid
            {
                get { return (valid & GPS_INDICATOR_TRACK_ANGLE) != 0; }
            }
            public bool IsMagneticVariationValid
            {
                get { return (valid & GPS_INDICATOR_MAGNETIC_VARIATION) != 0; }
            }
            public bool LatitudeChanged
            {
                get { return (changeIndicator & GPS_INDICATOR_LATITUDE) != 0; }
            }
            public bool LongitudeChanged
            {
                get { return (changeIndicator & GPS_INDICATOR_LONGITUDE) != 0; }
            }
            public bool AltitudeChanged
            {
                get { return (changeIndicator & GPS_INDICATOR_ALTITUDE) != 0; }
            }
            public bool SpeedChanged
            {
                get { return (changeIndicator & GPS_INDICATOR_SPEED) != 0; }
            }
            public bool HeadingChanged
            {
                get { return (changeIndicator & GPS_INDICATOR_HEADING) != 0; }
            }
            public bool TrackAngleChanged
            {
                get { return (changeIndicator & GPS_INDICATOR_TRACK_ANGLE) != 0; }
            }
            public bool MagneticVariationChanged
            {
                get { return (changeIndicator & GPS_INDICATOR_MAGNETIC_VARIATION) != 0; }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct IqDataHeader
        {
            public UInt32 version;
            public UInt32 sequenceNumber;
            public UInt32 numSamples;
            public DataType dataType;

            public IntPtr streamIdentifier;
            public IntPtr userWorkspace;
            public UInt32 userStreamId;

            public UInt32 timestampSec;
            public UInt32 timestampNsec;

            public UInt32 stateEventIndicator;

            public UInt32 rfStatus;
            public UInt32 changeIndicator;

            public AntennaType antenna;
            public double bandwidth;
            public double centerFrequency;
            public double scaleToVolts;
            public double dataFullScale;
            public double attenuation;
            public double sampleRate;
            public double temperature;

            public Gps gps;

            public IntPtr pSamples;

            // IQ stateEventIndicator bit values (same as agSal.h enum salSTATE_EVENT)
            public const int salSTATE_SAMPLE_LOSS = 0x01;
            public const int salSTATE_OVER_RANGE = 0x02;
            public const int salSTATE_BLOCK_MEASUREMENT_ERROR = 0x04;
            public const int salSTATE_SETUP_NOT_USER = 0x08;
            public const int salSTATE_LAST_BLOCK = 0x10;
            public const int salSTATE_REF_OSC_ADJUSTED = 0x20;
            public const int salSTATE_CPU_OVERLOAD = 0x100;
            public const int salSTATE_SYNC_PROBLEM = 0x200;

            // changeIndicator bit values (same as agSal.h enum salCHANGE)
            public const int salCHANGE_BANDWIDTH = 0x01;
            public const int salCHANGE_RF_REF_FREQ = 0x02;
            public const int salCHANGE_SCALE_TO_VOLTS = 0x04;
            public const int salCHANGE_DATA_FULL_SCALE = 0x08;
            public const int salCHANGE_ATTENUATION = 0x10;
            public const int salCHANGE_SAMPLE_RATE = 0x20;
            public const int salCHANGE_TEMPERATURE = 0x40;
            public const int salCHANGE_ANTENNA = 0x80;


            public DateTime TimeUtc
            {
                get { return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(timestampSec); }
            }

            public DateTime TimeLocal
            {
                get { return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(timestampSec).ToLocalTime(); }
            }
            public double TimeFractionalSeconds
            {
                get { return (double)timestampNsec / 1e9; }
            }

            public bool IsLastBlock
            {
                get { return (stateEventIndicator & salSTATE_LAST_BLOCK) != 0; }
            }
            public bool IsOverRange
            {
                get { return (stateEventIndicator & IqDataHeader.salSTATE_OVER_RANGE) != 0; }

            }
            public bool IsMeasurementError
            {
                get { return (stateEventIndicator & salSTATE_BLOCK_MEASUREMENT_ERROR) != 0; }
            }
            public bool IsSetupNotUser
            {
                get { return (stateEventIndicator & salSWEEP_DATA_SETUP_NOT_USER) != 0; }
            }
            public bool IsSampleLoss
            {
                get { return (stateEventIndicator & salSTATE_SAMPLE_LOSS) != 0; }
            }
            public bool CenterFrequencyChanged
            {
                get { return (changeIndicator & salCHANGE_RF_REF_FREQ) != 0; }
            }
            public bool BandwidthChanged
            {
                get { return (changeIndicator & salCHANGE_BANDWIDTH) != 0; }
            }
            public bool ScaleToVoltsChanged
            {
                get { return (changeIndicator & salCHANGE_SCALE_TO_VOLTS) != 0; }
            }
            public bool DataFullScaleChanged
            {
                get { return (changeIndicator & salCHANGE_DATA_FULL_SCALE) != 0; }
            }
            public bool AttenuationChanged
            {
                get { return (changeIndicator & salCHANGE_ATTENUATION) != 0; }
            }
            public bool AntennaChanged
            {
                get { return (changeIndicator & salCHANGE_ANTENNA) != 0; }
            }
            public bool SampleRateChanged
            {
                get { return (changeIndicator & salCHANGE_SAMPLE_RATE) != 0; }
            }
            public bool TemperatureChanged
            {
                get { return (changeIndicator & salCHANGE_TEMPERATURE) != 0; }
            }

            public UInt32 BytesPerSample
            {
                get
                {
                    switch (dataType)
                    {
                        case DataType.Complex16: return 4;
                        case DataType.Complex32: return 8;
                        default: return 0;
                    }
                }
            }

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IqArg
        {
            public UInt32 reserved1;
            public UInt32 reserved2;
            public UInt32 reserved3;
            public UInt32 id;  // user-defined ID
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SensorCapabilities
        {
            public Int32 supportsFrequencyData;  /**< if non-zero, sensor supports the frequency data interface */
            public Int32 supportsTimeData;       /**< if non-zero, sensor supports the time data interface */
            public Int32 fftMinBlocksize;        /**< the minimum FFT blocksize for this sensor */
            public Int32 fftMaxBlocksize;        /**< the maximum FFT blocksize for this sensor */
            public Int32 maxDecimations;         /**< maximum number of sample rate decimations */
            public Int32 hasDmaHardware;         /**< if non-zero, this sensor has DMA hardware, which allows higher data transfer rates */
            public UInt64 rfFifoBytes;
            public UInt64 dmaBufferBytes;
            private Int32 reserved1;             /**< reserved for future use */
            private Int32 reserved2;             /**< reserved for future use */
            private Int32 reserved3;             /**< reserved for future use */
            public double maxSampleRate;         /**< the maximum FFT blocksize for this sensor */
            public double maxSpan;               /**< the maximum valid measurement span for this sensor */
            public double sampleRateToSpanRatio; /**< the ratio of sample rate to valid frequency span */
            public double minFrequency;          /**< the minumum measurable frequency */
            public double maxFrequency;          /**< the maximum measurable frequency */
            public double fReserved1;            /**< reserved for future use */
            private double fReserved2;           /**< reserved for future use */
            private double fReserved3;           /**< reserved for future use */
            private double fReserved4;           /**< reserved for future use */
            private double fReserved5;           /**< reserved for future use */
            private double fReserved6;           /**< reserved for future use */
            private double fReserved7;           /**< reserved for future use */
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct SensorCapabilities2
        {
            public Int32 supportsFrequencyData;  /**< if non-zero, sensor supports the frequency data interface */
            public Int32 supportsTimeData;       /**< if non-zero, sensor supports the time data interface */
            public Int32 fftMinBlocksize;        /**< the minimum FFT blocksize for this sensor */
            public Int32 fftMaxBlocksize;        /**< the maximum FFT blocksize for this sensor */
            public Int32 maxDecimations;         /**< maximum number of sample rate decimations */
            public Int32 hasDmaHardware;         /**< if non-zero, this sensor has DMA hardware, which allows higher data transfer rates */
            public UInt32 maxDDCsAvailable;      /**< the maximum number of Digital Downconverter Channels this sensor supports. */
            public UInt64 rfFifoBytes;
            public UInt64 dmaBufferBytes;
            public double maxSampleRate;         /**< the maximum FFT blocksize for this sensor */
            public double maxSpan;               /**< the maximum valid measurement span for this sensor */
            public double sampleRateToSpanRatio; /**< the ratio of sample rate to valid frequency span */
            public double minFrequency;          /**< the minumum measurable frequency */
            public double maxFrequency;          /**< the maximum measurable frequency */
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct FreqExtenderInfo
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_FREQEXT_ACTIVATION)]
            public string activationCode;
            public int activationCodeisValid;                           /**< If 1, the activation code is valid. Otherwise, this is 0, which means the code is invalid. */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_FREQEXT_SERIAL)]
            public string serial;
            public FreqExtPairStatus pairStatus;                     /**< The current pair status between the sensor and the frequency extender. */
            public FreqExtUpconvertBandPresent upconvertBandPresent; /**< Whether or not the upconvert band is present on the frequency extender. */
            public int numErrorsInQueue;                                /**< Number of errors currently in the frequency extender's error queue. -1 if unknown. */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_FREQEXT_IP_STRING)]
            public string IP;                  /**< When paired, the IP address of the frequency extender the sensor is connected to. When not paired, an empty string. */
            public int port;                                            /**< When paired, the port number of the frequency extender the sensor is connected to. When not paired, 0. */
        };

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int SAL_IQ_DATA_CALLBACK(ref IqDataHeader dataHeader);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SAL_EVENT_CALLBACK(ref SensorEvent e, IntPtr userWorkspace);

        [StructLayout(LayoutKind.Sequential)]
        public struct salFlowControl
        {
            public Int32 pacingPolicy;        // Pacing policy, 0 == disabled (no pacing), 1 == wait when full policy, 2 == flush when full policy
            public float maxBacklogSeconds;   // Max backlogSeconds, 0 == disabled
            public float maxBytesPerSec;      // TX data rate threshold, 0 == disabled
            public Int32 maxBacklogBytes;     // Max bytes threshold, 0 == disabled
            public Int32 maxBacklogMessages;  // Max messages threshold, 0 == disabled
        };

        // ===================== salGetVersion( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetVersion", CharSet = CharSet.Ansi)]
        private static extern SalError salGetVersion_Win32(ref Int32 version);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetVersion", CharSet = CharSet.Ansi)]
        private static extern SalError salGetVersion_X64(ref Int32 version);

        public static SalError salGetVersion(ref Int32 version)
        {
            SalError err;

            if (IntPtr.Size == 8)
            {
                err = salGetVersion_X64(ref version);
            }
            else
            {
                err = salGetVersion_Win32(ref version);
            }
            return err;
        }

        // ===================== salGetVersionString( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetVersionString", CharSet = CharSet.Ansi)]
        private static extern SalError salGetVersionString_Win32(StringBuilder version, int versionLength);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetVersionString", CharSet = CharSet.Ansi)]
        private static extern SalError salGetVersionString_X64(StringBuilder version, int versionLength);

        public static string salGetVersionString()
        {
            StringBuilder version = new StringBuilder(512);
            String rtn;
            SalError err;

            if (IntPtr.Size == 8)
            {
                err = salGetVersionString_X64(version, version.Capacity);
            }
            else
            {
                err = salGetVersionString_X64(version, version.Capacity);
            }

            if (err != SalError.SAL_ERR_NONE)
            {
                rtn = "Unable to retrieve error message";
            }
            else
            {
                rtn = version.ToString();
            }

            return rtn;
        }

        // ===================== salSetSMSAddress( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSetSMSAddress", CharSet = CharSet.Ansi)]
        private static extern SalError salSetSMSAddress_Win32(string hostname, UInt16 port, string directory);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSetSMSAddress", CharSet = CharSet.Ansi)]
        private static extern SalError salSetSMSAddress_X64(string hostname, UInt16 port, string directory);

        public static SalError salSetSMSAddress(string hostname, UInt16 port, string directory)
        {
            if (IntPtr.Size == 8)
                return salSetSMSAddress_X64(hostname, port, directory); // Call 64-bit DLL
            else
                return salSetSMSAddress_Win32(hostname, port, directory); // Call 32-bit DLL

        }

        // ===================== salGetSMSAddress( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetSMSAddressSafe", CharSet = CharSet.Ansi)]
        private static extern SalError salGetSMSAddressSafe_Win32(
             StringBuilder hostnameSB, UInt32 hostnameSize, out UInt16 port, StringBuilder directorySB, UInt32 directorySize);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetSMSAddressSafe", CharSet = CharSet.Ansi)]
        private static extern SalError salGetSMSAddressSafe_X64(
             StringBuilder hostnameSB, UInt32 hostnameSize, out UInt16 port, StringBuilder directorySB, UInt32 directorySize);

        public static SalError salGetSMSAddress(
             out string hostname, out UInt16 port, out string directory)
        {
            SalError err;
            StringBuilder hostnameSB = new StringBuilder(MAX_SENSOR_NAME);
            StringBuilder directorySB = new StringBuilder(MAX_FILENAME);

            if (IntPtr.Size == 8)
            {
                err = salGetSMSAddressSafe_X64(hostnameSB, MAX_SENSOR_NAME, out port, directorySB, MAX_FILENAME); // Call 64-bit DLL
            }
            else
            {
                err = salGetSMSAddressSafe_Win32(hostnameSB, MAX_SENSOR_NAME, out port, directorySB, MAX_FILENAME); // Call 32-bit DLL
            }

            hostname = hostnameSB.ToString();
            directory = directorySB.ToString();
            return err;
        }

        // ===================== salOpenSms( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salOpenSms", CharSet = CharSet.Ansi)]
        private static extern SalError salOpenSms_Win32(out IntPtr smsHandle, string hostname, UInt16 port, string directory);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salOpenSms", CharSet = CharSet.Ansi)]
        private static extern SalError salOpenSms_X64(out IntPtr smsHandle, string hostname, UInt16 port, string directory);

        public static SalError salOpenSms(out IntPtr smsHandle, string hostname, UInt16 port, string directory)
        {
            if (IntPtr.Size == 8)

                return salOpenSms_X64(out smsHandle, hostname, port, directory); // Call 64-bit DLL
            else
                return salOpenSms_Win32(out smsHandle, hostname, port, directory); // Call 32-bit DLL
        }

        // ===================== salQuerySmsExceptionString( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salQuerySmsExceptionString", CharSet = CharSet.Ansi)]
        private static extern SalError salQuerySmsExceptionString_Win32(
             StringBuilder exceptionStringSB, UInt32 exceptionStringSBsize);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salQuerySmsExceptionString", CharSet = CharSet.Ansi)]
        private static extern SalError salQuerySmsExceptionString_X64(
             StringBuilder exceptionStringSB, UInt32 exceptionStringSBsize);

        public static SalError salQuerySmsExceptionString(
             out string exceptionString)
        {
            SalError err;
            UInt32 exceptionStringSBsize = 5000;
            StringBuilder exceptionStringSB = new StringBuilder((Int32)exceptionStringSBsize);

            if (IntPtr.Size == 8)
            {
                err = salQuerySmsExceptionString_X64(exceptionStringSB, exceptionStringSBsize); // Call 64-bit DLL
            }
            else
            {
                err = salQuerySmsExceptionString_Win32(exceptionStringSB, exceptionStringSBsize); // Call 32-bit DLL
            }

            exceptionString = exceptionStringSB.ToString();
            return err;
        }

        /**********************************************************/
        /*  Sensor list functions                                 */
        /**********************************************************/
        // ===================== salOpenSensorList( ) (deprecated) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salOpenSensorList", CharSet = CharSet.Ansi)]
        private static extern SalError salOpenSensorList_Win32(out IntPtr sensorListHandle, out UInt32 numSensors);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salOpenSensorList", CharSet = CharSet.Ansi)]
        private static extern SalError salOpenSensorList_X64(out IntPtr sensorListHandle, out UInt32 numSensors);

        public static SalError salOpenSensorList(out IntPtr sensorListHandle, out UInt32 numSensors)
        {
            if (IntPtr.Size == 8)
                return salOpenSensorList_X64(out sensorListHandle, out numSensors); // Call 64-bit DLL
            else
                return salOpenSensorList_Win32(out sensorListHandle, out numSensors); // Call 32-bit DLL
        }

        // ===================== salOpenSensorList2( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salOpenSensorList2", CharSet = CharSet.Ansi)]
        private static extern SalError salOpenSensorList2_Win32(out IntPtr sensorListHandle, IntPtr smsHandle, out UInt32 numSensors);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salOpenSensorList2", CharSet = CharSet.Ansi)]
        private static extern SalError salOpenSensorList2_X64(out IntPtr sensorListHandle, IntPtr smsHandle, out UInt32 numSensors);

        public static SalError salOpenSensorList2(out IntPtr sensorListHandle, IntPtr smsHandle, out UInt32 numSensors)
        {
            if (IntPtr.Size == 8)
                return salOpenSensorList2_X64(out sensorListHandle, smsHandle, out numSensors); // Call 64-bit DLL
            else
                return salOpenSensorList2_Win32(out sensorListHandle, smsHandle, out numSensors); // Call 32-bit DLL
        }

        // ===================== salGetNextSensor( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetNextSensor", CharSet = CharSet.Ansi)]
        private static extern SalError salGetNextSensor_Win32(IntPtr sensorListHandle, out SensorStatus sensorStatus);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetNextSensor", CharSet = CharSet.Ansi)]
        private static extern SalError salGetNextSensor_X64(IntPtr sensorListHandle, out SensorStatus sensorStatus);

        public static SalError salGetNextSensor(IntPtr sensorListHandle, out SensorStatus sensorStatus)
        {
            if (IntPtr.Size == 8)
                return salGetNextSensor_X64(sensorListHandle, out sensorStatus); // Call 64-bit DLL
            else
                return salGetNextSensor_Win32(sensorListHandle, out sensorStatus); // Call 32-bit DLL
        }

        // ===================== salGetNextSensor2( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetNextSensor2", CharSet = CharSet.Ansi)]
        private static extern SalError salGetNextSensor2_Win32(IntPtr sensorListHandle, out SensorStatus2 sensorStatus);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetNextSensor2", CharSet = CharSet.Ansi)]
        private static extern SalError salGetNextSensor2_X64(IntPtr sensorListHandle, out SensorStatus2 sensorStatus);

        public static SalError salGetNextSensor2(IntPtr sensorListHandle, out SensorStatus2 sensorStatus)
        {
            if (IntPtr.Size == 8)
                return salGetNextSensor2_X64(sensorListHandle, out sensorStatus); // Call 64-bit DLL
            else
                return salGetNextSensor2_Win32(sensorListHandle, out sensorStatus); // Call 32-bit DLL
        }

        // ===================== salCloseSensorList( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salCloseSensorList", CharSet = CharSet.Ansi)]
        private static extern SalError salCloseSensorList_Win32(IntPtr sensorListHandle);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salCloseSensorList", CharSet = CharSet.Ansi)]
        private static extern SalError salCloseSensorList_X64(IntPtr sensorListHandle);

        public static SalError salCloseSensorList(IntPtr sensorListHandle)
        {
            if (IntPtr.Size == 8)
                return salCloseSensorList_X64(sensorListHandle); // Call 64-bit DLL
            else
                return salCloseSensorList_Win32(sensorListHandle); // Call 32-bit DLL
        }

        // ===================== salAddSensorToSMS( ) (deprecated) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salAddSensorToSMS", CharSet = CharSet.Ansi)] // deprecated: use salAddSensor() instead
        private static extern SalError salAddSensorToSMS_Win32(string hostname, string sensorName);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salAddSensorToSMS", CharSet = CharSet.Ansi)] // deprecated: use salAddSensor() instead
        private static extern SalError salAddSensorToSMS_X64(string hostname, string sensorName);

        public static SalError salAddSensorToSMS(string hostname, string sensorName)
        {
            SalError err;

            if (IntPtr.Size == 8)
            {
                err = salAddSensorToSMS_X64(hostname, sensorName); // Call 64-bit DLL
            }
            else
            {
                err = salAddSensorToSMS_Win32(hostname, sensorName); // Call 32-bit DLL
            }

            return err;
        }

        // ===================== salRemoveSensorFromSMS( ) (deprecated) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salRemoveSensorFromSMS", CharSet = CharSet.Ansi)]
        private static extern SalError salRemoveSensorFromSMS_Win32(string sensorName);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salRemoveSensorFromSMS", CharSet = CharSet.Ansi)]
        private static extern SalError salRemoveSensorFromSMS_X64(string sensorName);

        public static SalError salRemoveSensorFromSMS(string sensorName)
        {

            if (IntPtr.Size == 8)
                return salRemoveSensorFromSMS_X64(sensorName);
            else
                return salRemoveSensorFromSMS_Win32(sensorName);

        }

        /**********************************************************/
        /*  Miscellaneous functions                               */
        /**********************************************************/
        // ===================== salGetAbsoluteLocation( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetAbsoluteLocation", CharSet = CharSet.Ansi)]
        private static extern SalError salGetAbsoluteLocation_Win32(ref Location relativeIn, out Location absoluteOut);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetAbsoluteLocation", CharSet = CharSet.Ansi)]
        private static extern SalError salGetAbsoluteLocation_X64(ref Location relativeIn, out Location absoluteOut);

        public static SalError salGetAbsoluteLocation(ref Location relativeIn, out Location absoluteOut)
        {
            if (IntPtr.Size == 8)
                return salGetAbsoluteLocation_X64(ref relativeIn, out absoluteOut); // Call 64-bit DLL
            else
                return salGetAbsoluteLocation_Win32(ref relativeIn, out absoluteOut); // Call 32-bit DLL
        }

        // ===================== salComputeLocationOffset( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salComputeLocationOffset", CharSet = CharSet.Ansi)]
        private static extern SalError salComputeLocationOffset_Win32(ref Location startLocation, ref Location endLocation, out double x_offset, out double y_offset);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salComputeLocationOffset", CharSet = CharSet.Ansi)]
        private static extern SalError salComputeLocationOffset_X64(ref Location startLocation, ref Location endLocation, out double x_offset, out double y_offset);

        public static SalError salComputeLocationOffset(ref Location startLocation, ref Location endLocation, out double x_offset, out double y_offset)
        {
            if (IntPtr.Size == 8)
                return salComputeLocationOffset_X64(ref startLocation, ref endLocation, out x_offset, out y_offset); // Call 64-bit DLL
            else
                return salComputeLocationOffset_Win32(ref startLocation, ref endLocation, out x_offset, out y_offset); // Call 32-bit DLL
        }

        // ===================== salSetSensorGroupMode( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSetSensorGroupMode", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSetSensorGroupMode_Win32(IntPtr sensorGroupHandle, SensorMode mode);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSetSensorGroupMode", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSetSensorGroupMode_X64(IntPtr sensorGroupHandle, SensorMode mode);

        public static SalError salSetSensorGroupMode(IntPtr sensorGroupHandle, SensorMode mode)
        {
            if (IntPtr.Size == 8)
                return salSetSensorGroupMode_X64(sensorGroupHandle, mode);
            else
                return salSetSensorGroupMode_Win32(sensorGroupHandle, mode);
        }

        // ===================== salRebootSensor( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salRebootSensor", CharSet = CharSet.Ansi)]
        private static extern SalError salRebootSensor_Win32(IntPtr smsHandle, string sensorName);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salRebootSensor", CharSet = CharSet.Ansi)]
        private static extern SalError salRebootSensor_X64(IntPtr smsHandle, string sensorName);

        public static SalError salRebootSensor(IntPtr smsHandle, string sensorName)
        {
            if (IntPtr.Size == 8)
                return salRebootSensor_X64(smsHandle, sensorName);
            else
                return salRebootSensor_Win32(smsHandle, sensorName);
        }

        /**********************************************************/
        /*  Connect/disconnect sensor                             */
        /**********************************************************/

        // ===================== salConnectSensor( ) (deprecated) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salConnectSensor", CharSet = CharSet.Ansi)]
        private static extern SalError salConnectSensor_Win32(out IntPtr sensorHandle, string sensorName, string applicationName, int reserved);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salConnectSensor", CharSet = CharSet.Ansi)]
        private static extern SalError salConnectSensor_X64(out IntPtr sensorHandle, string sensorName, string applicationName, int reserved);

        public static SalError salConnectSensor(out IntPtr sensorHandle, string sensorName, string applicationName, int reserved)
        {
            if (IntPtr.Size == 8)
                return salConnectSensor_X64(out sensorHandle, sensorName, applicationName, reserved); // Call 64-bit DLL
            else
                return salConnectSensor_Win32(out sensorHandle, sensorName, applicationName, reserved); // Call 32-bit DLL
        }

        // ===================== salConnectSensor2( ) (deprecated) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salConnectSensor2", CharSet = CharSet.Ansi)]
        private static extern SalError salConnectSensor2_Win32(out IntPtr sensorHandle, IntPtr smsHandle, string sensorName, string applicationName, int reserved);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salConnectSensor2", CharSet = CharSet.Ansi)]
        private static extern SalError salConnectSensor2_X64(out IntPtr sensorHandle, IntPtr smsHandle, string sensorName, string applicationName, int reserved);

        public static SalError salConnectSensor2(out IntPtr sensorHandle, IntPtr smsHandle, string sensorName, string applicationName, int reserved)
        {
            if (IntPtr.Size == 8)
                return salConnectSensor2_X64(out sensorHandle, smsHandle, sensorName, applicationName, reserved); // Call 64-bit DLL
            else
                return salConnectSensor2_Win32(out sensorHandle, smsHandle, sensorName, applicationName, reserved); // Call 32-bit DLL
        }

        // ===================== salConnectSensor3( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salConnectSensor3", CharSet = CharSet.Ansi)]
        private static extern SalError salConnectSensor3_Win32(out IntPtr sensorHandle, IntPtr smsHandle, string sensorName, UInt32 portNum, string applicationName, int reserved);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salConnectSensor3", CharSet = CharSet.Ansi)]
        private static extern SalError salConnectSensor3_X64(out IntPtr sensorHandle, IntPtr smsHandle, string sensorName, UInt32 portNum, string applicationName, int reserved);

        public static SalError salConnectSensor3(out IntPtr sensorHandle, IntPtr smsHandle, string sensorName, UInt32 portNum, string applicationName, int reserved)
        {
            if (IntPtr.Size == 8)
                return salConnectSensor3_X64(out sensorHandle, smsHandle, sensorName, portNum, applicationName, reserved); // Call 64-bit DLL
            else
                return salConnectSensor3_Win32(out sensorHandle, smsHandle, sensorName, portNum, applicationName, reserved); // Call 32-bit DLL
        }

        // ===================== salValidSensorHandle( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salValidSensorHandle", CharSet = CharSet.Ansi)]
        private static extern SalError salValidSensorHandle_Win32(IntPtr sensorHandle);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salValidSensorHandle", CharSet = CharSet.Ansi)]
        private static extern SalError salValidSensorHandle_X64(IntPtr sensorHandle);

        public static SalError salValidSensorHandle(IntPtr sensorHandle)
        {
            if (IntPtr.Size == 8)
                return salValidSensorHandle_X64(sensorHandle); // Call 64-bit DLL
            else
                return salValidSensorHandle_Win32(sensorHandle); // Call 32-bit DLL
        }

        // ===================== salDisconnectSensor( ) (deprecated) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salDisconnectSensor", CharSet = CharSet.Ansi)]
        private static extern SalError salDisconnectSensor_Win32(IntPtr sensorHandle);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salDisconnectSensor", CharSet = CharSet.Ansi)]
        private static extern SalError salDisconnectSensor_X64(IntPtr sensorHandle);

        public static SalError salDisconnectSensor(IntPtr sensorHandle)

        {
            if (IntPtr.Size == 8)
                return salDisconnectSensor_X64(sensorHandle); // Call 64-bit DLL
            else
                return salDisconnectSensor_Win32(sensorHandle); // Call 32-bit DLL
        }

        // ===================== salGetSensorLocation( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetSensorLocation", CharSet = CharSet.Ansi)]
        private static extern SalError salGetSensorLocation_Win32(IntPtr sensorHandle, out Location location);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetSensorLocation", CharSet = CharSet.Ansi)]
        private static extern SalError salGetSensorLocation_X64(IntPtr sensorHandle, out Location location);

        public static SalError salGetSensorLocation(IntPtr sensorHandle, out Location location)
        {
            if (IntPtr.Size == 8)
                return salGetSensorLocation_X64(sensorHandle, out location); // Call 64-bit DLL
            else
                return salGetSensorLocation_Win32(sensorHandle, out location); // Call 32-bit DLL
        }

        // ===================== salSetSensorLocation( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSetSensorLocation", CharSet = CharSet.Ansi)]
        private static extern SalError salSetSensorLocation_Win32(IntPtr sensorHandle, ref Location location);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSetSensorLocation", CharSet = CharSet.Ansi)]
        private static extern SalError salSetSensorLocation_X64(IntPtr sensorHandle, ref Location location);

        public static SalError salSetSensorLocation(IntPtr sensorHandle, ref Location location)
        {
            if (IntPtr.Size == 8)
                return salSetSensorLocation_X64(sensorHandle, ref location); // Call 64-bit DLL
            else
                return salSetSensorLocation_Win32(sensorHandle, ref location); // Call 32-bit DLL
        }

        // ===================== salGetSensorAutoAntenna( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetSensorAutoAntenna", CharSet = CharSet.Ansi)]
        private static extern SalError salGetSensorAutoAntenna_Win32(IntPtr sensorHandle, out UInt32 numConfig,
            byte[] configBytes, UInt32 configSize);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetSensorAutoAntenna", CharSet = CharSet.Ansi)]
        private static extern SalError salGetSensorAutoAntenna_X64(IntPtr sensorHandle, out UInt32 numConfig,
            byte[] configBytes, UInt32 configSize);

        public static SalError salGetSensorAutoAntenna(IntPtr sensorHandle, out UInt32 numConfig,
            ref AntennaAuto[] config)
        {
            AntennaAuto single = new AntennaAuto();
            UInt32 configSize = (UInt32)(config.Length * System.Runtime.InteropServices.Marshal.SizeOf(single));
            byte[] configBytes = new byte[configSize];

            SalError err;

            if (IntPtr.Size == 8)
                err = salGetSensorAutoAntenna_X64(sensorHandle, out numConfig, configBytes, configSize); // Call 64-bit DLL
            else
                err = salGetSensorAutoAntenna_Win32(sensorHandle, out numConfig, configBytes, configSize); // Call 32-bit DLL

            if (err != SalError.SAL_ERR_NONE) return err;

            int offset = 0;
            for (UInt32 idx = 0; idx < numConfig; idx++)
            {
                config[idx].startFreq = BitConverter.ToDouble(configBytes, offset); offset += 8;
                config[idx].stopFreq = BitConverter.ToDouble(configBytes, offset); offset += 8;
                config[idx].attenuation = BitConverter.ToDouble(configBytes, offset); offset += 8;
                config[idx].mixerLevel = BitConverter.ToDouble(configBytes, offset); offset += 8;
                config[idx].preamp = BitConverter.ToInt32(configBytes, offset); offset += 4;
                config[idx].antenna = (AgSalLib.AntennaType)BitConverter.ToInt32(configBytes, offset); offset += 4;
            }

            return err;
        }

        // ===================== salSetSensorAutoAntenna( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSetSensorAutoAntenna", CharSet = CharSet.Ansi)]
        private static extern SalError salSetSensorAutoAntenna_Win32(IntPtr sensorHandle, UInt32 numConfig,
            AntennaAuto[] config);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSetSensorAutoAntenna", CharSet = CharSet.Ansi)]
        private static extern SalError salSetSensorAutoAntenna_X64(IntPtr sensorHandle, UInt32 numConfig,
            AntennaAuto[] config);

        public static SalError salSetSensorAutoAntenna(IntPtr sensorHandle, AntennaAuto[] config)
        {
            UInt32 numConfig = (UInt32)config.Length;
            if (IntPtr.Size == 8)
                return salSetSensorAutoAntenna_X64(sensorHandle, numConfig, config); // Call 64-bit DLL
            else
                return salSetSensorAutoAntenna_Win32(sensorHandle, numConfig, config); // Call 32-bit DLL
        }

        // ===================== salGetServerVersion( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetServerVersion", CharSet = CharSet.Ansi)]
        private static extern SalError salGetServerVersion_Win32(IntPtr sensorHandle, out UInt32 version);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetServerVersion", CharSet = CharSet.Ansi)]
        private static extern SalError salGetServerVersion_X64(IntPtr sensorHandle, out UInt32 version);

        public static SalError salGetServerVersion(IntPtr sensorHandle, out UInt32 version)
        {
            if (IntPtr.Size == 8)
                return salGetServerVersion_X64(sensorHandle, out version); // Call 64-bit DLL
            else
                return salGetServerVersion_Win32(sensorHandle, out version); // Call 32-bit DLL
        }

        /**********************************************************/
        /*  Set sensor measurement mode  (deprecated)             */
        /**********************************************************/

        // ===================== salSetService( ) (deprecated) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSetService", CharSet = CharSet.Ansi)]
        private static extern SalError salSetService_Win32(IntPtr sensorHandle, Service service);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSetService", CharSet = CharSet.Ansi)]
        private static extern SalError salSetService_X64(IntPtr sensorHandle, Service service);

        public static SalError salSetService(IntPtr sensorHandle, Service service)
        {
            if (IntPtr.Size == 8)
                return salSetService_X64(sensorHandle, service); // Call 64-bit DLL
            else
                return salSetService_Win32(sensorHandle, service); // Call 32-bit DLL
        }

        // ===================== salSetSensorMode( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSetSensorMode", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSetSensorMode_Win32(string sensorname, SensorMode mode);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSetSensorMode", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSetSensorMode_X64(string sensorname, SensorMode mode);

        public static SalError salSetSensorMode(string sensorname, SensorMode mode)
        {
            if (IntPtr.Size == 8)
                return salSetSensorMode_X64(sensorname, mode);
            else
                return salSetSensorMode_Win32(sensorname, mode);
        }

        // ===================== salSetSensorMode2( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSetSensorMode2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSetSensorMode2_Win32(IntPtr sensorHandle, SensorMode mode);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSetSensorMode2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSetSensorMode2_X64(IntPtr sensorHandle, SensorMode mode);

        public static SalError salSetSensorMode2(IntPtr sensorHandle, SensorMode mode)
        {
            if (IntPtr.Size == 8)
                return salSetSensorMode2_X64(sensorHandle, mode);
            else
                return salSetSensorMode2_Win32(sensorHandle, mode);
        }

        // ===================== salGetSensorCapabilities( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetSensorCapabilities", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSensorCapabilities_Win32(IntPtr sensorHandle, out SensorCapabilities capabilities);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetSensorCapabilities", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSensorCapabilities_X64(IntPtr sensorHandle, out SensorCapabilities capabilities);

        public static SalError salGetSensorCapabilities(IntPtr sensorHandle, out SensorCapabilities capabilities)
        {
            SalError err;

            if (IntPtr.Size == 8)
            {
                err = salGetSensorCapabilities_X64(sensorHandle, out capabilities); // Call 64-bit DLL
            }
            else
            {
                err = salGetSensorCapabilities_Win32(sensorHandle, out capabilities); // Call 32-bit DLL
            }

            return err;
        }

        // ===================== salGetSensorCapabilities2( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetSensorCapabilities2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSensorCapabilities2_Win32(IntPtr sensorHandle, out SensorCapabilities2 capabilities);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetSensorCapabilities2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSensorCapabilities2_X64(IntPtr sensorHandle, out SensorCapabilities2 capabilities);

        public static SalError salGetSensorCapabilities2(IntPtr sensorHandle, out SensorCapabilities2 capabilities)
        {
            SalError err;

            if (IntPtr.Size == 8)
            {
                err = salGetSensorCapabilities2_X64(sensorHandle, out capabilities); // Call 64-bit DLL
            }
            else
            {
                err = salGetSensorCapabilities2_Win32(sensorHandle, out capabilities); // Call 32-bit DLL
            }

            return err;
        }

        // ===================== salGetSensorAttribute( ) ==================================
        // get integer attribute
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetSensorAttribute", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSensorAttribute2_Win32(IntPtr sensorHandle, SensorAttribute attribute, out int val, Int32 sizeInBytes);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetSensorAttribute", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSensorAttribute2_X64(IntPtr sensorHandle, SensorAttribute attribute, out int val, Int32 sizeInBytes);


        public static SalError salGetSensorAttribute(IntPtr sensorHandle, SensorAttribute attribute, out int val)
        {
            SalError err;

            if (IntPtr.Size == 8)
            {
                err = salGetSensorAttribute2_X64(sensorHandle, attribute, out val, 4); // Call 64-bit DLL
            }
            else
            {
                err = salGetSensorAttribute2_Win32(sensorHandle, attribute, out val, 4); // Call 32-bit DLL
            }

            return err;
        }

        // Get UInt32 value
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetSensorAttribute", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSensorAttribute2_Win32(IntPtr sensorHandle, SensorAttribute attribute, out UInt32 val, Int32 sizeInBytes);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetSensorAttribute", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSensorAttribute2_X64(IntPtr sensorHandle, SensorAttribute attribute, out UInt32 val, Int32 sizeInBytes);


        public static SalError salGetSensorAttribute(IntPtr sensorHandle, SensorAttribute attribute, out UInt32 val)
        {
            SalError err;

            if (IntPtr.Size == 8)
            {
                err = salGetSensorAttribute2_X64(sensorHandle, attribute, out val, 4); // Call 64-bit DLL
            }
            else
            {
                err = salGetSensorAttribute2_Win32(sensorHandle, attribute, out val, 4); // Call 32-bit DLL
            }

            return err;
        }


        // get UInt64 attribute
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetSensorAttribute", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSensorAttribute2_Win32(IntPtr sensorHandle, SensorAttribute attribute, out UInt64 val, Int32 sizeInBytes);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetSensorAttribute", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSensorAttribute2_X64(IntPtr sensorHandle, SensorAttribute attribute, out UInt64 val, Int32 sizeInBytes);


        public static SalError salGetSensorAttribute(IntPtr sensorHandle, SensorAttribute attribute, out UInt64 val)
        {
            SalError err;

            if (IntPtr.Size == 8)
            {
                err = salGetSensorAttribute2_X64(sensorHandle, attribute, out val, 8); // Call 64-bit DLL
            }
            else
            {
                err = salGetSensorAttribute2_Win32(sensorHandle, attribute, out val, 8); // Call 32-bit DLL
            }

            return err;
        }

        // get double attribute
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetSensorAttribute", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSensorAttribute2_Win32(IntPtr sensorHandle, SensorAttribute attribute, out double val, Int32 sizeInBytes);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetSensorAttribute", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSensorAttribute2_X64(IntPtr sensorHandle, SensorAttribute attribute, out double val, Int32 sizeInBytes);

        public static SalError salGetSensorAttribute(IntPtr sensorHandle, SensorAttribute attribute, out double val)
        {
            SalError err;

            if (IntPtr.Size == 8)
            {
                err = salGetSensorAttribute2_X64(sensorHandle, attribute, out val, 8); // Call 64-bit DLL
            }
            else
            {
                err = salGetSensorAttribute2_Win32(sensorHandle, attribute, out val, 8); // Call 32-bit DLL
            }

            return err;
        }

        // get float attribute
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetSensorAttribute", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSensorAttribute2_Win32(IntPtr sensorHandle, SensorAttribute attribute, out float val, Int32 sizeInBytes);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetSensorAttribute", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSensorAttribute2_X64(IntPtr sensorHandle, SensorAttribute attribute, out float val, Int32 sizeInBytes);

        public static SalError salGetSensorAttribute(IntPtr sensorHandle, SensorAttribute attribute, out float val)
        {
            SalError err;

            if (IntPtr.Size == 8)
            {
                err = salGetSensorAttribute2_X64(sensorHandle, attribute, out val, 4); // Call 64-bit DLL
            }
            else
            {
                err = salGetSensorAttribute2_Win32(sensorHandle, attribute, out val, 4); // Call 32-bit DLL
            }

            return err;
        }

        // get a string attribute
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetSensorAttribute", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSensorAttributeString_Win32(IntPtr sensorHandle, SensorAttribute attribute, StringBuilder buf, Int32 sizeInBytes);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetSensorAttribute", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSensorAttributeString_X64(IntPtr sensorHandle, SensorAttribute attribute, StringBuilder buf, Int32 sizeInBytes);

        public static SalError salGetSensorAttribute(IntPtr sensorHandle, SensorAttribute attribute, out String val)
        {
            SalError err;
            StringBuilder buf = new StringBuilder(1000);

            if (IntPtr.Size == 8)
            {
                err = salGetSensorAttributeString_X64(sensorHandle, attribute, buf, buf.Capacity); // Call 64-bit DLL
            }
            else
            {
                err = salGetSensorAttributeString_Win32(sensorHandle, attribute, buf, buf.Capacity); // Call 32-bit DLL
            }

            if (err != SalError.SAL_ERR_NONE)
            {
                val = "Unable to retrieve attribute";
            }
            else
            {
                val = buf.ToString();
            }
            return err;
        }


        // ===================== salGetSensorAttribute2( ) ==================================
        // get integer attribute
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetSensorAttribute2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSensorAttribute2_Win32(IntPtr sensorHandle, SensorAttribute attribute, out int val, Int32 sizeInBytes, byte skipRefresh);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetSensorAttribute2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSensorAttribute2_X64(IntPtr sensorHandle, SensorAttribute attribute, out int val, Int32 sizeInBytes, byte skipRefresh);


        public static SalError salGetSensorAttribute2(IntPtr sensorHandle, SensorAttribute attribute, out int val, byte skipRefresh)
        {
            SalError err;

            if (IntPtr.Size == 8)
            {
                err = salGetSensorAttribute2_X64(sensorHandle, attribute, out val, 4, skipRefresh); // Call 64-bit DLL
            }
            else
            {
                err = salGetSensorAttribute2_Win32(sensorHandle, attribute, out val, 4, skipRefresh); // Call 32-bit DLL
            }

            return err;
        }

        // Get UInt32 value
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetSensorAttribute2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSensorAttribute2_Win32(IntPtr sensorHandle, SensorAttribute attribute, out UInt32 val, Int32 sizeInBytes, byte skipRefresh);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetSensorAttribute2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSensorAttribute2_X64(IntPtr sensorHandle, SensorAttribute attribute, out UInt32 val, Int32 sizeInBytes, byte skipRefresh);


        public static SalError salGetSensorAttribute2(IntPtr sensorHandle, SensorAttribute attribute, out UInt32 val, byte skipRefresh)
        {
            SalError err;

            if (IntPtr.Size == 8)
            {
                err = salGetSensorAttribute2_X64(sensorHandle, attribute, out val, 4, skipRefresh); // Call 64-bit DLL
            }
            else
            {
                err = salGetSensorAttribute2_Win32(sensorHandle, attribute, out val, 4, skipRefresh); // Call 32-bit DLL
            }

            return err;
        }


        // get UInt64 attribute
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetSensorAttribute2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSensorAttribute2_Win32(IntPtr sensorHandle, SensorAttribute attribute, out UInt64 val, Int32 sizeInBytes, byte skipRefresh);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetSensorAttribute2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSensorAttribute2_X64(IntPtr sensorHandle, SensorAttribute attribute, out UInt64 val, Int32 sizeInBytes, byte skipRefresh);


        public static SalError salGetSensorAttribute2(IntPtr sensorHandle, SensorAttribute attribute, out UInt64 val, byte skipRefresh)
        {
            SalError err;

            if (IntPtr.Size == 8)
            {
                err = salGetSensorAttribute2_X64(sensorHandle, attribute, out val, 8, skipRefresh); // Call 64-bit DLL
            }
            else
            {
                err = salGetSensorAttribute2_Win32(sensorHandle, attribute, out val, 8, skipRefresh); // Call 32-bit DLL
            }

            return err;
        }

        // get double attribute
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetSensorAttribute2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSensorAttribute2_Win32(IntPtr sensorHandle, SensorAttribute attribute, out double val, Int32 sizeInBytes, byte skipRefresh);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetSensorAttribute2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSensorAttribute2_X64(IntPtr sensorHandle, SensorAttribute attribute, out double val, Int32 sizeInBytes, byte skipRefresh);

        public static SalError salGetSensorAttribute2(IntPtr sensorHandle, SensorAttribute attribute, out double val, byte skipRefresh)
        {
            SalError err;

            if (IntPtr.Size == 8)
            {
                err = salGetSensorAttribute2_X64(sensorHandle, attribute, out val, 8, skipRefresh); // Call 64-bit DLL
            }
            else
            {
                err = salGetSensorAttribute2_Win32(sensorHandle, attribute, out val, 8, skipRefresh); // Call 32-bit DLL
            }

            return err;
        }

        // get float attribute
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetSensorAttribute2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSensorAttribute2_Win32(IntPtr sensorHandle, SensorAttribute attribute, out float val, Int32 sizeInBytes, byte skipRefresh);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetSensorAttribute2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSensorAttribute2_X64(IntPtr sensorHandle, SensorAttribute attribute, out float val, Int32 sizeInBytes, byte skipRefresh);

        public static SalError salGetSensorAttribute2(IntPtr sensorHandle, SensorAttribute attribute, out float val, byte skipRefresh)
        {
            SalError err;

            if (IntPtr.Size == 8)
            {
                err = salGetSensorAttribute2_X64(sensorHandle, attribute, out val, 4, skipRefresh); // Call 64-bit DLL
            }
            else
            {
                err = salGetSensorAttribute2_Win32(sensorHandle, attribute, out val, 4, skipRefresh); // Call 32-bit DLL
            }

            return err;
        }

        // get a string attribute
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetSensorAttribute2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSensorAttributeString_Win32(IntPtr sensorHandle, SensorAttribute attribute, StringBuilder buf, Int32 sizeInBytes, byte skipRefresh);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetSensorAttribute2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSensorAttributeString_X64(IntPtr sensorHandle, SensorAttribute attribute, StringBuilder buf, Int32 sizeInBytes, byte skipRefresh);

        public static SalError salGetSensorAttribute2(IntPtr sensorHandle, SensorAttribute attribute, out String val, byte skipRefresh)
        {
            SalError err;
            StringBuilder buf = new StringBuilder(1000);

            if (IntPtr.Size == 8)
            {
                err = salGetSensorAttributeString_X64(sensorHandle, attribute, buf, buf.Capacity, skipRefresh); // Call 64-bit DLL
            }
            else
            {
                err = salGetSensorAttributeString_Win32(sensorHandle, attribute, buf, buf.Capacity, skipRefresh); // Call 32-bit DLL
            }

            if (err != SalError.SAL_ERR_NONE)
            {
                val = "Unable to retrieve attribute";
            }
            else
            {
                val = buf.ToString();
            }
            return err;
        }

        // ===================== salSetSensorAttribute( ) ==================================
        // set integer attribute
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSetSensorAttribute", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSetSensorAttribute_Win32(IntPtr sensorHandle, SensorAttribute attribute, ref int val);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSetSensorAttribute", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSetSensorAttribute_X64(IntPtr sensorHandle, SensorAttribute attribute, ref int val);

        public static SalError salSetSensorAttribute(IntPtr sensorHandle, SensorAttribute attribute, int val)
        {
            SalError err;

            if (IntPtr.Size == 8)
            {
                err = salSetSensorAttribute_X64(sensorHandle, attribute, ref val); // Call 64-bit DLL
            }
            else
            {
                err = salSetSensorAttribute_Win32(sensorHandle, attribute, ref val); // Call 32-bit DLL
            }

            return err;
        }


        // set double attribute
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSetSensorAttribute", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSetSensorAttribute_Win32(IntPtr sensorHandle, SensorAttribute attribute, ref double val);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSetSensorAttribute", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSetSensorAttribute_X64(IntPtr sensorHandle, SensorAttribute attribute, ref double val);


        public static SalError salSetSensorAttribute(IntPtr sensorHandle, SensorAttribute attribute, double val)
        {
            SalError err;

            if (IntPtr.Size == 8)
            {
                err = salSetSensorAttribute_X64(sensorHandle, attribute, ref val); // Call 64-bit DLL
            }
            else
            {
                err = salSetSensorAttribute_Win32(sensorHandle, attribute, ref val); // Call 32-bit DLL
            }

            return err;
        }

        // set float attribute
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSetSensorAttribute", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSetSensorAttribute_Win32(IntPtr sensorHandle, SensorAttribute attribute, ref float val);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSetSensorAttribute", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSetSensorAttribute_X64(IntPtr sensorHandle, SensorAttribute attribute, ref float val);

        public static SalError salSetSensorAttribute(IntPtr sensorHandle, SensorAttribute attribute, float val)
        {
            SalError err;

            if (IntPtr.Size == 8)
            {
                err = salSetSensorAttribute_X64(sensorHandle, attribute, ref val); // Call 64-bit DLL
            }
            else
            {
                err = salSetSensorAttribute_Win32(sensorHandle, attribute, ref val); // Call 32-bit DLL
            }

            return err;
        }

        // ===================== salGetErrorString( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetErrorString", CharSet = CharSet.Ansi)]
        private static extern SalError salGetErrorStringRaw_Win32(SalError salError, Localization language, StringBuilder errorMsg, UInt16 arrayLength);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetErrorString", CharSet = CharSet.Ansi)]
        private static extern SalError salGetErrorStringRaw_X64(SalError salError, Localization language, StringBuilder errorMsg, UInt16 arrayLength);

        public static string salGetErrorString(SalError salError, Localization language)
        {
            StringBuilder errorMsg = new StringBuilder(512);
            // int length = 512;
            // byte[] rawBytes = new byte[length];
            SalError err;
            String rtn;

            if (IntPtr.Size == 8)
            {
                err = salGetErrorStringRaw_X64(salError, language, errorMsg, (UInt16)(errorMsg.Capacity));
            }
            else
            {
                err = salGetErrorStringRaw_Win32(salError, language, errorMsg, (UInt16)(errorMsg.Capacity));
            }

            if (err != SalError.SAL_ERR_NONE)
            {
                rtn = "Unable to retrieve error message";
            }
            else
            {
                rtn = errorMsg.ToString();
            }

            return rtn;
        }

        // ===================== salTimePlusDuration( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salTimePlusDuration", CharSet = CharSet.Ansi)]
        private static extern SalError salTimePlusDuration_Win32(UInt32 inSec, UInt32 inNSec, Int32 durationSign, UInt32 durationSec, UInt32 durationNSec, out UInt32 outSec, out UInt32 outNSec);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salTimePlusDuration", CharSet = CharSet.Ansi)]
        private static extern SalError salTimePlusDuration_X64(UInt32 inSec, UInt32 inNSec, Int32 durationSign, UInt32 durationSec, UInt32 durationNSec, out UInt32 outSec, out UInt32 outNSec);

        public static SalError salTimePlusDuration(UInt32 inSec, UInt32 inNSec, Int32 durationSign, UInt32 durationSec, UInt32 durationNSec, out UInt32 outSec, out UInt32 outNSec)
        {
            if (IntPtr.Size == 8)
                return salTimePlusDuration_X64(inSec, inNSec, durationSign, durationSec, durationNSec, out outSec, out outNSec); // Call 64-bit DLL
            else
                return salTimePlusDuration_Win32(inSec, inNSec, durationSign, durationSec, durationNSec, out outSec, out outNSec); // Call 32-bit DLL
        }

        // ===================== salTimePlusDurationDouble( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salTimePlusDurationDouble", CharSet = CharSet.Ansi)]
        private static extern SalError salTimePlusDurationDouble_Win32(UInt32 inSec, UInt32 inNSec, double duration, out UInt32 outSec, out UInt32 outNSec);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salTimePlusDurationDouble", CharSet = CharSet.Ansi)]
        private static extern SalError salTimePlusDurationDouble_X64(UInt32 inSec, UInt32 inNSec, double duration, out UInt32 outSec, out UInt32 outNSec);

        public static SalError salTimePlusDurationDouble(UInt32 inSec, UInt32 inNSec, double duration, out UInt32 outSec, out UInt32 outNSec)
        {
            if (IntPtr.Size == 8)
                return salTimePlusDurationDouble_X64(inSec, inNSec, duration, out outSec, out outNSec); // Call 64-bit DLL
            else
                return salTimePlusDurationDouble_Win32(inSec, inNSec, duration, out outSec, out outNSec); // Call 32-bit DLL
        }

        // ===================== salTimeMinusTime( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salTimeMinusTime", CharSet = CharSet.Ansi)]
        private static extern SalError salTimeMinusTime_Win32(UInt32 inSecA, UInt32 inNSecA, UInt32 inSecB, UInt32 inNSecB, out Int32 durationSign, out UInt32 durationSec, out UInt32 durationNSec);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salTimeMinusTime", CharSet = CharSet.Ansi)]
        private static extern SalError salTimeMinusTime_X64(UInt32 inSecA, UInt32 inNSecA, UInt32 inSecB, UInt32 inNSecB, out Int32 durationSign, out UInt32 durationSec, out UInt32 durationNSec);

        public static SalError salTimeMinusTime(UInt32 inSecA, UInt32 inNSecA, UInt32 inSecB, UInt32 inNSecB, out Int32 durationSign, out UInt32 durationSec, out UInt32 durationNSec)
        {
            if (IntPtr.Size == 8)
                return salTimeMinusTime_X64(inSecA, inNSecA, inSecB, inNSecB, out durationSign, out durationSec, out durationNSec); // Call 64-bit DLL
            else
                return salTimeMinusTime_Win32(inSecA, inNSecA, inSecB, inNSecB, out durationSign, out durationSec, out durationNSec); // Call 32-bit DLL
        }

        // ===================== salTimeMinusTimeDouble( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salTimeMinusTimeDouble", CharSet = CharSet.Ansi)]
        private static extern SalError salTimeMinusTimeDouble_Win32(UInt32 inSecA, UInt32 inNSecA, UInt32 inSecB, UInt32 inNSecB, out double duration);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salTimeMinusTimeDouble", CharSet = CharSet.Ansi)]
        private static extern SalError salTimeMinusTimeDouble_X64(UInt32 inSecA, UInt32 inNSecA, UInt32 inSecB, UInt32 inNSecB, out double duration);

        public static SalError salTimeMinusTimeDouble(UInt32 inSecA, UInt32 inNSecA, UInt32 inSecB, UInt32 inNSecB, out double duration)
        {
            if (IntPtr.Size == 8)
                return salTimeMinusTimeDouble_X64(inSecA, inNSecA, inSecB, inNSecB, out duration); // Call 64-bit DLL
            else
                return salTimeMinusTimeDouble_Win32(inSecA, inNSecA, inSecB, inNSecB, out duration); // Call 32-bit DLL
        }

        // ===================== salCalculateFieldStrength( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salCalculateFieldStrength", CharSet = CharSet.Ansi)]
        private static extern SalError salCalculateFieldStrength_Win32(double fReceivedPwr_dBm, double fAntennaGain_dB, double fFreq_MHz, out double fFieldStrength);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salCalculateFieldStrength", CharSet = CharSet.Ansi)]
        private static extern SalError salCalculateFieldStrength_X64(double fReceivedPwr_dBm, double fAntennaGain_dB, double fFreq_MHz, out double fFieldStrength);

        public static SalError salCalculateFieldStrength(double fReceivedPwr_dBm, double fAntennaGain_dB, double fFreq_MHz, out double fFieldStrength)
        {
            if (IntPtr.Size == 8)
                return salCalculateFieldStrength_X64(fReceivedPwr_dBm, fAntennaGain_dB, fFreq_MHz, out fFieldStrength); // Call 64-bit DLL
            else
                return salCalculateFieldStrength_Win32(fReceivedPwr_dBm, fAntennaGain_dB, fFreq_MHz, out fFieldStrength); // Call 32-bit DLL
        }

        // ===================== salRegisterEventHandler( ) (deprecated) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salRegisterEventHandler", CharSet = CharSet.Ansi)]
        private static extern SalError salRegisterEventHandler_Win32(IntPtr sensorHandle,
                                                       SAL_EVENT_CALLBACK callback,
                                                       IntPtr userWorkspace);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salRegisterEventHandler", CharSet = CharSet.Ansi)]
        private static extern SalError salRegisterEventHandler_X64(IntPtr sensorHandle,
                                                       SAL_EVENT_CALLBACK callback,
                                                       IntPtr userWorkspace);

        public static SalError salRegisterEventHandler(IntPtr sensorHandle,
                                                      SAL_EVENT_CALLBACK callback,
                                                      IntPtr userWorkspace)
        {
            SalError err;

            if (IntPtr.Size == 8)
            {
                err = salRegisterEventHandler_X64(sensorHandle, callback, userWorkspace);
            }
            else
            {
                err = salRegisterEventHandler_Win32(sensorHandle, callback, userWorkspace);
            }
            return err;
        }

        // ===================== salSensorTest( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSensorTest", CharSet = CharSet.Ansi)]
        private static extern SalError salSensorTest_Win32(IntPtr sensorHandle, int reserved, [MarshalAs(UnmanagedType.LPStr)] StringBuilder testResults, int resultsSize);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSensorTest", CharSet = CharSet.Ansi)]
        private static extern SalError salSensorTest_X64(IntPtr sensorHandle, int reserved, [MarshalAs(UnmanagedType.LPStr)] StringBuilder testResults, int resultsSize);

        public static SalError salSensorTest(IntPtr sensorHandle, int reserved, [MarshalAs(UnmanagedType.LPStr)] StringBuilder testResults, int resultsSize)
        {
            if (IntPtr.Size == 8)
                return salSensorTest_X64(sensorHandle, reserved, testResults, resultsSize);
            else
                return salSensorTest_Win32(sensorHandle, reserved, testResults, resultsSize);
        }

        // ===================== salSensorBeep( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSensorBeep", CharSet = CharSet.Ansi)]
        private static extern SalError salSensorBeep_Win32(IntPtr sensorHandle);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSensorBeep", CharSet = CharSet.Ansi)]
        private static extern SalError salSensorBeep_X64(IntPtr sensorHandle);

        public static SalError salSensorBeep(IntPtr sensorHandle)
        {
            if (IntPtr.Size == 8)
                return salSensorBeep_X64(sensorHandle);
            else
                return salSensorBeep_Win32(sensorHandle);
        }

        // ===================== salGetFreqExtInfo( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetFreqExtInfo", CharSet = CharSet.Ansi)]
        private static extern SalError salGetFreqExtInfo_Win32(IntPtr sensorHandle, out FreqExtenderInfo freqExtInfo);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetFreqExtInfo", CharSet = CharSet.Ansi)]
        private static extern SalError salGetFreqExtInfo_X64(IntPtr sensorHandle, out FreqExtenderInfo freqExtInfo);

        public static SalError salGetFreqExtInfo(IntPtr sensorHandle, out FreqExtenderInfo freqExtInfo)
        {
            if (IntPtr.Size == 8)
                return salGetFreqExtInfo_X64(sensorHandle, out freqExtInfo);
            else
                return salGetFreqExtInfo_Win32(sensorHandle, out freqExtInfo);
        }

        // ===================== salSetFreqExtInfo( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSetFreqExtInfo", CharSet = CharSet.Ansi)]
        private static extern SalError salSetFreqExtInfo_Win32(IntPtr sensorHandle, ref FreqExtenderInfo freqExtInfo);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSetFreqExtInfo", CharSet = CharSet.Ansi)]
        private static extern SalError salSetFreqExtInfo_X64(IntPtr sensorHandle, ref FreqExtenderInfo freqExtInfo);

        public static SalError salSetFreqExtInfo(IntPtr sensorHandle, ref FreqExtenderInfo freqExtInfo)
        {
            if (IntPtr.Size == 8)
                return salSetFreqExtInfo_X64(sensorHandle, ref freqExtInfo);
            else
                return salSetFreqExtInfo_Win32(sensorHandle, ref freqExtInfo);
        }

        /**********************************************************/
        /*  Open/Close data stream   (deprecated)                 */
        /**********************************************************/
        // ===================== salIqOpenDataStream( ) (deprecated) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salIqOpenDataStream", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern SalError salIqOpenDataStream_Win32(
            out IntPtr streamHandle,
            IntPtr sensorHandle,
            DataProtocol protocol,
            SAL_IQ_DATA_CALLBACK callback,
            IntPtr userWorkspace);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salIqOpenDataStream", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern SalError salIqOpenDataStream_X64(
            out IntPtr streamHandle,
            IntPtr sensorHandle,
            DataProtocol protocol,
            SAL_IQ_DATA_CALLBACK callback,
            IntPtr userWorkspace);

        public static SalError salIqOpenDataStream(
            out IntPtr streamHandle,
            IntPtr sensorHandle,
            DataProtocol protocol,
            SAL_IQ_DATA_CALLBACK callback,
            IntPtr userWorkspace)
        {
            if (IntPtr.Size == 8)
                return salIqOpenDataStream_X64(out streamHandle, sensorHandle, protocol, callback, userWorkspace); // Call 64-bit DLL
            else
                return salIqOpenDataStream_Win32(out streamHandle, sensorHandle, protocol, callback, userWorkspace); // Call 32-bit DLL
        }

        // ===================== salIqCloseDataStream( ) (deprecated) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salIqCloseDataStream", CharSet = CharSet.Ansi)]
        private static extern SalError salIqCloseDataStream_Win32(IntPtr dataStreamHandle);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salIqCloseDataStream", CharSet = CharSet.Ansi)]
        private static extern SalError salIqCloseDataStream_X64(IntPtr dataStreamHandle);

        public static SalError salIqCloseDataStream(IntPtr dataStreamHandle)
        {
            if (IntPtr.Size == 8)
                return salIqCloseDataStream_X64(dataStreamHandle); // Call 64-bit DLL
            else
                return salIqCloseDataStream_Win32(dataStreamHandle); // Call 32-bit DLL
        }

        /*****************************************************************/
        /*  Stream functions (these take a stream handle)   (deprecated) */
        /*****************************************************************/
        // ===================== salIqSetParameters( ) (deprecated) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salIqSetParameters", CharSet = CharSet.Ansi)]
        private static extern SalError salIqSetParameters_Win32(IntPtr salDataStreamHandle, ref IqParameters iqParm);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salIqSetParameters", CharSet = CharSet.Ansi)]
        private static extern SalError salIqSetParameters_X64(IntPtr salDataStreamHandle, ref IqParameters iqParm);

        public static SalError salIqSetParameters(IntPtr salDataStreamHandle, ref IqParameters iqParm)
        {
            if (IntPtr.Size == 8)
                return salIqSetParameters_X64(salDataStreamHandle, ref iqParm); // Call 64-bit DLL
            else
                return salIqSetParameters_Win32(salDataStreamHandle, ref iqParm); // Call 32-bit DLL
        }

        // ===================== salIqStart( ) (deprecated) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salIqStart", CharSet = CharSet.Ansi)]
        private static extern SalError salIqStart_Win32(IntPtr salDataStreamHandle, ref IqArg arg);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salIqStart", CharSet = CharSet.Ansi)]
        private static extern SalError salIqStart_X64(IntPtr salDataStreamHandle, ref IqArg arg);

        public static SalError salIqStart(IntPtr salDataStreamHandle, ref IqArg arg)
        {
            if (IntPtr.Size == 8)
                return salIqStart_X64(salDataStreamHandle, ref arg); // Call 64-bit DLL
            else
                return salIqStart_Win32(salDataStreamHandle, ref arg); // Call 32-bit DLL
        }

        // ===================== salSetIqFlowControl( ) (deprecated) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSetIqFlowControl", CharSet = CharSet.Ansi)]
        private static extern SalError salSetIqFlowControl_Win32(IntPtr salDataStreamHandle, ref salFlowControl flowControl);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSetIqFlowControl", CharSet = CharSet.Ansi)]
        private static extern SalError salSetIqFlowControl_X64(IntPtr salDataStreamHandle, ref salFlowControl flowControl);

        public static SalError salSetIqFlowControl(IntPtr salDataStreamHandle, ref salFlowControl flowControl)
        {
            if (IntPtr.Size == 8)
                return salSetIqFlowControl_X64(salDataStreamHandle, ref flowControl); // Call 64-bit DLL
            else
                return salSetIqFlowControl_Win32(salDataStreamHandle, ref flowControl); // Call 32-bit DLL
        }

        // ===================== salIqStop( ) (deprecated) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salIqStop", CharSet = CharSet.Ansi)]
        private static extern SalError salIqStop_Win32(IntPtr salDataStreamHandle);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salIqStop", CharSet = CharSet.Ansi)]
        private static extern SalError salIqStop_X64(IntPtr salDataStreamHandle);

        public static SalError salIqStop(IntPtr salDataStreamHandle)
        {
            if (IntPtr.Size == 8)
                return salIqStop_X64(salDataStreamHandle); // Call 64-bit DLL
            else
                return salIqStop_Win32(salDataStreamHandle); // Call 32-bit DLL
        }

        // ===================== GetDataInt( ) (deprecated) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salIqGetData", CharSet = CharSet.Ansi)]
        private static extern SalError GetDataInt_Win32(IntPtr salDataStreamHandle, ref IqDataHeader iqHeader, int[] data, UInt32 maxDataBytes);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salIqGetData", CharSet = CharSet.Ansi)]
        private static extern SalError GetDataInt_X64(IntPtr salDataStreamHandle, ref IqDataHeader iqHeader, int[] data, UInt32 maxDataBytes);

        public static SalError GetDataInt(IntPtr salDataStreamHandle, ref IqDataHeader iqHeader, int[] data, UInt32 maxDataBytes)
        {
            if (IntPtr.Size == 8)
                return GetDataInt_X64(salDataStreamHandle, ref iqHeader, data, maxDataBytes); // Call 64-bit DLL
            else
                return GetDataInt_Win32(salDataStreamHandle, ref iqHeader, data, maxDataBytes); // Call 32-bit DLL
        }

        // ===================== salIqGetDataInt( ) (deprecated) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salIqGetData", CharSet = CharSet.Ansi)]
        private static extern SalError salIqGetDataInt_Win32(IntPtr salDataStreamHandle, ref IqDataHeader iqHeader, int[] data, UInt32 maxDataBytes);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salIqGetData", CharSet = CharSet.Ansi)]
        private static extern SalError salIqGetDataInt_X64(IntPtr salDataStreamHandle, ref IqDataHeader iqHeader, int[] data, UInt32 maxDataBytes);

        public static SalError salIqGetDataInt(IntPtr salDataStreamHandle, ref IqDataHeader iqHeader, int[] data, UInt32 maxDataBytes)
        {
            if (IntPtr.Size == 8)
                return salIqGetDataInt_X64(salDataStreamHandle, ref iqHeader, data, maxDataBytes); // Call 64-bit DLL
            else
                return salIqGetDataInt_Win32(salDataStreamHandle, ref iqHeader, data, maxDataBytes); // Call 32-bit DLL
        }

        private static int[] dataBufferInt = new int[MAX_SAMPLES_PER_TRANSFER_TCP * 2];
        /// <summary>
        /// Get 32 bit integer IQ Block data from the sensor. (deprecated)
        /// </summary>
        /// <param name="salDataStreamHandle"></param>
        /// <param name="dataHeader">Description of data</param>
        /// <param name="data">Data samples</param>
        /// <param name="numSamples">Number of valid samples returned</param>
        /// <returns>
        /// SAL_ERR_NONE if data is available
        /// </returns>
        /// 

        public static SalError salIqGetData(IntPtr salDataStreamHandle, ref IqDataHeader dataHeader, ref int[] data)
        {
            UInt32 maxBytes = (UInt32)dataBufferInt.Length * 4;

            SalError err = GetDataInt(salDataStreamHandle, ref dataHeader, dataBufferInt, maxBytes);

            if (err != SalError.SAL_ERR_NONE) return err;

            if (dataHeader.numSamples > 0)
            {
                if (dataHeader.dataType != DataType.Complex32)
                {
                    throw new Exception("salIqGetData: data not 32 bit integer");
                }

                data = dataBufferInt;
            }
            else
            {
                data = null;
            }

            return err;
        }

        // ===================== salIqGetDataShort( ) (deprecated) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salIqGetData", CharSet = CharSet.Ansi)]
        private static extern SalError salIqGetDataShort_Win32(IntPtr salDataStreamHandle, ref IqDataHeader iqHeader, short[] data, UInt32 maxDataBytes);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salIqGetData", CharSet = CharSet.Ansi)]
        private static extern SalError salIqGetDataShort_X64(IntPtr salDataStreamHandle, ref IqDataHeader iqHeader, short[] data, UInt32 maxDataBytes);

        public static SalError salIqGetDataShort(IntPtr salDataStreamHandle, ref IqDataHeader iqHeader, short[] data, UInt32 maxDataBytes)
        {
            if (IntPtr.Size == 8)
                return salIqGetDataShort_X64(salDataStreamHandle, ref iqHeader, data, maxDataBytes); // Call 64-bit DLL
            else
                return salIqGetDataShort_Win32(salDataStreamHandle, ref iqHeader, data, maxDataBytes); // Call 32-bit DLL
        }

        private static short[] dataBufferShort = new short[MAX_SAMPLES_PER_TRANSFER_TCP * 2];
        /// <summary>
        /// Get 16 bit integer IQ Block data from the sensor. (deprecated)
        /// </summary>
        /// <param name="salDataStreamHandle"></param>
        /// <param name="dataHeader">Description of data</param>
        /// <param name=atatype"data">Data samples</param>
        /// <returns>
        /// SAL_ERR_NONE if data is available
        /// </returns>
        public static SalError salIqGetData(IntPtr salDataStreamHandle, ref IqDataHeader dataHeader, ref short[] data)
        {
            UInt32 maxBytes = (UInt32)dataBufferShort.Length * 2;

            SalError err = salIqGetDataShort(salDataStreamHandle, ref dataHeader, dataBufferShort, maxBytes);
            if (err != SalError.SAL_ERR_NONE) return err;

            if (dataHeader.numSamples > 0)
            {
                if (dataHeader.dataType != AgSalLib.DataType.Complex16)
                {
                    throw new Exception("salIqGetData: data not 16 bit integer");
                }

                data = dataBufferShort;
            }
            else
            {
                data = null;
            }

            return err;
        }

        // ===================== salIqAdjustCenterFrequency( ) (deprecated) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salIqAdjustCenterFrequency", CharSet = CharSet.Ansi)]
        private static extern SalError salIqAdjustCenterFrequency_Win32(IntPtr salDataStreamHandle, double cfreq);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salIqAdjustCenterFrequency", CharSet = CharSet.Ansi)]
        private static extern SalError salIqAdjustCenterFrequency_X64(IntPtr salDataStreamHandle, double cfreq);

        public static SalError salIqAdjustCenterFrequency(IntPtr salDataStreamHandle, double cfreq)
        {
            if (IntPtr.Size == 8)
                return salIqAdjustCenterFrequency_X64(salDataStreamHandle, cfreq); // Call 64-bit DLL
            else
                return salIqAdjustCenterFrequency_Win32(salDataStreamHandle, cfreq); // Call 32-bit DLL
        }

        // ===================== salIqAdjustAttenuation( ) (deprecated) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salIqAdjustAttenuation", CharSet = CharSet.Ansi)]
        private static extern SalError salIqAdjustAttenuation_Win32(IntPtr salDataStreamHandle, double attenuation);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salIqAdjustAttenuation", CharSet = CharSet.Ansi)]
        private static extern SalError salIqAdjustAttenuation_X64(IntPtr salDataStreamHandle, double attenuation);

        public static SalError salIqAdjustAttenuation(IntPtr salDataStreamHandle, double attenuation)
        {
            if (IntPtr.Size == 8)
                return salIqAdjustAttenuation_X64(salDataStreamHandle, attenuation); // Call 64-bit DLL
            else
                return salIqAdjustAttenuation_Win32(salDataStreamHandle, attenuation); // Call 32-bit DLL
        }

        // ===================== salIqAdjustAntenna( ) (deprecated) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salIqAdjustAntenna", CharSet = CharSet.Ansi)]
        private static extern SalError salIqAdjustAntenna_Win32(IntPtr salDataStreamHandle, AntennaType antenna);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salIqAdjustAntenna", CharSet = CharSet.Ansi)]
        private static extern SalError salIqAdjustAntenna_X64(IntPtr salDataStreamHandle, AntennaType antenna);

        public static SalError salIqAdjustAntenna(IntPtr salDataStreamHandle, AntennaType antenna)
        {
            if (IntPtr.Size == 8)
                return salIqAdjustAntenna_X64(salDataStreamHandle, antenna); // Call 64-bit DLL
            else
                return salIqAdjustAntenna_Win32(salDataStreamHandle, antenna); // Call 32-bit DLL
        }

        // ===================== salIqSendCommand( ) (deprecated) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salIqSendCommand", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salIqSendCommand_Win32(IntPtr salDataStreamHandle, IqCommand command);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salIqSendCommand", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salIqSendCommand_X64(IntPtr salDataStreamHandle, IqCommand command);

        public static SalError salIqSendCommand(IntPtr salDataStreamHandle, IqCommand command)
        {
            if (IntPtr.Size == 8)
                return salIqSendCommand_X64(salDataStreamHandle, command); // Call 64-bit DLL
            else
                return salIqSendCommand_Win32(salDataStreamHandle, command); // Call 32-bit DLL
        }

        // ===================== salIqGetAttribute( ) (deprecated)==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salIqGetAttribute", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salIqGetAttribute_Win32(IntPtr salDataStreamHandle, IqAttribute attribute, ref int val);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salIqGetAttribute", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salIqGetAttribute_X64(IntPtr salDataStreamHandle, IqAttribute attribute, ref int val);

        public static SalError salIqGetAttribute(IntPtr salDataStreamHandle, IqAttribute attribute, ref int val)
        {
            if (IntPtr.Size == 8)
                return salIqGetAttribute_X64(salDataStreamHandle, attribute, ref val); // Call 64-bit DLL
            else
                return salIqGetAttribute_Win32(salDataStreamHandle, attribute, ref val); // Call 32-bit DLL
        }

        // ===================== salIqGetAttribute( ) (deprecated) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salIqGetAttribute", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salIqGetAttribute_Win32(IntPtr salDataStreamHandle, IqAttribute attribute, ref double val);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salIqGetAttribute", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salIqGetAttribute_X64(IntPtr salDataStreamHandle, IqAttribute attribute, ref double val);

        public static SalError salIqGetAttribute(IntPtr salDataStreamHandle, IqAttribute attribute, ref double val)
        {
            if (IntPtr.Size == 8)
                return salIqGetAttribute_X64(salDataStreamHandle, attribute, ref val); // Call 64-bit DLL
            else
                return salIqGetAttribute_Win32(salDataStreamHandle, attribute, ref val); // Call 32-bit DLL
        }

        /**********************************************************/
        /*  Geolocation functions                                 */
        /**********************************************************/

        public enum LocateStatus
        {
            NONE = 0,
            QUEUED = 1,  /**< Measurement not finished */
            MEASURING = 2,  /**< Measurement finished successfully */
            DONE = 3,  /**< Measurement was aborted by user command */
        }

        public enum Priority
        {
            LOW = 0,   /**< Lowest priority */
            MEDIUM = 1,   /**< Medium priority */
            HIGH = 2,   /**< High priority */
            CRITICAL = 3    /**< Highest priority */
        }

        public enum LocateOutput
        {
            All = 0,    /**< [out]    No results are available */
            None = 0,    /**< [in]     Request all outputs */
            CORRELATION = 1,	/**< Include Correlations */
            SPECTRUM = 2,	/**< Include Spectra */
            IMAGE = 4,	/**< Include Tentagram (PNG) */
            LOCATION = 8,	/**< Include "X marks the spot" */
            TIMESERIES = 16,   /**< [out]    Request timeseries info / timeseries info available */
            TIME_DATA = 32,   /**< [in|out]    Request timeseries data / timeseries data available */
        }

        public enum DopplerCompensation
        {
            OFF = 0, /**< disable doppler compensation (default) */
            ON = 1, /**< Force doppler compensation independent of other measurement parms */
            AUTO = 2  /**< Signal assumed to be moving at >20kph, Compensation applied if required */
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Region
        {
            public double north;
            public double south;
            public double east;
            public double west;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Region2
        {
            public double north;
            public double south;
            public double east;
            public double west;
            public double altitude;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LocateImageParms       // this describes the image that you want
        {
            public UInt32 width;		     /**< Number of location image pixels, X direction. 0 means "use default" */
            public UInt32 height;		     /**< Number of location image pixels, Y direction. 0 means "use default" */
            public Int32 reserved;

        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LocateImageHeader       // this describes the image that you got
        {
            public Region region;
            public UInt32 width;		      /**< Number of location image pixels, X direction. 0 means "use default" */
            public UInt32 height;		      /**< Number of location image pixels, Y direction. 0 means "use default" */
            public UInt32 numBytes;
            public Int32 reserved;
        }

        // old version of LocateResultParms; does not support geolocationAlgorithm
        [StructLayout(LayoutKind.Sequential)]
        public struct LocateResultParms
        {
            public Region region;
            public LocateImageParms image;
            public LocateOutput outputTypes;              /**< Type of output requested */
            public DopplerCompensation dopplerComp;		/**< Doppler compensation OFF/ON/AUTO */
            public IntPtr userWorkspace;

            public UInt32 numSensorsMax;
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int SAL_LOCATE_CALLBACK(IntPtr salLocateHandle, SalError status, IntPtr userWorkspace);

        [StructLayout(LayoutKind.Sequential)]
        public struct LocateResult
        {
            public SalError errorCode;	   /**< Error code */
            public Int32 measurementId;	   /**< Measurement internal identifier */
            public Int32 userWorkspace;	   /**< Request ID as passed in by the caller */
            public Location location;	   /**< Computed transmitter location */
            public double resultQuality;   /**< A quality indicator for the result (0..1) */
            public Int32 numSensors;       /**< how many sensors were used to make the measurement*/
            public Int32 numImageBytes;	   /**< Size (in bytes) of location measurement image */
            public LocateOutput validData; /* Bitmask of available data types for this measurement */

            public bool isLocationValid { get { return (validData & LocateOutput.LOCATION) != 0; } }
            public bool isImageValid { get { return (validData & LocateOutput.IMAGE) != 0; } }
            public bool isSpectraValid { get { return (validData & LocateOutput.SPECTRUM) != 0; } }
            public bool isCorrelationValid { get { return (validData & LocateOutput.CORRELATION) != 0; } }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LocateMeasParms
        {
            public IntPtr sensorGroupHandle; /**< Handle to the group of sensors to use for the measurement. 0 means use all sensors configured for TDOA. */
            public double frequency;
            public double bandwidth;
            public AntennaType antenna;
            public double attenuation;       /**< Attenuation in dB relative to measured optimum attenuation */
            public UInt32 numSamples;        /**< Number of samples to use for the measurement */
            public TriggerType triggerType;  /**< The type of triggering used for this measurement  */
            public double triggerLevel;      /**< Units are db or dBm depending on the triggerType */
            public Int32 triggerTimeSecs;   /**< Whole seconds part of trigger time; used only when using time trigger */
            public Int32 triggerTimeNSecs;
            public double timeout;
            public Priority priority;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct TimeseriesInfo
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_NAME)]
            public string sensorName;  /**< Sensor name */
            public Location location;                  /**< Location of sensor whe measurement was made */
            public double centerFrequency;
            public double sampleRate;
            public double bandwidth;
            public double recordLength;
            public UInt32 numSamples;                /**< Number of samples in the timeseries */
            public double triggerLevel;              /**< Trigger level for the timeseries */
            public double attenuation;               /**< The amount of attenuation the sensor used in dB */
            public UInt32 overload;                  /**< 1 if the sensor overloaded during the timeseries, 0 otherwise */
            public UInt32 timestampSeconds;          /**< Whole seconds part of the time timeseries start time */
            public UInt32 timestampNSeconds;         /**< Fractional seconds part of the time timeseries start time, in nano-seconds */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_NAME)]
            public string triggerSource; /**< Name of sensor that caused this sensor to trigger */
            public double triggerLatency;            /**< Time difference in seconds between when the triggerSource initiated the trigger and when this sensor triggered  */
            public double variance1588;              /**< Variance of IEEE-1588 time */

            public bool isOverload { get { return (overload != 0); } }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct LocateCorrelation
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_NAME)]
            public string sensorName1;	/**< Name of the first sensor involved in this correlation */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_NAME)]
            public string sensorName2;	/**< Name of the second sensor involved in this correlation */
            public double firstX;		/**< X value in seconds of first point */
            public double deltaX;		/**< Step in seconds between X values */
            public Int32 numPoints;		/**< Number of points */

        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct LocateSpectrum
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_NAME)]
            public string sensorName;		/**< Name of the sensor that captured the time series with this spectrum */
            public double centerFrequency;
            public double span;
            public UInt32 numPoints;
        }

        // ===================== salOpenSensorGroup( ) (deprecated) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salOpenSensorGroup", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salOpenSensorGroup_Win32(out IntPtr sensorGroupHandle);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salOpenSensorGroup", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salOpenSensorGroup_X64(out IntPtr sensorGroupHandle);

        public static SalError salOpenSensorGroup(out IntPtr sensorGroupHandle)
        {
            if (IntPtr.Size == 8)
                return salOpenSensorGroup_X64(out sensorGroupHandle);
            else
                return salOpenSensorGroup_Win32(out sensorGroupHandle);
        }

        // ===================== salOpenSensorGroup2( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salOpenSensorGroup2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salOpenSensorGroup2_Win32(out IntPtr sensorGroupHandle, IntPtr smsHandle);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salOpenSensorGroup2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salOpenSensorGroup2_X64(out IntPtr sensorGroupHandle, IntPtr smsHandle);

        public static SalError salOpenSensorGroup2(out IntPtr sensorGroupHandle, IntPtr smsHandle)
        {
            if (IntPtr.Size == 8)
                return salOpenSensorGroup2_X64(out sensorGroupHandle, smsHandle);
            else
                return salOpenSensorGroup2_Win32(out sensorGroupHandle, smsHandle);
        }

        // ===================== salSensorGroupAddSensor( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSensorGroupAddSensor", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSensorGroupAddSensor_Win32(IntPtr sensorGroupHandle, string sensorName);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSensorGroupAddSensor", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSensorGroupAddSensor_X64(IntPtr sensorGroupHandle, string sensorName);

        public static SalError salSensorGroupAddSensor(IntPtr sensorGroupHandle, string sensorName)
        {
            if (IntPtr.Size == 8)
                return salSensorGroupAddSensor_X64(sensorGroupHandle, sensorName);
            else
                return salSensorGroupAddSensor_Win32(sensorGroupHandle, sensorName);
        }

        // ===================== salSensorGroupAddSensorbyHandle( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSensorGroupAddSensorbyHandle", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSensorGroupAddSensorbyHandle_Win32(IntPtr sensorGroupHandle, IntPtr sensorHandle);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSensorGroupAddSensorbyHandle", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSensorGroupAddSensorbyHandle_X64(IntPtr sensorGroupHandle, IntPtr sensorHandle);

        public static SalError salSensorGroupAddSensorbyHandle(IntPtr sensorGroupHandle, IntPtr sensorHandle)
        {
            if (IntPtr.Size == 8)
                return salSensorGroupAddSensorbyHandle_X64(sensorGroupHandle, sensorHandle);
            else
                return salSensorGroupAddSensorbyHandle_Win32(sensorGroupHandle, sensorHandle);
        }

        // ===================== salSensorGroupGetNumber( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSensorGroupGetNumber", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSensorGroupGetNumber_Win32(IntPtr sensorGroupHandle, out UInt32 numSensors);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSensorGroupGetNumber", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSensorGroupGetNumber_X64(IntPtr sensorGroupHandle, out UInt32 numSensors);

        public static SalError salSensorGroupGetNumber(IntPtr sensorGroupHandle, out UInt32 numSensors)
        {
            if (IntPtr.Size == 8)
                return salSensorGroupGetNumber_X64(sensorGroupHandle, out numSensors);
            else
                return salSensorGroupGetNumber_Win32(sensorGroupHandle, out numSensors);
        }

        // ===================== salSensorGroupGetName( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSensorGroupGetName", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSensorGroupGetName_Win32(IntPtr sensorGroupHandle, UInt32 sensorIndex, StringBuilder sensorNameSB);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSensorGroupGetName", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSensorGroupGetName_X64(IntPtr sensorGroupHandle, UInt32 sensorIndex, StringBuilder sensorNameSB);

        public static SalError salSensorGroupGetName(IntPtr sensorGroupHandle, UInt32 sensorIndex, out string sensorName)
        {
            SalError err;
            StringBuilder sensorNameSB = new StringBuilder(MAX_SENSOR_NAME);
            if (IntPtr.Size == 8)
            {
                err = salSensorGroupGetName_X64(sensorGroupHandle, sensorIndex, sensorNameSB); // Call 64-bit DLL
            }
            else
            {
                err = salSensorGroupGetName_Win32(sensorGroupHandle, sensorIndex, sensorNameSB); // Call 32-bit DLL
            }

            sensorName = sensorNameSB.ToString();
            return err;
        }

        // ===================== salMeasureLocation( ) (deprecated) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salMeasureLocation", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salMeasureLocation_Win32(out IntPtr locateHandle, ref LocateMeasParms measParms, ref LocateResultParms resultParms, SAL_LOCATE_CALLBACK callback);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salMeasureLocation", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salMeasureLocation_X64(out IntPtr locateHandle, ref LocateMeasParms measParms, ref LocateResultParms resultParms, SAL_LOCATE_CALLBACK callback);

        public static SalError salMeasureLocation(out IntPtr locateHandle, ref LocateMeasParms measParms, ref LocateResultParms resultParms, SAL_LOCATE_CALLBACK callback)
        {
            if (IntPtr.Size == 8)
                return salMeasureLocation_X64(out locateHandle, ref measParms, ref resultParms, callback);
            else
                return salMeasureLocation_Win32(out locateHandle, ref measParms, ref resultParms, callback);
        }

        // ===================== salRecalculateLocation( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salRecalculateLocation", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salRecalculateLocation_Win32(out IntPtr salLocateHandle, Int32 measurementId, ref LocateResultParms parms, SAL_LOCATE_CALLBACK callback);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salRecalculateLocation", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salRecalculateLocation_X64(out IntPtr salLocateHandle, Int32 measurementId, ref LocateResultParms parms, SAL_LOCATE_CALLBACK callback);

        public static SalError salRecalculateLocation(out IntPtr salLocateHandle, Int32 measurementId, ref LocateResultParms parms, SAL_LOCATE_CALLBACK callback)
        {
            if (IntPtr.Size == 8)
                return salRecalculateLocation_X64(out salLocateHandle, measurementId, ref parms, callback);
            else
                return salRecalculateLocation_Win32(out salLocateHandle, measurementId, ref parms, callback);
        }

        // ===================== salGetLocationStatus( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetLocationStatus", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetLocationStatus_Win32(IntPtr salLocateHandle, out LocateStatus status);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetLocationStatus", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetLocationStatus_X64(IntPtr salLocateHandle, out LocateStatus status);

        public static SalError salGetLocationStatus(IntPtr salLocateHandle, out LocateStatus status)
        {
            if (IntPtr.Size == 8)
                return salGetLocationStatus_X64(salLocateHandle, out status);
            else
                return salGetLocationStatus_Win32(salLocateHandle, out status);
        }

        // ===================== salGetLocationResult( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetLocationResult", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetLocationResult_Win32(IntPtr salLocateHandle, out LocateResult result);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetLocationResult", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetLocationResult_X64(IntPtr salLocateHandle, out LocateResult result);

        public static SalError salGetLocationResult(IntPtr salLocateHandle, out LocateResult result)
        {
            if (IntPtr.Size == 8)
                return salGetLocationResult_X64(salLocateHandle, out result);
            else
                return salGetLocationResult_Win32(salLocateHandle, out result);
        }

        // ===================== salGetSpectrum( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetSpectrum", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSpectrum_Win32(IntPtr handle, UInt32 index, out LocateSpectrum spec, float[] buffer, UInt32 numBytesInBuffer);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetSpectrum", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSpectrum_X64(IntPtr handle, UInt32 index, out LocateSpectrum spec, float[] buffer, UInt32 numBytesInBuffer);

        public static SalError salGetSpectrum(IntPtr handle, UInt32 index, out LocateSpectrum spec, float[] buffer, UInt32 numBytesInBuffer)
        {
            if (IntPtr.Size == 8)
                return salGetSpectrum_X64(handle, index, out spec, buffer, numBytesInBuffer);
            else
                return salGetSpectrum_Win32(handle, index, out spec, buffer, numBytesInBuffer);
        }

        // ===================== salGetCorrelation( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetCorrelation", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetCorrelation_Win32(IntPtr salLocateHandle, UInt32 index1,
            UInt32 index2, out LocateCorrelation corr, float[] data, UInt32 numBytesInBuffer);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetCorrelation", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetCorrelation_X64(IntPtr salLocateHandle, UInt32 index1,
            UInt32 index2, out LocateCorrelation corr, float[] data, UInt32 numBytesInBuffer);

        public static SalError salGetCorrelation(IntPtr salLocateHandle, UInt32 index1,
            UInt32 index2, out LocateCorrelation corr, float[] data, UInt32 numBytesInBuffer)
        {
            if (IntPtr.Size == 8)
                return salGetCorrelation_X64(salLocateHandle, index1, index2, out corr, data, numBytesInBuffer);
            else
                return salGetCorrelation_Win32(salLocateHandle, index1, index2, out corr, data, numBytesInBuffer);
        }

        // ===================== salGetLocationImage( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetLocationImage", CharSet = CharSet.Ansi)]
        private static extern SalError salGetLocationImage_Win32(IntPtr handle, out LocateImageHeader imageInfo, byte[] data, UInt32 maxDataBytes);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetLocationImage", CharSet = CharSet.Ansi)]
        private static extern SalError salGetLocationImage_X64(IntPtr handle, out LocateImageHeader imageInfo, byte[] data, UInt32 maxDataBytes);


        private static byte[] imageBuffer = new byte[1024 * 1024];

        public static SalError salGetLocationImage(IntPtr handle, out LocateImageHeader imageInfo, ref byte[] buffer, UInt32 numBytesInBuffer)
        {
            UInt32 maxBytes = numBytesInBuffer;

            SalError err;

            if (IntPtr.Size == 8)
                err = salGetLocationImage_X64(handle, out imageInfo, imageBuffer, maxBytes);
            else
                err = salGetLocationImage_Win32(handle, out imageInfo, imageBuffer, maxBytes);

            if (err != SalError.SAL_ERR_NONE) return err;

            buffer = imageBuffer;

            return err;
        }

        // ===================== salGetTimeseriesInfo( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetTimeseriesInfo", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetTimeseriesInfo_Win32(IntPtr salLocateHandle, UInt32 sensorIndex, out TimeseriesInfo info);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetTimeseriesInfo", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetTimeseriesInfo_X64(IntPtr salLocateHandle, UInt32 sensorIndex, out TimeseriesInfo info);

        public static SalError salGetTimeseriesInfo(IntPtr salLocateHandle, UInt32 sensorIndex, out TimeseriesInfo info)
        {
            if (IntPtr.Size == 8)
                return salGetTimeseriesInfo_X64(salLocateHandle, sensorIndex, out info);
            else
                return salGetTimeseriesInfo_Win32(salLocateHandle, sensorIndex, out info);
        }

        // ===================== salGetLocationQueueStatus( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetLocationQueueStatus", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetLocationQueueStatus_Win32(out UInt32 numMeasQueued, out UInt32 numMeasTotal);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetLocationQueueStatus", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetLocationQueueStatus_X64(out UInt32 numMeasQueued, out UInt32 numMeasTotal);

        public static SalError salGetLocationQueueStatus(out UInt32 numMeasQueued, out UInt32 numMeasTotal)
        {
            if (IntPtr.Size == 8)
                return salGetLocationQueueStatus_X64(out numMeasQueued, out numMeasTotal);
            else
                return salGetLocationQueueStatus_Win32(out numMeasQueued, out numMeasTotal);
        }

        // ===================== salLocateHistoryOpen( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salLocateHistoryOpen", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salLocateHistoryOpen_Win32(out IntPtr salListHandle, IntPtr smsHandle, UInt32 firstEntry, UInt32 numEntries,
            out UInt32 actualNumEntries, out UInt32 totalMeasurements, out UInt32 maxMeasurementId, out UInt32 minMeasurementId);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salLocateHistoryOpen", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salLocateHistoryOpen_X64(out IntPtr salListHandle, IntPtr smsHandle, UInt32 firstEntry, UInt32 numEntries,
            out UInt32 actualNumEntries, out UInt32 totalMeasurements, out UInt32 maxMeasurementId, out UInt32 minMeasurementId);

        public static SalError salLocateHistoryOpen(out IntPtr salListHandle, IntPtr smsHandle, UInt32 firstEntry, UInt32 numEntries,
            out UInt32 actualNumEntries, out UInt32 totalMeasurements, out UInt32 maxMeasurementId, out UInt32 minMeasurementId)
        {
            if (IntPtr.Size == 8)
                return salLocateHistoryOpen_X64(out salListHandle, smsHandle, firstEntry, numEntries,
                    out actualNumEntries, out totalMeasurements, out maxMeasurementId, out minMeasurementId);
            else
                return salLocateHistoryOpen_Win32(out salListHandle, smsHandle, firstEntry, numEntries,
                    out actualNumEntries, out totalMeasurements, out maxMeasurementId, out minMeasurementId);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct HistoricMeasurementDescription
        {
            public int measId;
            public int armTimeSeconds;
            public int armTimeNSeconds;
            public double frequency;
            public double span;
            public int samples;
            public int numSeries;
            public int validCaptures;
            public int errorCaptures;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_COMMENT)]
            public string comment;
        };

        // ===================== salLocateHistoryGetNext( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salLocateHistoryGetNext", CharSet = CharSet.Ansi)]
        private static extern SalError salLocateHistoryGetNext_Win32(IntPtr salListHandle, out HistoricMeasurementDescription result);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salLocateHistoryGetNext", CharSet = CharSet.Ansi)]
        private static extern SalError salLocateHistoryGetNext_X64(IntPtr salListHandle, out HistoricMeasurementDescription result);

        public static SalError salLocateHistoryGetNext(IntPtr salListHandle, out HistoricMeasurementDescription result)
        {
            if (IntPtr.Size == 8)
                return salLocateHistoryGetNext_X64(salListHandle, out result); // Call 64-bit DLL
            else
                return salLocateHistoryGetNext_Win32(salListHandle, out result); // Call 32-bit DLL
        }

        // ===================== salSaveLocationData( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSaveLocationData", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSaveLocationData_Win32(Int32 id, string comment);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSaveLocationData", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSaveLocationData_X64(Int32 id, string comment);

        public static SalError salSaveLocationData(Int32 id, string comment)
        {
            if (IntPtr.Size == 8)
                return salSaveLocationData_X64(id, comment);
            else
                return salSaveLocationData_Win32(id, comment);
        }

        // ===================== salDeleteLocationData( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salDeleteLocationData", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salDeleteLocationData_Win32(Int32 id);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salDeleteLocationData", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salDeleteLocationData_X64(Int32 id);

        public static SalError salDeleteLocationData(Int32 id)
        {
            if (IntPtr.Size == 8)
                return salDeleteLocationData_X64(id);
            else
                return salDeleteLocationData_Win32(id);
        }

        // ===================== salClose( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salClose", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern SalError salClose_Win32(IntPtr handle);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salClose", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern SalError salClose_X64(IntPtr handle);

        public static SalError salClose(IntPtr handle)
        {
            if (IntPtr.Size == 8)
                return salClose_X64(handle); // Call 64-bit DLL
            else
                return salClose_Win32(handle); // Call 32-bit DLL
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SensorInfo
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_HOSTNAME)]
            public string macAddress;          /**< Media access control address as a string (for example "123456789abc"). */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_HOSTNAME)]
            public string modelNumber;         /**< Model number of sensor (for example "N6841A").*/
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_HOSTNAME)]
            public string serialNumber;        /**< Serial number of sensor (for example "A-N6841A-50001").*/
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_HOSTNAME)]
            public string hostName;            /**< Hostname of the  sensor.*/
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_HOSTNAME)]
            public string ipAddress;           /**< Internet protocol address of the sensor (for example "192.168.1.101").*/
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_HOSTNAME)]
            public string smsAddress;          /**< Host name or IP address of last SMS that this sensor was assigned to.*/
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_HOSTNAME)]
            public string revision;            /**< Firmware and FPGA revision information.*/
        };

        // ===================== salDiscoverSensors( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salDiscoverSensors", CharSet = CharSet.Ansi)]
        private static extern SalError salDiscoverSensors_Win32(out IntPtr discoveryHandle, IntPtr smsHandle, out UInt32 numSensors);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salDiscoverSensors", CharSet = CharSet.Ansi)]
        private static extern SalError salDiscoverSensors_X64(out IntPtr discoveryHandle, IntPtr smsHandle, out UInt32 numSensors);

        public static SalError salDiscoverSensors(out IntPtr discoveryHandle, IntPtr smsHandle, out UInt32 numSensors)
        {
            if (IntPtr.Size == 8)
                return salDiscoverSensors_X64(out discoveryHandle, smsHandle, out numSensors); // Call 64-bit DLL
            else
                return salDiscoverSensors_Win32(out discoveryHandle, smsHandle, out numSensors); // Call 32-bit DLL
        }

        // ===================== salGetNextDiscoveredSensor( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetNextDiscoveredSensor", CharSet = CharSet.Ansi)]
        private static extern SalError salGetNextDiscoveredSensor_Win32(IntPtr discoveredList, out SensorInfo sensorInfo);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetNextDiscoveredSensor", CharSet = CharSet.Ansi)]
        private static extern SalError salGetNextDiscoveredSensor_X64(IntPtr discoveredList, out SensorInfo sensorInfo);

        public static SalError salGetNextDiscoveredSensor(IntPtr discoveredList, out SensorInfo sensorInfo)
        {
            if (IntPtr.Size == 8)
                return salGetNextDiscoveredSensor_X64(discoveredList, out sensorInfo); // Call 64-bit DLL
            else
                return salGetNextDiscoveredSensor_Win32(discoveredList, out sensorInfo); // Call 32-bit DLL
        }

        // ===================== salAddSensor( ) (deprecated) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salAddSensor", CharSet = CharSet.Ansi)]
        private static extern SalError salAddSensor_Win32(IntPtr smsHandle, string hostname, string sensorName);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salAddSensor", CharSet = CharSet.Ansi)]
        private static extern SalError salAddSensor_X64(IntPtr smsHandle, string hostname, string sensorName);

        public static SalError salAddSensor(IntPtr smsHandle, string hostname, string sensorName)
        {
            SalError err;

            if (IntPtr.Size == 8)
                err = salAddSensor_X64(smsHandle, hostname, sensorName); // Call 64-bit DLL
            else
                err = salAddSensor_Win32(smsHandle, hostname, sensorName); // Call 32-bit DLL

            return err;
        }

        // ===================== salAddSensor2( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salAddSensor2", CharSet = CharSet.Ansi)]
        private static extern SalError salAddSensor2_Win32(IntPtr smsHandle, string hostname, UInt16 port, string sensorName);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salAddSensor2", CharSet = CharSet.Ansi)]
        private static extern SalError salAddSensor2_X64(IntPtr smsHandle, string hostname, UInt16 port, string sensorName);

        public static SalError salAddSensor2(IntPtr smsHandle, string hostname, UInt16 port, string sensorName)
        {
            SalError err;

            if (IntPtr.Size == 8)
                err = salAddSensor2_X64(smsHandle, hostname, port, sensorName); // Call 64-bit DLL
            else
                err = salAddSensor2_Win32(smsHandle, hostname, port, sensorName); // Call 32-bit DLL

            return err;
        }



        // ===================== salRemoveSensor( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salRemoveSensor", CharSet = CharSet.Ansi)]
        private static extern SalError salRemoveSensor_Win32(IntPtr smsHandle, string sensorName);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salRemoveSensor", CharSet = CharSet.Ansi)]
        private static extern SalError salRemoveSensor_X64(IntPtr smsHandle, string sensorName);

        public static SalError salRemoveSensor(IntPtr smsHandle, string sensorName)
        {
            SalError err;

            if (IntPtr.Size == 8)
                err = salRemoveSensor_X64(smsHandle, sensorName); // Call 64-bit DLL
            else
                err = salRemoveSensor_Win32(smsHandle, sensorName); // Call 32-bit DLL

            return err;
        }

        // ===================== salForceSmsGarbageCollection( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salForceSmsGarbageCollection", CharSet = CharSet.Ansi)]
        private static extern SalError salForceSmsGarbageCollection_Win32(IntPtr smsHandle);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salForceSmsGarbageCollection", CharSet = CharSet.Ansi)]
        private static extern SalError salForceSmsGarbageCollection_X64(IntPtr smsHandle);

        public static SalError salForceSmsGarbageCollection(IntPtr smsHandle)
        {
            SalError err;

            if (IntPtr.Size == 8)
                err = salForceSmsGarbageCollection_X64(smsHandle); // Call 64-bit DLL
            else
                err = salForceSmsGarbageCollection_Win32(smsHandle); // Call 32-bit DLL

            return err;
        }

        public enum Resource
        {
            Sensor = 0x1, /**< Lock all sensor resources */
            Tuner = 0x2, /**< Lock the tuner (center frequency, sample rate, antenna, attenuation) */
            Fft = 0x4, /**< Lock the FFT measurement engine */
            TimeData = 0x8  /**< Lock the time data engine (DDC) */
        }

        public enum Lock
        {
            salLock_none = 0,
            salLock_exclusive = 1
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct LockInfo
        {
            public Lock type;              /**< The type of lock */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SESSION_ID)]
            public string sessionId;         /**< String identifying the owner of the lock */
            public UInt32 timestampSeconds;  /**< Integer part of the time the lock was set (in UTC seconds since January 1, 1970). */
            public UInt32 timestampNSeconds; /**< Fractional part of the the lock was set. */
        }

        // ===================== salIqGetDataShort( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salLockResource", CharSet = CharSet.Ansi)]
        private static extern SalError salLockResource_Win32(IntPtr sensorHandle, Resource resource);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salLockResource", CharSet = CharSet.Ansi)]
        private static extern SalError salLockResource_X64(IntPtr sensorHandle, Resource resource);

        public static SalError salLockResource(IntPtr sensorHandle, Resource resource)
        {
            if (IntPtr.Size == 8)
                return salLockResource_X64(sensorHandle, resource); // Call 64-bit DLL
            else
                return salLockResource_Win32(sensorHandle, resource); // Call 32-bit DLL
        }

        // ===================== salUnlockResource( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salUnlockResource", CharSet = CharSet.Ansi)]
        private static extern SalError salUnlockResource_Win32(IntPtr sensorHandle, Resource resource);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salUnlockResource", CharSet = CharSet.Ansi)]
        private static extern SalError salUnlockResource_X64(IntPtr sensorHandle, Resource resource);

        public static SalError salUnlockResource(IntPtr sensorHandle, Resource resource)
        {
            if (IntPtr.Size == 8)
                return salUnlockResource_X64(sensorHandle, resource); // Call 64-bit DLL
            else
                return salUnlockResource_Win32(sensorHandle, resource); // Call 32-bit DLL
        }

        // ===================== salAbortAll( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salAbortAll", CharSet = CharSet.Ansi)]
        private static extern SalError salAbortAll_Win32(IntPtr sensorHandle);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salAbortAll", CharSet = CharSet.Ansi)]
        private static extern SalError salAbortAll_X64(IntPtr sensorHandle);

        public static SalError salAbortAll(IntPtr sensorHandle)
        {
            if (IntPtr.Size == 8)
                return salAbortAll_X64(sensorHandle); // Call 64-bit DLL
            else
                return salAbortAll_Win32(sensorHandle); // Call 32-bit DLL
        }

        // ==================== salBreakResourceLock() ============================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salBreakResourceLock", CharSet = CharSet.Ansi)]
        private static extern SalError salBreakResourceLock_Win32(IntPtr sensorHandle, Resource resource);


        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salBreakResourceLock", CharSet = CharSet.Ansi)]
        private static extern SalError salBreakResourceLock_X64(IntPtr sensorHandle, Resource resource);

        public static SalError salBreakResourceLock(IntPtr sensorHandle, Resource resource)
        {
            if (IntPtr.Size == 8)
                return salBreakResourceLock_X64(sensorHandle, resource); // Call 64-bit DLL
            else
                return salBreakResourceLock_Win32(sensorHandle, resource);
        }

        // ===================== salQuesryResource( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salQueryResource", CharSet = CharSet.Ansi)]
        private static extern SalError salQueryResource_Win32(IntPtr sensorHandle, Resource resource, out LockInfo lockInfo);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salQueryResource", CharSet = CharSet.Ansi)]
        private static extern SalError salQueryResource_X64(IntPtr sensorHandle, Resource resource, out LockInfo lockInfo);

        public static SalError salQueryResource(IntPtr sensorHandle, Resource resource, out LockInfo lockInfo)
        {
            if (IntPtr.Size == 8)
                return salQueryResource_X64(sensorHandle, resource, out lockInfo); // Call 64-bit DLL
            else
                return salQueryResource_Win32(sensorHandle, resource, out lockInfo); // Call 32-bit DLL
        }

        public enum TimeSync
        {
            TimeSync_unknown = -1,
            TimeSync_none,
            TimeSync_gps,          /**< GPS */
            TimeSync_ieee1588,     /**< IEEE 1588 */
            TimeSync_internal,     /**<  internal clock */
            TimeSync_gps_1588gm,   /**< GPS / IEEE 1588 Grand Master */
            TimeSync_num_modes     /* number of enums starting at 0 */
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct TimeInfo
        {
            public UInt32 timestampSeconds;            /**< Integer part of the timestamp (in UTC seconds since January 1, 1970). */
            public UInt32 timestampNSeconds;           /**< Fractional part of the timestamp (in Nanoseconds). */
            public UInt32 isNotSynced;                 /**< If nonzero, the clock is not synced. */

            public TimeInfo(UInt32 sec, UInt32 nsec)
            {
                timestampSeconds = sec;
                timestampNSeconds = nsec;
                while (timestampNSeconds > 1000000000)
                {
                    timestampNSeconds -= 1000000000;
                    timestampSeconds++;
                }
                isNotSynced = 0;
            }

            public TimeInfo(UInt32 sec, UInt32 nsec, UInt32 notSynced)
            {
                timestampSeconds = sec;
                timestampNSeconds = nsec;
                while (timestampNSeconds > 1000000000)
                {
                    timestampNSeconds -= 1000000000;
                    timestampSeconds++;
                }
                isNotSynced = notSynced;
            }

            public void set(UInt32 sec, UInt32 nsec, UInt32 notSynced)
            {
                timestampSeconds = sec;
                timestampNSeconds = nsec;
                while (timestampNSeconds > 1000000000)
                {
                    timestampNSeconds -= 1000000000;
                    timestampSeconds++;
                }
                isNotSynced = notSynced;
            }

            public void Add(TimeDuration delta)
            {
                if (delta.sign > 0)
                {
                    // Add
                    timestampSeconds += delta.timestampSeconds;
                    timestampNSeconds += delta.timestampNSeconds;
                    while (timestampNSeconds > 1000000000)
                    {
                        timestampNSeconds -= 1000000000;
                        timestampSeconds++;
                    }
                }
                else
                {
                    // Subtract
                    if (timestampNSeconds < delta.timestampNSeconds)
                    {
                        timestampNSeconds += 1000000000;
                        timestampSeconds--;
                    }
                    timestampNSeconds -= delta.timestampNSeconds;
                    if (timestampSeconds > delta.timestampSeconds)
                        timestampSeconds -= delta.timestampSeconds;
                    else
                    {
                        // Illegal operation
                        throw new Exception("Time value can't be represents");
                    }
                }
            }

            // Subtract: C =  A - B
            // Where A == "this"
            public TimeDuration subtract(TimeInfo B)
            {
                TimeDuration rtn = new TimeDuration();
                uint secA = timestampSeconds;
                uint nsecA = timestampNSeconds;
                uint secB = B.timestampSeconds;
                uint nsecB = B.timestampNSeconds;

                if (secA > secB ||
                   (secA == secB && nsecA > nsecB))
                {
                    // A > B
                    if (nsecA < nsecB)
                    {
                        if (secA == 0)
                        {
                            throw new Exception("Time value can't subtract");
                        }
                        nsecA += 1000000000;
                        secA--;
                    }
                    rtn.timestampSeconds = secA - secB;
                    rtn.timestampNSeconds = nsecA - nsecB;
                    rtn.sign = +1;
                }
                else
                {
                    // A < B
                    if (nsecA > nsecB)
                    {
                        if (secB == 0)
                        {
                            throw new Exception("Time value can't subtract");
                        }
                        nsecB += 1000000000;
                        secB--;
                    }
                    rtn.timestampSeconds = secB - secA;
                    rtn.timestampNSeconds = nsecB - nsecA;
                    rtn.sign = -1;
                }

                return rtn;
            }

            override public String ToString()
            {
                double nsec = timestampNSeconds / 1e9;
                String rtn = timestampSeconds + String.Format("{0:#.000000000}", nsec);
                return rtn;
            }
        }

        // ===================== salGetSensorTime( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetSensorTime", CharSet = CharSet.Ansi)]
        private static extern SalError salGetSensorTime_Win32(IntPtr sensorHandle, out TimeInfo timeInfo);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetSensorTime", CharSet = CharSet.Ansi)]
        private static extern SalError salGetSensorTime_X64(IntPtr sensorHandle, out TimeInfo timeInfo);

        public static SalError salGetSensorTime(IntPtr sensorHandle, out TimeInfo timeInfo)
        {
            if (IntPtr.Size == 8)
                return salGetSensorTime_X64(sensorHandle, out timeInfo); // Call 64-bit DLL
            else
                return salGetSensorTime_Win32(sensorHandle, out timeInfo); // Call 32-bit DLL
        }

        // ===================== salSetSensorTime( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSetSensorTime", CharSet = CharSet.Ansi)]
        private static extern SalError salSetSensorTime_Win32(IntPtr sensorHandle, UInt32 timestampSeconds, UInt32 timestampNSeconds);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSetSensorTime", CharSet = CharSet.Ansi)]
        private static extern SalError salSetSensorTime_X64(IntPtr sensorHandle, UInt32 timestampSeconds, UInt32 timestampNSeconds);

        public static SalError salSetSensorTime(IntPtr sensorHandle, UInt32 timestampSeconds, UInt32 timestampNSeconds)
        {
            if (IntPtr.Size == 8)
                return salSetSensorTime_X64(sensorHandle, timestampSeconds, timestampNSeconds); // Call 64-bit DLL
            else
                return salSetSensorTime_Win32(sensorHandle, timestampSeconds, timestampNSeconds); // Call 32-bit DLL
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct salBacklogStatus
        {
            public salFlowControl flowControlStatus;
            public UInt64 backlogBytes;      // Number of bytes waiting to be transferred
            public UInt64 discardBytes;      // Number of bytes discarded because of backlog condition
            public UInt32 backlogMessages;   // Number of messages waiting to be transferred
            public UInt64 discardMessages;   // Number of messages discarded because of backlog condition
            public float rxBytesPerSec;      // incoming data rate from the measurement HW
            public float txBytesPerSec;      // TX data rate leaving the sensor
            public float backlogSeconds;	  // Backlog in seconds
        };

        //////// END agSal.h ////////

        //////// BEGIN agSalLocation.h ////////

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct TimeseriesInfo2
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_NAME)]
            public string sensorName;  /**< Sensor name */
            public Location location;                  /**< Location of sensor whe measurement was made */
            public double centerFrequency;
            public double sampleRate;
            public double bandwidth;
            public double recordLength;
            public UInt32 numSamples;                /**< Number of samples in the timeseries */
            public double triggerLevel;              /**< Trigger level for the timeseries */
            public double attenuation;               /**< The amount of attenuation the sensor used in dB */
            public UInt32 overload;                  /**< 1 if the sensor overloaded during the timeseries, 0 otherwise */
            public UInt32 timestampSeconds;          /**< Whole seconds part of the time timeseries start time */
            public UInt32 timestampNSeconds;         /**< Fractional seconds part of the time timeseries start time, in nano-seconds */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_NAME)]
            public string triggerSource;             /**< Name of sensor that caused this sensor to trigger */
            public double triggerLatency;            /**< Time difference in seconds between when the triggerSource initiated the trigger and when this sensor triggered  */
            public double variance1588;              /**< Variance of IEEE-1588 time */
            public UInt32 usedInCalculation;         /**< If non-zero, this time series was used in geolocation calculation */
            public DataType dataType;
            public bool UsedInCalculation { get { return (usedInCalculation != 0); } }
            public bool isOverload { get { return (overload != 0); } }
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct TimeseriesInfo3
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_NAME)]
            public string sensorName;  /**< Sensor name */
            public Location location;                  /**< Location of sensor whe measurement was made */
            public double centerFrequency;
            public double sampleRate;
            public double bandwidth;
            public double recordLength;
            public UInt32 numSamples;                /**< Number of samples in the timeseries */
            public double triggerLevel;              /**< Trigger level for the timeseries */
            public double attenuation;               /**< The amount of attenuation the sensor used in dB */
            public UInt32 overload;                  /**< 1 if the sensor overloaded during the timeseries, 0 otherwise */
            public UInt32 timestampSeconds;          /**< Whole seconds part of the time timeseries start time */
            public UInt32 timestampNSeconds;         /**< Fractional seconds part of the time timeseries start time, in nano-seconds */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_NAME)]
            public string triggerSource;             /**< Name of sensor that caused this sensor to trigger */
            public double triggerLatency;            /**< Time difference in seconds between when the triggerSource initiated the trigger and when this sensor triggered  */
            public double variance1588;              /**< Variance of IEEE-1588 time */
            public UInt32 usedInCalculation;         /**< If non-zero, this time series was used in geolocation calculation */
            public DataType dataType;
            public IntPtr sensorHandle;              /**< Connection handle to the sensor */
            public bool UsedInCalculation { get { return (usedInCalculation != 0); } }
            public bool isOverload { get { return (overload != 0); } }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct LocateCorrelation2
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_NAME)]
            public string sensorName1;	/**< Name of the first sensor involved in this correlation */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_NAME)]
            public string sensorName2;	/**< Name of the second sensor involved in this correlation */
            public double firstX;		/**< X value in seconds of first point */
            public double deltaX;		/**< Step in seconds between X values */
            public Int32 numPoints;		/**< Number of points */
            public double peakX;         /**< X location of peak Y value */
            public double rho;			  /**< rho for this cross-correlation */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_UNIT)]
            public string xUnit;				  /**< X unit label */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_UNIT)]
            public string yUnit;				  /**< Y unit label */

        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct LocateSpectrum2
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_SENSOR_NAME)]
            public string sensorName;		/**< Name of the sensor that captured the time series with this spectrum */
            public double centerFrequency;
            public double span;
            public UInt32 numPoints;
            public double firstX;                    /**< Start frequency */
            public double deltaX;					  /**< Bin width */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_UNIT)]
            public string xUnit;				  /**< X unit label */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_UNIT)]
            public string yUnit;				  /**< Y unit label */
            public Int32 usedInCalculation;

            public bool UsedInCalculation { get { return (usedInCalculation != 0); } }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GeolocationAlgorithm
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_FILENAME)]
            public string id;   /**< ID for this algorithm. This must be provided in the salLocateResultParms struct */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_FILENAME)]
            public string name;			 /**< Descriptive name for this algorithm. */
        };

        // ===================== salGetAlgorithms( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetAlgorithms", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetAlgorithms_Win32(IntPtr smsHandle, out UInt32 numAlgorithms, byte[] buffer, UInt32 bufferSize);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetAlgorithms", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetAlgorithms_X64(IntPtr smsHandle, out UInt32 numAlgorithms, byte[] buffer, UInt32 bufferSize);

        public static SalError salGetAlgorithms(IntPtr smsHandle, out GeolocationAlgorithm[] algorithms)
        {

            UInt32 numAlgorithms = 0;
            UInt32 bufferSize = 1024 * 32;
            byte[] buffer = new byte[bufferSize];

            SalError err;
            GeolocationAlgorithm[] ga = new GeolocationAlgorithm[0];
            algorithms = ga; // Need to assign in case of early exit

            if (IntPtr.Size == 8)
                err = salGetAlgorithms_X64(smsHandle, out numAlgorithms, buffer, bufferSize);
            else
                err = salGetAlgorithms_Win32(smsHandle, out numAlgorithms, buffer, bufferSize);

            if (err != SalError.SAL_ERR_NONE) return err;

            if (numAlgorithms == 0) return SalError.SAL_ERR_NONE;

            ga = new GeolocationAlgorithm[numAlgorithms];

            int offset = 0;
            int zeroPosition = 0;
            for (UInt32 i = 0; i < numAlgorithms; i++)
            {
                for (zeroPosition = offset; zeroPosition < offset + MAX_FILENAME; zeroPosition++)
                {
                    if (buffer[zeroPosition] == 0) break;
                }
                ga[i].id = System.Text.Encoding.ASCII.GetString(buffer, offset, zeroPosition - offset);
                offset += MAX_FILENAME;
                for (zeroPosition = offset; zeroPosition < offset + MAX_FILENAME; zeroPosition++)
                {
                    if (buffer[zeroPosition] == 0) break;
                }
                ga[i].name = System.Text.Encoding.ASCII.GetString(buffer, offset, zeroPosition - offset);
                offset += MAX_FILENAME;
            }

            algorithms = ga;
            return err;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LocateMeasParms2
        {
            public UInt32 numSensors; /**< Number of sensors. 0 means use all sensors configured for TDOA. */
            public double frequency;
            public double bandwidth;
            public AntennaType antenna;
            public double attenuation;       /**< Attenuation in dB relative to measured optimum attenuation */
            public UInt32 numSamples;        /**< Number of samples to use for the measurement */
            public TriggerType triggerType;  /**< The type of triggering used for this measurement  */
            public double triggerLevel;      /**< Units are db or dBm depending on the triggerType */
            public Int32 triggerTimeSecs;   /**< Whole seconds part of trigger time; used only when using time trigger */
            public Int32 triggerTimeNSecs;
            public double timeout;
            public Priority priority;
            public double triggerHoldoff;
            public double triggerMinimumInterval;
        }

        public enum DataDeliveryMode
        {
            salDataDeliveryMode_Normal = 0, /**< Default: Send all data back to the Sensor Management Server. */
            salDataDeliveryMode_NoIQ = 1, /**< Do not send the IQ data float[] back to the Sensor Management Server. */
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LocateMeasParms3
        {
            public UInt32 numSensors; /**< Number of sensors. 0 means use all sensors configured for TDOA. */
            public double frequency;
            public double bandwidth;
            public AntennaType antenna;
            public double attenuation;       /**< Attenuation in dB relative to measured optimum attenuation */
            public UInt32 numSamples;        /**< Number of samples to use for the measurement */
            public TriggerType triggerType;  /**< The type of triggering used for this measurement  */
            public double triggerLevel;      /**< Units are db or dBm depending on the triggerType */
            public Int32 triggerTimeSecs;   /**< Whole seconds part of trigger time; used only when using time trigger */
            public Int32 triggerTimeNSecs;
            public double timeout;
            public Priority priority;
            public double triggerHoldoff;
            public double triggerMinimumInterval;

            public DataDeliveryMode dataDeliveryMode;  /**< Which data the sensor will return back to the Sensor Management Server during a geolocation measurement. */
            public UInt32 numMeasurements;         /**< Number of consecutive IQ measurements the sensor will execute when the sensor receives this measurement request.
                                                            If this is 0, then this number is changed to 1, and 1 IQ measurement is executed. */
            public double measurePeriod;           /**< Amount of time in seconds between each consecutive IQ trigger times on the sensor. */
        }

        public enum AntennaOverride
        {
            None = 0,
            UseAntenna1 = 1,
            UseAntenna2 = 2
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct GeoSensorSetup
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_FILENAME)]
            public string sensorName;
            public Int32 disableTrigger;        /**<  if non-zero, do not use this sensor for level triggering */
            public AntennaOverride antennaOverride;       /**<  allows using a different antenna than specified by salLocateMeasParms2.antenna */
            public Int32 useTriggerLevel;
            public Int32 reserved1;
            public double triggerLevel;
            public double addAttenuation;        /**<  amount of attenuation in dB to add to salLocateMeasParms2.attenuation for this sensor. */
            public double addThreshold;          /**<  additional dB to add to salLocateMeasParms2.triggerLevel for this sensor */
            public double reserved2;
        }

        // LocateResultParms2 supports geolocationAlgorithm
        [StructLayout(LayoutKind.Sequential)]
        public struct LocateResultParms2
        {
            public Region region;
            public LocateImageParms image;
            public LocateOutput outputTypes;              /**< Type of output requested */
            public DopplerCompensation dopplerComp;		/**< Doppler compensation OFF/ON/AUTO */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_FILENAME)]
            public string geolocationAlgorithm;
            public IntPtr userWorkspace;
            public UInt32 numSensorsMax;
            public IntPtr excludeSensorGroup; /**< Handle to the group of sensors to exclude from the measurement. 0 means use all sensors with timeseries data. */
        }

        // LocateResultParms3 supports geolocationAlgorithm
        [StructLayout(LayoutKind.Sequential)]
        public struct LocateResultParms3
        {
            public Region region;
            public LocateImageParms image;
            public LocateOutput outputTypes;              /**< Type of output requested */
            public DopplerCompensation dopplerComp;		/**< Doppler compensation OFF/ON/AUTO */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_FILENAME)]
            public string geolocationAlgorithm;
            public IntPtr userWorkspace;
            public UInt32 numSensorsMax;
            public IntPtr excludeSensorGroup; /**< Handle to the group of sensors to exclude from the measurement. 0 means use all sensors with timeseries data. */
            public double maxDopplerSpeed;
            public Int32 trimTimeSeriesMode; /**< Control trimming of Timeseries during recomputation: 
								      0 == disabled
									  1 == time offset from first sample time
									  2 == start index in beginTimeSec, number of samples in endTimeSec */
            public Int32 beginTimeSec;					/**< Seconds portion of begin time. Also startIndex when trimTimeSeriesMode==2 */
            public Int32 beginTimeNsec;					/**< Nsec portion of begin time */
            public Int32 endTimeSec;					/**< Seconds portion of end time. Also numSamples when trimTimeSeriesMode==2 */
            public Int32 endTimeNsec;                   /**< Nsec portion of end time */
        }

        // LocateResultParms4 supports geolocationAlgorithm
        [StructLayout(LayoutKind.Sequential)]
        public struct LocateResultParms4
        {
            public Region2 region;
            public LocateImageParms image;
            public LocateOutput outputTypes;              /**< Type of output requested */
            public DopplerCompensation dopplerComp;		/**< Doppler compensation OFF/ON/AUTO */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_FILENAME)]
            public string geolocationAlgorithm;
            public IntPtr userWorkspace;
            public UInt32 numSensorsMax;
            public IntPtr excludeSensorGroup; /**< Handle to the group of sensors to exclude from the measurement. 0 means use all sensors with timeseries data. */
            public double maxDopplerSpeed;
            public Int32 trimTimeSeriesMode; /**< Control trimming of Timeseries during recomputation: 
								      0 == disabled
									  1 == time offset from first sample time
									  2 == start index in beginTimeSec, number of samples in endTimeSec */
            public Int32 beginTimeSec;					/**< Seconds portion of begin time. Also startIndex when trimTimeSeriesMode==2 */
            public Int32 beginTimeNsec;					/**< Nsec portion of begin time */
            public Int32 endTimeSec;					/**< Seconds portion of end time. Also numSamples when trimTimeSeriesMode==2 */
            public Int32 endTimeNsec;					/**< Nsec portion of end time */
        }

        // LocateResultParms5 supports geolocationAlgorithm with manual error probability and timing uncertainty
        [StructLayout(LayoutKind.Sequential)]
        public struct LocateResultParms5
        {
            public Region2 region;
            public LocateImageParms image;
            public LocateOutput outputTypes;              /**< Type of output requested */
            public DopplerCompensation dopplerComp;		/**< Doppler compensation OFF/ON/AUTO */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_FILENAME)]
            public string geolocationAlgorithm;
            public IntPtr userWorkspace;
            public UInt32 numSensorsMax;
            public IntPtr excludeSensorGroup; /**< Handle to the group of sensors to exclude from the measurement. 0 means use all sensors with timeseries data. */
            public double maxDopplerSpeed;
            public Int32 trimTimeSeriesMode; /**< Control trimming of Timeseries during recomputation: 
								      0 == disabled
									  1 == time offset from first sample time
									  2 == start index in beginTimeSec, number of samples in endTimeSec */
            public Int32 beginTimeSec;					/**< Seconds portion of begin time. Also startIndex when trimTimeSeriesMode==2 */
            public Int32 beginTimeNsec;					/**< Nsec portion of begin time */
            public Int32 endTimeSec;					/**< Seconds portion of end time. Also numSamples when trimTimeSeriesMode==2 */
            public Int32 endTimeNsec;					/**< Nsec portion of end time */
            public UInt32 probability;                  /**< EEP probability (percent) */
            public UInt32 timeUncertainty;              /**< EEP time uncertainty (s) */
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ScaleAndZoomParms
        {
            float scaleFactor;            /**< Factor to scale up by. All sides of the computation plane will be scaled up by this value. Zero indicates no up-scale. */
            float zoomFactor;             /**< Factor to zoom by. All sides of the computation plane will be scaled down by this value. Zero indicates no zoom.  */
            UInt16 sensorInfoTimeout;      /**< Time (seconds) to wait for sensor info to be returned from the SMS. Default: 60 */
        }

        // ===================== salMeasureLocation2( ) (deprecated) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salMeasureLocation2", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern SalError salMeasureLocation2_Win32(out IntPtr locateHandle, IntPtr smsHandle, ref LocateMeasParms2 measParms, ref LocateResultParms2 resultParms,
            SAL_LOCATE_CALLBACK callback, Byte[] sensorTable);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salMeasureLocation2", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern SalError salMeasureLocation2_X64(out IntPtr locateHandle, IntPtr smsHandle, ref LocateMeasParms2 measParms, ref LocateResultParms2 resultParms,
            SAL_LOCATE_CALLBACK callback, Byte[] sensorTable);

        public static SalError salMeasureLocation2(out IntPtr locateHandle, IntPtr smsHandle, ref LocateMeasParms2 measParms,
            ref LocateResultParms2 resultParms, SAL_LOCATE_CALLBACK callback, ref GeoSensorSetup[] sensorTable)
        {
            byte[] tableBytes = null;

            if ((sensorTable != null) && (sensorTable.Length > 0))
            {
                uint numBytes = (uint)Marshal.SizeOf(sensorTable[0]);
                numBytes *= measParms.numSensors;

                tableBytes = new byte[numBytes];
                int offset = 0;

                System.Text.Encoding encoding = System.Text.Encoding.ASCII;

                for (int i = 0; i < measParms.numSensors; i++)
                {
                    byte[] b = encoding.GetBytes(sensorTable[i].sensorName);
                    int len = b.Length;
                    Array.Copy(b, 0, tableBytes, offset, len);
                    offset += len;
                    for (int j = 0; j < MAX_SENSOR_NAME - len; j++)
                    {
                        tableBytes[offset++] = 0;
                    }
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].disableTrigger), 0, tableBytes, offset, 4); offset += 4;
                    Array.Copy(BitConverter.GetBytes((int)sensorTable[i].antennaOverride), 0, tableBytes, offset, 4); offset += 4;
                    Array.Copy(BitConverter.GetBytes((int)sensorTable[i].useTriggerLevel), 0, tableBytes, offset, 4); offset += 4;
                    Array.Copy(BitConverter.GetBytes((int)sensorTable[i].reserved1), 0, tableBytes, offset, 4); offset += 4;
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].triggerLevel), 0, tableBytes, offset, 8); offset += 8;
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].addAttenuation), 0, tableBytes, offset, 8); offset += 8;
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].addThreshold), 0, tableBytes, offset, 8); offset += 8;
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].reserved2), 0, tableBytes, offset, 8); offset += 8;

                }
            }

            if (IntPtr.Size == 8)
                return salMeasureLocation2_X64(out locateHandle, smsHandle, ref measParms, ref resultParms, callback, tableBytes);
            else
                return salMeasureLocation2_Win32(out locateHandle, smsHandle, ref measParms, ref resultParms, callback, tableBytes);
        }

        // ===================== salMeasureLocation3( ) (deprecated)=================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salMeasureLocation3", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern SalError salMeasureLocation3_Win32(out IntPtr locateHandle, IntPtr smsHandle, ref LocateMeasParms2 measParms, ref LocateResultParms3 resultParms,
            SAL_LOCATE_CALLBACK callback, Byte[] sensorTable);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salMeasureLocation3", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern SalError salMeasureLocation3_X64(out IntPtr locateHandle, IntPtr smsHandle, ref LocateMeasParms2 measParms, ref LocateResultParms3 resultParms,
            SAL_LOCATE_CALLBACK callback, Byte[] sensorTable);

        public static SalError salMeasureLocation3(out IntPtr locateHandle, IntPtr smsHandle, ref LocateMeasParms2 measParms,
            ref LocateResultParms3 resultParms, SAL_LOCATE_CALLBACK callback, ref GeoSensorSetup[] sensorTable)
        {
            byte[] tableBytes = null;

            if ((sensorTable != null) && (sensorTable.Length > 0))
            {
                uint numBytes = (uint)Marshal.SizeOf(sensorTable[0]);
                numBytes *= measParms.numSensors;

                tableBytes = new byte[numBytes];
                int offset = 0;

                System.Text.Encoding encoding = System.Text.Encoding.ASCII;

                for (int i = 0; i < measParms.numSensors; i++)
                {
                    byte[] b = encoding.GetBytes(sensorTable[i].sensorName);
                    int len = b.Length;
                    Array.Copy(b, 0, tableBytes, offset, len);
                    offset += len;
                    for (int j = 0; j < MAX_SENSOR_NAME - len; j++)
                    {
                        tableBytes[offset++] = 0;
                    }
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].disableTrigger), 0, tableBytes, offset, 4); offset += 4;
                    Array.Copy(BitConverter.GetBytes((int)sensorTable[i].antennaOverride), 0, tableBytes, offset, 4); offset += 4;
                    Array.Copy(BitConverter.GetBytes((int)sensorTable[i].useTriggerLevel), 0, tableBytes, offset, 4); offset += 4;
                    Array.Copy(BitConverter.GetBytes((int)sensorTable[i].reserved1), 0, tableBytes, offset, 4); offset += 4;
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].triggerLevel), 0, tableBytes, offset, 8); offset += 8;
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].addAttenuation), 0, tableBytes, offset, 8); offset += 8;
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].addThreshold), 0, tableBytes, offset, 8); offset += 8;
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].reserved2), 0, tableBytes, offset, 8); offset += 8;

                }
            }

            if (IntPtr.Size == 8)
                return salMeasureLocation3_X64(out locateHandle, smsHandle, ref measParms, ref resultParms, callback, tableBytes);
            else
                return salMeasureLocation3_Win32(out locateHandle, smsHandle, ref measParms, ref resultParms, callback, tableBytes);
        }

        // ===================== salMeasureLocation4( ) (deprecated) ================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salMeasureLocation4", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern SalError salMeasureLocation4_Win32(out IntPtr locateHandle, IntPtr smsHandle, ref LocateMeasParms2 measParms, ref LocateResultParms4 resultParms,
            SAL_LOCATE_CALLBACK callback, Byte[] sensorTable);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salMeasureLocation4", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern SalError salMeasureLocation4_X64(out IntPtr locateHandle, IntPtr smsHandle, ref LocateMeasParms2 measParms, ref LocateResultParms4 resultParms,
            SAL_LOCATE_CALLBACK callback, Byte[] sensorTable);

        public static SalError salMeasureLocation4(out IntPtr locateHandle, IntPtr smsHandle, ref LocateMeasParms2 measParms,
            ref LocateResultParms4 resultParms, SAL_LOCATE_CALLBACK callback, ref GeoSensorSetup[] sensorTable)
        {
            byte[] tableBytes = null;

            if ((sensorTable != null) && (sensorTable.Length > 0))
            {
                uint numBytes = (uint)Marshal.SizeOf(sensorTable[0]);
                numBytes *= measParms.numSensors;

                tableBytes = new byte[numBytes];
                int offset = 0;

                System.Text.Encoding encoding = System.Text.Encoding.ASCII;

                for (int i = 0; i < measParms.numSensors; i++)
                {
                    byte[] b = encoding.GetBytes(sensorTable[i].sensorName);
                    int len = b.Length;
                    Array.Copy(b, 0, tableBytes, offset, len);
                    offset += len;
                    for (int j = 0; j < MAX_SENSOR_NAME - len; j++)
                    {
                        tableBytes[offset++] = 0;
                    }
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].disableTrigger), 0, tableBytes, offset, 4); offset += 4;
                    Array.Copy(BitConverter.GetBytes((int)sensorTable[i].antennaOverride), 0, tableBytes, offset, 4); offset += 4;
                    Array.Copy(BitConverter.GetBytes((int)sensorTable[i].useTriggerLevel), 0, tableBytes, offset, 4); offset += 4;
                    Array.Copy(BitConverter.GetBytes((int)sensorTable[i].reserved1), 0, tableBytes, offset, 4); offset += 4;
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].triggerLevel), 0, tableBytes, offset, 8); offset += 8;
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].addAttenuation), 0, tableBytes, offset, 8); offset += 8;
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].addThreshold), 0, tableBytes, offset, 8); offset += 8;
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].reserved2), 0, tableBytes, offset, 8); offset += 8;

                }
            }

            if (IntPtr.Size == 8)
                return salMeasureLocation4_X64(out locateHandle, smsHandle, ref measParms, ref resultParms, callback, tableBytes);
            else
                return salMeasureLocation4_Win32(out locateHandle, smsHandle, ref measParms, ref resultParms, callback, tableBytes);
        }

        // ===================== salMeasureLocation5( ) ================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salMeasureLocation5", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern SalError salMeasureLocation5_Win32(out IntPtr locateHandle, IntPtr smsHandle, ref LocateMeasParms2 measParms, ref LocateResultParms5 resultParms,
            SAL_LOCATE_CALLBACK callback, Byte[] sensorTable);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salMeasureLocation5", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern SalError salMeasureLocation5_X64(out IntPtr locateHandle, IntPtr smsHandle, ref LocateMeasParms2 measParms, ref LocateResultParms5 resultParms,
            SAL_LOCATE_CALLBACK callback, Byte[] sensorTable);

        public static SalError salMeasureLocation5(out IntPtr locateHandle, IntPtr smsHandle, ref LocateMeasParms2 measParms,
            ref LocateResultParms5 resultParms, SAL_LOCATE_CALLBACK callback, ref GeoSensorSetup[] sensorTable)
        {
            byte[] tableBytes = null;

            if ((sensorTable != null) && (sensorTable.Length > 0))
            {
                uint numBytes = (uint)Marshal.SizeOf(sensorTable[0]);
                numBytes *= measParms.numSensors;

                tableBytes = new byte[numBytes];
                int offset = 0;

                System.Text.Encoding encoding = System.Text.Encoding.ASCII;

                for (int i = 0; i < measParms.numSensors; i++)
                {
                    byte[] b = encoding.GetBytes(sensorTable[i].sensorName);
                    int len = b.Length;
                    Array.Copy(b, 0, tableBytes, offset, len);
                    offset += len;
                    for (int j = 0; j < MAX_SENSOR_NAME - len; j++)
                    {
                        tableBytes[offset++] = 0;
                    }
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].disableTrigger), 0, tableBytes, offset, 4); offset += 4;
                    Array.Copy(BitConverter.GetBytes((int)sensorTable[i].antennaOverride), 0, tableBytes, offset, 4); offset += 4;
                    Array.Copy(BitConverter.GetBytes((int)sensorTable[i].useTriggerLevel), 0, tableBytes, offset, 4); offset += 4;
                    Array.Copy(BitConverter.GetBytes((int)sensorTable[i].reserved1), 0, tableBytes, offset, 4); offset += 4;
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].triggerLevel), 0, tableBytes, offset, 8); offset += 8;
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].addAttenuation), 0, tableBytes, offset, 8); offset += 8;
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].addThreshold), 0, tableBytes, offset, 8); offset += 8;
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].reserved2), 0, tableBytes, offset, 8); offset += 8;

                }
            }

            if (IntPtr.Size == 8)
                return salMeasureLocation5_X64(out locateHandle, smsHandle, ref measParms, ref resultParms, callback, tableBytes);
            else
                return salMeasureLocation5_Win32(out locateHandle, smsHandle, ref measParms, ref resultParms, callback, tableBytes);
        }

        // ===================== salAcquireLocationData( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salAcquireLocationData", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salAcquireLocationData_Win32(out IntPtr salLocateHandle, IntPtr smsHandle,
            ref LocateMeasParms2 measParms, IntPtr userWorkspace, SAL_LOCATE_CALLBACK callback, Byte[] sensorTable);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salAcquireLocationData", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salAcquireLocationData_X64(out IntPtr salLocateHandle, IntPtr smsHandle,
            ref LocateMeasParms2 measParms, IntPtr userWorkspace, SAL_LOCATE_CALLBACK callback, Byte[] sensorTable);

        public static SalError salAcquireLocationData(out IntPtr salLocateHandle, IntPtr smsHandle, ref LocateMeasParms2 measParms,
            IntPtr userWorkspace, SAL_LOCATE_CALLBACK callback, ref GeoSensorSetup[] sensorTable)
        {
            byte[] tableBytes = null;

            if ((sensorTable != null) && (sensorTable.Length > 0))
            {
                uint numBytes = (uint)Marshal.SizeOf(sensorTable[0]);
                numBytes *= measParms.numSensors;
                tableBytes = new byte[numBytes];
                int offset = 0;

                System.Text.Encoding encoding = System.Text.Encoding.ASCII;

                for (int i = 0; i < measParms.numSensors; i++)
                {
                    byte[] b = encoding.GetBytes(sensorTable[i].sensorName);
                    int len = b.Length;
                    Array.Copy(b, 0, tableBytes, offset, len);
                    offset += len;
                    for (int j = 0; j < MAX_SENSOR_NAME - len; j++)
                    {
                        tableBytes[offset++] = 0;
                    }
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].disableTrigger), 0, tableBytes, offset, 4); offset += 4;
                    Array.Copy(BitConverter.GetBytes((int)sensorTable[i].antennaOverride), 0, tableBytes, offset, 4); offset += 4;
                    Array.Copy(BitConverter.GetBytes((int)sensorTable[i].useTriggerLevel), 0, tableBytes, offset, 4); offset += 4;
                    Array.Copy(BitConverter.GetBytes((int)sensorTable[i].reserved1), 0, tableBytes, offset, 4); offset += 4;
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].triggerLevel), 0, tableBytes, offset, 8); offset += 8;
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].addAttenuation), 0, tableBytes, offset, 8); offset += 8;
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].addThreshold), 0, tableBytes, offset, 8); offset += 8;
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].reserved2), 0, tableBytes, offset, 8); offset += 8;

                }
            }

            if (IntPtr.Size == 8)
                return salAcquireLocationData_X64(out salLocateHandle, smsHandle, ref measParms, userWorkspace, callback, tableBytes);
            else
                return salAcquireLocationData_Win32(out salLocateHandle, smsHandle, ref measParms, userWorkspace, callback, tableBytes);
        }

        // ===================== salAcquireLocationData2( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salAcquireLocationData2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salAcquireLocationData2_Win32(out IntPtr salLocateHandle, IntPtr smsHandle,
            ref LocateMeasParms3 measParms, IntPtr userWorkspace, SAL_LOCATE_CALLBACK callback, Byte[] sensorTable);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salAcquireLocationData2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salAcquireLocationData2_X64(out IntPtr salLocateHandle, IntPtr smsHandle,
            ref LocateMeasParms3 measParms, IntPtr userWorkspace, SAL_LOCATE_CALLBACK callback, Byte[] sensorTable);

        public static SalError salAcquireLocationData2(out IntPtr salLocateHandle, IntPtr smsHandle, ref LocateMeasParms3 measParms,
            IntPtr userWorkspace, SAL_LOCATE_CALLBACK callback, ref GeoSensorSetup[] sensorTable)
        {
            byte[] tableBytes = null;

            if ((sensorTable != null) && (sensorTable.Length > 0))
            {
                uint numBytes = (uint)Marshal.SizeOf(sensorTable[0]);
                numBytes *= measParms.numSensors;
                tableBytes = new byte[numBytes];
                int offset = 0;

                System.Text.Encoding encoding = System.Text.Encoding.ASCII;

                for (int i = 0; i < measParms.numSensors; i++)
                {
                    byte[] b = encoding.GetBytes(sensorTable[i].sensorName);
                    int len = b.Length;
                    Array.Copy(b, 0, tableBytes, offset, len);
                    offset += len;
                    for (int j = 0; j < MAX_SENSOR_NAME - len; j++)
                    {
                        tableBytes[offset++] = 0;
                    }
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].disableTrigger), 0, tableBytes, offset, 4); offset += 4;
                    Array.Copy(BitConverter.GetBytes((int)sensorTable[i].antennaOverride), 0, tableBytes, offset, 4); offset += 4;
                    Array.Copy(BitConverter.GetBytes((int)sensorTable[i].useTriggerLevel), 0, tableBytes, offset, 4); offset += 4;
                    Array.Copy(BitConverter.GetBytes((int)sensorTable[i].reserved1), 0, tableBytes, offset, 4); offset += 4;
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].triggerLevel), 0, tableBytes, offset, 8); offset += 8;
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].addAttenuation), 0, tableBytes, offset, 8); offset += 8;
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].addThreshold), 0, tableBytes, offset, 8); offset += 8;
                    Array.Copy(BitConverter.GetBytes(sensorTable[i].reserved2), 0, tableBytes, offset, 8); offset += 8;

                }
            }

            if (IntPtr.Size == 8)
                return salAcquireLocationData2_X64(out salLocateHandle, smsHandle, ref measParms, userWorkspace, callback, tableBytes);
            else
                return salAcquireLocationData2_Win32(out salLocateHandle, smsHandle, ref measParms, userWorkspace, callback, tableBytes);
        }

        // ===================== salRecalculateLocation2( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salRecalculateLocation2", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern SalError salRecalculateLocation2_Win32(out IntPtr salLocateHandle, IntPtr smsHandle, Int32 measurementId, ref LocateResultParms2 parms, SAL_LOCATE_CALLBACK callback);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salRecalculateLocation2", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern SalError salRecalculateLocation2_X64(out IntPtr salLocateHandle, IntPtr smsHandle, Int32 measurementId, ref LocateResultParms2 parms, SAL_LOCATE_CALLBACK callback);

        public static SalError salRecalculateLocation2(out IntPtr salLocateHandle, IntPtr smsHandle, Int32 measurementId, ref LocateResultParms2 parms, SAL_LOCATE_CALLBACK callback)
        {
            if (IntPtr.Size == 8)
                return salRecalculateLocation2_X64(out salLocateHandle, smsHandle, measurementId, ref parms, callback);
            else
                return salRecalculateLocation2_Win32(out salLocateHandle, smsHandle, measurementId, ref parms, callback);
        }
        // ===================== salRecalculateLocation3( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salRecalculateLocation3", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern SalError salRecalculateLocation3_Win32(out IntPtr salLocateHandle, IntPtr smsHandle, Int32 measurementId, ref LocateResultParms3 parms, SAL_LOCATE_CALLBACK callback);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salRecalculateLocation3", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern SalError salRecalculateLocation3_X64(out IntPtr salLocateHandle, IntPtr smsHandle, Int32 measurementId, ref LocateResultParms3 parms, SAL_LOCATE_CALLBACK callback);

        public static SalError salRecalculateLocation3(out IntPtr salLocateHandle, IntPtr smsHandle, Int32 measurementId, ref LocateResultParms3 parms, SAL_LOCATE_CALLBACK callback)
        {
            if (IntPtr.Size == 8)
                return salRecalculateLocation3_X64(out salLocateHandle, smsHandle, measurementId, ref parms, callback);
            else
                return salRecalculateLocation3_Win32(out salLocateHandle, smsHandle, measurementId, ref parms, callback);
        }

        // ===================== salRecalculateLocation4( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salRecalculateLocation4", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern SalError salRecalculateLocation4_Win32(out IntPtr salLocateHandle, IntPtr smsHandle, Int32 measurementId, ref LocateResultParms4 parms, SAL_LOCATE_CALLBACK callback);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salRecalculateLocation4", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern SalError salRecalculateLocation4_X64(out IntPtr salLocateHandle, IntPtr smsHandle, Int32 measurementId, ref LocateResultParms4 parms, SAL_LOCATE_CALLBACK callback);

        public static SalError salRecalculateLocation4(out IntPtr salLocateHandle, IntPtr smsHandle, Int32 measurementId, ref LocateResultParms4 parms, SAL_LOCATE_CALLBACK callback)
        {
            if (IntPtr.Size == 8)
                return salRecalculateLocation4_X64(out salLocateHandle, smsHandle, measurementId, ref parms, callback);
            else
                return salRecalculateLocation4_Win32(out salLocateHandle, smsHandle, measurementId, ref parms, callback);
        }

        // ===================== salRecalculateLocation5( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salRecalculateLocation5", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern SalError salRecalculateLocation5_Win32(out IntPtr salLocateHandle, IntPtr smsHandle, Int32 measurementId, ref LocateResultParms5 parms, SAL_LOCATE_CALLBACK callback);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salRecalculateLocation5", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern SalError salRecalculateLocation5_X64(out IntPtr salLocateHandle, IntPtr smsHandle, Int32 measurementId, ref LocateResultParms5 parms, SAL_LOCATE_CALLBACK callback);

        public static SalError salRecalculateLocation5(out IntPtr salLocateHandle, IntPtr smsHandle, Int32 measurementId, ref LocateResultParms5 parms, SAL_LOCATE_CALLBACK callback)
        {
            if (IntPtr.Size == 8)
                return salRecalculateLocation5_X64(out salLocateHandle, smsHandle, measurementId, ref parms, callback);
            else
                return salRecalculateLocation5_Win32(out salLocateHandle, smsHandle, measurementId, ref parms, callback);
        }

        // ===================== salRecalculateLocationScaleAndZoom( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salRecalculateLocationScaleAndZoom", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern SalError salRecalculateLocationScaleAndZoom_Win32(out IntPtr salLocateHandle, IntPtr smsHandle, Int32 measurementId, ScaleAndZoomParms scaleParms, ref LocateResultParms5 parms, SAL_LOCATE_CALLBACK callback);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salRecalculateLocationScaleAndZoom", CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern SalError salRecalculateLocationScaleAndZoom_X64(out IntPtr salLocateHandle, IntPtr smsHandle, Int32 measurementId, ScaleAndZoomParms scaleParms, ref LocateResultParms5 parms, SAL_LOCATE_CALLBACK callback);

        public static SalError salRecalculateLocationScaleAndZoom(out IntPtr salLocateHandle, IntPtr smsHandle, Int32 measurementId, ScaleAndZoomParms scaleParms, ref LocateResultParms5 parms, SAL_LOCATE_CALLBACK callback)
        {
            if (IntPtr.Size == 8)
                return salRecalculateLocationScaleAndZoom_X64(out salLocateHandle, smsHandle, measurementId, scaleParms, ref parms, callback);
            else
                return salRecalculateLocationScaleAndZoom_Win32(out salLocateHandle, smsHandle, measurementId, scaleParms, ref parms, callback);
        }

        // ===================== salMeasureLocationAbort( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salMeasureLocationAbort", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salMeasureLocationAbort_Win32(IntPtr salLocateHandle);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salMeasureLocationAbort", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salMeasureLocationAbort_X64(IntPtr salLocateHandle);

        public static SalError salMeasureLocationAbort(IntPtr salLocateHandle)
        {
            if (IntPtr.Size == 8)
                return salMeasureLocationAbort_X64(salLocateHandle);
            else
                return salMeasureLocationAbort_Win32(salLocateHandle);
        }

        // ===================== salGetTimeseriesInfo2( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetTimeseriesInfo2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetTimeseriesInfo2_Win32(IntPtr salLocateHandle, UInt32 sensorIndex, out TimeseriesInfo2 info,
             float[] buffer, UInt32 numBytesInBuffer);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetTimeseriesInfo2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetTimeseriesInfo2_X64(IntPtr salLocateHandle, UInt32 sensorIndex, out TimeseriesInfo2 info,
             float[] buffer, UInt32 numBytesInBuffer);

        public static SalError salGetTimeseriesInfo2(IntPtr salLocateHandle, UInt32 sensorIndex, out TimeseriesInfo2 info,
             float[] buffer, UInt32 numBytesInBuffer)
        {
            if (IntPtr.Size == 8)
                return salGetTimeseriesInfo2_X64(salLocateHandle, sensorIndex, out info, buffer, numBytesInBuffer);
            else
                return salGetTimeseriesInfo2_Win32(salLocateHandle, sensorIndex, out info, buffer, numBytesInBuffer);
        }

        // ===================== salGetSpectrum2( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetSpectrum2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSpectrum2_Win32(IntPtr handle, UInt32 index, out LocateSpectrum2 spec, float[] buffer, UInt32 numBytesInBuffer);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetSpectrum2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSpectrum2_X64(IntPtr handle, UInt32 index, out LocateSpectrum2 spec, float[] buffer, UInt32 numBytesInBuffer);

        public static SalError salGetSpectrum2(IntPtr handle, UInt32 index, out LocateSpectrum2 spec, float[] buffer, UInt32 numBytesInBuffer)
        {
            if (IntPtr.Size == 8)
                return salGetSpectrum2_X64(handle, index, out spec, buffer, numBytesInBuffer);
            else
                return salGetSpectrum2_Win32(handle, index, out spec, buffer, numBytesInBuffer);
        }

        // ===================== salGetCorrelation2( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetCorrelation2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetCorrelation2_Win32(IntPtr salLocateHandle, UInt32 index1,
            UInt32 index2, out LocateCorrelation2 corr, float[] data, UInt32 numBytesInBuffer);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetCorrelation2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetCorrelation2_X64(IntPtr salLocateHandle, UInt32 index1,
            UInt32 index2, out LocateCorrelation2 corr, float[] data, UInt32 numBytesInBuffer);

        public static SalError salGetCorrelation2(IntPtr salLocateHandle, UInt32 index1,
            UInt32 index2, out LocateCorrelation2 corr, float[] data, UInt32 numBytesInBuffer)
        {
            if (IntPtr.Size == 8)
                return salGetCorrelation2_X64(salLocateHandle, index1, index2, out corr, data, numBytesInBuffer);
            else
                return salGetCorrelation2_Win32(salLocateHandle, index1, index2, out corr, data, numBytesInBuffer);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LocateResult2
        {
            public SalError errorCode;	     /**< Error code */
            public Int32 measurementId;	     /**< Measurement internal identifier */
            public Int32 userWorkspace;	     /**< Request ID as passed in by the caller */
            public Location location;	     /**< Computed transmitter location */
            public double resultQuality;	 /**< A quality indicator for the result (0..1) */
            public Int32 numSensors;         /**< how many sensors were used to make the measurement*/
            public Int32 numImageBytes;	     /**< Size (in bytes) of location measurement image */
            public LocateOutput validData;   /* Bitmask of available data types for this measurement */
            public double cepRadius;         /**< Radius in meters of circular error probable  */
            public double cepProbability;    /**< Probabilty (0-1.0) used for cepRadiusMeters calculation */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_FILENAME)]
            public string reserved1;         /**< reserved for future use */
            public double reserved2;         /**< reserved for future use */
            public double reserved3;         /**< reserved for future use */
            public Int32 numValidSensors;    /**< how many sensors were used in the geo-location computation (i.e. they provided valid measurements) */
            public Int32 reserved5;          /**< reserved for future use */

            public bool isLocationValid { get { return (validData & LocateOutput.LOCATION) != 0; } }
            public bool isImageValid { get { return (validData & LocateOutput.IMAGE) != 0; } }
            public bool isSpectraValid { get { return (validData & LocateOutput.SPECTRUM) != 0; } }
            public bool isCorrelationValid { get { return (validData & LocateOutput.CORRELATION) != 0; } }
            public bool isTimeInfoValid { get { return (validData & LocateOutput.TIMESERIES) != 0; } }
            public bool isTimeDataValid { get { return (validData & LocateOutput.TIME_DATA) != 0; } }
        };

        // ===================== salGetLocationResult2( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetLocationResult2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetLocationResult2_Win32(IntPtr salLocateHandle, out LocateResult2 result);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetLocationResult2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetLocationResult2_X64(IntPtr salLocateHandle, out LocateResult2 result);

        public static SalError salGetLocationResult2(IntPtr salLocateHandle, out LocateResult2 result)
        {
            if (IntPtr.Size == 8)
                return salGetLocationResult2_X64(salLocateHandle, out result);
            else
                return salGetLocationResult2_Win32(salLocateHandle, out result);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LocateResult3
        {
            public SalError errorCode;	     /**< Error code */
            public Int32 measurementId;	     /**< Measurement internal identifier */
            public Int32 userWorkspace;	     /**< Request ID as passed in by the caller */
            public Location location;	     /**< Computed transmitter location */
            public double resultQuality;	 /**< A quality indicator for the result (0..1) */
            public Int32 numSensors;         /**< how many sensors were used to make the measurement*/
            public Int32 numImageBytes;	     /**< Size (in bytes) of location measurement image */
            public LocateOutput validData;   /* Bitmask of available data types for this measurement */
            public Int32 numValidSensors;    /**< how many sensors were used in the geo-location computation (i.e. they provided valid measurements) */
            public double cepRadius;         /**< Radius in meters of circular error probable, or major radius of ellipse */
            public double cepProbability;    /**< Probabilty (0-1.0) used for cepRadiusMeters calculation */
            public double eepMinorRadius;    /**< Minor radius in meters of elliptical error probable */
            public double eepRotation;       /**< Rotation in degrees of elliptical error probable */
            public double eepCxx;            /**< EEP covariant X,X */
            public double eepCxy;            /**< EEP covariant X,Y */
            public double eepCyy;            /**< EEP covariant Y,Y */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_FILENAME)]
            public string reserved1;         /**< reserved for future use */
            public double reserved2;         /**< reserved for future use */
            public double reserved3;         /**< reserved for future use */
            public Int32 reserved4;          /**< reserved for future use */
            public Int32 reserved5;          /**< reserved for future use */

            public bool isLocationValid { get { return (validData & LocateOutput.LOCATION) != 0; } }
            public bool isImageValid { get { return (validData & LocateOutput.IMAGE) != 0; } }
            public bool isSpectraValid { get { return (validData & LocateOutput.SPECTRUM) != 0; } }
            public bool isCorrelationValid { get { return (validData & LocateOutput.CORRELATION) != 0; } }
            public bool isTimeInfoValid { get { return (validData & LocateOutput.TIMESERIES) != 0; } }
            public bool isTimeDataValid { get { return (validData & LocateOutput.TIME_DATA) != 0; } }
        };

        // ===================== salGetLocationResult3( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetLocationResult3", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetLocationResult3_Win32(IntPtr salLocateHandle, out LocateResult3 result);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetLocationResult3", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetLocationResult3_X64(IntPtr salLocateHandle, out LocateResult3 result);

        public static SalError salGetLocationResult3(IntPtr salLocateHandle, out LocateResult3 result)
        {
            if (IntPtr.Size == 8)
                return salGetLocationResult3_X64(salLocateHandle, out result);
            else
                return salGetLocationResult3_Win32(salLocateHandle, out result);
        }

        // ===================== salCreateLocationMeasurement( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salCreateLocationMeasurement", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salCreateLocationMeasurement_Win32(IntPtr salLocateHandle, ref LocateMeasParms2 parms, out Int32 measurementId);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salCreateLocationMeasurement", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salCreateLocationMeasurement_X64(IntPtr salLocateHandle, ref LocateMeasParms2 parms, out Int32 measurementId);

        public static SalError salCreateLocationMeasurement(IntPtr salLocateHandle, ref LocateMeasParms2 parms, out Int32 measurementId)
        {
            if (IntPtr.Size == 8)
                return salCreateLocationMeasurement_X64(salLocateHandle, ref parms, out measurementId);
            else
                return salCreateLocationMeasurement_Win32(salLocateHandle, ref parms, out measurementId);
        }

        // ===================== salAddOrUpdateTimeseries( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salAddOrUpdateTimeseries", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salAddOrUpdateTimeseries_Win32(IntPtr smsHandle, Int32 measurementId, ref TimeseriesInfo2 info, float[] buffer);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salAddOrUpdateTimeseries", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salAddOrUpdateTimeseries_X64(IntPtr smsHandle, Int32 measurementId, ref TimeseriesInfo2 info, float[] buffer);

        public static SalError salAddOrUpdateTimeseries(IntPtr smsHandle, Int32 measurementId, ref TimeseriesInfo2 info, float[] buffer)
        {
            if (IntPtr.Size == 8)
                return salAddOrUpdateTimeseries_X64(smsHandle, measurementId, ref info, buffer);
            else
                return salAddOrUpdateTimeseries_Win32(smsHandle, measurementId, ref info, buffer);
        }

        // ===================== salGetTimeseries( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetTimeseries", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetTimeseries_Win32(IntPtr smsHandle, Int32 measurementId, ref TimeseriesInfo2 info, float[] buffer, ref UInt32 numBytesInData);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetTimeseries", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetTimeseries_X64(IntPtr smsHandle, Int32 measurementId, ref TimeseriesInfo2 info, float[] buffer, ref UInt32 numBytesInData);

        public static SalError salGetTimeseries(IntPtr smsHandle, Int32 measurementId, ref TimeseriesInfo2 info, float[] buffer, ref UInt32 numBytesInData)
        {
            if (IntPtr.Size == 8)
                return salGetTimeseries_X64(smsHandle, measurementId, ref info, buffer, ref numBytesInData);
            else
                return salGetTimeseries_Win32(smsHandle, measurementId, ref info, buffer, ref numBytesInData);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int SAL_TIMESERIES_CALLBACK(ref TimeseriesInfo3 info, ref IntPtr iqDataArray, UInt32 numFloatsInArray, IntPtr userWorkspace);

        // ===================== salListenForTimeseriesData( )  ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salListenForTimeseriesData", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salListenForTimeseriesData_Win32(IntPtr smsHandle, SAL_TIMESERIES_CALLBACK callback, UInt32 startStop, IntPtr userWorkspace);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salListenForTimeseriesData", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salListenForTimeseriesData_X64(IntPtr smsHandle, SAL_TIMESERIES_CALLBACK callback, UInt32 startStop, IntPtr userWorkspace);

        public static SalError salListenForTimeseriesData(IntPtr smsHandle, SAL_TIMESERIES_CALLBACK callback, UInt32 startStop, IntPtr userWorkspace)
        {
            if (IntPtr.Size == 8)
                return salListenForTimeseriesData_X64(smsHandle, callback, startStop, userWorkspace);
            else
                return salListenForTimeseriesData_Win32(smsHandle, callback, startStop, userWorkspace);
        }

        //////// END agSalLocation.h ////////

        //////// BEGIN agSalFrequency.h ////////

        /**********************************************************/
        /*  Frequency data functions                              */
        /**********************************************************/

        public const UInt32 FREQ_DATA_MIN_VERSION = 0x10002;

        public const int FFT_POINTS_MIN = 8;
        public const int FFT_POINTS_MAX = 16384;

        public const int SAL_FFT_RECOMMENDED_BYTES = FFT_POINTS_MAX * sizeof(float);

        public enum WindowType
        {
            Window_hann,         /**< Hann window (sometimes called the Hanning window)*/
            Window_gaussTop,     /**< Gausstop window  */
            Window_flatTop,      /**< Flattop window  */
            Window_uniform       /**< Uniform window */
        }

        public enum AverageType
        {
            Average_off,         /**< No averaging */
            Average_rms,         /**< RMS averaging */
            Average_peak,        /**< Peak-hold averaging */
        }

        public enum FftDataType
        {
            FftData_db,         /**< dBm data from sensor, 2 bytes/bin */
            FftData_mag         /**< v^2 data from sensor, 4 bytes/bin */
        }

        public enum OverlapType
        {
            OverlapType_on,
            OverlapType_off,
        }

        public enum MonitorMode
        {
            MonitorMode_off,            /**< Do not use monitor mode */
            MonitorMode_on,             /**< If there is an FFT measurement running on the sensor,
                                              send data in "eavesdrop mode" */
        }

        // FFT SWEEP_FLAGS bit values
        // These values represent the valid bits int the FFT header sweepFlags field.
        //Same as agSalFrequency.h enum salSWEEP_FLAGS.
        public const int salSWEEP_MEAS_ERROR = 0x0001;  /**< Measurement hardware error */
        public const int salSWEEP_SETUP_NOT_USER = 0x0002;  /**< setup changed by differnt measurement operation */
        public const int salSWEEP_SEGMENT_TOO_LATE = 0x0004;  /**< FFT segment too late */
        public const int salSWEEP_END_OF_DATA = 0x0008;  /**< This is the last block of data for the current measurement; measurement may have terminated early */
        public const int salSWEEP_MONITOR_MODE = 0x0010;  /**< Monitor mode FFT */
        public const int salSWEEP_REF_OSC_ADJUSTED = 0x0020;  /**< If set; the sensor clock reference oscillator was adjusted during the measurement  */
        public const int salSWEEP_OVERLOAD_DETECTED = 0x0040;  /**< Overload detected */
        public const int salSWEEP_FREQ_OUT_OF_BOUNDS = 0x0080;  /**< Center frequency out of bounds, value clamped to valid range */
        public const int salSWEEP_CONNECTION_ERROR = 0x1000;  /**< Connection problem to sensor */
        public const int salSWEEP_LAST_SEGMENT = 0x4000;  /**< This is the last block of data for the current measurement */
        public const int salSWEEP_STOPPING = 0x8000;  /**< FFT sweep is stopping */
        public const int salSWEEP_MISSING_DATA = 0x10000; /**< Gap in FFT data */
        public const int salSWEEP_CPU_OVERLOAD = 0x20000; /**< If set; the sensor's CPU is compute bound */
        public const int salSWEEP_SYNC_PROBLEM = 0x40000; /**< If set; the sensor's synchronization is suspect */

        // salSweepReturnDataControl bit values
        // These values control what FFT data is returned to the user
        //Same as agSalFrequency.h enum salFFT_DATA_CONTROL.
        public const uint salSWEEP_DATA_SAMPLE_LOSS = 0x01;    /**< Indicates that this data block is not contiguous with previous block */
        public const uint salSWEEP_DATA_OVER_RANGE = 0x02;    /**< RF Sensor input overload */
        public const uint salSWEEP_DATA_BLOCK_MEASUREMENT_ERROR = 0x04;    /**< Measurement hardware error */
        public const uint salSWEEP_DATA_SETUP_NOT_USER = 0x08;    /**< The measurement setup is different than requested */
        public const uint salSWEEP_DATA_LAST_BLOCK = 0x10;    /**< This is the last block of data for the current measurement */
        public const uint salSWEEP_DATA_OSCILLATOR_ADJUSTED = 0x20;    /**< If set; the sensor clock reference oscillator was adjusted during the measurement  */
        public const uint salSWEEP_DATA_SEGMENT_TIMEOUT = 0x40;    /**< If set; synchronized FFT segment was not completed in scheduled time */
        public const uint salSWEEP_DATA_CPU_OVERLOAD = 0x80;    /**< If set, the sensor's CPU is compute bound */
        public const uint salSWEEP_DATA_SYNC_PROBLEM = 0x100;   /**< If set, the sensor's synchronization is suspect */
        public const uint salSWEEP_IGNORE_SYNC = 0x1000;  /**< If set, this segment will begin immediately (rather than being time-triggered) */
        public const uint salSWEEP_DATA_SEGMENT_NO_ARRAY = 0x4000;  /**< If set; the data array will not be transferred (i.e. header info only) */
        public const uint salSWEEP_DATA_SEGMENT_SILENT = 0x8000;  /**< If set; silent mode (i.e. no transfer) */

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct FrequencySegment
        {
            public AntennaType antenna;     /**< Antenna input for this segment */
            public Int32 preamp;            /**< Preamp input state (0=off; otherwise, on). Used only in the 750 to 1800 MHz range */
            public UInt32 numFftPoints;     /**< FFT points; must be power of 2 between ::SAL_FFT_POINTS_MIN and ::SAL_FFT_POINTS_MAX */
            public AverageType averageType; /**< Average type for this segment */
            public UInt32 numAverages;      /**< Number of averages for this segment */
            public UInt32 firstPoint;       /**< Index of first point to return; must be less than numFftPoints */
            public UInt32 numPoints;        /**< Number of points to return; must be less than or equal to numFftPoints */
            public UInt32 repeatAverage;    /**< If true, repeat the measurement until duration has elapsed */
            public double attenuation;      /**< Input attenuation in dB for this segment */
            public double centerFrequency;  /**< Center frequency of RF data */
            public double sampleRate;       /**< Sample rate of RF data */
            public double duration;         /**< Time interval (sec) between the start of this segment and the start of the next segment */
            public double mixerLevel;       /**< Mixer level in dB; range is -10 to 10 dB, 0 dB gives best compromise between SNR and distortion. */
            public OverlapType overlapType; /**< 0 means use overlap averaging; 1 means do not overlap */
            public FftDataType dataType;    /**< FFT Data type for this segment */
            public Int32 noTunerChange;     /**< Set this to non-zero value if you do not want to modify tuner for this segment. */
            public UInt32 noDataTransfer;   /**< Set this to non-zero value to control return data for this segment. See salSweepReturnDataControl. */
            public double reserved3;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct SweepParms
        {
            public UInt32 numSweeps;        /**< Number of sweeps to perform; 0 means sweep until a stop command is sent */
            public UInt32 numSegments;      /**< Number of segments in the sweep */
            public WindowType window;       /**< Window applied to time record before performing FFT  */
            public IntPtr userWorkspace;    /**< User-defined value that will be returned with each data message */
            public DataType dataType;       /**< Data type for returned power spectrum; */
            public Int32 reserved1;         /**< reserved */
            public Int32 syncSweepEnable;   /**< Set to non-zero when performing synchronous sweeps. */
            public double sweepInterval;    /**< Interval between end of last sweep and start of next one, in seconds. */
            public UInt32 syncSweepSec;     /**< "sec" start time for first segment (synchrounous sweep only). */
            public UInt32 syncSweepNSec;    /**< "nsec" start time for first segment (synchrounous sweep only). */
            public MonitorMode monitorMode; /**< Enable/disable monitor mode */
            public double monitorInterval;  /**< When monitorMode is salMonitorMode_on, send results back at this interval */
            public IntPtr reserved;         /**< Used internally */
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct SegmentData
        {
            public IntPtr userWorkspace;    /**< User-defined value set in :: salSweepParms */
            public UInt32 segmentIndex;     /**< 0-based index of this segment in the segmentTable  */
            public UInt32 sequenceNumber;   /**< starts at 0; incremented by 1 for each frequency result  */
            public UInt32 sweepIndex;       /**< starts at 0; incremented by 1 at the end of a sweep */

            public UInt32 timestampSec;     /**< Integer seconds part of timestamp of first time point in this segment */
            public UInt32 timestampNSec;    /**< Fractional seconds part of timestamp of first time point in this segment */
            public UInt32 timeQuality;      /**< Measure of time quality of timestamp */
            public Location location;       /**< Sensor location when this segment was measured */

            public double startFrequency;   /**< Frequency of first point returnded by this measurement */
            public double frequencyStep;    /**< Frequency spacing in Hertz of frequency data */
            public UInt32 numPoints;        /**< Number of frequency points returned by this measurement */
            public UInt32 overload;         /**< If not 0, the sensor input overloaded during this segment */
            public AmplitudeType dataType;  /**< Data type of returned amplitude data */
            public UInt32 lastSegment;      /**< If not zero, this is the last segment before measurement stops */

            public WindowType window;       /**< Window used for this measurement */
            public AverageType averageType; /**< Average type used in this measurement */
            public UInt32 numAverages;      /**< Number of averages used in this measurement */
            public double fftDuration;      /**< Duration of one FFT result */
            public double averageDuration;  /**< Duration of this complete measurement (all numAverages)  */

            public UInt32 isMonitor;
            public SalError errorNum;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_ERROR_STRING)]
            public string errorInfo;
            public UInt32 sweepFlags;       /**< Mask of indicators for various conditions (see ::salSWEEP_FLAGS). */
            public UInt32 timeAlarms;
            public double sampleRate;
            public IntPtr measHandle;       /**< Handle to the running sweep (see salStartSweep()) */
            public IntPtr sensorHandle;     /**< Connection handle to the sensor */
            public IntPtr reserved;


            public bool IsMonitor() { return isMonitor != 0; }
            public bool IsMissingData
            {
                //returns true if single-segment sweep has gaps
                get { return (sweepFlags & salSWEEP_MISSING_DATA) != 0; }
            }
            public bool IsSegmentTimeout
            {
                get { return (sweepFlags & salSWEEP_SEGMENT_TOO_LATE) != 0; }
            }
            public bool IsTimeQuestionable
            {
                get { return (timeAlarms & (UInt32)TimeSyncAlarmBits.TimeQuestionable) != 0; }
            }
            public bool IsClockNotSet
            {
                get { return (timeAlarms & (UInt32)TimeSyncAlarmBits.ClockSet) != 0; }
            }

            public bool IsLastBlock
            {
                get { return (sweepFlags & (UInt32)salSWEEP_LAST_SEGMENT) != 0; }
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct SweepComputationParms
        {
            public double startFrequency;   /**< Start frequency for the sweep (Hz) */
            public double stopFrequency;    /**< Stop frequency for the sweep (Hz) */
            public double rbw;              /**< Resolution band-width (Hz) */
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct SweepComputationResults
        {
            public double stepFreq;						/**< Computed desired FFT bin size (converted from rbw and window) */
            public double fftBinSize;						/**< Actual FFT bin size (some power of 2) */
            public double actualRbw;						/**< Actual RBW (related to fftBinSize by window type) */
            public double tunerSampleRate;				    /**< Actual tuner sample rate (Hz) */
            public UInt32 fftBlockSize;					/**< FFT size */
            public double nyquistFactor;					/**< Either 1.4 or 1.28 depending on tunerSampleRate */
            public UInt32 numBinsReturned;				    /**< Number of FFT bins returned in each segment */
            public UInt32 numBinsReturnedLastSegment;		/**< Number of FFT bins returned in the last segment */
            public UInt32 firstPointIdx;					/**< Index of first FFT bin returned */
            public UInt32 firstPointIdxLastSegment;		/**< Index of first FFT bin returned in the last segment */
            public UInt32 numSegments;					    /**< Number of FFT segments to cover the span */
            public double centerFrequencyFirstSegment;	    /**< Center frequency of the first segment */
            public double centerFrequencyLastSegment;		/**< Center frequency of the last segment */
        }

        public unsafe struct salFftTimestampParam
        {
            public UInt32 timestampSec;    /**< Integer seconds part of FFT timestamp */
            public UInt32 timestampNSec;   /**< Fractional seconds part of FFT timestamp */
            public UInt32 numSamples;      /**< Number of IQ samples */
            public UInt32 edgeSelect;      /**< Edge selection 0==Rising, 1==Falling */
            public double centerFrequency; /**< IQ Center Frequency [Hz] */
            public double sampleRate;      /**< IQ sample rate [Hz] */
            public double triggerLevel;    /**< IQ trigger level [dBm] */
            public double lowSignalTime;   /**< Time before (or after) the edge where the signal must be low [sec] */
            public double retrySpan;       /**< Minimum span between peak and firstPoint for retry [dB] */
        }

        public unsafe struct salFftTimestampResult
        {
            public UInt32 timestampSec;   /**< Integer seconds part of FFT timestamp */
            public UInt32 timestampNSec;  /**< Fractional seconds part of FFT timestamp */
            public UInt32 edgeDetected;   /**< flag indicating edge was detected */
            public double peak;           /**< peak IQ magnitude (dBm) */
            public double firstPoint;     /**< First Point IQ magnitude (dBm) */
            public double lastPoint;      /**< Last Point IQ magnitude (dBm) */
        }

        // ===================== salComputeFftSegmentTableSize( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salComputeFftSegmentTableSize", CharSet = CharSet.Ansi)]
        private static extern SalError salComputeFftSegmentTableSize_Win32(ref SweepComputationParms computeParms, ref SweepParms sweepParms, out SweepComputationResults result);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salComputeFftSegmentTableSize", CharSet = CharSet.Ansi)]
        private static extern SalError salComputeFftSegmentTableSize_X64(ref SweepComputationParms computeParms, ref SweepParms sweepParms, out SweepComputationResults result);

        public static SalError salComputeFftSegmentTableSize(ref SweepComputationParms computeParms, ref SweepParms sweepParms, out SweepComputationResults result)
        {
            if (IntPtr.Size == 8)
                return salComputeFftSegmentTableSize_X64(ref computeParms, ref sweepParms, out result); // Call 64-bit DLL
            else
                return salComputeFftSegmentTableSize_Win32(ref computeParms, ref sweepParms, out result); // Call 32-bit DLL
        }

        // ===================== salInitializeFftSegmentTable( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salInitializeFftSegmentTable", CharSet = CharSet.Ansi)]
        private static extern SalError salInitializeFftSegmentTable_Win32(ref SweepComputationParms computeParms,
            ref SweepParms sweepParms, ref FrequencySegment exampleSegment, IntPtr buffer, out SweepComputationResults result);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salInitializeFftSegmentTable", CharSet = CharSet.Ansi)]
        private static extern SalError salInitializeFftSegmentTable_X64(ref SweepComputationParms computeParms,
            ref SweepParms sweepParms, ref FrequencySegment exampleSegment, IntPtr buffer, out SweepComputationResults result);

        public static SalError salInitializeFftSegmentTable(ref SweepComputationParms computeParms, ref SweepParms sweepParms,
            ref FrequencySegment exampleSegment, out FrequencySegment[] segTable, out SweepComputationResults result)
        {
            segTable = new AgSalLib.FrequencySegment[sweepParms.numSegments];
            SalError err;

            int iStructSize = Marshal.SizeOf(exampleSegment);
            IntPtr buffer = Marshal.AllocCoTaskMem(iStructSize * segTable.Length);

            if (IntPtr.Size == 8)
                err = salInitializeFftSegmentTable_X64(ref computeParms, ref sweepParms, ref exampleSegment,
                    buffer, out result); // Call 64-bit DLL
            else
                err = salInitializeFftSegmentTable_Win32(ref computeParms, ref sweepParms, ref exampleSegment,
                    buffer, out result); // Call 32-bit DLL

            if (err != AgSalLib.SalError.SAL_ERR_NONE) return err; // Early exit

            // Marshal back the result
            int offset = 0;
            for (int i = 0; i < segTable.Length; i++)
            {
                IntPtr ptr = new IntPtr(buffer.ToInt32() + offset);
                segTable[i] = (FrequencySegment)Marshal.PtrToStructure(ptr, typeof(FrequencySegment));
                offset += iStructSize;
            }

            Marshal.FreeCoTaskMem(buffer);
            return err;
        }

        // ===================== SAL_SEGMENT_CALLBACK( ) ==================================
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int SAL_SEGMENT_CALLBACK(ref SegmentData dataHeader, IntPtr data);

        // ===================== salStartSweep( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salStartSweep", CharSet = CharSet.Ansi)]
        private static extern SalError salStartSweep_Win32(out IntPtr measHandle, IntPtr sensorHandle, ref SweepParms parms, Byte[] segTable, SAL_SEGMENT_CALLBACK callback);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salStartSweep", CharSet = CharSet.Ansi)]
        private static extern SalError salStartSweep_X64(out IntPtr measHandle, IntPtr sensorHandle, ref SweepParms parms, Byte[] segTable, SAL_SEGMENT_CALLBACK callback);

        public static byte[] makeBytes(ref FrequencySegment[] segTable)
        {
            uint numBytes = (uint)Marshal.SizeOf(segTable[0]);
            numBytes *= (uint)segTable.Length;

            byte[] segTableBytes = new byte[numBytes];
            int offset = 0;

            for (int i = 0; i < segTable.Length; i++)
            {
                Array.Copy(BitConverter.GetBytes((int)segTable[i].antenna), 0, segTableBytes, offset, 4); offset += 4;
                Array.Copy(BitConverter.GetBytes(segTable[i].preamp), 0, segTableBytes, offset, 4); offset += 4;
                Array.Copy(BitConverter.GetBytes(segTable[i].numFftPoints), 0, segTableBytes, offset, 4); offset += 4;

                Array.Copy(BitConverter.GetBytes((int)segTable[i].averageType), 0, segTableBytes, offset, 4); offset += 4;
                Array.Copy(BitConverter.GetBytes(segTable[i].numAverages), 0, segTableBytes, offset, 4); offset += 4;

                Array.Copy(BitConverter.GetBytes(segTable[i].firstPoint), 0, segTableBytes, offset, 4); offset += 4;
                Array.Copy(BitConverter.GetBytes(segTable[i].numPoints), 0, segTableBytes, offset, 4); offset += 4;
                Array.Copy(BitConverter.GetBytes(segTable[i].repeatAverage), 0, segTableBytes, offset, 4); offset += 4;
                Array.Copy(BitConverter.GetBytes(segTable[i].attenuation), 0, segTableBytes, offset, 8); offset += 8;
                Array.Copy(BitConverter.GetBytes(segTable[i].centerFrequency), 0, segTableBytes, offset, 8); offset += 8;
                Array.Copy(BitConverter.GetBytes(segTable[i].sampleRate), 0, segTableBytes, offset, 8); offset += 8;
                Array.Copy(BitConverter.GetBytes(segTable[i].duration), 0, segTableBytes, offset, 8); offset += 8;
                Array.Copy(BitConverter.GetBytes(segTable[i].mixerLevel), 0, segTableBytes, offset, 8); offset += 8;
                Array.Copy(BitConverter.GetBytes((int)segTable[i].overlapType), 0, segTableBytes, offset, 4); offset += 4;
                Array.Copy(BitConverter.GetBytes((int)segTable[i].dataType), 0, segTableBytes, offset, 4); offset += 4;
                Array.Copy(BitConverter.GetBytes(segTable[i].noTunerChange), 0, segTableBytes, offset, 4); offset += 4;
                Array.Copy(BitConverter.GetBytes(segTable[i].noDataTransfer), 0, segTableBytes, offset, 4); offset += 4;
                Array.Copy(BitConverter.GetBytes(segTable[i].reserved3), 0, segTableBytes, offset, 8); offset += 8;

            }

            return segTableBytes;

        }

        public static SalError salStartSweep(out IntPtr measHandle, IntPtr sensorHandle, ref SweepParms parms, ref FrequencySegment[] segTable, SAL_SEGMENT_CALLBACK callback)
        {
            byte[] segTableBytes = makeBytes(ref segTable);

            if (IntPtr.Size == 8)
                return salStartSweep_X64(out measHandle, sensorHandle, ref parms, segTableBytes, callback); // Call 64-bit DLL
            else
                return salStartSweep_Win32(out measHandle, sensorHandle, ref parms, segTableBytes, callback); // Call 32-bit DLL
        }

        // ===================== salStartSweep2( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salStartSweep2", CharSet = CharSet.Ansi)]
        private static extern SalError salStartSweep2_Win32(out IntPtr measHandle, IntPtr sensorHandle, ref SweepParms parms, Byte[] segTable,
            ref salFlowControl flowControl, SAL_SEGMENT_CALLBACK callback);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salStartSweep2", CharSet = CharSet.Ansi)]
        private static extern SalError salStartSweep2_X64(out IntPtr measHandle, IntPtr sensorHandle, ref SweepParms parms, Byte[] segTable,
            ref salFlowControl flowControl, SAL_SEGMENT_CALLBACK callback);

        public static SalError salStartSweep2(out IntPtr measHandle, IntPtr sensorHandle, ref SweepParms parms, ref FrequencySegment[] segTable,
            ref salFlowControl flowControl, SAL_SEGMENT_CALLBACK callback)
        {
            byte[] segTableBytes = makeBytes(ref segTable);

            if (IntPtr.Size == 8)
                return salStartSweep2_X64(out measHandle, sensorHandle, ref parms, segTableBytes, ref flowControl, callback); // Call 64-bit DLL
            else
                return salStartSweep2_Win32(out measHandle, sensorHandle, ref parms, segTableBytes, ref flowControl, callback); // Call 32-bit DLL
        }

        // ===================== salGetSegmentData( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetSegmentData", CharSet = CharSet.Ansi)]
        private static extern SalError salGetSegmentData_Win32(IntPtr measHandle, out SegmentData dataHeader, float[] data, UInt32 maxDataBytes);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetSegmentData", CharSet = CharSet.Ansi)]
        private static extern SalError salGetSegmentData_X64(IntPtr measHandle, out SegmentData dataHeader, float[] data, UInt32 maxDataBytes);

        public static SalError salGetSegmentData(IntPtr measHandle, out SegmentData dataHeader, float[] data, UInt32 maxDataBytes)
        {
            if (IntPtr.Size == 8)
                return salGetSegmentData_X64(measHandle, out dataHeader, data, maxDataBytes); // Call 64-bit DLL
            else
                return salGetSegmentData_Win32(measHandle, out dataHeader, data, maxDataBytes); // Call 32-bit DLL
        }

        // ===================== salGetSweepBacklogStatus( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetSweepBacklogStatus", CharSet = CharSet.Ansi)]
        private static extern SalError salGetSweepBacklogStatus_Win32(IntPtr measHandle, out salBacklogStatus backlogStatus);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetSweepBacklogStatus", CharSet = CharSet.Ansi)]
        private static extern SalError salGetSweepBacklogStatus_X64(IntPtr measHandle, out salBacklogStatus backlogStatus);

        public static SalError salGetSweepBacklogStatus(IntPtr measHandle, out salBacklogStatus backlogStatus)
        {
            if (IntPtr.Size == 8)
                return salGetSweepBacklogStatus_X64(measHandle, out backlogStatus); // Call 64-bit DLL
            else
                return salGetSweepBacklogStatus_Win32(measHandle, out backlogStatus); // Call 32-bit DLL
        }

        // ===================== salRefineFftTimestamp( ) ==================================
        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salRefineFftTimestamp", CharSet = CharSet.Ansi)]
        private static extern SalError salRefineFftTimestamp_X64(IntPtr measHandle, ref salFftTimestampParam input, out salFftTimestampResult result);

        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salRefineFftTimestamp", CharSet = CharSet.Ansi)]
        private static extern SalError salRefineFftTimestamp_Win32(IntPtr measHandle, ref salFftTimestampParam input, out salFftTimestampResult result);

        public static SalError salRefineFftTimestamp(IntPtr measHandle, ref salFftTimestampParam input, out salFftTimestampResult result)
        {
            if (IntPtr.Size == 8)
                return salRefineFftTimestamp_X64(measHandle, ref input, out result); // Call 64-bit DLL
            else
                return salRefineFftTimestamp_Win32(measHandle, ref input, out result); // Call 32-bit DLL
        }

        public enum SweepCommand
        {
            SweepCommand_stop,         /**< Stop a sweep when the sweep is finished */
            SweepCommand_abort,        /**< Stop a sweep as soon as possible*/
            SweepCommand_flush         /**< Flush the sweep backlog */
        }

        // ===================== salSendSweepCommand( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSendSweepCommand", CharSet = CharSet.Ansi)]
        private static extern SalError salSendSweepCommand_Win32(IntPtr measHandle, SweepCommand command);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSendSweepCommand", CharSet = CharSet.Ansi)]
        private static extern SalError salSendSweepCommand_X64(IntPtr measHandle, SweepCommand command);

        public static SalError salSendSweepCommand(IntPtr measHandle, SweepCommand command)
        {
            if (IntPtr.Size == 8)
                return salSendSweepCommand_X64(measHandle, command); // Call 64-bit DLL
            else
                return salSendSweepCommand_Win32(measHandle, command); // Call 32-bit DLL
        }

        public enum SweepStatus
        {
            SweepStatus_stopped,               /**< Sweep is waiting to start  */
            SweepStatus_running,            /**< Sweep is running */
        }

        // ===================== salGetSweepStatus( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetSweepStatus", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSweepStatus_Win32(IntPtr measHandle, out SweepStatus status);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetSweepStatus", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSweepStatus_X64(IntPtr measHandle, out SweepStatus status);

        public static SalError salGetSweepStatus(IntPtr measHandle, out SweepStatus status)
        {
            if (IntPtr.Size == 8)
                return salGetSweepStatus_X64(measHandle, out status); // Call 64-bit DLL
            else
                return salGetSweepStatus_Win32(measHandle, out status); // Call 32-bit DLL
        }

        // ===================== salGetSweepStatus2( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetSweepStatus2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSweepStatus2_Win32(IntPtr measHandle, int fromNow, out SweepStatus status, out double elapsed);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetSweepStatus2", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetSweepStatus2_X64(IntPtr measHandle, int fromNow, out SweepStatus status, out double elapsed);

        public static SalError salGetSweepStatus2(IntPtr measHandle, int fromNow, out SweepStatus status, out double elapsed)
        {
            if (IntPtr.Size == 8)
                return salGetSweepStatus2_X64(measHandle, fromNow, out status, out elapsed); // Call 64-bit DLL
            else
                return salGetSweepStatus2_Win32(measHandle, fromNow, out status, out elapsed); // Call 32-bit DLL
        }

        //////// END agSalFrequency.h ////////

        //////// BEGIN agSalTimeData.h ////////

        /**********************************************************/
        /*  Time data functions                                   */
        /**********************************************************/

        public const UInt32 TIME_DATA_MIN_VERSION = 0x10002;

        public enum Demodulation
        {
            None = 0,  /**< No Demodulation */
            AM = 1,  /**< AM Demodulation */
            FM = 2   /**< FM Demodulation */
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct AudioParms
        {
            public double sampleRate;    /**< Sample rate of returned audio */
            public Int32 squelchState;   /**< 0 = off, 1 = on */
            public double squelchLevel;  /**< Squelch level (0-10) */
        }

        public enum TriggerSlope
        {
            RISING = 0,  /**< IF Magnitude trigger on rising edge */
            FALLING = 1,  /**< IF Magnitude trigger on falling edge */
            EITHER = 2   /**< IF Magnitude trigger on either edge */
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct IfMagTriggerParms
        {
            public UInt16 ifMagTriggerEnabled; /**< set to 1 to enable the IF magnitude trigger mechanism */
            public UInt16 useLearnEnv;         /**< set to 1 if using the learn environment as the trigger level */
            public UInt16 triggerSlope;        /**< 0 == rising edge, 1 == falling edge, 2 == either edge */
            public float userTriggerLevel;     /**< trigger level (dBm or dB, see useLearnEnv param) */
            public double ifSearchTimeout;     /**< maximum time to search (seconds) */
            public double triggerHoldoff;      /**< adjustment to the trigger time (sec) */
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct IqPacingParms
        {
            public UInt32 pacingEnabled;       /**< set to 1 to enable the IQ Pacing */
            public UInt32 measCount;           /**< number of IQ measurements desired, 0 == continuous */
            public UInt32 measIntervalSec;     /**< secs portion of measurement period */
            public UInt32 measIntervalNSec;    /**< nsecs portion of measurement period */
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct TimeDataParms
        {
            public IntPtr userWorkspace;       /**< User-defined value that will be returned with each data header. */
            public double centerFrequency;     /**< The center frequency of the signal. */
            public double sampleRate;          /**< Sample rate in Hertz.*/
            public UInt32 triggerTimeSecs;     /**< If triggerTimeSecs is 0, live IQ data is returned. Otherwise, absolute trigger time in POSIX time of the time data. */
            public UInt32 triggerTimeNSecs;    /**< Trigger fractional seconds */
            public UInt32 numTransferSamples;  /**< Number of samples to accumulate in sensor before sending over network; must be <= numSamples*/
            public UInt64 numSamples;          /**< Total number of samples to acquire */
            public DataType dataType;          /**< Resolution of the returned data */
            public Demodulation demodulation;  /**< Demodulation type */
            public AudioParms audioParms;      /**< Parameters for demodulated audio; only valid if demodulation is specified */
            public UInt32 reserved;	           /**< Reserved for future use */
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct TimeDataParms2
        {
            public IntPtr userWorkspace;          /**< User-defined value that will be returned with each data header. */
            public double centerFrequency;        /**< The center frequency of the signal. */
            public double sampleRate;             /**< Sample rate in Hertz.*/
            public UInt32 triggerTimeSecs;        /**< If triggerTimeSecs is 0, live IQ data is returned. Otherwise, absolute trigger time in POSIX time of the time data. */
            public UInt32 triggerTimeNSecs;       /**< Trigger fractional seconds */
            public UInt32 numTransferSamples;     /**< Number of samples to accumulate in sensor before sending over network; must be <= numSamples*/
            public UInt64 numSamples;             /**< Total number of samples to acquire */
            public DataType dataType;             /**< Resolution of the returned data */
            public Demodulation demodulation;     /**< Demodulation type */
            public AudioParms audioParms;         /**< Parameters for demodulated audio; only valid if demodulation is specified */
            public IfMagTriggerParms ifTrigParms; /**< Parameters for IF magnitude triggering */
            public UInt32 reserved;	              /**< Reserved for future use */
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct TimeDataParms3
        {
            public IntPtr userWorkspace;           /**< User-defined value that will be returned with each data header. */
            public double centerFrequency;         /**< The center frequency of the signal. */
            public double sampleRate;              /**< Sample rate in Hertz.*/
            public UInt32 triggerTimeSecs;         /**< If triggerTimeSecs is 0, live IQ data is returned. Otherwise, absolute trigger time in POSIX time of the time data. */
            public UInt32 triggerTimeNSecs;        /**< Trigger fractional seconds */
            public UInt32 numTransferSamples;      /**< Number of samples to accumulate in sensor before sending over network; must be <= numSamples*/
            public UInt64 numSamples;              /**< Total number of samples to acquire */
            public DataType dataType;              /**< Resolution of the returned data */
            public Demodulation demodulation;      /**< Demodulation type */
            public AudioParms audioParms;          /**< Parameters for demodulated audio; only valid if demodulation is specified */
            public IfMagTriggerParms ifTrigParms;  /**< Parameters for IF magnitude triggering */
            public IqPacingParms iqPacingParms;    /**< Parameters for pacing control */
            public salFlowControl flowControl;     /**< Parameters for flow control */
            public UInt32 reserved;	               /**< Reserved for future use */
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct TimeData
        {
            public UInt64 sequenceNumber;               /**< starts at 0; incremented by 1 for each data block */
            public DataType dataType;                   /**< Type of data (real or complex) and rewsolution (16 bit, 32 bit, etc) */
            public UInt32 numSamples;                   /**< Number of samples in this data block. A complex pair is considered 1 sample. */
            public double scaleToVolts;                 /**< Multiply data samples by this value to convert to Volts.   */
            public UInt32 stateEventIndicator;          /**< Mask of indicators for various conditions (see ::salSTATE_EVENT). */
            public UInt32 changeIndicator;              /**< Bitmap indicating data description values that have changed during this data block
                                                             (see ::salCHANGE for bit values).*/
            public UInt32 timestampSeconds;            /**< Integer part of the timestamp (in UTC seconds since January 1, 1970). */
            public UInt32 timestampNSeconds;
            public Location location;                  /**< Location of sensor when data was acquired */

            public AntennaType antenna;                /**< Antenna input active for this data block. */
            public double attenuation;                 /**< Attenuation in dB; negative values indicate gain.  */
            public double centerFrequency;             /**< RF center frequency in Hertz for this data block. */
            public double sampleRate;                  /**< Sample rate in Hertz.  */
            public double variance1588;                /**< TODO.  */
            public double triggerLatency;              /**< TODO.  */
            public IntPtr userWorkspace;               /**< Value set by the userWorkspace parameter in the request.  */
            public UInt32 timeAlarms;                  /**< Indicates status of sensor time sycnh (bit map of ::salTimeAlarm values) */
            public SalError error;                     /**< Indicates errors that cause the measurement to fail or stop. */
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_ERROR_STRING)]
            public string errorInfo;
            public TriggerSlope triggerSlope;          /**< Triggers slope (valid when in IF Magnitude trigger mode */
            public IntPtr measHandle;                  /**< Handle to the IQ Measurement (see salRequestTimeData()) */
            public IntPtr sensorHandle;                /**< Connection handle to the sensor */

            // stateEventIndicator bit values
            public bool IsOverload
            {
                get { return (stateEventIndicator & IqDataHeader.salSTATE_OVER_RANGE) != 0; }
            }

            public bool IsLastBlock
            {
                get { return (stateEventIndicator & IqDataHeader.salSTATE_LAST_BLOCK) != 0; }
            }
            public bool IsTimeQuestionable
            {
                get { return (timeAlarms & (UInt32)TimeSyncAlarmBits.TimeQuestionable) != 0; }
            }
            public bool IsClockNotSet
            {
                get { return (timeAlarms & (UInt32)TimeSyncAlarmBits.ClockSet) != 0; }
            }
            public UInt32 BytesPerSample
            {
                get
                {
                    switch (dataType)
                    {
                        case DataType.Complex16: return 4;
                        case DataType.Complex32: return 8;
                        default: return 0;
                    }
                }
            }
        }

        // ===================== SAL_TIME_DATA_CALLBACK( ) ==================================
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int SAL_TIME_DATA_CALLBACK(ref TimeData dataHeader, IntPtr data);

        // ===================== salRequestTimeData( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salRequestTimeData", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern SalError salRequestTimeData_Win32(out IntPtr measHandle, IntPtr sensorHandle, ref TimeDataParms parms, SAL_TIME_DATA_CALLBACK callback);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salRequestTimeData", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern SalError salRequestTimeData_X64(out IntPtr measHandle, IntPtr sensorHandle, ref TimeDataParms parms, SAL_TIME_DATA_CALLBACK callback);

        public static SalError salRequestTimeData(out IntPtr measHandle, IntPtr sensorHandle, ref TimeDataParms parms, SAL_TIME_DATA_CALLBACK callback)
        {
            if (IntPtr.Size == 8)
                return salRequestTimeData_X64(out measHandle, sensorHandle, ref parms, callback); // Call 64-bit DLL
            else
                return salRequestTimeData_Win32(out measHandle, sensorHandle, ref parms, callback); // Call 32-bit DLL
        }

        // ===================== salRequestTimeData2( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salRequestTimeData2", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern SalError salRequestTimeData2_Win32(out IntPtr measHandle, IntPtr sensorHandle, ref TimeDataParms2 parms, SAL_TIME_DATA_CALLBACK callback);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salRequestTimeData2", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern SalError salRequestTimeData2_X64(out IntPtr measHandle, IntPtr sensorHandle, ref TimeDataParms2 parms, SAL_TIME_DATA_CALLBACK callback);

        public static SalError salRequestTimeData2(out IntPtr measHandle, IntPtr sensorHandle, ref TimeDataParms2 parms, SAL_TIME_DATA_CALLBACK callback)
        {
            if (IntPtr.Size == 8)
                return salRequestTimeData2_X64(out measHandle, sensorHandle, ref parms, callback); // Call 64-bit DLL
            else
                return salRequestTimeData2_Win32(out measHandle, sensorHandle, ref parms, callback); // Call 32-bit DLL
        }

        // ===================== salRequestTimeData3( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salRequestTimeData3", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern SalError salRequestTimeData3_Win32(out IntPtr measHandle, IntPtr sensorHandle, ref TimeDataParms3 parms, SAL_TIME_DATA_CALLBACK callback);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salRequestTimeData3", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern SalError salRequestTimeData3_X64(out IntPtr measHandle, IntPtr sensorHandle, ref TimeDataParms3 parms, SAL_TIME_DATA_CALLBACK callback);

        public static SalError salRequestTimeData3(out IntPtr measHandle, IntPtr sensorHandle, ref TimeDataParms3 parms, SAL_TIME_DATA_CALLBACK callback)
        {
            if (IntPtr.Size == 8)
                return salRequestTimeData3_X64(out measHandle, sensorHandle, ref parms, callback); // Call 64-bit DLL
            else
                return salRequestTimeData3_Win32(out measHandle, sensorHandle, ref parms, callback); // Call 32-bit DLL
        }

        // ===================== salGetTimeData( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetTimeData", CharSet = CharSet.Ansi)]
        private static extern SalError salGetTimeData_Win32(IntPtr measHandle, out TimeData dataHdr, short[] data, UInt32 maxDataBytes);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetTimeData", CharSet = CharSet.Ansi)]
        private static extern SalError salGetTimeData_X64(IntPtr measHandle, out TimeData dataHdr, short[] data, UInt32 maxDataBytes);

        public static SalError salGetTimeData(IntPtr measHandle, out TimeData dataHdr, short[] data, UInt32 maxDataBytes)
        {
            if (IntPtr.Size == 8)
                return salGetTimeData_X64(measHandle, out dataHdr, data, maxDataBytes); // Call 64-bit DLL
            else
                return salGetTimeData_Win32(measHandle, out dataHdr, data, maxDataBytes); // Call 32-bit DLL
        }

        // ===================== salGetTimeData( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetTimeData", CharSet = CharSet.Ansi)]
        private static extern SalError salGetTimeData_Win32(IntPtr measHandle, out TimeData dataHdr, int[] data, UInt32 maxDataBytes);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetTimeData", CharSet = CharSet.Ansi)]
        private static extern SalError salGetTimeData_X64(IntPtr measHandle, out TimeData dataHdr, int[] data, UInt32 maxDataBytes);

        public static SalError salGetTimeData(IntPtr measHandle, out TimeData dataHdr, int[] data, UInt32 maxDataBytes)
        {
            if (IntPtr.Size == 8)
                return salGetTimeData_X64(measHandle, out dataHdr, data, maxDataBytes); // Call 64-bit DLL
            else
                return salGetTimeData_Win32(measHandle, out dataHdr, data, maxDataBytes); // Call 32-bit DLL
        }

        // ===================== salTimeDataGetAttribute( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salTimeDataGetAttribute", CharSet = CharSet.Ansi)]
        private static extern SalError salTimeDataGetAttribute_Win32(IntPtr measHandle, ref IqAttribute iqAttribute, ref byte[] buffer);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salTimeDataGetAttribute", CharSet = CharSet.Ansi)]
        private static extern SalError salTimeDataGetAttribute_X64(IntPtr measHandle, ref IqAttribute iqAttribute, ref byte[] buffer);

        public static SalError salTimeDataGetAttribute(IntPtr measHandle, ref IqAttribute iqAttribute, ref byte[] buffer)
        {
            if (IntPtr.Size == 8)
                return salTimeDataGetAttribute_X64(measHandle, ref iqAttribute, ref buffer); // Call 64-bit DLL
            else
                return salTimeDataGetAttribute_Win32(measHandle, ref iqAttribute, ref buffer); // Call 32-bit DLL
        }

        public enum TimeDataCmd
        {
            Stop,         /**< Stop a time data request, but keep sends data acquired so far */
            Abort         /**< Stop a time data request and discard any data not sent */
        }

        // ===================== salSendTimeDataCommand( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSendTimeDataCommand", CharSet = CharSet.Ansi)]
        private static extern SalError salSendTimeDataCommand_Win32(IntPtr measHandle, TimeDataCmd command);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSendTimeDataCommand", CharSet = CharSet.Ansi)]
        private static extern SalError salSendTimeDataCommand_X64(IntPtr measHandle, TimeDataCmd command);

        public static SalError salSendTimeDataCommand(IntPtr measHandle, TimeDataCmd command)
        {
            if (IntPtr.Size == 8)
                return salSendTimeDataCommand_X64(measHandle, command); // Call 64-bit DLL
            else
                return salSendTimeDataCommand_Win32(measHandle, command); // Call 32-bit DLL
        }

        // ===================== salModifyTimeDataFrequency( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salModifyTimeDataFrequency", CharSet = CharSet.Ansi)]
        private static extern SalError salModifyTimeDataFrequency_Win32(IntPtr measHandle, double centerFrequency);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salModifyTimeDataFrequency", CharSet = CharSet.Ansi)]
        private static extern SalError salModifyTimeDataFrequency_X64(IntPtr measHandle, double centerFrequency);

        public static SalError salModifyTimeDataFrequency(IntPtr measHandle, double centerFrequency)
        {
            if (IntPtr.Size == 8)
                return salModifyTimeDataFrequency_X64(measHandle, centerFrequency); // Call 64-bit DLL
            else
                return salModifyTimeDataFrequency_Win32(measHandle, centerFrequency); // Call 32-bit DLL
        }

        public const double MIN_SQUELCH = -135.0;
        public const double MAX_SQUELCH = -20.0;

        // ===================== salModifyTimeDataSquelch( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salModifyTimeDataSquelch", CharSet = CharSet.Ansi)]
        private static extern SalError salModifyTimeDataSquelch_Win32(IntPtr measHandle, Int32 squelchState, double centerFrequency);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salModifyTimeDataSquelch", CharSet = CharSet.Ansi)]
        private static extern SalError salModifyTimeDataSquelch_X64(IntPtr measHandle, Int32 squelchState, double centerFrequency);

        public static SalError salModifyTimeDataSquelch(IntPtr measHandle, Int32 squelchState, double centerFrequency)
        {
            if (IntPtr.Size == 8)
                return salModifyTimeDataSquelch_X64(measHandle, squelchState, centerFrequency); // Call 64-bit DLL
            else
                return salModifyTimeDataSquelch_Win32(measHandle, squelchState, centerFrequency); // Call 32-bit DLL
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public unsafe struct TunerParms
        {
            public double centerFrequency;
            public double sampleRate;
            public double attenuation;
            public double mixerLevel;
            public AntennaType antenna;
            public Int32 preamp;  // Used only in the 750 to 1800 MHz range
            public Int32 reserved;
        }

        // ===================== salSetTuner( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSetTuner", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSetTuner_Win32(IntPtr sensorHandle, ref TunerParms tunerParms);


        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSetTuner", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSetTuner_X64(IntPtr sensorHandle, ref TunerParms tunerParms);

        public static SalError salSetTuner(IntPtr sensorHandle, ref TunerParms tunerParms)
        {
            if (IntPtr.Size == 8)
                return salSetTuner_X64(sensorHandle, ref tunerParms); // Call 64-bit DLL
            else
                return salSetTuner_Win32(sensorHandle, ref tunerParms); // Call 32-bit DLL
        }

        // ===================== salGetTuner( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetTuner", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetTuner_Win32(IntPtr sensorHandle, out TunerParms tunerParms);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetTuner", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salGetTuner_X64(IntPtr sensorHandle, out TunerParms tunerParms);

        public static SalError salGetTuner(IntPtr sensorHandle, out TunerParms tunerParms)
        {
            if (IntPtr.Size == 8)
                return salGetTuner_X64(sensorHandle, out tunerParms); // Call 64-bit DLL
            else
                return salGetTuner_Win32(sensorHandle, out tunerParms); // Call 32-bit DLL
        }

        // ===================== salSetTunerGroup( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSetTunerGroup", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSetTunerGroup_Win32(IntPtr sensorHandle, ref TunerParms parmsIn);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSetTunerGroup", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSetTunerGroup_X64(IntPtr sensorHandle, ref TunerParms parmsIn);

        public static SalError salSetTunerGroup(IntPtr sensorHandle, ref TunerParms parmsIn)
        {
            if (IntPtr.Size == 8)
                return salSetTunerGroup_X64(sensorHandle, ref parmsIn); // Call 64-bit DLL
            else
                return salSetTunerGroup_Win32(sensorHandle, ref parmsIn); // Call 32-bit DLL
        }

        // ===================== salGetIqBacklogStatus( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salGetIqBacklogStatus", CharSet = CharSet.Ansi)]
        private static extern SalError salGetIqBacklogStatus_Win32(IntPtr measHandle, out salBacklogStatus backlogStatus);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salGetIqBacklogStatus", CharSet = CharSet.Ansi)]
        private static extern SalError salGetIqBacklogStatus_X64(IntPtr measHandle, out salBacklogStatus backlogStatus);

        public static SalError salGetIqBacklogStatus(IntPtr measHandle, out salBacklogStatus backlogStatus)
        {
            if (IntPtr.Size == 8)
                return salGetSweepBacklogStatus_X64(measHandle, out backlogStatus); // Call 64-bit DLL
            else
                return salGetSweepBacklogStatus_Win32(measHandle, out backlogStatus); // Call 32-bit DLL
        }

        //////// END agSalTimeData.h ////////

        //////// BEGIN agSalSyncSweep.h ////////

        /**********************************************************/
        /* Sync Sweep                                           */
        /**********************************************************/

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct SyncSweepControl
        {
            public double minSegmentDurationClamp; /**< seconds: minimum duration for a segment */
            public double minSweepDurationClamp;    /**< seconds: minimum duration of sweep */
            public double segmentPadTime;        /**< Percentage to pad the calculated segment duration (>=0)*/
            public double sweepStartDelay;          /**< seconds: amount of time to delay start of first sweep (used to start all sensors at the same time) */
        }

        // ===================== salSyncSweepInit( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSyncSweepInit", CharSet = CharSet.Ansi)]
        private static extern SalError salSyncSweepInit_Win32(out IntPtr measHandle, IntPtr smsHandle,
            IntPtr groupHandle, ref SweepParms parms, Byte[] segTable, ref SyncSweepControl control, SAL_SEGMENT_CALLBACK callback, IntPtr[] userWorkspaces);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSyncSweepInit", CharSet = CharSet.Ansi)]
        private static extern SalError salSyncSweepInit_X64(out IntPtr measHandle, IntPtr smsHandle,
            IntPtr groupHandle, ref SweepParms parms, Byte[] segTable, ref SyncSweepControl control, SAL_SEGMENT_CALLBACK callback, IntPtr[] userWorkspaces);

        public static SalError salSyncSweepInit(out IntPtr measHandle, IntPtr smsHandle,
            IntPtr groupHandle, ref SweepParms parms, ref FrequencySegment[] segTable, ref SyncSweepControl control, SAL_SEGMENT_CALLBACK callback, IntPtr[] userWorkspaces)
        {
            byte[] segTableBytes = makeBytes(ref segTable);

            if (IntPtr.Size == 8)
                return salSyncSweepInit_X64(out measHandle, smsHandle, groupHandle, ref parms, segTableBytes, ref control, callback, userWorkspaces);
            else
                return salSyncSweepInit_Win32(out measHandle, smsHandle, groupHandle, ref parms, segTableBytes, ref control, callback, userWorkspaces);
        }

        // ===================== salSyncSweepStart( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSyncSweepStart", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSyncSweepStart_Win32(IntPtr measHandle, UInt32 sec, UInt32 nsec);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSyncSweepStart", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSyncSweepStart_X64(IntPtr measHandle, UInt32 sec, UInt32 nsec);

        public static SalError salSyncSweepStart(IntPtr measHandle, UInt32 sec, UInt32 nsec)
        {
            if (IntPtr.Size == 8)
                return salSyncSweepStart_X64(measHandle, sec, nsec);
            else
                return salSyncSweepStart_Win32(measHandle, sec, nsec);
        }

        // ===================== salSyncSweepSendCommand( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSyncSweepSendCommand", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSyncSweepSendCommand_Win32(IntPtr measHandle, SweepCommand command);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSyncSweepSendCommand", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSyncSweepSendCommand_X64(IntPtr measHandle, SweepCommand command);

        public static SalError salSyncSweepSendCommand(IntPtr measHandle, SweepCommand command)
        {
            if (IntPtr.Size == 8)
                return salSyncSweepSendCommand_X64(measHandle, command);
            else
                return salSyncSweepSendCommand_Win32(measHandle, command);
        }

        // ===================== salSyncSweepGetData( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSyncSweepGetData", CharSet = CharSet.Ansi)]
        private static extern SalError salSyncSweepGetData_Win32(IntPtr measHandle,
            [In, Out] SegmentData[] dataHeader, UInt32 maxHeaderBytes,
            float[] data, UInt32 maxDataBytes);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSyncSweepGetData", CharSet = CharSet.Ansi)]
        private static extern SalError salSyncSweepGetData_X64(IntPtr measHandle,
            [In, Out] SegmentData[] dataHeader, UInt32 maxHeaderBytes,
            float[] data, UInt32 maxDataBytes);

        public static SalError salSyncSweepGetData(IntPtr measHandle,
            ref SegmentData[] dataHeader, UInt32 numSensors,
            float[] data, UInt32 maxDataBytes)
        {
            UInt32 numBytes = (uint)Marshal.SizeOf(dataHeader[0]);
            UInt32 totalNumBytes = (uint)(numBytes * numSensors);

            SalError err;

            if (IntPtr.Size == 8)
                err = salSyncSweepGetData_X64(measHandle, dataHeader, totalNumBytes, data, maxDataBytes);
            else
                err = salSyncSweepGetData_Win32(measHandle, dataHeader, totalNumBytes, data, maxDataBytes);

            return err;
        }

        // ===================== salSyncSweepGetNumSensors( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSyncSweepGetNumSensors", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSyncSweepGetNumSensors_Win32(IntPtr measHandle, out UInt32 numSensors);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSyncSweepGetNumSensors", CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern SalError salSyncSweepGetNumSensors_X64(IntPtr measHandle, out UInt32 numSensors);

        public static SalError salSyncSweepGetNumSensors(IntPtr measHandle, out UInt32 numSensors)
        {
            if (IntPtr.Size == 8)
                return salSyncSweepGetNumSensors_X64(measHandle, out numSensors);
            else
                return salSyncSweepGetNumSensors_Win32(measHandle, out numSensors);
        }


        // ===================== salSyncSweepGetSensorNames( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSyncSweepGetSensorNames", CharSet = CharSet.Ansi)]
        private static extern SalError salSyncSweepGetSensorNames_Win32(IntPtr measHandle, byte[] nameBytes, UInt32 bufferSize);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSyncSweepGetSensorNames", CharSet = CharSet.Ansi)]
        private static extern SalError salSyncSweepGetSensorNames_X64(IntPtr measHandle, byte[] nameBytes, UInt32 bufferSize);

        public static SalError salSyncSweepGetSensorNames(IntPtr measHandle, string[] sensorNames)
        {
            UInt32 numBytes = (UInt32)(sensorNames.Length * MAX_SENSOR_NAME);

            byte[] bytes = new byte[numBytes];

            SalError err;
            if (IntPtr.Size == 8)
                err = salSyncSweepGetSensorNames_X64(measHandle, bytes, numBytes);
            else
                err = salSyncSweepGetSensorNames_Win32(measHandle, bytes, numBytes);

            if (err != SalError.SAL_ERR_NONE) return err;

            int offset = 0;
            for (int i = 0; i < sensorNames.Length; i++)
            {
                int len;
                for (len = 0; bytes[offset + len] != 0; len++) ;
                sensorNames[i] = Encoding.ASCII.GetString(bytes, offset, len);
                offset += MAX_SENSOR_NAME;

            }

            return err;

        }

        // ===================== salSyncSweepGetSensorHandles( ) ==================================
        [DllImport(AGSAL_DLL_NAME_Win32, EntryPoint = "salSyncSweepGetSensorHandles", CharSet = CharSet.Ansi)]
        private static extern SalError salSyncSweepGetSensorHandles_Win32(IntPtr measHandle, IntPtr[] handles, uint bufferSize);

        [DllImport(AGSAL_DLL_NAME_X64, EntryPoint = "salSyncSweepGetSensorHandles", CharSet = CharSet.Ansi)]
        private static extern SalError salSyncSweepGetSensorHandles_X64(IntPtr measHandle, IntPtr[] handles, uint bufferSize);

        public static SalError salSyncSweepGetSensorHandles(IntPtr measHandle, IntPtr[] handles)
        {
            uint numBytes = (uint)(handles.Length * IntPtr.Size);
            for (UInt32 i = 0; i < handles.Length; i++)
            {
                handles[i] = (IntPtr)(i + 100);

            }

            if (IntPtr.Size == 8)
                return salSyncSweepGetSensorHandles_X64(measHandle, handles, numBytes);
            else
                return salSyncSweepGetSensorHandles_Win32(measHandle, handles, numBytes);
        }

        //////// END agSalSyncSweep.h ////////

        //////// BEGIN pretzel.h ////////

        public enum GpsMode
        {
            Static,
            Mobile
        };

        //////// END pretzel.h ////////

        //////// BEGIN Tentcl.h ////////

        public enum MeasurementMode
        {
            None = 0,      // No Measurement
            SpectrumMonitoring = 1,      // Spectrum Monitoring
            WaveformMonitoring = 2,      // Waveform Monitoring
            TdoaWaveformMonitoring = 3,      // TDOA Waveform
            Error = 65536   // Error Mode*/      
        };

        //////// END Tentcl.h ////////

        public enum StatusBits
        {
            StatusNotRead = 1,
            CommunicationDown = 2,
            PowerQuestionable = 4,
            FrequencyQuestionable = 8,
            TemperatureQuestionable = 16,
            CalibrationQuestionable = 32
        };

        public const string SMS_ENV_VARIABLE_NAME = "KEYSIGHTSMS";

        private const int AgSalSensorStatusSize = 2048;

        public const int MAX_SAMPLES_PER_TRANSFER_TCP = 32768;
        public const int MAX_SAMPLES_PER_TRANSFER_UDP = 4096;

        public const int salRECOMMENDED_DATA_BUFFER_SIZE = 32768 * 2 * 4;

        public enum SensorAttributeType
        {
            SensorAttribute_double,
            SensorAttribute_float,
            SensorAttribute_int32,
            SensorAttribute_uint32,
            SensorAttribute_uint64,
            SensorAttribute_String
        };

        // Caution: must be same size as SensorAttribute
        static public SensorAttributeType[] SensorAttributeTypes = new SensorAttributeType[] // true if Double, false if Int32
        {
            SensorAttributeType.SensorAttribute_double, // COMPLEX_SAMPLE_RATE_MAX,     /**< returns salFloat64 (Hertz) */

            SensorAttributeType.SensorAttribute_int32, // DECIMATION_MAX,              /**< returns salInt32 */
            SensorAttributeType.SensorAttribute_int32, // DECIMATION_TYPE,             /**< returns salDecimation enum */

            SensorAttributeType.SensorAttribute_double, // FREQ_SPAN_FULL,             /**< returns valid analog freq span at full span salFloat64 (Hertz) */
            SensorAttributeType.SensorAttribute_double, // FREQ_SPAN_DECIMATING,       /**< returns (sample rate)/(valid freq span) when decimating */

            SensorAttributeType.SensorAttribute_double, // MEASURABLE_FREQ_MIN,        /**< returns salFloat64 Hertz */
            SensorAttributeType.SensorAttribute_double, // MEASURABLE_FREQ_MAX,        /**< returns salFloat64 Hertz */

            SensorAttributeType.SensorAttribute_double, // CENTER_FREQ_MIN,            /**< returns salFloat64 Hertz */
            SensorAttributeType.SensorAttribute_double, // CENTER_FREQ_MAX,            /**< returns salFloat64 Hertz */
            SensorAttributeType.SensorAttribute_double, // CENTER_FREQ_RESOLUTION,     /**< returns salFloat64 Hertz */

            SensorAttributeType.SensorAttribute_int32, // RESAMPLER_CAPABILITY,       /**< returns salInt32; value is 1 if resampling is supported, 0 if not */

            SensorAttributeType.SensorAttribute_double, // ATTENUATION_MIN,            /**< returns salFloat64 */
            SensorAttributeType.SensorAttribute_double, // ATTENUATION_MAX,            /**< returns salFloat64 */
            SensorAttributeType.SensorAttribute_double, // ATTENUATION_STEP,           /**< returns salFloat64 */

            SensorAttributeType.SensorAttribute_int32, // PREAMPLIFIER_CAPABILITY,    /**< returns salInt32 */

            SensorAttributeType.SensorAttribute_int32, // IQ_CHANNELS_MAX,           /**< returns salInt32 */

            SensorAttributeType.SensorAttribute_int32, // IQ_SAMPLES_MIN,            /**< returns salInt32 */
            SensorAttributeType.SensorAttribute_int32, // IQ_SAMPLES_MAX_16BIT,      /**< returns salInt32 */
            SensorAttributeType.SensorAttribute_int32, // IQ_SAMPLES_MAX_32BIT,      /**< returns salInt32 */

            SensorAttributeType.SensorAttribute_int32, // SAMPLES_PER_XFER_MAX_TCP, /**< returns salInt32 */
            SensorAttributeType.SensorAttribute_int32, // SAMPLES_PER_XFER_MAX_UDP, /**< returns salInt32 */

            SensorAttributeType.SensorAttribute_String, // MODEL_NUMBER,               /**< returns (char *) */
            SensorAttributeType.SensorAttribute_String, // SERIAL_NUMBER,              /**< returns (char *) */

            SensorAttributeType.SensorAttribute_String, // SENSOR_NAME,               /**< returns character array terminated by NULL; value is the SMS name of the sensor */
            SensorAttributeType.SensorAttribute_String, // SENSOR_HOSTNAME,            /**< returns character array terminated by NULL; value is the hostname of the sensor */
            SensorAttributeType.SensorAttribute_String, // DATE,                     /** < returns character array terminated by NULL; value is sensor time and date (e.g. Wed Jul 15 16:14:20 UTC 2009) */
            SensorAttributeType.SensorAttribute_int32, // FFT_POINTS_MIN,           /**< returns salInt32; returns the minimum size of FFT that can be requested from the sensor */
            SensorAttributeType.SensorAttribute_int32, // FFT_POINTS_MAX,           /**< returns salInt32; returns the maximum size of FFT that can be requested from the sensor */
            SensorAttributeType.SensorAttribute_int32, // ATTRIBUTE_DMA_HW,      /**< returns salInt32; if non-zero, this sensor has DMA hardware, which allows higher data transfer rates */

            SensorAttributeType.SensorAttribute_int32, // ATTRIBUTE_LO_ADJ_MODE,         /** [in|out] sets/gets current sensor LO adjustment mode */
            SensorAttributeType.SensorAttribute_int32, // ATTRIBUTE_TIME_SYNC_MODE,      /** [out]  returns salInt32; value is one of the ::salTimeSync enumerated values indicating the current time sysc source */
            SensorAttributeType.SensorAttribute_uint64, // ATTRIBUTE_TUNER_FIFO_BYTES,    /** [out] get the size in bytes of the sensor's high speed FIFO (uint64) */
            SensorAttributeType.SensorAttribute_uint64, // ATTRIBUTE_DMA_BUFFER_BYTES,    /** [out] get the size in bytes of the sensor's DMA buffer (uint64) */
            SensorAttributeType.SensorAttribute_int32, // ATTRIBUTE_IEEE1588_STATE,       /**< [out] gets current IEEE1588 state (int32, see salIeee1588State) */
            SensorAttributeType.SensorAttribute_uint32, // ATTRIBUTE_IEEE1588_DOMAIN,     /**< [in|out] sets/gets current IEEE1588 domain (uint32) */
            SensorAttributeType.SensorAttribute_uint32, // ATTRIBUTE_IEEE1588_PRIORITY1,  /**< [in|out] sets/gets current IEEE1588 priority1 (uint32) */
            SensorAttributeType.SensorAttribute_uint32, // ATTRIBUTE_IEEE1588_PRIORITY2,  /**< [in|out] sets/gets current IEEE1588 priority2 (uint32) */

            SensorAttributeType.SensorAttribute_double, // ATTRIBUTE_SENSOR_VARIANCE,     /**< [out] gets current "overall" sensor time-sync variance (double, sec^2) */
            SensorAttributeType.SensorAttribute_double, // ATTRIBUTE_SENSOR_OFFSET,       /**< [out] gets current "overall" sensor time-sync "offset from master" (double, sec) */
            SensorAttributeType.SensorAttribute_double, // ATTRIBUTE_IEEE1588_VARIANCE,   /**< [out] gets current sensor IEEE1588 variance (double, sec^2) */
            SensorAttributeType.SensorAttribute_double, // ATTRIBUTE_IEEE1588_OFFSET,     /**< [out] gets current sensor IEEE1588 "offset from master" (double, sec) */
            SensorAttributeType.SensorAttribute_double, // ATTRIBUTE_GPS_VARIANCE,        /**< [out] gets current sensor GPS variance (double, sec^2) */
            SensorAttributeType.SensorAttribute_double, // ATTRIBUTE_GPS_OFFSET,          /**< [out] gets current sensor GPS "offset from GPS module" (double, sec) */
            SensorAttributeType.SensorAttribute_double, // ATTRIBUTE_FPGA_VARIANCE,       /**< [out] gets current sensor FPGA variance (double, sec^2) */
            SensorAttributeType.SensorAttribute_double, // ATTRIBUTE_FPGA_OFFSET,         /**< [out] gets current sensor FPGA "offset from PHY or GPS module" (double, sec) */

            SensorAttributeType.SensorAttribute_float, // ATTRIBUTE_CPU_1MIN,            /**< [out] gets 1 minute CPU load (float) */
            SensorAttributeType.SensorAttribute_float, // ATTRIBUTE_CPU_5MIN,            /**< [out] gets 5 minute CPU load (float) */
            SensorAttributeType.SensorAttribute_float, // ATTRIBUTE_CPU_10MIN,           /**< [out] gets 10 minute CPU load (float) */

            SensorAttributeType.SensorAttribute_double, // ATTRIBUTE_VARIANCE_ALARM_THRESHOLD,  /**< [in|out] sets/gets current variance alarm threshold (double, sec^2) */
            SensorAttributeType.SensorAttribute_double, // ATTRIBUTE_OFFSET_ALARM_THRESHOLD,    /**< [in|out] sets/gets current time offset alarm threshold (double, sec) */
            SensorAttributeType.SensorAttribute_float,  // ATTRIBUTE_CPU_ALARM_THRESHOLD,       /**< [in|out] sets/gets CPU alarm threshold (float, 0 to 1) */

            SensorAttributeType.SensorAttribute_double, // ATTRIBUTE_RF_TEMPERATURE,			/**< [out] RF Board temperature (double, deg C) */
            SensorAttributeType.SensorAttribute_double, // ATTRIBUTE_DIG_TEMPERATURE,			/**< [out] Digital Board temperature (double, deg C) */

            SensorAttributeType.SensorAttribute_float, // ATTRIBUTE_UP_TIME,			    /**< [out] Up-time (float, sec) */
            SensorAttributeType.SensorAttribute_float, // ATTRIBUTE_IDLE_TIME,			    /**< [out] Idle-time (float, sec) */

            SensorAttributeType.SensorAttribute_uint32, // ATTRIBUTE_TIME_ALARMS,			/**< [out] Time alarm bitfield (unit32, see salTimeAlarms) */
            SensorAttributeType.SensorAttribute_uint32, // ATTRIBUTE_SA_ALARMS,				/**< [out] Spectrum Analyzer alarm bitfield (unit32, see salSpectrumAnalyzerAlarms) */
            SensorAttributeType.SensorAttribute_uint32, // ATTRIBUTE_SYS_ALARMS,			/**< [out] System alarm bitfield (unit32, see salSystemAlarms) */
            SensorAttributeType.SensorAttribute_uint32, // ATTRIBUTE_INTEG_ALARMS,			/**< [out] Integrity alarm bitfield (unit32, see salIntegrityAlarms) */

            SensorAttributeType.SensorAttribute_uint64, // ATTRIBUTE_RAM_TOTAL_KIB,			/**< [out] get the total RAM installed on the sensor (salUInt64, KiB (1024 bytes)) */
            SensorAttributeType.SensorAttribute_uint64, // ATTRIBUTE_RAM_FREE_KIB,			/**< [out] get the free RAM on the sensor (salUInt64, KiB (1024 bytes)) */
            SensorAttributeType.SensorAttribute_uint64, // ATTRIBUTE_JFFS2_FLASH_TOTAL_KIB,	/**< [out] get the total flash space mounted on the /jffs2 (salUInt64, KiB (1024 bytes)) */
            SensorAttributeType.SensorAttribute_uint64, // ATTRIBUTE_JFFS2_FLASH_FREE_KIB,	/**< [out] get the free flash space mounted on the /jffs2 (salUInt64, KiB (1024 bytes)) */
            SensorAttributeType.SensorAttribute_uint64, // ATTRIBUTE_DATA_FLASH_TOTAL_KIB,	/**< [out] get the total storage space on the USB drive. 0 if the USB drive is not present. (salUInt64, KiB (1024 bytes)) */
            SensorAttributeType.SensorAttribute_uint64, // ATTRIBUTE_DATA_FLASH_FREE_KIB,	/**< [out] get the free storage space on the USB drive. 0 if the USB drive is not present. (salUInt64, KiB (1024 bytes)) */


             SensorAttributeType.SensorAttribute_int32, // ATTRIBUTE_GPS_OPERATING_STATE,   /**< [out] gets the GPS opearting state. See ::salGPSOperatingState enumerated values */
             SensorAttributeType.SensorAttribute_int32  // ATTRIBUTE_GPS_TIMING_MODE,       /**< [out] gets the GPS timing mode. See ::salGPSTimingMode enumerated values */
        
        };

        public static SalError salIqGetState(IntPtr salDataStreamHandle, ref IqState state)
        {
            int val = 0;

            SalError err = salIqGetAttribute(salDataStreamHandle, IqAttribute.STATE, ref val);

            if (err != SalError.SAL_ERR_NONE) return err;

            state = (IqState)val;

            return SalError.SAL_ERR_NONE;

        }

        /// <summary>
        /// Takes an array of bytes and converts it to a string. The array of bytes is assumed to be 8 bit
        /// ASCII terminated by a null character.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static string BytesToString(Byte[] data, int offset)
        {
            int len;
            for (len = 0; data[offset + len] != 0; len++) ;
            return Encoding.ASCII.GetString(data, offset, len);
        }

        public enum AmplitudeType
        {
            salAmplitudeType_float32_dBm /**< data is returned as 32 bit floats in units of dBm (assuming 50 ohm load) */
        }

        // ===================== salRetrieveFftData( ) =================================
        // Helper function to retrieve the FFT data from SAL to C#
        // This is the simplest retrieve function as memory is automatically allocated
        public static SalError salRetrieveFftData(ref AgSalLib.SegmentData dataHeader, IntPtr data, out float[] bins)
        {
            if (dataHeader.numPoints > 0)
            {
                bins = new float[dataHeader.numPoints];

                return salRetrieveFftDataReuseMemory(ref dataHeader, data, ref bins);
            }
            else
            {
                bins = new float[0]; // Nothing to return
                return SalError.SAL_ERR_NO_DATA_AVAILABLE;
            }
        }

        // Helper function to retrieve the IQ data from SAL to C#
        // This retrieve function is more efficient as memory can be pre-allocated and reused
        public static SalError salRetrieveFftDataReuseMemory(ref AgSalLib.SegmentData dataHeader, IntPtr data, ref float[] bins)
        {
            if (bins.Length < dataHeader.numPoints)
            {
                return SalError.SAL_ERR_BUFFER_TOO_SMALL;
            }

            if (dataHeader.numPoints > 0)
            {
                System.Runtime.InteropServices.Marshal.Copy(data, bins, 0, (int)dataHeader.numPoints);
                return SalError.SAL_ERR_NONE;
            }
            else
            {
                bins = new float[0]; // Nothing to return
                return SalError.SAL_ERR_NO_DATA_AVAILABLE;
            }
        }

        public enum CaptureStatus
        {
            OFF = 0,      /**< The tuner is not capturing data */
            ON = 1,      /**< The tuner is  capturing data */
        }

        // ===================== salRetrieveIqData( ) =================================
        // Helper function to retrieve the IQ data from SAL to C#
        // This is the simplest retrieve function as memory is automatically allocated
        public static SalError salRetrieveIqData(ref AgSalLib.TimeData dataHeader, IntPtr data, out float[] re, out float[] im)
        {
            if (dataHeader.numSamples > 0)
            {
                int[] dataArray = new int[dataHeader.numSamples * 2];

                re = new float[dataHeader.numSamples];
                im = new float[dataHeader.numSamples];

                return salRetrieveIqDataReuseMemory(ref dataHeader, data, ref dataArray, ref re, ref im);
            }
            else
            {
                re = new float[0]; // Nothing to return
                im = new float[0];
                return SalError.SAL_ERR_NO_DATA_AVAILABLE;
            }
        }

        // Helper function to retrieve the IQ data from SAL to C#
        // This retrieve function is more efficient as memory can be pre-allocated and reused
        public static SalError salRetrieveIqDataReuseMemory(ref AgSalLib.TimeData dataHeader, IntPtr data, ref int[] dataArray, ref float[] re, ref float[] im)
        {
            if (dataArray.Length < dataHeader.numSamples * 2 ||
                re.Length < dataHeader.numSamples ||
                im.Length < dataHeader.numSamples)
            {
                return SalError.SAL_ERR_BUFFER_TOO_SMALL;
            }

            if (dataHeader.numSamples > 0)
            {
                System.Runtime.InteropServices.Marshal.Copy(data, dataArray, 0, (int)(dataHeader.numSamples * 2));

                for (int i = 0; i < dataHeader.numSamples; i++)
                {
                    re[i] = (float)(dataHeader.scaleToVolts * dataArray[i * 2]);
                    im[i] = (float)(dataHeader.scaleToVolts * dataArray[i * 2 + 1]);
                }
                return SalError.SAL_ERR_NONE;
            }
            else
            {
                return SalError.SAL_ERR_NO_DATA_AVAILABLE;
            }
        }

        public struct TimeDuration
        {
            public Int16 sign;                          /**< +1 == positive -1 == negative */
            public UInt32 timestampSeconds;            /**< Integer part of the timestamp (in UTC seconds since January 1, 1970). */
            public UInt32 timestampNSeconds;           /**< Fractional part of the timestamp (in Nanoseconds). */

            public TimeDuration(double val)
            {
                if (val >= 0)
                {
                    sign = 1;
                }
                else
                {
                    sign = -1;
                    val = Math.Abs(val);
                }
                timestampSeconds = (UInt32)Math.Floor(val);
                timestampNSeconds = (UInt32)((val - timestampSeconds) * 1e9);
            }

            public TimeDuration(UInt32 sec, UInt32 nsec)
            {
                sign = 1;
                timestampSeconds = sec;
                timestampNSeconds = nsec;
                while (timestampNSeconds > 1000000000)
                {
                    timestampNSeconds -= 1000000000;
                    timestampSeconds++;
                }
            }

            public TimeDuration(Int16 _sign, UInt32 sec, UInt32 nsec)
            {
                sign = _sign;
                timestampSeconds = sec;
                timestampNSeconds = nsec;
                while (timestampNSeconds > 1000000000)
                {
                    timestampNSeconds -= 1000000000;
                    timestampSeconds++;
                }
            }

            public void set(Int16 _sign, UInt32 sec, UInt32 nsec)
            {
                sign = _sign;
                timestampSeconds = sec;
                timestampNSeconds = nsec;
                while (timestampNSeconds > 1000000000)
                {
                    timestampNSeconds -= 1000000000;
                    timestampSeconds++;
                }
            }

            override public String ToString()
            {
                double nsec = timestampNSeconds / 1e9;
                String rtn = sign == -1 ? "-" : "" +
                    timestampSeconds + String.Format("{0:#.000000000}", nsec);
                return rtn;
            }

            public double ToDouble()
            {
                double rtn = timestampSeconds + timestampNSeconds / 1e9;
                if (sign == -1) rtn *= sign;
                return rtn;
            }

        };
    }
}
