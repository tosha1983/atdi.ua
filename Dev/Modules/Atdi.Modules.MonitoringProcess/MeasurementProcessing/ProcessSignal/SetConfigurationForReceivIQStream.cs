//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Atdi.Modules.MonitoringProcess.SingleHound;

//namespace Atdi.Modules.MonitoringProcess.ProcessSignal
//{
//    class SetConfigurationForReceivIQStream
//    {
//        #region peremeters
//        public int return_len;
//        public int samples_per_sec;
//        public double bandwidth;
//        #endregion 
//        public SetConfigurationForReceivIQStream(ref SDRBB60C SDR, Double freqMHz, Double spankHz)
//        {
//            //// пока параметры будут константами временное решение для тестирования
//            //int downsampleFactor = 1; //Коэфициент прореживания IQ потока
//            //Double f = freqMHz * 1000000;
//            //Double span = spankHz * 1000; 
//            //bb_api.bbConfigureLevel(SDR.id_dev, -40.0, bb_api.BB_AUTO_ATTEN);
//            //bb_api.bbConfigureGain(SDR.id_dev, bb_api.BB_AUTO_GAIN);
//            //bb_api.bbConfigureCenterSpan(SDR.id_dev, f, span);
//            //bb_api.bbConfigureIQ(SDR.id_dev, downsampleFactor, span);
//            //SDR.status = bb_api.bbInitiate(SDR.id_dev, bb_api.BB_STREAMING, bb_api.BB_STREAM_IQ);
//            //if (SDR.status != bbStatus.bbNoError) { return; } //Выход с ошибкой 
//            //int _return_len = 0; int _samples_per_sec = 0; double _bandwidth = 0.0;
//            //bb_api.bbQueryStreamInfo(SDR.id_dev, ref _return_len, ref _bandwidth, ref _samples_per_sec);
//            //return_len = _return_len;
//            //samples_per_sec =_samples_per_sec;
//            //bandwidth = _bandwidth;
//        }
//    }
//}
