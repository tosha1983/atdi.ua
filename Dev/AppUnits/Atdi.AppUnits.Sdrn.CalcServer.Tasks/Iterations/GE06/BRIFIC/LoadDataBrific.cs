using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows;
using Atdi.DataModels.Sdrn.CalcServer.Internal.Iterations;
using Atdi.Modules.CalcServer.BrificDataLayer;

namespace Atdi.AppUnits.Sdrn.CalcServer.Tasks.Iterations
{

    /// <summary>
    /// Загрузка данных с БД BRIFIC
    /// </summary>
    public static class LoadDataBrific
    {


        public static void SetBRIFICDirectory(string fileName)
        {
            DbSqLite.SetBrificDirectory(fileName);
        }

        /// Системы радиовещательной службы
        /// DVB-T 
        /// 
        public static FmtvTerra[] LoadBroadcastingService_TDAB(string adm, double freq)
        {
            return DbSqLite.LoadBroadcastingService_TDAB(adm, freq);
        }

        /// <summary>
        /// Системы радиовещательной службы
        /// T-DAB 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static FmtvTerra[] LoadBroadcastingService_DVBT(string adm, double freq)
        {
            return DbSqLite.LoadBroadcastingService_DVBT(adm, freq);
        }

        //
        /// Системы радиовещательной службы
        /// Аналоговое ТВ 
        /// 
        public static FmtvTerra[] LoadBroadcastingServiceAnalog_TV(string adm, double freq)
        {
            return DbSqLite.LoadBroadcastingServiceAnalog_TV(adm, freq);
        }







        ///	Системы подвижной службы 
        ///	NV
        public static FmtvTerra[] LoadBroadcastingService_NV(string adm, double freq)
        {
            return DbSqLite.LoadBroadcastingService_NV(adm, freq);
        }

        ///	Системы подвижной службы 
        ///	NR
        public static FmtvTerra[] LoadBroadcastingService_NR(string adm, double freq)
        {
            return DbSqLite.LoadBroadcastingService_NR(adm, freq);
        }

        ///	Системы подвижной службы 
        ///	NS
        public static FmtvTerra[] LoadBroadcastingService_NS(string adm, double freq)
        {
            return DbSqLite.LoadBroadcastingService_NS(adm, freq);
        }

        ///	Системы подвижной службы 
        ///	NT
        public static FmtvTerra[] LoadBroadcastingService_NT(string adm, double freq)
        {
            return DbSqLite.LoadBroadcastingService_NT(adm, freq);
        }

        ///	Системы подвижной службы 
        ///	NA
        public static FmtvTerra[] LoadBroadcastingService_NA(string adm, double freq)
        {
            return DbSqLite.LoadBroadcastingService_NA(adm, freq);
        }

        ///	Системы подвижной службы 
        ///	NB
        public static FmtvTerra[] LoadBroadcastingService_NB(string adm, double freq)
        {
            return DbSqLite.LoadBroadcastingService_NB(adm, freq);
        }

        ///	Системы подвижной службы 
        ///	XN
        public static FmtvTerra[] LoadBroadcastingService_XN(string adm, double freq)
        {
            return DbSqLite.LoadBroadcastingService_XN(adm, freq);
        }

        ///	Системы подвижной службы 
        ///	YN
        public static FmtvTerra[] LoadBroadcastingService_YN(string adm, double freq)
        {
            return DbSqLite.LoadBroadcastingService_YN(adm, freq);
        }

        ///	Системы подвижной службы 
        ///	ZC
        public static FmtvTerra[] LoadBroadcastingService_ZC(string adm, double freq)
        {
            return DbSqLite.LoadBroadcastingService_ZC(adm, freq);
        }







        ///	Системы радионавигационной и воздушной радионавигационной служб
        ///	XG
        public static FmtvTerra[] LoadNavigationServices_XG(string adm, double freq)
        {
            return DbSqLite.LoadNavigationServices_XG(adm, freq);
        }

        ///	Системы радионавигационной и воздушной радионавигационной служб
        ///	AB
        public static FmtvTerra[] LoadNavigationServices_AB(string adm, double freq)
        {
            return DbSqLite.LoadNavigationServices_AB(adm, freq);
        }

        ///	Системы радионавигационной и воздушной радионавигационной служб
        ///	AA8
        public static FmtvTerra[] LoadNavigationServices_AA8(string adm, double freq)
        {
            return DbSqLite.LoadNavigationServices_AA8(adm, freq);
        }

        ///	Системы радионавигационной и воздушной радионавигационной служб
        ///	BD
        public static FmtvTerra[] LoadNavigationServices_BD(string adm, double freq)
        {
            return DbSqLite.LoadNavigationServices_BD(adm, freq);
        }

        ///	Системы радионавигационной и воздушной радионавигационной служб
        ///	BA
        public static FmtvTerra[] LoadNavigationServices_BA(string adm, double freq)
        {
            return DbSqLite.LoadNavigationServices_BA(adm, freq);
        }







        ///	Системы фиксированной службы
        ///	FF
        public static FmtvTerra[] LoadFixedServices_FF(string adm, double freq)
        {
            return DbSqLite.LoadFixedServices_FF(adm, freq);
        }


        ///	Системы фиксированной службы
        ///	FN
        public static FmtvTerra[] LoadFixedServices_FN(string adm, double freq)
        {
            return DbSqLite.LoadFixedServices_FN(adm, freq);
        }

        ///	Системы фиксированной службы
        ///	FK
        public static FmtvTerra[] LoadFixedServices_FK(string adm, double freq)
        {
            return DbSqLite.LoadFixedServices_FK(adm, freq);
        }








        //
        /// Системы подвижной службы
        /// MU
        /// 
        public static FmtvTerra[] LoadMobileServices_MU(string adm, double freq)
        {
            return DbSqLite.LoadMobileServices_MU(adm, freq);
        }

        //
        /// Системы подвижной службы
        /// M1 и RA
        /// 
        public static FmtvTerra[] LoadMobileServices_M1_RA(string adm, double freq)
        {
            return DbSqLite.LoadMobileServices_M1_RA(adm, freq);
        }


        //
        /// Системы подвижной службы
        /// M2
        /// 
        public static FmtvTerra[] LoadMobileServices_M2(string adm, double freq)
        {
            return DbSqLite.LoadMobileServices_M2(adm, freq);
        }

        //
        /// Системы подвижной службы
        /// XA
        /// 
        public static FmtvTerra[] LoadMobileServices_XA(string adm, double freq)
        {
            return DbSqLite.LoadMobileServices_XA(adm, freq);
        }

        //
        /// Системы подвижной службы
        /// XM
        /// 
        public static FmtvTerra[] LoadMobileServices_XM(string adm, double freq)
        {
            return DbSqLite.LoadMobileServices_XM(adm, freq);
        }

        //
        /// Системы подвижной службы
        /// MA
        /// 
        public static FmtvTerra[] LoadMobileServices_MA(string adm, double freq)
        {
            return DbSqLite.LoadMobileServices_MA(adm, freq);
        }

        //
        /// Системы подвижной службы
        /// MT
        /// 
        public static FmtvTerra[] LoadMobileServices_MT(string adm, double freq)
        {
            return DbSqLite.LoadMobileServices_MT(adm, freq);
        }

    }
}
