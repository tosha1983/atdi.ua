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
        public int? Id; // из class ResultsMeasurementsStation
        public string globalSid;
    }

    public class Hit
    {
        public DateTime? dateTime;
        public int? Id;
        public string globalSid= "";
    }

    public class SOFrequencyTemp
    {
        public double Frequency_MHz;
        public int hit = 0;
        public List<string> StantionIDs = new List<string>();
        public int[] HitByHuors = new int[24];
        public List<string> measglobalsid = new List<string>();
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


        public SOFrequency[] CalcAppUnit2(List<double> Frequencies_MHz, double BW_kHz, List<int> MeasResultID, double LonMax, double LonMin, double LatMax, double LatMin, double TrLevel_dBm)
        {
            List<SOFrequency> L_OUT = new List<SOFrequency>();
            try
            {
                System.Threading.Thread thread = new System.Threading.Thread(() =>
                {
                    logger.Trace("Start procedure CalcAppUnit2...");
                    try
                    {
                        int count = Frequencies_MHz.Count;
                        List<Hit> HitVal = new List<Hit>();
                        List<SOFrequencyTemp> listLevelMeasFreq = new List<SOFrequencyTemp>();
                        for (int i = 0; i < Frequencies_MHz.Count; i++)
                        {
                            listLevelMeasFreq.Add(new SOFrequencyTemp() { Frequency_MHz = Frequencies_MHz[i] });
                        }
                        
                        List<LevelMeasurementsCarForSO> listLevelMeas2 = new List<LevelMeasurementsCarForSO>();
                        double? MaxFreq = Frequencies_MHz.Max();
                        double? MinFreq = Frequencies_MHz.Min();
                        string SQL = string.Format("(LON{0}) AND (LON{1}) AND (LAT{2}) AND (LAT{3}) AND  (ID IN ({4})) AND ({5} > CENTRALFREQUENCY) AND ({6} < CENTRALFREQUENCY )", ">='" + LonMin.ToString()+"'", "<='" + LonMax.ToString()+"'", ">='" + LatMin.ToString()+"'", "<='" + LatMax.ToString()+"'", string.Join(",", MeasResultID),"'"+ (MaxFreq + (BW_kHz / 2000)).ToString()+"'", "'"+ (MinFreq - (BW_kHz / 2000)).ToString()+"'");
                        var XvAppUnit2 = new YXvUnit2();
                        XvAppUnit2.Format("*");
                        XvAppUnit2.Filter = SQL;
                        for (XvAppUnit2.OpenRs(); !XvAppUnit2.IsEOF(); XvAppUnit2.MoveNext())
                        {
                            double? BW = null;
                            if ((XvAppUnit2.m_bw != null) && (XvAppUnit2.m_bw <= 0) && (XvAppUnit2.m_specrumsteps != null) && (XvAppUnit2.m_t1 != null) && (XvAppUnit2.m_t2 != null))
                            {
                                BW = XvAppUnit2.m_specrumsteps * (XvAppUnit2.m_t2 - XvAppUnit2.m_t1);
                            }
                            else if ((XvAppUnit2.m_bw != null) && (XvAppUnit2.m_bw > 0))
                            {
                                BW = XvAppUnit2.m_bw;
                            }
                            else
                            {
                                BW = BW_kHz;
                            }

                            if (BW != null)
                            {
                                var prm = new LevelMeasurementsCarForSO()
                                {
                                    BW = BW,
                                    CentralFrequency = (decimal?)XvAppUnit2.m_centralfrequency,
                                    Idstation = XvAppUnit2.m_idstation,
                                    TimeOfMeasurements = XvAppUnit2.m_timeofmeasurements,
                                    Id = XvAppUnit2.m_id,
                                    globalSid = XvAppUnit2.m_measglobalsid
                                };
                                listLevelMeas2.Add(prm);
                            }
                            for (int i = 0; i < Frequencies_MHz.Count; i++)
                            {
                                BW = null;
                                if ((XvAppUnit2.m_bw != null) && (XvAppUnit2.m_bw <= 0) && (XvAppUnit2.m_specrumsteps != null) && (XvAppUnit2.m_t1 != null) && (XvAppUnit2.m_t2 != null))
                                {
                                    BW = XvAppUnit2.m_specrumsteps * (XvAppUnit2.m_t2 - XvAppUnit2.m_t1);
                                }
                                else if ((XvAppUnit2.m_bw != null) && (XvAppUnit2.m_bw > 0))
                                {
                                    BW = XvAppUnit2.m_bw;
                                }
                                else
                                {
                                    // костыль
                                    BW = BW_kHz;
                                }
                                if (BW != null)
                                {
                                    if (((((XvAppUnit2.m_centralfrequency - (BW / 2000)) < Frequencies_MHz[i]) && ((XvAppUnit2.m_centralfrequency + (BW / 2000)) > Frequencies_MHz[i]))
                                    || (((Frequencies_MHz[i] - (BW_kHz / 2000)) < XvAppUnit2.m_centralfrequency) && ((Frequencies_MHz[i] + (BW_kHz / 2000)) > XvAppUnit2.m_centralfrequency))) && ((XvAppUnit2.m_leveldbm > TrLevel_dBm)))
                                    {
                                        listLevelMeasFreq[i].hit++;
                                        if ((XvAppUnit2.m_idstation.HasValue) && (XvAppUnit2.m_idstation.Value != 0))
                                        {
                                            if (listLevelMeasFreq[i].StantionIDs.Find(z => z == XvAppUnit2.m_idstation.Value.ToString()) == null)
                                            {
                                                listLevelMeasFreq[i].StantionIDs.Add(XvAppUnit2.m_idstation.Value.ToString());
                                            }
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty(XvAppUnit2.m_measglobalsid))
                                            {
                                                if (listLevelMeasFreq[i].measglobalsid.Find(z => z == XvAppUnit2.m_measglobalsid) == null)
                                                {
                                                    listLevelMeasFreq[i].measglobalsid.Add(XvAppUnit2.m_measglobalsid);
                                                }
                                            }
                                        }
                                        for (int t = 0; t < 24; t++)
                                        {
                                            if (XvAppUnit2.m_timeofmeasurements.Value.Hour == t)
                                            {
                                                listLevelMeasFreq[i].HitByHuors[t]++;
                                                break;
                                            }

                                        }
                                    }
                                }
                            }
                        }
                        XvAppUnit2.Close();
                        XvAppUnit2.Dispose();
                        for (int k = 0; k < listLevelMeas2.Count; k++)
                        {
                            var hitValue = new Hit();
                            hitValue.dateTime = listLevelMeas2[k].TimeOfMeasurements.Value;
                            hitValue.Id = listLevelMeas2[k].Id;
                            hitValue.globalSid = listLevelMeas2[k].globalSid;

                            if (HitVal.Find(z => Math.Abs(z.dateTime.Value.Subtract(hitValue.dateTime.Value).Seconds) <= 3 && z.Id.Value == hitValue.Id.Value) == null)
                            {
                                HitVal.Add(hitValue);
                            }

                        }
                        int sumHit = HitVal.Count();
                        int[] listTimeHours24 = new int[24];
                        for (int m = 0; m < HitVal.Count; m++)
                        {
                            for (int t = 0; t < 24; t++)
                            {
                                if (HitVal[m].dateTime.Value.Hour == t)
                                {
                                    listTimeHours24[t] = listTimeHours24[t] + 1;
                                    break;
                                }

                            }
                        }

                        for (int i = 0; i < Frequencies_MHz.Count; i++)
                        {
                            var freqOut = new SOFrequency();
                            freqOut.Frequency_MHz = listLevelMeasFreq[i].Frequency_MHz;
                            freqOut.StantionIDs = string.Join(";", listLevelMeasFreq[i].StantionIDs);
                            if (freqOut.StantionIDs.Length > 300) freqOut.StantionIDs = freqOut.StantionIDs.Substring(0, 300);
                            freqOut.countStation = listLevelMeasFreq[i].StantionIDs.Count + listLevelMeasFreq[i].measglobalsid.Count;
                            listLevelMeasFreq[i].hit = 0;
                            List<double> stringByHours = new List<double>();
                            for (int t = 0; t < 24; t++)
                            {
                                double val = -1;
                                if (listTimeHours24[t] != 0)
                                {
                                    
                                    if (listTimeHours24[t] < listLevelMeasFreq[i].HitByHuors[t])
                                    {
                                        listLevelMeasFreq[i].HitByHuors[t] = listTimeHours24[t];
                                    }
                                    listLevelMeasFreq[i].hit = listLevelMeasFreq[i].hit + listLevelMeasFreq[i].HitByHuors[t];
                                    val =  (100 * listLevelMeasFreq[i].HitByHuors[t] / listTimeHours24[t]) ;
                                   
                                }
                                stringByHours.Add(val);
                            }
                            freqOut.hit = listLevelMeasFreq[i].hit;
                            if (sumHit == 0)
                                freqOut.Occupation = -1;
                            else
                                freqOut.Occupation = 100 * freqOut.hit / sumHit;
                            if (freqOut.Occupation > 100) freqOut.Occupation = 100;

                            freqOut.OccupationByHuors = string.Join(";", stringByHours);
                            L_OUT.Add(freqOut);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Trace("Error in procedure CalcAppUnit2... " + ex.Message);
                    }
                    logger.Trace("End procedure CalcAppUnit2...");
                });
                thread.Start();
                thread.Join();
            }
            catch (Exception ex)
            {
                logger.Error("Error in procedure CalcAppUnit2..." + ex.Message);
            }
            return L_OUT.ToArray();
        }

    }
}

