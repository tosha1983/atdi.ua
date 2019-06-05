using System.Collections.Generic;
using Atdi.Contracts.Api.EventSystem;
using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.DataConstraint;
using Atdi.Platform.Logging;
using System;
using MD = Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.Contracts.WcfServices.Sdrn.Server;
using System.Linq;

namespace Atdi.WcfServices.Sdrn.Server
{


    public class AnaliticsUnit 
    {
        private readonly IDataLayer<EntityDataOrm> _dataLayer;
        private readonly ILogger _logger;


        public AnaliticsUnit(IDataLayer<EntityDataOrm> dataLayer, ILogger logger)
        {
            this._dataLayer = dataLayer;
            this._logger = logger;
        }




        public SOFrequency[] CalcAppUnit(GetSOformMeasResultStationValue options)
        {
            var listOUT = new List<SOFrequency>();
            try
            {
                this._logger.Info(Contexts.ThisComponent, Categories.Processing, Events.HandlerCalcAppUnitMethod.Text);
                var hitVal = new List<Hit>();
                var listLevelMeasFreq = new List<SOFrequencyTemp>();
                var massFrequencies_MHz = options.Frequencies_MHz.ToArray();
                for (int i = 0; i < massFrequencies_MHz.Length; i++)
                {
                    listLevelMeasFreq.Add(new SOFrequencyTemp() { Frequency_MHz = massFrequencies_MHz[i] });
                }

                var listLevelMeas2 = new List<LevelMeasurementsCarForSO>();
                double? MaxFreq = options.Frequencies_MHz.Max();
                double? MinFreq = options.Frequencies_MHz.Min();
                long[] MeasResultIDConvert = options.MeasResultID.Select(n => (long)(n)).ToArray();

                var queryExecuter = this._dataLayer.Executor<SdrnServerDataContext>();
                var builderXVUnit2 = this._dataLayer.GetBuilder<MD.IXVUnit2>().From();
                builderXVUnit2.Select(c => c.BW);
                builderXVUnit2.Select(c => c.CentralFrequency);
                builderXVUnit2.Select(c => c.Id);
                builderXVUnit2.Select(c => c.IdStation);
                builderXVUnit2.Select(c => c.Lat);
                builderXVUnit2.Select(c => c.LeveldBm);
                builderXVUnit2.Select(c => c.Lon);
                builderXVUnit2.Select(c => c.MeasGlobalSid);
                builderXVUnit2.Select(c => c.SpecrumSteps);
                builderXVUnit2.Select(c => c.T1);
                builderXVUnit2.Select(c => c.T2);
                builderXVUnit2.Select(c => c.TimeOfMeasurements);
                builderXVUnit2.Where(c => c.Lon, ConditionOperator.GreaterEqual, options.LonMin);
                builderXVUnit2.Where(c => c.Lon, ConditionOperator.LessEqual, options.LonMax);
                builderXVUnit2.Where(c => c.Lat, ConditionOperator.GreaterEqual, options.LatMin);
                builderXVUnit2.Where(c => c.Lat, ConditionOperator.LessEqual, options.LatMax);
                builderXVUnit2.Where(c => c.Id, ConditionOperator.In, MeasResultIDConvert);
                builderXVUnit2.Where(c => c.CentralFrequency, ConditionOperator.GreaterThan, (MinFreq - (options.BW_kHz / 2000)));
                builderXVUnit2.Where(c => c.CentralFrequency, ConditionOperator.LessThan, (MaxFreq + (options.BW_kHz / 2000)));
                queryExecuter.Fetch(builderXVUnit2, readerXVUnit2 =>
                {
                    while (readerXVUnit2.Read())
                    {
                        double? BW = null;
                        if ((readerXVUnit2.GetValue(c => c.BW) != null) && (readerXVUnit2.GetValue(c => c.BW) <= 0) && (readerXVUnit2.GetValue(c => c.SpecrumSteps) != null) && (readerXVUnit2.GetValue(c => c.T1) != null) && (readerXVUnit2.GetValue(c => c.T2) != null))
                        {
                            BW = readerXVUnit2.GetValue(c => c.SpecrumSteps) * (readerXVUnit2.GetValue(c => c.T2) - readerXVUnit2.GetValue(c => c.T1));
                        }
                        else if ((readerXVUnit2.GetValue(c => c.BW) != null) && (readerXVUnit2.GetValue(c => c.BW) > 0))
                        {
                            BW = readerXVUnit2.GetValue(c => c.BW);
                        }
                        else
                        {
                            BW = options.BW_kHz;
                        }

                        if (BW != null)
                        {
                            var prm = new LevelMeasurementsCarForSO()
                            {
                                BW = BW,
                                CentralFrequency = (decimal?)readerXVUnit2.GetValue(c => c.CentralFrequency),
                                Idstation = readerXVUnit2.GetValue(c => c.IdStation),
                                TimeOfMeasurements = readerXVUnit2.GetValue(c => c.TimeOfMeasurements),
                                Id = readerXVUnit2.GetValue(c => c.Id),
                                globalSid = readerXVUnit2.GetValue(c => c.MeasGlobalSid)
                            };
                            listLevelMeas2.Add(prm);
                        }
                        for (int i = 0; i < massFrequencies_MHz.Length; i++)
                        {
                            BW = null;
                            if ((readerXVUnit2.GetValue(c => c.BW) != null) && (readerXVUnit2.GetValue(c => c.BW) <= 0) && (readerXVUnit2.GetValue(c => c.SpecrumSteps) != null) && (readerXVUnit2.GetValue(c => c.T1) != null) && (readerXVUnit2.GetValue(c => c.T2) != null))
                            {
                                BW = readerXVUnit2.GetValue(c => c.SpecrumSteps) * (readerXVUnit2.GetValue(c => c.T2) - readerXVUnit2.GetValue(c => c.T1));
                            }
                            else if ((readerXVUnit2.GetValue(c => c.BW) != null) && (readerXVUnit2.GetValue(c => c.BW) > 0))
                            {
                                BW = readerXVUnit2.GetValue(c => c.BW);
                            }
                            else
                            {
                            // костыль
                            BW = options.BW_kHz;
                            }
                            if (BW != null)
                            {
                                if (((((readerXVUnit2.GetValue(c => c.CentralFrequency) - (BW / 2000)) < massFrequencies_MHz[i]) && ((readerXVUnit2.GetValue(c => c.CentralFrequency) + (BW / 2000)) > massFrequencies_MHz[i]))
                                || (((massFrequencies_MHz[i] - (options.BW_kHz / 2000)) < readerXVUnit2.GetValue(c => c.CentralFrequency)) && ((massFrequencies_MHz[i] + (options.BW_kHz / 2000)) > readerXVUnit2.GetValue(c => c.CentralFrequency)))) && ((readerXVUnit2.GetValue(c => c.LeveldBm) > options.TrLevel_dBm)))
                                {
                                    listLevelMeasFreq[i].hit++;
                                    if ((readerXVUnit2.GetValue(c => c.IdStation).HasValue) && (readerXVUnit2.GetValue(c => c.IdStation) != 0))
                                    {
                                        if (listLevelMeasFreq[i].StantionIDs.Find(z => z == readerXVUnit2.GetValue(c => c.IdStation).Value.ToString()) == null)
                                        {
                                            listLevelMeasFreq[i].StantionIDs.Add(readerXVUnit2.GetValue(c => c.IdStation).Value.ToString());
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(readerXVUnit2.GetValue(c => c.MeasGlobalSid)))
                                        {
                                            if (listLevelMeasFreq[i].measglobalsid.Find(z => z == readerXVUnit2.GetValue(c => c.MeasGlobalSid)) == null)
                                            {
                                                listLevelMeasFreq[i].measglobalsid.Add(readerXVUnit2.GetValue(c => c.MeasGlobalSid));
                                            }
                                        }
                                    }
                                    for (int t = 0; t < 24; t++)
                                    {
                                        if (readerXVUnit2.GetValue(c => c.TimeOfMeasurements) != null)
                                        {
                                            if (readerXVUnit2.GetValue(c => c.TimeOfMeasurements).Value.Hour == t)
                                            {
                                                listLevelMeasFreq[i].HitByHuors[t]++;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return true;
                });

                for (int k = 0; k < listLevelMeas2.Count; k++)
                {
                    var hitValue = new Hit();
                    hitValue.dateTime = listLevelMeas2[k].TimeOfMeasurements.Value;
                    hitValue.Id = listLevelMeas2[k].Id;
                    hitValue.globalSid = listLevelMeas2[k].globalSid;

                    if (hitVal.Find(z => Math.Abs(z.dateTime.Value.Subtract(hitValue.dateTime.Value).Seconds) <= 3 && z.Id.Value == hitValue.Id.Value) == null)
                    {
                        hitVal.Add(hitValue);
                    }

                }
                int sumHit = hitVal.Count();
                int[] listTimeHours24 = new int[24];
                for (int m = 0; m < hitVal.Count; m++)
                {
                    for (int t = 0; t < 24; t++)
                    {
                        if (hitVal[m].dateTime.Value.Hour == t)
                        {
                            listTimeHours24[t] = listTimeHours24[t] + 1;
                            break;
                        }

                    }
                }


                int[] listTimeHours24MaxByFreqs = new int[24];
                for (int t = 0; t < 24; t++)
                {
                    int val1 = 0;
                    for (int i = 0; i < massFrequencies_MHz.Length; i++)
                    {
                        if (val1 < listLevelMeasFreq[i].HitByHuors[t])
                        {
                            val1 = listLevelMeasFreq[i].HitByHuors[t];
                        }
                    }
                    listTimeHours24MaxByFreqs[t] = val1;
                }

                for (int t = 0; t < 24; t++)
                {
                    if (listTimeHours24MaxByFreqs[t] > 2 * listTimeHours24[t])
                    {
                        listTimeHours24[t] = (int)(listTimeHours24MaxByFreqs[t] * 0.8);
                    }
                }


                for (int i = 0; i < massFrequencies_MHz.Length; i++)
                {
                    var freqOut = new SOFrequency();
                    freqOut.Frequency_MHz = listLevelMeasFreq[i].Frequency_MHz;
                    freqOut.StantionIDs = string.Join("/", listLevelMeasFreq[i].StantionIDs);
                    if (freqOut.StantionIDs.Length > 300) freqOut.StantionIDs = freqOut.StantionIDs.Substring(0, 300);
                    freqOut.countStation = listLevelMeasFreq[i].StantionIDs.Count + listLevelMeasFreq[i].measglobalsid.Count;
                    listLevelMeasFreq[i].hit = 0;
                    List<double> stringByHours = new List<double>();
                    int sumHitValue = 0;
                    int hitbyFreq = 0;
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
                            val = (100 * listLevelMeasFreq[i].HitByHuors[t] / listTimeHours24[t]);
                        }
                        stringByHours.Add(val);
                        sumHitValue += listTimeHours24[t];
                        hitbyFreq += listLevelMeasFreq[i].HitByHuors[t];
                    }
                    freqOut.hit = hitbyFreq;////listLevelMeasFreq[i].hit;
                    if (sumHitValue == 0)
                        freqOut.Occupation = -1;
                    else
                        freqOut.Occupation = 100 * freqOut.hit / sumHitValue;
                    if (freqOut.Occupation > 100) freqOut.Occupation = 100;

                    freqOut.OccupationByHuors = string.Join(";", stringByHours);
                    listOUT.Add(freqOut);
                }
            }
            catch (Exception e)
            {
                this._logger.Exception(Contexts.ThisComponent, e);
            }
            return listOUT.ToArray();
        }

    }
}
