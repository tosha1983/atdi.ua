using System;
using System.Collections.Generic;
using Atdi.AppServer.Contracts.Sdrns;
using Atdi.Oracle.DataAccess;
using Atdi.AppServer;
using System.Collections;
using System.Linq;

namespace Atdi.SDNRS.AppServer.ManageDB.Adapters
{

    public class LevelMeasurementsCarForSO
    {
        public DateTime? TimeOfMeasurements; // из class LevelMeasurementsCar
        public decimal? CentralFrequency; // из class LevelMeasurementsCar
        public double? BW; // из class LevelMeasurementsCar если значение <= 0, то вытаскивать из MeasurementsParameterGeneral причем = SpecrumSteps*(T2-T1);
        public int? Idstation; // из class ResultsMeasurementsStation
    }


    public class AnaliticsUnit1 : IDisposable
    {
        public static ILogger logger;
        public AnaliticsUnit1(ILogger log)
        {
            if (logger == null) logger = log;
        }
        /// <summary>
        /// Деструктор.
        /// </summary>
        ~AnaliticsUnit1()
        {
            Dispose();
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        
        public SOFrequency[] CalcAppUnit2(List<double> Frequencies_MHz, double? BW_kHz, List<int> MeasResultID, double? LonMax, double? LonMin,  double? LatMax, double? LatMin, double? TrLevel_dBm)
        {
            List<SOFrequency> L_OUT = new List<SOFrequency>();
            try
            {
                System.Threading.Thread thread = new System.Threading.Thread(()=>
                {
                logger.Trace("Start procedure ConvertTo_MEAS_TASKObjects...");
                    try
                    {
                        for (int i = 0; i < Frequencies_MHz.Count; i++)
                        {
                            List<LevelMeasurementsCarForSO> listLevelMeas = new List<LevelMeasurementsCarForSO>();
                            string SQL = string.Format("(LON{0}) AND (LON{1}) AND (LAT{2}) AND (LAT{3}) AND (LEVELDBM{4}) AND (ID IN ({5}))", LonMin.HasValue ? ">=" + LonMin.ToString().Replace(",", ".") : " IS NULL", LonMax.HasValue ? "<=" + LonMax.ToString().Replace(",", ".") : " IS NULL", LatMin.HasValue ? ">=" + LatMin.ToString().Replace(",", ".") : " IS NULL", LatMax.HasValue ? "<=" + LatMax.ToString().Replace(",", ".") : " IS NULL", TrLevel_dBm.HasValue ? ">" + TrLevel_dBm.ToString().Replace(",", ".") : " IS NULL", string.Join(",", MeasResultID));
                            YXvUnit2 XvAppUnit2 = new YXvUnit2();
                            XvAppUnit2.Format("*");
                            XvAppUnit2.Filter = SQL;

                            for (XvAppUnit2.OpenRs(); !XvAppUnit2.IsEOF(); XvAppUnit2.MoveNext())
                            {
                                if ((XvAppUnit2.m_bw != null) && (XvAppUnit2.m_bw <= 0) && (XvAppUnit2.m_specrumsteps != null) && (XvAppUnit2.m_t1 != null) && (XvAppUnit2.m_t2 != null))
                                {
                                    double? BW = XvAppUnit2.m_specrumsteps * (XvAppUnit2.m_t2 - XvAppUnit2.m_t1);
                                    if (BW != null)
                                    {
                                        if ((((XvAppUnit2.m_centralfrequency - (BW / 2000)) < Frequencies_MHz[i]) && ((XvAppUnit2.m_centralfrequency + (BW / 2000)) > Frequencies_MHz[i]))
                                        || (((Frequencies_MHz[i] - (BW_kHz / 2000)) < XvAppUnit2.m_centralfrequency) && ((Frequencies_MHz[i] + (BW_kHz / 2000)) > XvAppUnit2.m_centralfrequency)))
                                        {
                                            var prm = new LevelMeasurementsCarForSO()
                                            {
                                                BW = BW,
                                                CentralFrequency = (decimal?)XvAppUnit2.m_centralfrequency,
                                                Idstation = XvAppUnit2.m_idstation,
                                                TimeOfMeasurements = XvAppUnit2.m_timeofmeasurements
                                            };
                                            listLevelMeas.Add(prm);
                                        }
                                    }
                                }
                                else if ((XvAppUnit2.m_bw != null) && (XvAppUnit2.m_bw > 0))
                                {
                                    double? BW = XvAppUnit2.m_bw;
                                    if (BW != null)
                                    {
                                        if ((((XvAppUnit2.m_centralfrequency - (BW / 2000)) < Frequencies_MHz[i]) && ((XvAppUnit2.m_centralfrequency + (BW / 2000)) > Frequencies_MHz[i]))
                                        || (((Frequencies_MHz[i] - (BW_kHz / 2000)) < XvAppUnit2.m_centralfrequency) && ((Frequencies_MHz[i] + (BW_kHz / 2000)) > XvAppUnit2.m_centralfrequency)))
                                        {
                                            var prm = new LevelMeasurementsCarForSO()
                                            {
                                                BW = BW,
                                                CentralFrequency = (decimal?)XvAppUnit2.m_centralfrequency,
                                                Idstation = XvAppUnit2.m_idstation,
                                                TimeOfMeasurements = XvAppUnit2.m_timeofmeasurements
                                            };
                                            listLevelMeas.Add(prm);
                                        }
                                    }
                                }
                            }
                            XvAppUnit2.Close();
                            XvAppUnit2.Dispose();


                            double? MaxFreq = Frequencies_MHz.Max();
                            double? MinFreq = Frequencies_MHz.Min();
                            int[] listTimeHours24 = new int[24];


                            List<DateTime?> DiffVal = new List<DateTime?>();
                            List<LevelMeasurementsCarForSO> listLevelMeas2 = new List<LevelMeasurementsCarForSO>();

                            SQL = string.Format("(LON{0}) AND (LON{1}) AND (LAT{2}) AND (LAT{3}) AND  (ID IN ({4}))", LonMin.HasValue ? ">=" + LonMin.ToString().Replace(",", ".") : " IS NULL", LonMax.HasValue ? "<=" + LonMax.ToString().Replace(",", ".") : " IS NULL", LatMin.HasValue ? ">=" + LatMin.ToString().Replace(",", ".") : " IS NULL", LatMax.HasValue ? "<=" + LatMax.ToString().Replace(",", ".") : " IS NULL", string.Join(",", MeasResultID));
                            XvAppUnit2 = new YXvUnit2();
                            XvAppUnit2.Format("*");
                            XvAppUnit2.Filter = SQL;

                            for (XvAppUnit2.OpenRs(); !XvAppUnit2.IsEOF(); XvAppUnit2.MoveNext())
                            {
                                if ((XvAppUnit2.m_bw != null) && (XvAppUnit2.m_bw <= 0) && (XvAppUnit2.m_specrumsteps != null) && (XvAppUnit2.m_t1 != null) && (XvAppUnit2.m_t2 != null))
                                {
                                    double? BW = XvAppUnit2.m_specrumsteps * (XvAppUnit2.m_t2 - XvAppUnit2.m_t1);
                                    if (BW != null)
                                    {
                                        if ((((XvAppUnit2.m_centralfrequency - (BW / 2000)) < Frequencies_MHz[i]) && ((XvAppUnit2.m_centralfrequency + (BW / 2000)) > Frequencies_MHz[i]))
                                         || ((MaxFreq + (BW_kHz / 2000)) > XvAppUnit2.m_centralfrequency) && ((MinFreq - (BW_kHz / 2000)) < XvAppUnit2.m_centralfrequency))
                                        {
                                            var prm = new LevelMeasurementsCarForSO()
                                            {
                                                BW = BW,
                                                CentralFrequency = (decimal?)XvAppUnit2.m_centralfrequency,
                                                Idstation = XvAppUnit2.m_idstation,
                                                TimeOfMeasurements = XvAppUnit2.m_timeofmeasurements
                                            };
                                            if (DiffVal.Find(z => z.Value.Subtract(XvAppUnit2.m_timeofmeasurements.Value).Seconds <= 3) == null)
                                            {
                                                DiffVal.Add(XvAppUnit2.m_timeofmeasurements);
                                            }
                                            listLevelMeas2.Add(prm);
                                        }
                                    }
                                }
                                else if ((XvAppUnit2.m_bw != null) && (XvAppUnit2.m_bw > 0))
                                {
                                    double? BW = XvAppUnit2.m_bw;
                                    if (BW != null)
                                    {
                                        if ((((XvAppUnit2.m_centralfrequency - (BW / 2000)) < Frequencies_MHz[i]) && ((XvAppUnit2.m_centralfrequency + (BW / 2000)) > Frequencies_MHz[i]))
                                         || ((MaxFreq + (BW_kHz / 2000)) > XvAppUnit2.m_centralfrequency) && ((MinFreq - (BW_kHz / 2000)) < XvAppUnit2.m_centralfrequency))
                                        {
                                            var prm = new LevelMeasurementsCarForSO()
                                            {
                                                BW = BW,
                                                CentralFrequency = (decimal?)XvAppUnit2.m_centralfrequency,
                                                Idstation = XvAppUnit2.m_idstation,
                                                TimeOfMeasurements = XvAppUnit2.m_timeofmeasurements
                                            };

                                            listLevelMeas2.Add(prm);
                                        }
                                    }
                                }
                            }
                            XvAppUnit2.Close();
                            XvAppUnit2.Dispose();


                            for (int k = 0; k < listLevelMeas2.Count; k++)
                            {
                                if (DiffVal.Find(z => z.Value.Subtract(listLevelMeas2[k].TimeOfMeasurements.Value).Seconds <= 3) == null)
                                {
                                    DiffVal.Add(listLevelMeas2[k].TimeOfMeasurements.Value);
                                }
                            }
                            int sumHit = DiffVal.Count();

                            for (int t = 0; t < 24; t++)
                            {
                                var DiffValCheck = new List<DateTime?>();
                                for (int k = 0; k < listLevelMeas2.Count; k++)
                                {
                                    if ((DiffValCheck.Find(z => z.Value.Subtract(listLevelMeas2[k].TimeOfMeasurements.Value).Seconds <= 3) == null) && (listLevelMeas2[k].TimeOfMeasurements.Value.Hour == t))
                                    {
                                        DiffValCheck.Add(listLevelMeas2[k].TimeOfMeasurements.Value);
                                    }
                                }
                                listTimeHours24[t] = DiffValCheck.Count;
                            }

                            SOFrequency soFreq = new SOFrequency();
                            soFreq.Frequency_MHz = Frequencies_MHz[i];
                            soFreq.hit = listLevelMeas.Count;
                            soFreq.Occupation =  sumHit!=0 ?  100 * soFreq.hit / sumHit : 0;
                            if (soFreq.Occupation > 100) soFreq.Occupation = 100;

                            List<int?> distinctStation = new List<int?>();
                            for (int p=0; p< listLevelMeas.Count; p++)
                            {
                                if (!distinctStation.Contains(listLevelMeas[p].Idstation))
                                    distinctStation.Add(listLevelMeas[p].Idstation);
                            }
                            soFreq.StantionIDs = string.Join(";", distinctStation);
                            soFreq.countStation = distinctStation.Count;

                            string[] hit_00_23 = new string[24];
                            for (int t = 0; t < 24; t++)
                            {
                                if (listTimeHours24[t] != 0)
                                {
                                    double? v = Math.Round((double)(100 * listLevelMeas.Count / listTimeHours24[t]), 2);
                                    if (v>100)
                                    {
                                        v = 100;
                                    }
                                    hit_00_23[t] = v.ToString();
                                }
                                else
                                {
                                    hit_00_23[t] = "----";
                                }
                            }
                            soFreq.OccupationByHuors = string.Join(";", hit_00_23);
                            L_OUT.Add(soFreq);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure CalcAppUnit1... " + ex.Message);
                    }
                logger.Trace("End procedure CalcAppUnit1...");
                });
                thread.Start();
                thread.Join();
            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure CalcAppUnit1..." + ex.Message);
            }
            return L_OUT.ToArray();
        }

    }
}
