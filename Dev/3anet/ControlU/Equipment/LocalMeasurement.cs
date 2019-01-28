using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlU.Equipment
{
    public class LocalMeasurement
    {

        public int[] GetMeasNDB(tracepoint[] tr, int freqMeasPeak, double NdBLevel, decimal freqCentrMeas, decimal MeasBWMax, decimal MeasBWMin)
        {
            #region
            int[] outarr = new int[3] { freqMeasPeak, -1, -1 };
            try
            {
                if (freqMeasPeak > -1 && freqMeasPeak < tr.Length)
                {
                    int id = FindMarkerIndOnTrace(tr, (decimal)(freqCentrMeas - MeasBWMin / 2)), 
                        iu = FindMarkerIndOnTrace(tr, (decimal)(freqCentrMeas + MeasBWMin / 2)),
                        outindd = FindMarkerIndOnTrace(tr, freqCentrMeas - MeasBWMax / 2),
                        outindu = FindMarkerIndOnTrace(tr, freqCentrMeas + MeasBWMax / 2);
                    int outD = freqMeasPeak, outU = freqMeasPeak;
                    double idn = double.MaxValue, iun = double.MaxValue;
                    int outD2 = freqMeasPeak, outU2 = freqMeasPeak;
                    double idn2 = double.MaxValue, iun2 = double.MaxValue;
                    double levelM = tr[freqMeasPeak].level - NdBLevel; // искомый уровень маркера

                    for (int i = 0; i < tr.Length - 1; i++)
                    {
                        if (id > outindd) { id -= 1; }
                        if (iu < outindu) { iu += 1; }
                        #region
                        if (tr[id].level <= levelM && tr[id + 1].level >= levelM)
                        {
                            if (tr[id].level >= levelM) outD = id;
                            if (tr[id + 1].level >= levelM) outD = id + 1;
                            idn = tr[outD].level - levelM;
                        }
                        if (tr[iu].level <= levelM && tr[iu - 1].level >= levelM)
                        {
                            if (tr[iu].level >= levelM) outU = iu;
                            if (tr[iu - 1].level >= levelM) outU = iu - 1;
                            iun = tr[outU].level - levelM;
                        }
                        #endregion

                        #region
                        if (tr[id].level - levelM < idn2 && tr[id].level - levelM > 0)
                        {
                            idn2 = tr[id].level - levelM;
                            outD2 = id;
                        }
                        if (tr[iu].level - levelM < iun2 && tr[iu].level - levelM > 0)
                        {
                            iun2 = tr[iu].level - levelM;
                            outU2 = iu;
                        }
                        #endregion
                    }
                    if (outD != freqMeasPeak) { outarr[1] = outD; }
                    else { outarr[1] = outD2; }
                    if (outU != freqMeasPeak) { outarr[2] = outU; }
                    else { outarr[2] = outU2; }

                }
            }
            #region Exception
            catch (Exception exp)
            {
                App.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "LocalMeasurement", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            #endregion
            return outarr;
            #endregion
        }
        public int[] GetMeasNDB(tracepoint[] tr, int freqMeasPeak, double NdBLevel)
        {
            #region
            int[] outarr = new int[3] { freqMeasPeak, -1, -1 };
            try
            {
                if (freqMeasPeak > -1 && freqMeasPeak < tr.Length)
                {
                    int id = freqMeasPeak, iu = freqMeasPeak;
                    int outindd = 0;
                    int outindu = tr.Length - 1;
                    int outD = freqMeasPeak, outU = freqMeasPeak;
                    double idn = double.MaxValue, iun = double.MaxValue;
                    int outD2 = freqMeasPeak, outU2 = freqMeasPeak;
                    double idn2 = double.MaxValue, iun2 = double.MaxValue;
                    double levelM = tr[freqMeasPeak].level - NdBLevel; // искомый уровень маркера

                    for (int i = 0; i < tr.Length - 1; i++)
                    {
                        if (id > outindd) { id -= 1; }
                        if (iu < outindu) { iu += 1; }
                        #region
                        if (tr[id].level <= levelM && tr[id + 1].level >= levelM)
                        {
                            if (tr[id].level >= levelM) outD = id;
                            if (tr[id + 1].level >= levelM) outD = id + 1;
                            idn = tr[outD].level - levelM;
                        }
                        if (tr[iu].level <= levelM && tr[iu - 1].level >= levelM)
                        {
                            if (tr[iu].level >= levelM) outU = iu;
                            if (tr[iu - 1].level >= levelM) outU = iu - 1;
                            iun = tr[outU].level - levelM;
                        }
                        #endregion

                        #region
                        if (tr[id].level - levelM < idn2 && tr[id].level - levelM > 0)
                        {
                            idn2 = tr[id].level - levelM;
                            outD2 = id;
                        }
                        if (tr[iu].level - levelM < iun2 && tr[iu].level - levelM > 0)
                        {
                            iun2 = tr[iu].level - levelM;
                            outU2 = iu;
                        }
                        #endregion
                    }
                    if (outD != freqMeasPeak) { outarr[1] = outD; }
                    else { outarr[1] = outD2; }
                    if (outU != freqMeasPeak) { outarr[2] = outU; }
                    else { outarr[2] = outU2; }

                }
            }
            #region Exception
            catch (Exception exp)
            {
                App.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "LocalMeasurement", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
                }));
            }
            #endregion
            return outarr;
            #endregion
        }

        #region old ndb
        ///// <summary>
        ///// NDb но с поиском минимум характерно для CDMA
        ///// </summary>
        ///// <param name="tr"></param>
        ///// <param name="freqMeasPeak"></param>
        ///// <param name="NdBLevel"></param>
        ///// <param name="freqCentrMeas"></param>
        ///// <param name="MeasBWMax"></param>
        ///// <param name="MeasBWMin"></param>
        ///// <returns></returns>
        //public int[] GetMeasNDBMinLevel(TracePoint[] tr, int freqMeasPeak, decimal NdBLevel, decimal freqCentrMeas, decimal MeasBWMax, decimal MeasBWMin)
        //{
        //    #region
        //    int[] outarr = new int[2] { -1, -1 };
        //    try
        //    {
        //        if (freqMeasPeak > -1)
        //        {
        //            if (freqMeasPeak < 0) { freqMeasPeak = 0; }
        //            if (freqMeasPeak > tr.Length - 1) { freqMeasPeak = tr.Length - 1; }
        //            //int id = freqMeasPeak, iu = freqMeasPeak, pd = freqMeasPeak, pu = freqMeasPeak;
        //            int id = FindMarkerIndOnTrace(tr, (decimal)(freqCentrMeas - MeasBWMin / 2)), iu = FindMarkerIndOnTrace(tr, (decimal)(freqCentrMeas + MeasBWMin / 2)), pd = freqMeasPeak, pu = freqMeasPeak;
        //            decimal levelM = tr[freqMeasPeak].Level - NdBLevel;
        //            decimal idn = decimal.MaxValue, iun = decimal.MaxValue;
        //            int outindu = -1, outindd = -1;
        //            outindd = FindMarkerIndOnTrace(tr, freqCentrMeas - MeasBWMax / 2);
        //            outindu = FindMarkerIndOnTrace(tr, freqCentrMeas + MeasBWMax / 2);
        //            for (int i = 0; i < tr.Length - 1; i++)
        //            {
        //                if (id > 0/* && id < outindd*/) { id -= 1; }
        //                if (iu < tr.Length - 1 /*&& iu < outindu + 1*/) { iu += 1; }

        //                if (Math.Abs(tr[id].Level - levelM) < idn/* && id < pd * 1.1*/)
        //                {
        //                    idn = Math.Abs(tr[id].Level - levelM);
        //                    pd = id;
        //                }
        //                if (Math.Abs(tr[iu].Level - levelM) < iun/* && iu < pu * 1.1*/)
        //                {
        //                    iun = Math.Abs(tr[iu].Level - levelM);
        //                    pu = iu;
        //                }
        //                if (id >= outindd)
        //                {
        //                    outarr[0] = pd;
        //                }
        //                if (iu <= outindu)
        //                {
        //                    outarr[1] = pu;
        //                }
        //            }
        //        }
        //    }
        //    #region Exception
        //    catch (Exception exp)
        //    {
        //        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "LocalMeasurement", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
        //    }
        //    #endregion
        //    return outarr;
        //    #endregion
        //}
        //public int[] GetMeasNDB3(TracePoint[] tr, int freqMeasPeak, decimal NdBLevel)
        //{
        //    #region

        //    int[] outarr = new int[2] { -1, -1 };
        //    try
        //    {
        //        if (freqMeasPeak > -1 && freqMeasPeak < tr.Length)
        //        {
        //            int id = freqMeasPeak, iu = freqMeasPeak;
        //            int outindd = 0;
        //            int outindu = tr.Length - 1;
        //            int outD = freqMeasPeak, outU = freqMeasPeak;
        //            decimal levelM = tr[freqMeasPeak].Level - NdBLevel; // искомый уровень маркера

        //            for (int i = 0; i < tr.Length - 1; i++)
        //            {
        //                if (id > outindd && id > 0) { id -= 1; }
        //                if (iu < outindu && iu < tr.Length - 1) { iu += 1; }

        //                if (tr[id].Level <= levelM && tr[id + 1].Level >= levelM)
        //                {
        //                    if (tr[id].Level >= levelM) outD = id;
        //                    if (tr[id + 1].Level >= levelM) outD = id + 1;
        //                }
        //                if (tr[iu].Level <= levelM && tr[iu - 1].Level >= levelM)
        //                {
        //                    if (tr[iu].Level >= levelM) outU = iu;
        //                    if (tr[iu - 1].Level >= levelM) outU = iu - 1;
        //                }
        //            }
        //            outarr[0] = outD;
        //            outarr[1] = outU;
        //        }
        //    }
        //    #region Exception
        //    catch (Exception exp)
        //    {
        //        App.Current.Dispatcher.BeginInvoke((Action)(() =>
        //        {
        //            MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "LocalMeasurement", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
        //        }));
        //    }
        //    #endregion
        //    return outarr;
        //    #endregion
        //}
        //public int[] GetMeasNDB2(TracePoint[] tr, int freqMeasPeak, decimal NdBLevel)
        //{
        //    #region
        //    int[] outarr = new int[2] { -1, -1 };
        //    try
        //    {
        //        if (freqMeasPeak > -1 && freqMeasPeak < tr.Length)
        //        {
        //            int indD = freqMeasPeak, indU = freqMeasPeak;
        //            int outD = freqMeasPeak, outU = freqMeasPeak;
        //            decimal LD = decimal.MaxValue, LU = decimal.MaxValue;
        //            decimal levelM = tr[freqMeasPeak].Level - NdBLevel;//уровень который ищем NDB должен бытьвыше него
        //            for (int i = 0; i < tr.Length - 1; i++)
        //            {
        //                if (indD > 0) { indD -= 1; }
        //                if (indU < tr.Length - 1) { indU += 1; }
        //                if (tr[indD].Level > levelM && Math.Abs(tr[indD].Level - levelM) < LD) { LD = Math.Abs(tr[indD].Level - levelM); outD = indD; }
        //                if (tr[indU].Level > levelM) { LU = levelM - tr[indU].Level; outU = indU; }

        //                if (indD == 0) outarr[0] = outD;
        //                if (indU == tr.Length - 1) outarr[1] = outU;
        //            }
        //        }
        //    }
        //    #region Exception
        //    catch (Exception exp)
        //    {
        //        App.Current.Dispatcher.BeginInvoke((Action)(() =>
        //        {
        //            MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "LocalMeasurement", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
        //        }));
        //    }
        //    #endregion
        //    return outarr;
        //    #endregion
        //}
        //public int[] GetMeasNDB1(TracePoint[] tr, int freqMeasPeak, decimal NdBLevel)
        //{
        //    #region
        //    int[] outarr = new int[2] { -1, -1 };
        //    try
        //    {
        //        if (freqMeasPeak > -1 && freqMeasPeak < tr.Length)
        //        {
        //            int indD = freqMeasPeak, indU = freqMeasPeak;
        //            int outD = freqMeasPeak, outU = freqMeasPeak;
        //            decimal levelM = tr[freqMeasPeak].Level - NdBLevel;//уровень который ищем NDB должен бытьвыше него
        //            for (int i = 0; i < tr.Length - 1; i++)
        //            {
        //                if (indD > 0) { indD -= 1; }
        //                if (indU < tr.Length - 1) { indU += 1; }
        //                if (tr[indD].Level > levelM) outD = indD;
        //                if (tr[indU].Level > levelM) outU = indU;

        //                if (indD == 0) outarr[0] = outD;
        //                if (indU == tr.Length - 1) outarr[1] = outU;
        //            }
        //        }
        //    }
        //    #region Exception
        //    catch (Exception exp)
        //    {
        //        App.Current.Dispatcher.BeginInvoke((Action)(() =>
        //        {
        //            MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "LocalMeasurement", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
        //        }));
        //    }
        //    #endregion
        //    return outarr;
        //    #endregion
        //}
        //public int[] GetMeasNDB(TracePoint[] tr, int freqMeasPeak, decimal NdBLevel, decimal MeasBW)
        //{
        //    #region

        //    int[] outarr = new int[2] { -1, -1 };
        //    try
        //    {
        //        if (freqMeasPeak > -1 && freqMeasPeak < tr.Length)
        //        {
        //            int CountLineLeftDown = 0; // количевство пересечений лини где уровень на i меньше чем на i+1
        //            int CountLineLeftUp = 0; // количевство пересечений лини где уровень на i меньше чем на i+1
        //            //CountLineDown > CountLineUp на один то ищем ближайший индекс и пишем в LineLeft
        //            int indLineLeft = 0;

        //            int CountLineRightDown = 0; // количевство пересечений лини где уровень на i меньше чем на i+1
        //            int CountLineRightUp = 0; // количевство пересечений лини где уровень на i меньше чем на i+1
        //            //CountLineDown > CountLineUp на один то ищем ближайший индекс и пишем в LineLeft
        //            int indLineRight = 0;

        //            int id = freqMeasPeak, iu = freqMeasPeak;
        //            int outindd = FindMarkerIndOnTrace(tr, (decimal)(tr[freqMeasPeak].Freq - (decimal)(MeasBW / 2)));
        //            int outindu = FindMarkerIndOnTrace(tr, (decimal)(tr[freqMeasPeak].Freq + (decimal)(MeasBW / 2)));
        //            decimal levelM = 1000 + tr[freqMeasPeak].Level - NdBLevel; // искомый уровень маркера

        //            for (int i = 0; i < tr.Length - 1; i++)
        //            {
        //                if (id > outindd && id > 0) { id -= 1; }
        //                if (iu < outindu && iu < tr.Length - 1) { iu += 1; }
        //                if (1000 + tr[id].Level <= levelM && 1000 + tr[id + 1].Level >= levelM)
        //                {
        //                    CountLineLeftDown++;
        //                    if (CountLineLeftDown > CountLineLeftUp)
        //                    {
        //                        decimal ti1 = (decimal)Math.Abs((double)(1000 + tr[id].Level - levelM));
        //                        decimal ti2 = (decimal)Math.Abs((double)(1000 + tr[id + 1].Level - levelM));
        //                        if (ti1 < ti2)
        //                            indLineLeft = id;
        //                        else if (ti1 > ti2)
        //                            indLineLeft = id + 1;
        //                    }
        //                }
        //                if (1000 + tr[id].Level >= levelM && 1000 + tr[id + 1].Level <= levelM) { CountLineLeftUp++; }

        //            }
        //            outarr[0] = indLineLeft;
        //        }
        //    }
        //    #region Exception
        //    catch (Exception exp)
        //    {
        //        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "LocalMeasurement", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
        //    }
        //    #endregion
        //    return outarr;
        //    #endregion
        //}
        //public int[] GetMeasNDBold(TracePoint[] tr, int freqMeasPeak, decimal NdBLevel, decimal freqCentrMeas, decimal MeasBW)
        //{
        //    #region
        //    int[] outarr = new int[2] { -1, -1 };
        //    try
        //    {
        //        if (freqMeasPeak > -1)
        //        {
        //            if (freqMeasPeak < 0) { freqMeasPeak = 0; }
        //            if (freqMeasPeak > tr.Length - 1) { freqMeasPeak = tr.Length - 1; }
        //            //int id = freqMeasPeak, iu = freqMeasPeak, pd = freqMeasPeak, pu = freqMeasPeak;
        //            int id = FindMarkerIndOnTrace(tr, (decimal)(freqCentrMeas)), iu = FindMarkerIndOnTrace(tr, (decimal)(freqCentrMeas)), pd = freqMeasPeak, pu = freqMeasPeak;
        //            decimal levelM = 1000 + tr[freqMeasPeak].Level - NdBLevel;
        //            decimal idn = decimal.MaxValue, iun = decimal.MaxValue;
        //            int outindu = -1, outindd = -1;
        //            outindd = FindMarkerIndOnTrace(tr, (decimal)(freqCentrMeas - (decimal)(MeasBW / 2)));
        //            outindu = FindMarkerIndOnTrace(tr, (decimal)(freqCentrMeas + (decimal)(MeasBW / 2)));
        //            int frdn = id, frup = iu;
        //            for (int i = 0; i < tr.Length - 1; i++)
        //            {
        //                if (id > 0) { id -= 1; }
        //                if (iu < tr.Length - 1) { iu += 1; }

        //                if (1000 + tr[id].Level <= levelM && 1000 + tr[id + 1].Level >= levelM && tr[id].Freq < tr[frdn].Freq)
        //                {
        //                    decimal ti1 = (decimal)Math.Abs((double)(1000 + tr[id].Level - levelM));
        //                    decimal ti2 = (decimal)Math.Abs((double)(1000 + tr[id + 1].Level - levelM));
        //                    if (ti1 < ti2)
        //                        frdn = id;
        //                    else if (ti1 > ti2)
        //                        frdn = id + 1;
        //                    //idn = (decimal)Math.Abs((double)(tr[id].Level - levelM));
        //                    pd = frdn;
        //                }
        //                if (1000 + tr[iu].Level <= levelM && 1000 + tr[iu - 1].Level >= levelM && tr[iu].Freq > tr[frup].Freq)
        //                {
        //                    decimal ti1 = (decimal)Math.Abs((double)(1000 + tr[iu - 1].Level - levelM));
        //                    decimal ti2 = (decimal)Math.Abs((double)(1000 + tr[iu].Level - levelM));
        //                    if (ti1 < ti2)
        //                        frup = iu - 1;
        //                    else if (ti1 > ti2)
        //                        frup = iu;
        //                    //frup = iu;
        //                    //iun = (decimal)Math.Abs((double)(tr[iu].Level - levelM));
        //                    pu = frup;
        //                }
        //                //выходные данные
        //                if (id >= outindd)
        //                {
        //                    outarr[0] = pd;
        //                }
        //                if (iu <= outindu)
        //                {
        //                    outarr[1] = pu;
        //                }
        //            }
        //        }
        //    }
        //    #region Exception
        //    catch (Exception exp)
        //    {
        //        MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "LocalMeasurement", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
        //    }
        //    #endregion
        //    return outarr;
        //    #endregion
        //}
        #endregion old ndb

        /// <summary>
        /// ищет ближайший индекс в массиве по частоте
        /// </summary>
        /// <param name="tracepoints"></param>
        /// <param name="freq"></param>
        /// <returns></returns>
        public int FindMarkerIndOnTrace(tracepoint[] tracepoints, decimal freq)
        {
            int ind = -1;
            if (freq >= tracepoints[0].freq && freq <= tracepoints[tracepoints.Length - 1].freq)
            {
                decimal deviation = decimal.MaxValue;
                for (int i = 0; i < tracepoints.Length; i++)
                {
                    if (Math.Abs(tracepoints[i].freq - freq) < deviation)
                    {
                        deviation = Math.Abs(tracepoints[i].freq - freq);
                        ind = i;
                    }
                }
            }
            else if (freq < tracepoints[0].freq)
            { ind = 0; }
            else if (freq > tracepoints[tracepoints.Length - 1].freq)
            { ind = tracepoints.Length - 1; }
            return ind;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tr">трейс</param>
        /// <param name="IndPoint">индес точки в массиве</param>
        /// <param name="NumbAverage"> количество точек, полоса усредения (должна быть четной на всякий случай)</param>
        /// <returns></returns>
        public double AverageLevelNearPoint(tracepoint[] tr, int IndPoint, int NumbAverage)
        {
            #region
            double outlevel = -1;
            try
            {
                int start = IndPoint - NumbAverage / 2, stop = IndPoint + NumbAverage / 2;
                if (start < 0) start = 0;
                if (stop > tr.Length - 1) start = tr.Length - 1;
                int ii = 0;
                for (int i = start; i <= stop; i++)
                {
                    if (i == start) outlevel = tr[i].level;
                    else { outlevel += tr[i].level; }
                    ii++;
                }
                outlevel = outlevel / ii;
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "LocalMeasurement", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            return outlevel;
            #endregion
        }
        public double AverageLevelNearPointTrue(tracepoint[] tr, int IndPoint, int NumbAverage)
        {
            #region
            double outlevel = -1;
            try
            {
                int start = IndPoint - NumbAverage / 2, stop = IndPoint + NumbAverage / 2;
                if (start < 0) start = 0;
                if (stop > tr.Length - 1) stop = tr.Length - 1;
                int ii = 0;
                for (int i = start; i <= stop; i++)
                {
                    if (i == start) outlevel = Math.Pow(10, tr[i].level / 10);
                    else { outlevel += Math.Pow(10, tr[i].level / 10); }
                    ii++;
                }
                outlevel = 10 * Math.Log10(outlevel / ii);
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "RsReceiver", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            return outlevel;
            #endregion
        }
        
        public int PeakSearch(tracepoint[] tr)
        {
            #region
            int index = 0;
            try
            {
                if (tr != null && tr.Length > 0)
                {
                    double level = double.MinValue;
                    for (int i = 0; i < tr.Length; i++)
                    {
                        if (tr[i].level > level) { level = tr[i].level; index = i; }
                    }
                }
            }
            #region Exception
            catch (Exception exp)
            {
                MainWindow.exp.ExceptionData = new ExData() { ex = exp, ClassName = "LocalMeasurement", AdditionalInformation = System.Reflection.MethodInfo.GetCurrentMethod().Name };
            }
            #endregion
            return index;
            #endregion
        }

        public double MeasChannelPower(tracepoint[] trace, decimal freqCentrMeas, decimal BW)
        {
            double pow = 0;
            try
            {
                if (trace.Length > 0)
                {
                    decimal mf = freqCentrMeas - BW / 2;
                    decimal pf = freqCentrMeas + BW / 2;
                    if (mf < trace[0].freq) { mf = trace[0].freq; }
                    if (pf > trace[trace.Length - 1].freq) { pf = trace[trace.Length - 1].freq; }

                    int start = FindMarkerIndOnTrace(trace, mf);
                    int stop = FindMarkerIndOnTrace(trace, pf);
                    if (start != stop)
                    {
                        double powsum = 0;
                        for (int i = start; i < stop; i++)
                        {
                            if (i == 0) { powsum = Math.Pow(10, trace[i].level / 10); }
                            else powsum += Math.Pow(10, trace[i].level / 10);
                        }
                        pow = 10 * Math.Log10(powsum);
                    }
                    else { pow = trace[start].level; }

                    //if (LevelUnit == 1/*"dBµV"*/) pow += 106.98m;// + kf;
                }
            }
            catch { }
            return pow;
        }
        public double MeasChannelPowerOld(tracepoint[] trace, decimal freqCentrMeas, decimal BW, int LevelUnit)
        {
            double pow = 0;
            try
            {
                if (trace.Length > 0)
                {
                    decimal mf = freqCentrMeas - BW / 2;
                    decimal pf = freqCentrMeas + BW / 2;
                    if (mf < trace[0].freq) { mf = trace[0].freq; }
                    if (pf > trace[trace.Length - 1].freq) { pf = trace[trace.Length - 1].freq; }

                    int start = FindMarkerIndOnTrace(trace, mf);
                    int stop = FindMarkerIndOnTrace(trace, pf);
                    if (start != stop)
                    {
                        double[] tracebw = new double[stop - start];

                        for (int i = start; i < stop; i++)
                        {
                            tracebw[i - start] = trace[i].level;
                        }
                        double powsum = 0;
                        //decimal[] PowDot = new decimal[stop - start];
                        for (int i = 0; i < tracebw.Length; i++)
                        {
                            if (i == 0) { powsum = Math.Pow(10, (tracebw[i] - 106.98) / 10); }
                            else powsum += Math.Pow(10, (tracebw[i] - 106.98) / 10);
                        }
                        pow = 10 * Math.Log10(powsum);
                    }
                    else { pow = trace[start].level - 106.98; }
                    if (LevelUnit == 0) pow += 106.98;
                    else if (LevelUnit == 1/*"dBµV/m"*/) pow += 106.98;// + kf;
                }
            }
            catch { }
            return pow;
        }

        /// <summary>
        /// Function for change freq grid
        /// </summary>
        /// <param name="Levels">Arr with levels in dBm</param>
        /// <param name="StartOldFreq_MHz">Central Freq</param>
        /// <param name="StartOldStep_kHz"></param>
        /// <param name="StartNewFreq_MHz">Central Freq</param>
        /// <param name="StartNewStep_kHz"></param>
        /// <returns></returns>
        public tracepoint[] ChangeFreqGrid(ref tracepoint[] Levels, decimal StartOldFreq, decimal OldStep, decimal StartNewFreq, decimal NewStep, int PointsInNewLevelsArr, double LevelForEmptySteps_dBm_Hz = -158)
        {
            int CountOldFreq = 0;
            int CountNewFreq = 0;
            tracepoint[] NewLevels = new tracepoint[PointsInNewLevelsArr];
            for (int i = 0; i < PointsInNewLevelsArr; i++) { NewLevels[i] = new tracepoint() { freq = StartNewFreq + NewStep * i, level = -99999 }; }
            double CurLevel_mW = 0;
            double NewStep_dBHz = 10 * Math.Log10((double)NewStep);

            // простановка стартовых индексов идем от меньших к большим
            if ((StartNewFreq - (NewStep / 2)) <= (StartOldFreq - (OldStep / 2)))
            {
                // новый массив необходимо заполнить от начала 
                CountNewFreq = (int)Math.Floor((StartOldFreq - (OldStep / 2) - StartNewFreq + (NewStep / 2)) / NewStep);
                for (int i = 0; i < CountNewFreq; i++)
                {
                    NewLevels[i].level = LevelForEmptySteps_dBm_Hz + NewStep_dBHz;
                }
                NewLevels[CountNewFreq].level = LevelForEmptySteps_dBm_Hz + 10 * Math.Log10((double)(StartOldFreq - (OldStep / 2) - (StartNewFreq + ((CountNewFreq - 0.5m) * NewStep))));
            }
            else
            {
                CountOldFreq = (int)Math.Floor((StartNewFreq - (NewStep / 2) - StartOldFreq + (OldStep / 2)) / OldStep);
            }

            decimal LowNewFreq = StartNewFreq + ((decimal)(CountNewFreq - 0.5) * NewStep);
            decimal UpNewFreq = LowNewFreq + NewStep;
            decimal LowOldFreq = StartOldFreq + ((decimal)(CountOldFreq - 0.5) * OldStep);
            decimal UpOldFreq = LowOldFreq + OldStep;
            decimal DeltaFreqInterseption;
            CurLevel_mW = Math.Pow(10, NewLevels[CountNewFreq].level / 10);

            while ((CountNewFreq < PointsInNewLevelsArr) && (CountOldFreq < Levels.Length))
            {
                //Расчет полосы пересечения 
                if (UpOldFreq > UpNewFreq)
                {
                    DeltaFreqInterseption = UpNewFreq - Math.Max(LowNewFreq, LowOldFreq);
                    CurLevel_mW = CurLevel_mW + Math.Pow(10, Levels[CountOldFreq].level / 10) * (double)(DeltaFreqInterseption / OldStep);
                    NewLevels[CountNewFreq].level = 10 * (Math.Log10(CurLevel_mW));
                    CurLevel_mW = 0;
                    CountNewFreq++;
                    LowNewFreq = UpNewFreq;
                    UpNewFreq = UpNewFreq + NewStep;
                }
                else
                {
                    DeltaFreqInterseption = UpOldFreq - Math.Max(LowNewFreq, LowOldFreq);
                    CurLevel_mW = CurLevel_mW + Math.Pow(10, Levels[CountOldFreq].level / 10) * (double)(DeltaFreqInterseption / OldStep);
                    CountOldFreq++;
                    LowOldFreq = UpOldFreq;
                    UpOldFreq = UpOldFreq + OldStep;
                }
            }
            if (CountNewFreq < PointsInNewLevelsArr)
            {
                DeltaFreqInterseption = UpNewFreq - LowOldFreq;
                NewLevels[CountNewFreq].level = 10 * Math.Log10(CurLevel_mW + ((double)DeltaFreqInterseption) * Math.Pow(10, LevelForEmptySteps_dBm_Hz / 10));
                CountNewFreq++;
                for (int i = CountNewFreq; i < PointsInNewLevelsArr; i++)
                {
                    NewLevels[i].level = LevelForEmptySteps_dBm_Hz + NewStep_dBHz;
                }
            }
            return NewLevels;
        }
        /// <summary>
        /// Function for change freq grid
        /// </summary>
        /// <param name="Levels">Arr with levels in dBm</param>
        /// <param name="StartOldFreq_MHz">Central Freq</param>
        /// <param name="StartOldStep_kHz"></param>
        /// <param name="StartNewFreq_MHz">Central Freq</param>
        /// <param name="StartNewStep_kHz"></param>
        /// <returns></returns>
        public tracepoint[] ChangeFreqGrid_v2(ref tracepoint[] Levels, decimal StartOldFreq, decimal OldStep, decimal StartNewFreq, decimal NewStep, int PointsInNewLevelsArr, double LevelForEmptySteps_dBm_Hz = -158)
        {
            int CountOldFreq = 0;
            int CountNewFreq = 0;
            tracepoint[] NewLevels = new tracepoint[PointsInNewLevelsArr];
            //for (int i = 0; i < PointsInNewLevelsArr; i++) { NewLevels[i] = new tracepoint() { freq = StartNewFreq + NewStep * i, level = -99999 }; }
            double CurLevel_mW = 0;
            double NewStep_dBHz = 10 * Math.Log10((double)NewStep);

            // простановка стартовых индексов идем от меньших к большим
            if ((StartNewFreq - (NewStep / 2)) <= (StartOldFreq - (OldStep / 2)))
            {
                // новый массив необходимо заполнить от начала 
                CountNewFreq = (int)Math.Floor((StartOldFreq - (OldStep / 2) - StartNewFreq + (NewStep / 2)) / NewStep);
                for (int i = 0; i < CountNewFreq; i++)
                {
                    NewLevels[i] = new tracepoint() { freq = StartNewFreq + NewStep * i, level = LevelForEmptySteps_dBm_Hz + NewStep_dBHz };
                    //NewLevels[i].level = LevelForEmptySteps_dBm_Hz + NewStep_dBHz;
                }
                NewLevels[CountNewFreq] = new tracepoint() { freq = StartNewFreq + NewStep * CountNewFreq, level = LevelForEmptySteps_dBm_Hz + 10 * Math.Log10((double)(StartOldFreq - (OldStep / 2) - (StartNewFreq + ((CountNewFreq - 0.5m) * NewStep))))}; ;
                //NewLevels[CountNewFreq].level = LevelForEmptySteps_dBm_Hz + 10 * (decimal)(Math.Log10((double)(StartOldFreq - (OldStep / 2) - (StartNewFreq + ((CountNewFreq - 0.5m) * NewStep)))));
            }
            else
            {
                CountOldFreq = (int)Math.Floor((StartNewFreq - (NewStep / 2) - StartOldFreq + (OldStep / 2)) / OldStep);
            }

            decimal LowNewFreq = StartNewFreq + ((decimal)(CountNewFreq - 0.5) * NewStep);
            decimal UpNewFreq = LowNewFreq + NewStep;
            decimal LowOldFreq = StartOldFreq + ((decimal)(CountOldFreq - 0.5) * OldStep);
            decimal UpOldFreq = LowOldFreq + OldStep;
            decimal DeltaFreqInterseption;
            CurLevel_mW = Math.Pow(10, NewLevels[CountNewFreq].level / 10);

            while ((CountNewFreq < PointsInNewLevelsArr) && (CountOldFreq < Levels.Length))
            {
                //Расчет полосы пересечения 
                if (UpOldFreq > UpNewFreq)
                {
                    DeltaFreqInterseption = UpNewFreq - Math.Max(LowNewFreq, LowOldFreq);
                    CurLevel_mW = CurLevel_mW + Math.Pow(10, Levels[CountOldFreq].level / 10) *(double) (DeltaFreqInterseption / OldStep);
                    NewLevels[CountNewFreq] = new tracepoint() { freq = StartNewFreq + NewStep * CountNewFreq, level = 10 * Math.Log10(CurLevel_mW) }; 
                    CurLevel_mW = 0;
                    CountNewFreq++;
                    LowNewFreq = UpNewFreq;
                    UpNewFreq = UpNewFreq + NewStep;
                }
                else
                {
                    DeltaFreqInterseption = UpOldFreq - Math.Max(LowNewFreq, LowOldFreq);
                    CurLevel_mW = CurLevel_mW + Math.Pow(10, Levels[CountOldFreq].level / 10) * (double)(DeltaFreqInterseption / OldStep);
                    CountOldFreq++;
                    LowOldFreq = UpOldFreq;
                    UpOldFreq = UpOldFreq + OldStep;
                }
            }
            if (CountNewFreq < PointsInNewLevelsArr)
            {
                DeltaFreqInterseption = UpNewFreq - LowOldFreq;
                NewLevels[CountNewFreq] = new tracepoint() { freq = StartNewFreq + NewStep * CountNewFreq, level = 10 * Math.Log10(CurLevel_mW + (double)DeltaFreqInterseption * Math.Pow(10, LevelForEmptySteps_dBm_Hz / 10)) };  ;
                CountNewFreq++;
                for (int i = CountNewFreq; i < PointsInNewLevelsArr; i++)
                {
                    NewLevels[i] = new tracepoint() { freq = StartNewFreq + NewStep * CountNewFreq, level = LevelForEmptySteps_dBm_Hz + NewStep_dBHz };
                }
            }
            return NewLevels;
        }

        /// <summary>
        /// Function for change freq grid
        /// </summary>
        /// <param name="Levels">Arr with levels in dBm</param>
        /// <param name="StartOldFreq_MHz">Central Freq</param>
        /// <param name="StartOldStep_kHz"></param>
        /// <param name="StartNewFreq_MHz">Central Freq</param>
        /// <param name="StartNewStep_kHz"></param>
        /// <returns></returns>
        static public double[] ChangeGrid(ref double[] Levels, double StartOldFreq_MHz, double OldStep_kHz, double StartNewFreq_MHz, double NewStep_kHz, int PointsInNewLevelsArr, double LevelForEmptySteps_dBm_Hz = -158)
        {
            int CountOldFreq = 0;
            int CountNewFreq = 0;
            double[] NewLevels = new double[PointsInNewLevelsArr];
            for (int i = 0; i < PointsInNewLevelsArr; i++) { NewLevels[i] = -99999; }
            double CurLevel_mW = 0;
            double OldStep_MHz = OldStep_kHz / 1000.0;
            double NewStep_MHz = NewStep_kHz / 1000.0;
            double NewStep_dBHz = 10 * Math.Log10(NewStep_kHz) + 30;

            // простановка стартовых индексов идем от меньших к большим
            if ((StartNewFreq_MHz - (NewStep_MHz / 2.0)) <= (StartOldFreq_MHz - (OldStep_MHz / 2.0)))
            {
                // новый массив необходимо заполнить от начала 
                CountNewFreq = (int)Math.Floor((StartOldFreq_MHz - (OldStep_MHz / 2.0) - StartNewFreq_MHz + (NewStep_MHz / 2.0)) / NewStep_MHz);
                for (int i = 0; i < CountNewFreq; i++)
                {
                    NewLevels[i] = LevelForEmptySteps_dBm_Hz + NewStep_dBHz;
                }
                NewLevels[CountNewFreq] = LevelForEmptySteps_dBm_Hz + 10 * Math.Log10(StartOldFreq_MHz - (OldStep_MHz / 2.0) - (StartNewFreq_MHz + ((CountNewFreq - 0.5) * NewStep_MHz))) +60;
            }
            else
            {
                CountOldFreq = (int)Math.Floor((StartNewFreq_MHz - (NewStep_MHz / 2.0) - StartOldFreq_MHz + (OldStep_MHz / 2.0)) / OldStep_MHz);
            }

            double LowNewFreq_MHz = StartNewFreq_MHz + ((CountNewFreq - 0.5) * NewStep_MHz);
            double UpNewFreq_MHz = LowNewFreq_MHz + NewStep_MHz;
            double LowOldFreq_MHz = StartOldFreq_MHz + ((CountOldFreq - 0.5) * OldStep_MHz);
            double UpOldFreq_MHz = LowOldFreq_MHz + OldStep_MHz;
            double DeltaFreqInterseption_MHz;
            CurLevel_mW = Math.Pow(10, NewLevels[CountNewFreq] / 10);

            while ((CountNewFreq < PointsInNewLevelsArr) && (CountOldFreq < Levels.Length))
            {
                //Расчет полосы пересечения 
                if (UpOldFreq_MHz > UpNewFreq_MHz)
                {
                    DeltaFreqInterseption_MHz = UpNewFreq_MHz - Math.Max(LowNewFreq_MHz, LowOldFreq_MHz);
                    CurLevel_mW = CurLevel_mW + (Math.Pow(10, Levels[CountOldFreq] / 10) * DeltaFreqInterseption_MHz / OldStep_MHz);
                    NewLevels[CountNewFreq] = 10 * Math.Log10(CurLevel_mW);
                    CurLevel_mW = 0;
                    CountNewFreq++;
                    LowNewFreq_MHz = UpNewFreq_MHz;
                    UpNewFreq_MHz = UpNewFreq_MHz + NewStep_MHz;
                }
                else
                {
                    DeltaFreqInterseption_MHz = UpOldFreq_MHz - Math.Max(LowNewFreq_MHz, LowOldFreq_MHz);
                    CurLevel_mW = CurLevel_mW + (Math.Pow(10, Levels[CountOldFreq] / 10) * DeltaFreqInterseption_MHz / OldStep_MHz);
                    CountOldFreq++;
                    LowOldFreq_MHz = UpOldFreq_MHz;
                    UpOldFreq_MHz = UpOldFreq_MHz + OldStep_MHz;
                }
            }
            if (CountNewFreq < PointsInNewLevelsArr)
            {
                DeltaFreqInterseption_MHz = UpNewFreq_MHz - LowOldFreq_MHz;
                NewLevels[CountNewFreq] = 10 * Math.Log10(CurLevel_mW + DeltaFreqInterseption_MHz * Math.Pow(10, (LevelForEmptySteps_dBm_Hz + 60) / 10));
                CountNewFreq++;
                for (int i = CountNewFreq; i < PointsInNewLevelsArr; i++)
                {
                    NewLevels[i] = LevelForEmptySteps_dBm_Hz + NewStep_dBHz;
                }
            }
            return NewLevels;
        }
    }
}
