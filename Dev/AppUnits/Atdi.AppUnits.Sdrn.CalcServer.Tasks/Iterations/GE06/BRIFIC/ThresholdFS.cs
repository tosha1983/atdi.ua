using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.Sdrn.CalcServer;
using Atdi.Contracts.Sdrn.DeepServices;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Maps;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.Platform.Logging;
using Atdi.Contracts.Sdrn.DeepServices.Gis;
using Atdi.Platform.Data;
using Atdi.DataModels.Sdrn.DeepServices.EarthGeometry;
using Atdi.Contracts.Sdrn.DeepServices.EarthGeometry;
using Atdi.DataModels.Sdrn.DeepServices.GN06;


namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{
    /// <summary>
    /// Определение напряженности поля для затронутой службы 
    /// </summary>
    /// <param name="freq_MHz"></param>
    /// <param name="staClass"></param>
    /// <param name="isDigital"></param>
    /// <returns></returns>
    public static class ThresholdFS
    {

        public static List<ThresholdFieldStrengthParameters> ThresholdFieldStrengthParameters = new List<ThresholdFieldStrengthParameters>();
        public static bool _isLoaded;

        static ThresholdFS()
        {
            if (!_isLoaded)
            {
                FillPrimaryServicesThresholdFSParameters();
                FillFixedServicesTDABThresholdFSParameters();
                FillMobileServicesThresholdFSParameters();
                FillRadioNavigationServicesThresholdFSParameters();
                FillFixedServicesThresholdFSParameters();
                _isLoaded = true;
            }
        }



        /// <summary>
        /// Определение пороговых напряженностей для затронутых служб
        /// (Allotments)
        /// </summary>
        /// <param name="broadcastingContextBase"></param>
        /// <param name="arrThresholdFieldStrengths"></param>
        public static ThresholdFieldStrength[] GetThresholdFieldStrengthByAllotments(BroadcastingAllotment  broadcastingAllotment, TypeThresholdFS typeThresholdFS)
        {
            var arrThresholdFieldStrength = new List<ThresholdFieldStrength>();
            float thresholdFS = -9999;
            ///список затронутых служб для Allotments
            if (broadcastingAllotment != null)
            {
                var fndThresholdFS = CalcThresholdFieldStrength(broadcastingAllotment.Target.Freq_MHz, broadcastingAllotment.AdminData.StnClass, typeThresholdFS, broadcastingAllotment.AdminData.IsDigital);
                if (fndThresholdFS != null)
                {
                    thresholdFS = fndThresholdFS.ThresholdFS;

                    var thresholdFieldStrength = new ThresholdFieldStrength()
                    {
                        Freq_MHz = broadcastingAllotment.Target.Freq_MHz,
                        StaClass = broadcastingAllotment.AdminData.StnClass,
                        System_type = fndThresholdFS.System_type,
                        ThresholdFS = thresholdFS,
                        IsDigital = broadcastingAllotment.AdminData.IsDigital,
                        Height_m = fndThresholdFS.Height_m,
                        Time_pc = fndThresholdFS.Time_pc,
                        MinFreq_MHz = fndThresholdFS.MinFreq_MHz,
                        MaxFreq_MHz = fndThresholdFS.MaxFreq_MHz
                    };
                    if (arrThresholdFieldStrength.Find(x => x.Freq_MHz == broadcastingAllotment.Target.Freq_MHz && x.StaClass == broadcastingAllotment.AdminData.StnClass && x.IsDigital == broadcastingAllotment.AdminData.IsDigital) == null)
                    {
                        arrThresholdFieldStrength.Add(thresholdFieldStrength);
                    }
                }
            }
            return arrThresholdFieldStrength.ToArray();
        }

        /// <summary>
        /// Определение пороговых напряженностей для затронутых служб
        /// (Assignments)
        /// </summary>
        /// <param name="broadcastingContextBase"></param>
        /// <param name="arrThresholdFieldStrengths"></param>
        public static ThresholdFieldStrength[] GetThresholdFieldStrengthByAssignments(BroadcastingAssignment[]  broadcastingAssignment, TypeThresholdFS  typeThresholdFS)
        {
            var arrThresholdFieldStrength = new List<ThresholdFieldStrength>();
            float thresholdFS = -9999;
            ///список затронутых служб для Assignments
            if (broadcastingAssignment != null)
            {
                for (int i = 0; i < broadcastingAssignment.Length; i++)
                {
                    var fndThresholdFS = CalcThresholdFieldStrength(broadcastingAssignment[i].Target.Freq_MHz, broadcastingAssignment[i].AdmData.StnClass, typeThresholdFS, broadcastingAssignment[i].AdmData.IsDigital);
                    if (fndThresholdFS != null)
                    {
                        thresholdFS = fndThresholdFS.ThresholdFS;

                        var thresholdFieldStrength = new ThresholdFieldStrength()
                        {
                            Freq_MHz = broadcastingAssignment[i].Target.Freq_MHz,
                            StaClass = broadcastingAssignment[i].AdmData.StnClass,
                            System_type = fndThresholdFS.System_type,
                            ThresholdFS = thresholdFS,
                            IsDigital = broadcastingAssignment[i].AdmData.IsDigital,
                            Height_m = fndThresholdFS.Height_m,
                            Time_pc = fndThresholdFS.Time_pc,
                            MinFreq_MHz = fndThresholdFS.MinFreq_MHz,
                            MaxFreq_MHz = fndThresholdFS.MaxFreq_MHz
                        };
                        if (arrThresholdFieldStrength.Find(x => x.Freq_MHz == broadcastingAssignment[i].Target.Freq_MHz && x.StaClass == broadcastingAssignment[i].AdmData.StnClass && x.IsDigital == broadcastingAssignment[i].AdmData.IsDigital) == null)
                        {
                            arrThresholdFieldStrength.Add(thresholdFieldStrength);
                        }
                    }
                }
            }
            return arrThresholdFieldStrength.ToArray();
        }



        public static ThresholdFieldStrength[] GetThresholdFieldStrengthByFmtvTerra(FmtvTerra[] fmtvTerra, TypeThresholdFS typeThresholdFS)
        {
            var arrThresholdFieldStrength = new List<ThresholdFieldStrength>();
            float thresholdFS = -9999;
            ///список затронутых служб для Assignments
            if (fmtvTerra != null)
            {
                for (int i = 0; i < fmtvTerra.Length; i++)
                {
                    var fndThresholdFS = CalcThresholdFieldStrength(fmtvTerra[i].FreqAssgn_MHz, fmtvTerra[i].StnClass, typeThresholdFS, fmtvTerra[i].IsDigital=="TRUE" ? true : false);
                    if (fndThresholdFS != null)
                    {
                        thresholdFS = fndThresholdFS.ThresholdFS;

                        var thresholdFieldStrength = new ThresholdFieldStrength()
                        {
                            Freq_MHz = fmtvTerra[i].FreqAssgn_MHz,
                            System_type = fmtvTerra[i].System_type,
                            StaClass = fmtvTerra[i].StnClass,
                            ThresholdFS = thresholdFS,
                            IsDigital = fmtvTerra[i].IsDigital == "TRUE" ? true : false,
                            Height_m = fndThresholdFS.Height_m,
                            Time_pc = fndThresholdFS.Time_pc,
                            MinFreq_MHz = fndThresholdFS.MinFreq_MHz,
                            MaxFreq_MHz = fndThresholdFS.MaxFreq_MHz
                        };

                        if (arrThresholdFieldStrength.Find(x => x.Freq_MHz == fmtvTerra[i].FreqAssgn_MHz && x.StaClass == fmtvTerra[i].StnClass && x.System_type == fmtvTerra[i].System_type) == null)
                        {
                            arrThresholdFieldStrength.Add(thresholdFieldStrength);
                        }
                    }
                }
            }
            return arrThresholdFieldStrength.ToArray();
        }


        public static ThresholdFieldStrengthParameters CalcThresholdFieldStrength(double freq_MHz, string staClass, TypeThresholdFS typeThresholdFS, bool? isDigitalTemp = true)
        {
            ThresholdFieldStrengthParameters thresholdFieldStrengthParameters = null;
            bool isDigital = isDigitalTemp.GetValueOrDefault();
            if (typeThresholdFS== TypeThresholdFS.All)
            {
                thresholdFieldStrengthParameters = ThresholdFieldStrengthParameters.Find(x => x.MinFreq_MHz <= freq_MHz && x.MaxFreq_MHz >= freq_MHz && x.IsDigital == isDigital && x.StaClass == staClass);
            }
            else if (typeThresholdFS == TypeThresholdFS.OnlyBroadcastingService)
            {
                thresholdFieldStrengthParameters = ThresholdFieldStrengthParameters.Find(x => x.MinFreq_MHz <= freq_MHz && x.MaxFreq_MHz >= freq_MHz && x.IsDigital == isDigital && x.StaClass == staClass && (x.StaClass == "BT" || x.StaClass == "BC"));
            }
            return thresholdFieldStrengthParameters;
        }

        /// <summary>
        /// Таблица А.1.1
        /// Пороговые напряженности для защиты систем радиовещательной службы (первичные службы)
        /// </summary>
        public static void FillPrimaryServicesThresholdFSParameters()
        {
            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "DVB-T",
                StaClass = "BT",
                IsDigital = true,
                MinFreq_MHz = 174,
                MaxFreq_MHz = 230,
                ThresholdFS = 17,
                Height_m = 10
            });

            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "DVB-T",
                StaClass = "BT",
                IsDigital = true,
                MinFreq_MHz = 470,
                MaxFreq_MHz = 582,
                ThresholdFS = 21,
                Height_m = 10
            });

            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "DVB-T",
                StaClass = "BT",
                IsDigital = true,
                MinFreq_MHz = 582,
                MaxFreq_MHz = 718,
                ThresholdFS = 23,
                Height_m = 10
            });

            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "DVB-T",
                StaClass = "BT",
                IsDigital = true,
                MinFreq_MHz = 718,
                MaxFreq_MHz = 862,
                ThresholdFS = 25,
                Height_m = 10
            });


            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "Analog TV",
                StaClass = "BT",
                IsDigital = false,
                MinFreq_MHz = 174,
                MaxFreq_MHz = 230,
                ThresholdFS = 10,
                Height_m = 10
            });

            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "Analog TV",
                StaClass = "BT",
                IsDigital = false,
                MinFreq_MHz = 470,
                MaxFreq_MHz = 582,
                ThresholdFS = 18,
                Height_m = 10
            });

            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "Analog TV",
                StaClass = "BT",
                IsDigital = false,
                MinFreq_MHz = 582,
                MaxFreq_MHz = 718,
                ThresholdFS = 20,
                Height_m = 10
            });

            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "Analog TV",
                StaClass = "BT",
                IsDigital = false,
                MinFreq_MHz = 718,
                MaxFreq_MHz = 862,
                ThresholdFS = 22,
                Height_m = 10
            });

            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "T-DAB",
                StaClass = "BC",
                IsDigital = false,
                MinFreq_MHz = 174,
                MaxFreq_MHz = 230,
                ThresholdFS = 12,
                Height_m = 10
            });
        }


        /// <summary>
        /// Таблица А.1.2
        /// Пороговые напряженности поля, определяющие необходимость координации,
        /// для защиты систем в подвижной службе в полосе частот 174-230 МГц
        /// </summary>
        public static void FillFixedServicesTDABThresholdFSParameters()
        {
            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "T-DAB",
                StaClass = "MU",
                IsDigital = true,
                MinFreq_MHz = 174,
                MaxFreq_MHz = 230,
                ThresholdFS = 16,
                Height_m = 10
            });

            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "T-DAB",
                StaClass = "M1",
                IsDigital = true,
                MinFreq_MHz = 174,
                MaxFreq_MHz = 230,
                ThresholdFS = 19,
                Height_m = 20
            });

            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "T-DAB",
                StaClass = "M1",
                IsDigital = true,
                MinFreq_MHz = 174,
                MaxFreq_MHz = 230,
                ThresholdFS = 27,
                Height_m = 1.5f
            });

            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "T-DAB",
                StaClass = "RA",
                IsDigital = true,
                MinFreq_MHz = 174,
                MaxFreq_MHz = 230,
                ThresholdFS = 19,
                Height_m = 20
            });

            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "T-DAB",
                StaClass = "RA",
                IsDigital = true,
                MinFreq_MHz = 174,
                MaxFreq_MHz = 230,
                ThresholdFS = 27,
                Height_m = 1.5f
            });

            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "T-DAB",
                StaClass = "M2",
                IsDigital = true,
                MinFreq_MHz = 174,
                MaxFreq_MHz = 230,
                ThresholdFS = 48,
                Height_m = 10.0f
            });

            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "T-DAB",
                StaClass = "XA",
                IsDigital = true,
                MinFreq_MHz = 174,
                MaxFreq_MHz = 230,
                ThresholdFS = 27,
                Height_m = 10.0f
            });

            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "T-DAB",
                StaClass = "XM",
                IsDigital = true,
                MinFreq_MHz = 174,
                MaxFreq_MHz = 230,
                ThresholdFS = 30,
                Height_m = 10.0f
            });

            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "T-DAB",
                StaClass = "MA",
                IsDigital = true,
                MinFreq_MHz = 174,
                MaxFreq_MHz = 230,
                ThresholdFS = 21,
                Height_m = 10.0f
            });

            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "T-DAB",
                StaClass = "MT",
                IsDigital = true,
                MinFreq_MHz = 174,
                MaxFreq_MHz = 230,
                ThresholdFS = 5,
                Height_m = 10.0f
            });
        }

        /// <summary>
        /// Таблица А.1.3
        /// Пороговые напряженности поля, определяющие необходимость координации, для
        /// защиты систем подвижной службы от DVB-T
        /// </summary>
        public static void FillMobileServicesThresholdFSParameters()
        {
            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "DVB-T",
                StaClass = "NV",
                MinFreq_MHz = 174,
                MaxFreq_MHz = 230,
                ThresholdFS = 30,
                IsBaseStation = true,
                Height_m = 20
            });
            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "DVB-T",
                StaClass = "NV",
                MinFreq_MHz = 174,
                MaxFreq_MHz = 230,
                ThresholdFS = 38,
                IsBaseStation = false,
                Height_m = 1.5f
            });

            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "DVB-T",
                StaClass = "NR",
                MinFreq_MHz = 790,
                MaxFreq_MHz = 862,
                ThresholdFS = 58,
                IsBaseStation = true,
                Height_m = 1.5f
            });
            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "DVB-T",
                StaClass = "NR",
                MinFreq_MHz = 174,
                MaxFreq_MHz = 230,
                ThresholdFS = 50,
                IsBaseStation = false,
                Height_m = 1.5f
            });

            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "DVB-T",
                StaClass = "NS",
                MinFreq_MHz = 790,
                MaxFreq_MHz = 862,
                ThresholdFS = 45,
                IsBaseStation = true,
                Height_m = 10.0f
            });
            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "DVB-T",
                StaClass = "NS",
                MinFreq_MHz = 174,
                MaxFreq_MHz = 230,
                ThresholdFS = 37,
                IsBaseStation = false,
                Height_m = 10.0f
            });

            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "DVB-T",
                StaClass = "NT",
                MinFreq_MHz = 790,
                MaxFreq_MHz = 862,
                ThresholdFS = 47,
                IsBaseStation = true,
                Height_m = 1.5f
            });
            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "DVB-T",
                StaClass = "NT",
                MinFreq_MHz = 174,
                MaxFreq_MHz = 230,
                ThresholdFS = 39,
                IsBaseStation = false,
                Height_m = 1.5f
            });


            //ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            //{
            //    Standard = "DVB-T",
            //    StaClass = "NA",
            //    MinFreq_MHz = 470,
            //    MaxFreq_MHz = 862,
            //    ThresholdFS = 18,
            //    IsBaseStation = true,
            //    Height_m = 20.0f
            //});

            //ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            //{
            //    Standard = "DVB-T",
            //    StaClass = "NA",
            //    MinFreq_MHz = 790,
            //    MaxFreq_MHz = 862,
            //    ThresholdFS = 18,
            //    IsBaseStation = true,
            //    Height_m = 20.0f
            //});

            //ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            //{
            //    Standard = "DVB-T",
            //    StaClass = "NB",
            //    MinFreq_MHz = 174,
            //    MaxFreq_MHz = 230,
            //    ThresholdFS = -9999, ///??????????????? расчет по формуле
            //    IsBaseStation = true,
            //    Height_m = 20.0f
            //});

            //ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            //{
            //    Standard = "DVB-T",
            //    StaClass = "NB",
            //    MinFreq_MHz = 470,
            //    MaxFreq_MHz = 862,
            //    ThresholdFS = -9999, ///??????????????? расчет по формуле
            //    IsBaseStation = true,
            //    Height_m = 20.0f
            //});


            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "DVB-T",
                StaClass = "XN",
                MinFreq_MHz = 174,
                MaxFreq_MHz = 230,
                ThresholdFS = 38,
                IsBaseStation = true,
                Height_m = 1.5f
            });

            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "DVB-T",
                StaClass = "YN",
                MinFreq_MHz = 480,
                MaxFreq_MHz = 480,
                ThresholdFS = 41,
                IsBaseStation = true,
                Height_m = 1.5f
            });

            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "DVB-T",
                StaClass = "ZC",
                MinFreq_MHz = 620,
                MaxFreq_MHz = 620,
                ThresholdFS = 43,
                IsBaseStation = true,
                Height_m = 1.5f
            });


        }

        /// <summary>
        /// Таблица А.1.6
        /// Пороговые напряженности поля, определяющие необходимость координации,
        /// для защиты радионавигационной  и воздушной радионавигационных служб от DVB-T
        /// </summary>
        public static void FillRadioNavigationServicesThresholdFSParameters()
        {
            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "DVB-T",
                StaClass = "XG",
                MinFreq_MHz = 590,
                MaxFreq_MHz = 598,
                ThresholdFS = -12,
                IsBaseStation = true,
                Height_m = 7
            });
            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "DVB-T",
                StaClass = "AB",
                MinFreq_MHz = 585,
                MaxFreq_MHz = 610,
                ThresholdFS = 13,
                IsBaseStation = true,
                Height_m = 10
            });
            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "DVB-T",
                StaClass = "AA8",
                MinFreq_MHz = 645,
                MaxFreq_MHz = 862,
                ThresholdFS = 36,
                IsBaseStation = true,
                Height_m = 10
            });
            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "DVB-T",
                StaClass = "AA8",
                MinFreq_MHz = 645,
                MaxFreq_MHz = 862,
                ThresholdFS = 42,
                IsBaseStation = true,
                Height_m = 10000
            });
            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "DVB-T",
                StaClass = "AB",
                MinFreq_MHz = 645,
                MaxFreq_MHz = 862,
                ThresholdFS = 13,
                IsBaseStation = true,
                Height_m = 10
            });
            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "DVB-T",
                StaClass = "BD",
                MinFreq_MHz = 645,
                MaxFreq_MHz = 862,
                ThresholdFS = 49,
                IsBaseStation = true,
                Height_m = 10000
            });
            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                Standard = "DVB-T",
                StaClass = "BA",
                MinFreq_MHz = 645,
                MaxFreq_MHz = 862,
                ThresholdFS = 29,
                IsBaseStation = true,
                Height_m = 10
            });
        }

        /// <summary>
        /// Таблица А.1.7
        /// Значения пороговой напряженности поля, определяющие необходимость координации,
        /// для защиты систем фиксированной службы от 
        /// </summary>
        public static void FillFixedServicesThresholdFSParameters()
        {
            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                StaClass = "FF",
                MinFreq_MHz = 790,
                MaxFreq_MHz = 862,
                ThresholdFS = 24,
                IsBaseStation = true,
                Height_m = 37.5f
            });

            ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            {
                StaClass = "FH",
                MinFreq_MHz = 790,
                MaxFreq_MHz = 862,
                ThresholdFS = 13,
                IsBaseStation = true,
                Height_m = 37.5f
            });

            //ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            //{
            //    StaClass = "FK",
            //    MinFreq_MHz = 174,
            //    MaxFreq_MHz = 230,
            //    ThresholdFS = -9999, // необходимость применения формулы
            //    IsBaseStation = true,
            //    Height_m = 37.5f
            //});

            //ThresholdFieldStrengthParameters.Add(new ThresholdFieldStrengthParameters()
            //{
            //    StaClass = "FK",
            //    MinFreq_MHz = 470,
            //    MaxFreq_MHz = 862,
            //    ThresholdFS = -9999, // необходимость применения формулы
            //    IsBaseStation = true,
            //    Height_m = 37.5f
            //});
        }
    }
}
