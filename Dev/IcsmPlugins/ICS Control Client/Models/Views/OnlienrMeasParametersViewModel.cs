using Atdi.DataModels.Sdrns.Device.OnlineMeasurement;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using XICSM.ICSControlClient.Environment.Wpf;
using XICSM.ICSControlClient.Handlers.OnlineMeasurement.Calculation;
using XICSM.ICSControlClient.ViewModels;

namespace XICSM.ICSControlClient.Models.Views
{
    public class OnlieneMeasBandwidthResult : WpfViewModelBase
    {
        private double? _t1_MHz;
        private double? _t2_MHz;
        private double? _marker_MHz;
        private double? _BW_kHz;
        private string _correctnessEstimations;

        public double? T1_MHz
        {
            get => this._t1_MHz;
            set => this.Set(ref this._t1_MHz, value, () => { });
        }

        public double? T2_MHz
        {
            get => this._t2_MHz;
            set => this.Set(ref this._t2_MHz, value, () => { });
        }

        public double? Marker_MHz
        {
            get => this._marker_MHz;
            set => this.Set(ref this._marker_MHz, value, () => { });
        }

        public double? BW_kHz
        {
            get => this._BW_kHz;
            set => this.Set(ref this._BW_kHz, value, () => { });
        }

        public string CorrectnessEstimations
        {
            get => this._correctnessEstimations;
            set => this.Set(ref this._correctnessEstimations, value, () => { });
        }

        public void Apply(CalcBWForChart.ResultBWForChart result)
        {
            this.T1_MHz = result.T1_MHz;
            this.T2_MHz = result.T2_MHz;
            this.Marker_MHz = result.Marker_MHz;
            this.BW_kHz = result.BW_kHz;
            this.CorrectnessEstimations = result.CorrectnessEstimations ? "TRUE" : "FALSE";
        }
    }

    public class OnlienrMeasParametersViewModel : WpfViewModelBase, IDataErrorInfo
    {
        private string _connectButtonText;
        private string _runButtonText;
        private bool _isReadOnlyProperties;
        private bool _isEnabledConnectButton;
        private bool _isEnabledRunButton;
        private string _currentStatus;
        private double _freqStart_MHz;
        private double _freqStop_MHz;

        private TraceType _traceType;
        private int _traceCount;
        private double _RBW_kHz;
        private double _sweepTime_s;
        private DetectorType _detectorType;
        private int _refLevel_dBm;
        private int _preAmp_dB;
        private int _att_dB;
        private double _antennaFactor;

        private BandWidthEstimation.BandwidthEstimationType _bandwidthEstimationType;
        private double _xBeta; 
        private int _maximumIgnorPoint;

        public OnlienrMeasParametersViewModel(OnlineMeasurementViewModel parent)
        {
            this.Parent = parent;
            this._isReadOnlyProperties = true;
            this._isEnabledConnectButton = true;
            this._isEnabledRunButton = false;
        }

        public BandWidthEstimation.BandwidthEstimationType EstimationType
        {
            get => this._bandwidthEstimationType;
            set => this.Set(ref this._bandwidthEstimationType, value, () => { });
        }
        public IList<BandWidthEstimation.BandwidthEstimationType> EstimationTypeValues => Enum.GetValues(typeof(BandWidthEstimation.BandwidthEstimationType)).Cast <BandWidthEstimation.BandwidthEstimationType > ().ToList();

        public double X_Beta
        {
            get => this._xBeta;
            set => this.Set(ref this._xBeta, value, () => { });
        }

        public int MaximumIgnorPoint
        {
            get => this._maximumIgnorPoint;
            set => this.Set(ref this._maximumIgnorPoint, value, () => { });
        }

        public double[] Freq_Hz { get; set; }

        public double FreqStart_MHz
        {
            get => this._freqStart_MHz;
            set => this.Set(ref this._freqStart_MHz, value, () => { });
        }

        public double FreqStop_MHz
        {
            get => this._freqStop_MHz;
            set => this.Set(ref this._freqStop_MHz, value, () => { });
        }

        public TraceType TraceType
        {
            get => this._traceType;
            set => this.Set(ref this._traceType, value, () => 
            {
                if (value == TraceType.ClearWhrite)
                {
                    if (this.TraceCount != 1)
                    {
                        this.TraceCount = 1;
                    }
                }

            });
        }

        public IList<TraceType> TraceTypeValues => Enum.GetValues(typeof(TraceType)).Cast<TraceType>().ToList();

        public int TraceCount
        {
            get => this._traceCount;
            set => this.Set(ref this._traceCount, value, () => { });
        }

        public double RBW_kHz
        {
            get => this._RBW_kHz;
            set => this.Set(ref this._RBW_kHz, value, () => { });
        }

        public double SweepTime_s
        {
            get => this._sweepTime_s;
            set => this.Set(ref this._sweepTime_s, value, () => { });
        }

        public DetectorType DetectorType
        {
            get => this._detectorType;
            set => this.Set(ref this._detectorType, value, () => { });
        }

        public IList<DetectorType> DetectorTypeValues => Enum.GetValues(typeof(DetectorType)).Cast<DetectorType>().ToList();

        public OnlineMeasType OnlineMeasType { get; set; }

        public int RefLevel_dBm
        {
            get => this._refLevel_dBm;
            set => this.Set(ref this._refLevel_dBm, value, () => { });
        }

        public int PreAmp_dB
        {
            get => this._preAmp_dB;
            set => this.Set(ref this._preAmp_dB, value, () => { });
        }

        public int Att_dB
        {
            get => this._att_dB;
            set => this.Set(ref this._att_dB, value, () => { });
        }

        

        public double AntennaFactor
        {
            get => this._antennaFactor;
            set => this.Set(ref this._antennaFactor, value, () => { });
        }

        public string CurrentStatus
        {
            get => this._currentStatus;
            set => this.Set(ref this._currentStatus, value, () => { });
        }

        public string ConnectButtonText
        {
            get => this._connectButtonText;
            set => this.Set(ref this._connectButtonText, value, () => { });
        }

        public string RunButtonText
        {
            get => this._runButtonText;
            set => this.Set(ref this._runButtonText, value, () => { });
        }

        public OnlineMeasurementViewModel Parent { get; }

        public bool IsReadOnlyProperties
        {
            get => this._isReadOnlyProperties;
            set => this.Set(ref this._isReadOnlyProperties, value, () => { });
        }

        public bool IsEnabledConnectButton
        {
            get => this._isEnabledConnectButton;
            set => this.Set(ref this._isEnabledConnectButton, value, () => { });
        }

        public bool IsEnabledRunButton
        {
            get => this._isEnabledRunButton;
            set => this.Set(ref this._isEnabledRunButton, value, () => { });
        }

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                var error = string.Empty;
                switch (columnName)
                {
                    case "FreqStart_MHz":
                        if ((FreqStart_MHz <= 0) )
                        {
                            error = "The value must be greater than zero";
                        }
                        if (FreqStart_MHz >= FreqStop_MHz)
                        {
                            error = "The start frequency should be less than the stop frequency";
                        }
                        break;
                    case "FreqStop_MHz":
                        if ((FreqStop_MHz <= 0))
                        {
                            error = "The value must be greater than zero";
                        }
                        if (FreqStart_MHz >= FreqStop_MHz)
                        {
                            error = "The start frequency should be less than the stop frequency";
                        }
                        break;
                    case "TraceCount":
                        if (TraceCount <= 0)
                        {
                            error = "The value must be greater than zero";
                        }
                        if (TraceType == TraceType.ClearWhrite && TraceCount != 1)
                        {
                            error = "When the TraceType field is ClearWhrite, then the value of TraceCount must be equal to 1";
                        }
                        break;
                    case "RBW_kHz":
                        if ((RBW_kHz <= 0))
                        {
                            error = "The value must be greater than zero";
                        }
                        var freqRange = (1000.0 * (this.FreqStop_MHz - this.FreqStart_MHz));
                        if (RBW_kHz > freqRange)
                        {
                            error = $"The value must be less than the frequency range '{freqRange} kHz' (FreqStop - FreqStart)";
                        }
                        break;
                    case "SweepTime_s":
                        if ((SweepTime_s <= 0 || SweepTime_s > 10) && SweepTime_s != -1)
                        {
                            error = "The value must be greater than zero and less than or equal to '10 sec'. Or be '-1' in cases of 'Auto'";
                        }
                        break;
                    case "RefLevel_dBm":
                        if ((RefLevel_dBm < -140 || RefLevel_dBm > 30) && RefLevel_dBm != 1000000000)
                        {
                            error = "The value must be greater or equal to '-140 dBm' and less than or equal to '30 dBm'. Or be '1000000000' in cases of 'Auto'";
                        }
                        break;
                    case "Att_dB":
                        if ((Att_dB < 0 || Att_dB > 50) && Att_dB != -1)
                        {
                            error = "The value must be greater or equal to '0 dB' and less than or equal to '50 dB'. Or be '1' in cases of 'Auto'";
                        }
                        break;
                    case "PreAmp_dB":
                        if ((PreAmp_dB < 0 || PreAmp_dB > 50) && PreAmp_dB != -1)
                        {
                            error = "The value must be greater or equal to '0 dB' and less than or equal to '50 dB'. Or be '1' in cases of 'Auto'";
                        }
                        break;
                    case "X_Beta":
                        if ((X_Beta < 0.01 || X_Beta > 50) && EstimationType == BandWidthEstimation.BandwidthEstimationType.beta)
                        {
                            error = "The value must be greater or equal to '0.01%' and less than or equal to '505%'";
                        }
                        else if ((X_Beta < 10 || X_Beta > 35) && EstimationType != BandWidthEstimation.BandwidthEstimationType.beta)
                        {
                            error = "The value must be greater or equal to '10%' and less than or equal to '35%'";
                        }
                        break;
                    case "MaximumIgnorPoint":
                        if (MaximumIgnorPoint < 0 || MaximumIgnorPoint > 5)
                        {
                            error = "The value must be greater or equal to '0' and less than or equal to '5'";
                        }
                        break;
                }
                return error;
            }
        }

        public void ValidateStateModel()
        {
            var errors = new StringBuilder();

            var properties = new string[]
            {
                "FreqStart_MHz",
                "FreqStop_MHz",
                "TraceCount",
                "RBW_kHz",
                "SweepTime_s",
                "RefLevel_dBm",
                "Att_dB",
                "PreAmp_dB",
                "X_Beta",
                "MaximumIgnorPoint"
            };

            if (this.TraceType == TraceType.ClearWhrite)
            {
                if (this.TraceCount != 1)
                {
                    this.TraceCount = 1;
                }
            }

            for (int i = 0; i < properties.Length; i++)
            {
                var propertyName = properties[i];
                var error = this[propertyName];
                if (!string.IsNullOrEmpty(error))
                {
                    errors.AppendLine($" - invalid value of the {propertyName}: {error}");
                }
            }
            
            if (errors.Length > 0)
            {
                throw new InvalidOperationException("Invalid input task properties: \n" + errors.ToString());
            }
        }
    }
}
