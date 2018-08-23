//-------------------------------------------------------------------------
//FILE GENERATED BY ICS MANAGER FROM SCHEMA 'SensorPlugin' DON'T MODIFY IT;
//-------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Reflection;
using Oracle.DataAccess.Client;

namespace Atdi.Oracle.DataAccess
{
    public class YXbsSensor : Yyy
    {
        public YXbsSensor()
        {
            TableName = "XBS_SENSOR";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsSensor)));
        }
        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public int? m_sensoridentifier_id { get { return getInt(1); } set { setInt(1, value); } }
        public string m_status { get { return getString(2); } set { setString(2, 25, value); } }
        public string m_name { get { return getString(3); } set { setString(3, 150, value); } }
        public string m_administration { get { return getString(4); } set { setString(4, 50, value); } }
        public string m_networkid { get { return getString(5); } set { setString(5, 150, value); } }
        public string m_remark { get { return getString(6); } set { setString(6, 250, value); } }
        public DateTime? m_biusedate { get { return getDateTime(7); } set { setDateTime(7, value); } }
        public DateTime? m_eousedate { get { return getDateTime(8); } set { setDateTime(8, value); } }
        public double? m_azimuth { get { return getDouble(9); } set { setDouble(9, value); } }
        public double? m_elevation { get { return getDouble(10); } set { setDouble(10, value); } }
        public double? m_agl { get { return getDouble(11); } set { setDouble(11, value); } }
        public string m_idsysargus { get { return getString(12); } set { setString(12, 50, value); } }
        public string m_typesensor { get { return getString(13); } set { setString(13, 50, value); } }
        public double? m_stepmeastime { get { return getDouble(14); } set { setDouble(14, value); } }
        public double?  m_rxloss { get { return getDouble(15); } set { setDouble(15, value); } }
        public double? m_ophhfr { get { return getDouble(16); } set { setDouble(16, value); } }
        public double? m_ophhto { get { return getDouble(17); } set { setDouble(17, value); } }
        public string m_opdays { get { return getString(18); } set { setString(18, 50, value); } }
        public string m_custtxt1 { get { return getString(19); } set { setString(19, 50, value); } }
        public DateTime? m_custdata1 { get { return getDateTime(20); } set { setDateTime(20, value); } }
        public double? m_custnbr1 { get { return getDouble(21); } set { setDouble(21, value); } }
        public DateTime? m_datecreated { get { return getDateTime(22); } set { setDateTime(22, value); } }
        public string m_createdby { get { return getString(23); } set { setString(23, 50, value); } }
    }

    public class YXbsSensorantenna : Yyy
    {
        public YXbsSensorantenna()
        {
            TableName = "XBS_SENSORANTENNA";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsSensorantenna)));
        }
        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public int? m_sensorid { get { return getInt(1); } set { setInt(1, value); } }
        public string m_code { get { return getString(2); } set { setString(2, 50, value); } }
        public double? m_slewang { get { return getDouble(3); } set { setDouble(3, value); } }
        public string m_manufacturer { get { return getString(4); } set { setString(4, 50, value); } }
        public string m_name { get { return getString(5); } set { setString(5, 50, value); } }
        public string m_techid { get { return getString(6); } set { setString(6, 50, value); } }
        public string m_antdir { get { return getString(7); } set { setString(7, 50, value); } }
        public double? m_hbeamwidth { get { return getDouble(8); } set { setDouble(8, value); } }
        public double? m_vbeamwidth { get { return getDouble(9); } set { setDouble(9, value); } }
        public string m_polarization { get { return getString(10); } set { setString(10, 50, value); } }
        public string m_usetype { get { return getString(11); } set { setString(11, 50, value); } }
        public string m_category { get { return getString(12); } set { setString(12, 50, value); } }
        public string m_gaintype { get { return getString(13); } set { setString(13, 50, value); } }
        public double? m_gainmax { get { return getDouble(14); } set { setDouble(14, value); } }
        public double? m_lowerfreq { get { return getDouble(15); } set { setDouble(15, value); } }
        public double? m_upperfreq { get { return getDouble(16); } set { setDouble(16, value); } }
        public double? m_addloss { get { return getDouble(17); } set { setDouble(17, value); } }
        public double? m_xpd { get { return getDouble(18); } set { setDouble(18, value); } }
        public string m_antclass { get { return getString(19); } set { setString(19, 50, value); } }
        public string m_remark { get { return getString(20); } set { setString(20, 250, value); } }
        public string m_custtxt1 { get { return getString(21); } set { setString(21, 250, value); } }
        public string m_custdata1 { get { return getString(22); } set { setString(22, 250, value); } }
        public double? m_custnbr1 { get { return getDouble(23); } set { setDouble(23, value); } }
    }

    public class YXbsSensorequip : Yyy
    {
        public YXbsSensorequip()
        {
            TableName = "XBS_SENSOREQUIP";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsSensorequip)));
        }
        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public int? m_sensorid { get { return getInt(1); } set { setInt(1, value); } }
        public string m_code { get { return getString(2); } set { setString(2, 50, value); } }
        public string m_manufacturer { get { return getString(3); } set { setString(3, 50, value); } }
        public string m_name { get { return getString(4); } set { setString(4, 50, value); } }
        public string m_family { get { return getString(5); } set { setString(5, 50, value); } }
        public string m_techid { get { return getString(6); } set { setString(6, 50, value); } }
        public string m_version { get { return getString(7); } set { setString(7, 50, value); } }
        public double? m_lowerfreq { get { return getDouble(8); } set { setDouble(8, value); } }
        public double? m_upperfreq { get { return getDouble(9); } set { setDouble(9, value); } }
        public double? m_rbwmin { get { return getDouble(10); } set { setDouble(10, value); } }
        public double? m_rbwmax { get { return getDouble(11); } set { setDouble(11, value); } }
        public double? m_vbwmin { get { return getDouble(12); } set { setDouble(12, value); } }
        public double? m_vbwmax { get { return getDouble(13); } set { setDouble(13, value); } }
        public int? m_mobility { get { return getInt(14); } set { setInt(14, value); } }
        public double? m_fftpointmax { get { return getDouble(15); } set { setDouble(15, value); } }
        public double? m_refleveldbm { get { return getDouble(16); } set { setDouble(16, value); } }
        public string m_operationmode { get { return getString(17); } set { setString(17, 50, value); } }
        public string m_type { get { return getString(18); } set { setString(18, 50, value); } }
        public string m_equipclass { get { return getString(19); } set { setString(19, 50, value); } }
        public double? m_tuningstep { get { return getDouble(20); } set { setDouble(20, value); } }
        public string m_usetype { get { return getString(21); } set { setString(21, 50, value); } }
        public string m_category { get { return getString(22); } set { setString(22, 50, value); } }
        public string m_remark { get { return getString(23); } set { setString(23, 250, value); } }
        public string m_custtxt1 { get { return getString(24); } set { setString(24, 250, value); } }
        public DateTime? m_custdata1 { get { return getDateTime(25); } set { setDateTime(25, value); } }
        public double? m_custnbr1 { get { return getDouble(26); } set { setDouble(26, value); } }
    }

    public class YXbsSensorequipsens : Yyy
    {
        public YXbsSensorequipsens()
        {
            TableName = "XBS_SENSOREQUIPSENS";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsSensorequipsens)));
        }
        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public int? m_sensorequip_id { get { return getInt(1); } set { setInt(1, value); } }
        public double? m_freq { get { return getDouble(2); } set { setDouble(2, value); } }
        public double? m_ktbf { get { return getDouble(3); } set { setDouble(3, value); } }
        public double? m_noisef { get { return getDouble(4); } set { setDouble(4, value); } }
        public double? m_freqstability { get { return getDouble(5); } set { setDouble(5, value); } }
        public double? m_addloss { get { return getDouble(6); } set { setDouble(6, value); } }
    }

    public class YXbsSensorlocation : Yyy
    {
        public YXbsSensorlocation()
        {
            TableName = "XBS_SENSORLOCATION";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsSensorlocation)));
        }
        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public int? m_sensorid { get { return getInt(1); } set { setInt(1, value); } }
        public DateTime? m_datafrom { get { return getDateTime(2); } set { setDateTime(2, value); } }
        public DateTime? m_datato { get { return getDateTime(3); } set { setDateTime(3, value); } }
        public DateTime? m_datacreated { get { return getDateTime(4); } set { setDateTime(4, value); } }
        public string m_status { get { return getString(5); } set { setString(5, 25, value); } }
        public double? m_lon { get { return getDouble(6); } set { setDouble(6, value); } }
        public double? m_lat { get { return getDouble(7); } set { setDouble(7, value); } }
        public double? m_asl { get { return getDouble(8); } set { setDouble(8, value); } }
    }

    public class YXbsAntennapattern : Yyy
    {
        public YXbsAntennapattern()
        {
            TableName = "XBS_ANTENNAPATTERN";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsAntennapattern)));
        }
        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public int? m_sensorantenna_id { get { return getInt(1); } set { setInt(1, value); } }
        public double? m_freq { get { return getDouble(2); } set { setDouble(2, value); } }
        public double? m_gain { get { return getDouble(3); } set { setDouble(3, value); } }
        public string m_diaga { get { return getString(4); } set { setString(4, 1000, value); } }
        public string m_diagh { get { return getString(5); } set { setString(5, 1000, value); } }
        public string m_diagv { get { return getString(6); } set { setString(6, 1000, value); } }
    }

    public class YXbsSensorpolig : Yyy
    {
        public YXbsSensorpolig()
        {
            TableName = "XBS_SENSORPOLIG";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsSensorpolig)));
        }
        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public int? m_sensorid { get { return getInt(1); } set { setInt(1, value); } }
        public double? m_lon { get { return getDouble(2); } set { setDouble(2, value); } }
        public double? m_lat { get { return getDouble(3); } set { setDouble(3, value); } }
    }

    public class YXbsMeastask : Yyy
    {
        public YXbsMeastask()
        {
            TableName = "XBS_MEASTASK";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsMeastask)));
        }
        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public string m_status { get { return getString(1); } set { setString(1, 50, value); } }
        public int? m_orderid { get { return getInt(2); } set { setInt(2, value); } }
        public string m_type { get { return getString(3); } set { setString(3, 50, value); } }
        public string m_name { get { return getString(4); } set { setString(4, 100, value); } }
        public string m_executionmode { get { return getString(5); } set { setString(5, 50, value); } }
        public string m_task { get { return getString(6); } set { setString(6, 50, value); } }
        public int? m_prio { get { return getInt(7); } set { setInt(7, value); } }
        public string m_resulttype { get { return getString(8); } set { setString(8, 50, value); } }
        public int? m_maxtimebs { get { return getInt(9); } set { setInt(9, value); } }
        public DateTime? m_datecreated { get { return getDateTime(10); } set { setDateTime(10, value); } }
        public string m_createdby { get { return getString(11); } set { setString(11, 50, value); } }
        public string m_id_start { get { return getString(12); } set { setString(12, 50, value); } }
    }

    public class YXbsStationdatform : Yyy
    {
        public YXbsStationdatform()
        {
            TableName = "XBS_STATIONDATFORM";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsStationdatform)));
        }
        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public string m_globalsid { get { return getString(1); } set { setString(1, 50, value); } }
        public string m_status { get { return getString(2); } set { setString(2, 20, value); } }
        public string m_standart { get { return getString(3); } set { setString(3, 50, value); } }
        public int? m_id_xbs_meastask { get { return getInt(4); } set { setInt(4, value); } }

    }

    public class YXbsMeassubtask : Yyy
    {
        public YXbsMeassubtask()
        {
            TableName = "XBS_MEASSUBTASK";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsMeassubtask)));
        }

        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public DateTime? m_timestart { get { return getDateTime(1); } set { setDateTime(1, value); } }
        public DateTime? m_timestop { get { return getDateTime(2); } set { setDateTime(2, value); } }
        public string m_status { get { return getString(3); } set { setString(3, 50, value); } }
        public int? m_interval { get { return getInt(4); } set { setInt(4, value); } }
        public int? m_id_xbs_meastask { get { return getInt(5); } set { setInt(5, value); } }
    }

    public class YXbsMeasstation : Yyy
    {
        public YXbsMeasstation()
        {
            TableName = "XBS_MEASSTATION";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsMeasstation)));
        }
        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public int? m_stationid { get { return getInt(1); } set { setInt(1, value); } }
        public string m_stationtype { get { return getString(2); } set { setString(2, 50, value); } }
        public int? m_id_xbs_meastask { get { return getInt(3); } set { setInt(3, value); } }
    }

    public class YXbsMeassubtasksta : Yyy
    {
        public YXbsMeassubtasksta()
        {
            TableName = "XBS_MEASSUBTASKSTA";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsMeassubtasksta)));
        }


        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public string m_status { get { return getString(1); } set { setString(1, 50, value); } }
        public int? m_count { get { return getInt(2); } set { setInt(2, value); } }
        public DateTime? m_timenexttask { get { return getDateTime(3); } set { setDateTime(3, value); } }
        public int? m_id_xbs_sensor { get { return getInt(4); } set { setInt(4, value); } }
        public int? m_id_xb_meassubtask { get { return getInt(5); } set { setInt(5, value); } }
    }

    public class YXbsMeasother : Yyy
    {
        public YXbsMeasother()
        {
            TableName = "XBS_MEASOTHER";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsMeasother)));
        }


        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public int? m_swnumber { get { return getInt(1); } set { setInt(1, value); } }
        public string m_typespectrumscan { get { return getString(2); } set { setString(2, 50, value); } }
        public string m_typespectrumoccupation { get { return getString(3); } set { setString(3, 50, value); } }
        public double? m_levelminoccup { get { return getDouble(4); } set { setDouble(4, value); } }
        public int? m_nchenal { get { return getInt(5); } set { setInt(5, value); } }
        public int? m_id_xbs_meastask { get { return getInt(6); } set { setInt(6, value); } }
    }

    public class YXbsMeastimeparaml : Yyy
    {

        public YXbsMeastimeparaml()
        {
            TableName = "XBS_MEASTIMEPARAML";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsMeastimeparaml)));
        }
        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public DateTime? m_perstart { get { return getDateTime(1); } set { setDateTime(1, value); } }
        public DateTime? m_perstop { get { return getDateTime(2); } set { setDateTime(2, value); } }
        public DateTime? m_timestart { get { return getDateTime(3); } set { setDateTime(3, value); } }
        public DateTime? m_timestop { get { return getDateTime(4); } set { setDateTime(4, value); } }
        public string m_days { get { return getString(5); } set { setString(5, 250, value); } }
        public double? m_perinterval { get { return getDouble(6); } set { setDouble(6, value); } }
        public int? m_id_xbs_meastask { get { return getInt(7); } set { setInt(7, value); } }
    }

    public class YXbsMeaslocparam : Yyy
    {
        public YXbsMeaslocparam()
        {
            TableName = "XBS_MEASLOCPARAM";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsMeaslocparam)));
        }


        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public double? m_lon { get { return getDouble(1); } set { setDouble(1, value); } }
        public double? m_lat { get { return getDouble(2); } set { setDouble(2, value); } }
        public double? m_asl { get { return getDouble(3); } set { setDouble(3, value); } }
        public double? m_maxdist { get { return getDouble(4); } set { setDouble(4, value); } }
        public int? m_id_xbs_meastask { get { return getInt(5); } set { setInt(5, value); } }
    }

    public class YXbsMeasdtparam : Yyy
    {
        public YXbsMeasdtparam()
        {
            TableName = "XBS_MEASDTPARAM";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsMeasdtparam)));
        }


        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public string m_typemeasurements { get { return getString(1); } set { setString(1, 50, value); } }
        public string m_detecttype { get { return getString(2); } set { setString(2, 50, value); } }
        public double? m_rfattenuation { get { return getDouble(3); } set { setDouble(3, value); } }
        public double? m_ifattenuation { get { return getDouble(4); } set { setDouble(4, value); } }
        public double? m_meastime { get { return getDouble(5); } set { setDouble(5, value); } }
        public string m_demod { get { return getString(6); } set { setString(6, 50, value); } }
        public int? m_preamplification { get { return getInt(7); } set { setInt(7, value); } }
        public string m_mode { get { return getString(8); } set { setString(8, 50, value); } }
        public double? m_rbw { get { return getDouble(9); } set { setDouble(9, value); } }
        public double? m_vbw { get { return getDouble(10); } set { setDouble(10, value); } }
        public int? m_id_xbs_meastask { get { return getInt(11); } set { setInt(11, value); } }
    }

    public class YXbsMeasfreqparam : Yyy
    {
        public YXbsMeasfreqparam()
        {
            TableName = "XBS_MEASFREQPARAM";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsMeasfreqparam)));
        }

        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public string m_mode { get { return getString(1); } set { setString(1, 50, value); } }
        public double? m_rgl { get { return getDouble(2); } set { setDouble(2, value); } }
        public double? m_rgu { get { return getDouble(3); } set { setDouble(3, value); } }
        public double? m_step { get { return getDouble(4); } set { setDouble(4, value); } }
        public int? m_id_xbs_meastask { get { return getInt(5); } set { setInt(5, value); } }
    }

    public class YXbsMeasfreq : Yyy
    {
        public YXbsMeasfreq()
        {
            TableName = "XBS_MEASFREQ";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsMeasfreq)));
        }

        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public double? m_freq { get { return getDouble(1); } set { setDouble(1, value); } }
        public int? m_id_xbs_measfreqparam { get { return getInt(2); } set { setInt(2, value); } }
    }

    public class YXbsOwnerdata : Yyy
    {
        public YXbsOwnerdata()
        {
            TableName = "XBS_OWNERDATA";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsOwnerdata)));
        }

        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public string m_ownername { get { return getString(1); } set { setString(1, 100, value); } }
        public string m_okpo { get { return getString(2); } set { setString(2, 50, value); } }
        public string m_zip { get { return getString(3); } set { setString(3, 50, value); } }
        public string m_code { get { return getString(4); } set { setString(4, 50, value); } }
        public string m_addres { get { return getString(5); } set { setString(5, 2000, value); } }
        public int? m_id_stationdatform { get { return getInt(6); } set { setInt(6, value); } }
    }

    public class YXbsSitestformeas : Yyy
    {
        public YXbsSitestformeas()
        {
            TableName = "XBS_SITESTFORMEAS";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsSitestformeas)));
        }

        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public double? m_lon { get { return getDouble(1); } set { setDouble(1, value); } }
        public double? m_lat { get { return getDouble(2); } set { setDouble(2, value); } }
        public string m_addres { get { return getString(3); } set { setString(3, 2000, value); } }
        public string m_region { get { return getString(4); } set { setString(4, 50, value); } }
        public int?  m_id_stationdatform { get { return getInt(5); } set { setInt(5, value); } }
    }

    public class YXbsSectstformeas : Yyy
    {

        public YXbsSectstformeas()
        {
            TableName = "XBS_SECTSTFORMEAS";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsSectstformeas)));
        }


        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public double? m_agl { get { return getDouble(1); } set { setDouble(1, value); } }
        public double? m_eirp { get { return getDouble(2); } set { setDouble(2, value); } }
        public double? m_azimut { get { return getDouble(3); } set { setDouble(3, value); } }
        public double? m_bw { get { return getDouble(4); } set { setDouble(4, value); } }
        public string m_classemission { get { return getString(5); } set { setString(5, 20, value); } }
        public int? m_id_stationdatform { get { return getInt(6); } set { setInt(6, value); } }
    }

    public class YXbsPermassign : Yyy
    {

        public YXbsPermassign()
        {
            TableName = "XBS_PERMASSIGN";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsPermassign)));
        }


        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public DateTime? m_startdate { get { return getDateTime(1); } set { setDateTime(1, value); } }
        public DateTime? m_enddate { get { return getDateTime(2); } set { setDateTime(2, value); } }
        public DateTime? m_closedate { get { return getDateTime(3); } set { setDateTime(3, value); } }
        public string m_dozvilname { get { return getString(4); } set { setString(4, 100, value); } }
        public int? m_id_stationdatform { get { return getInt(5); } set { setInt(5, value); } }

    }

    public class YXbsMaskelements : Yyy
    {
        public YXbsMaskelements()
        {
            TableName = "XBS_MASKELEMENTS";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsMaskelements)));
        }
        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public double? m_level { get { return getDouble(1); } set { setDouble(1, value); } }
        public double? m_bw { get { return getDouble(2); } set { setDouble(2, value); } }
        public int? m_id_sectstformeas { get { return getInt(3); } set { setInt(3, value); } }
    }

    public class YXbsFreqforsectics : Yyy
    {
        public YXbsFreqforsectics()
        {
            TableName = "XBS_FREQFORSECTICS";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsFreqforsectics)));
        }

        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public int? m_idplan { get { return getInt(1); } set { setInt(1, value); } }
        public int? m_channalnumber { get { return getInt(2); } set { setInt(2, value); } }
        public double? m_frequency { get { return getDouble(3); } set { setDouble(3, value); } }
        public int? m_id_sectstformeas { get { return getInt(4); } set { setInt(4, value); } }
    }

    public class YXbsMeasurementres : Yyy
    {

        public YXbsMeasurementres()
        {
            TableName = "XBS_MEASUREMENTRES";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsMeasurementres)));
        }

        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public int? m_meastaskid { get { return getInt(1); } set { setInt(1, value); } }
        public int? m_submeastaskid { get { return getInt(2); } set { setInt(2, value); } }
        public int? m_submeastaskstationid { get { return getInt(3); } set { setInt(3, value); } }
        public int? m_sensorid { get { return getInt(4); } set { setInt(4, value); } }
        public double? m_antval { get { return getDouble(5); } set { setDouble(5, value); } }
        public DateTime? m_timemeas { get { return getDateTime(6); } set { setDateTime(6, value); } }
        public int? m_datarank { get { return getInt(7); } set { setInt(7, value); } }
        public int? m_n { get { return getInt(8); } set { setInt(8, value); } }
        public string m_status { get { return getString(9); } set { setString(9, 50, value); } }
        public string m_typemeasurements { get { return getString(10); } set { setString(10, 50, value); } }
    }

    public class YXbsStationmeas : Yyy
    {
        public YXbsStationmeas()
        {
            TableName = "XBS_STATIONMEAS";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsStationmeas)));
        }

        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public int? m_id_xbs_sensor { get { return getInt(1); } set { setInt(1, value); } }
        public int? m_id_xbs_measurementres { get { return getInt(2); } set { setInt(2, value); } }
    }

    public class YXbsLocationsensorm : Yyy
    {
        public YXbsLocationsensorm()
        {
            TableName = "XBS_LOCATIONSENSORM";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsLocationsensorm)));
        }
        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public double? m_lon { get { return getDouble(1); } set { setDouble(1, value); } }
        public double? m_lat { get { return getDouble(2); } set { setDouble(2, value); } }
        public double? m_asl { get { return getDouble(3); } set { setDouble(3, value); } }
        public int? m_id_xbs_measurementres { get { return getInt(4); } set { setInt(4, value); } }
    }

    public class YXbsFrequencymeas : Yyy
    {
        public YXbsFrequencymeas()
        {
            TableName = "XBS_FREQUENCYMEAS";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsFrequencymeas)));
        }

        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public double? m_freq { get { return getDouble(1); } set { setDouble(1, value); } }
        public int? m_id_xbs_measurementres { get { return getInt(2); } set { setInt(2, value); } }
        public int? m_num { get { return getInt(3); } set { setInt(3, value); } }
    }

    public class YXbsLevelmeasres : Yyy
    {
        public YXbsLevelmeasres()
        {
            TableName = "XBS_LEVELMEASRES";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsLevelmeasres)));
        }

        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public double? m_value { get { return getDouble(1); } set { setDouble(1, value); } }
        public double? m_stddev { get { return getDouble(2); } set { setDouble(2, value); } }
        public double? m_vmin { get { return getDouble(3); } set { setDouble(3, value); } }
        public double? m_vmmax { get { return getDouble(4); } set { setDouble(4, value); } }
        public double? m_limit { get { return getDouble(5); } set { setDouble(5, value); } }
        public double? m_occupancy { get { return getDouble(6); } set { setDouble(6, value); } }
        public double? m_pmin { get { return getDouble(7); } set { setDouble(7, value); } }
        public double? m_pmax { get { return getDouble(8); } set { setDouble(8, value); } }
        public double? m_pdiff { get { return getDouble(9); } set { setDouble(9, value); } }
        public int? m_id_xbs_measurementres { get { return getInt(10); } set { setInt(10, value); } }
    }

    public class YXbsSpectoccupmeas : Yyy
    {
        public YXbsSpectoccupmeas()
        {
            TableName = "XBS_SPECTOCCUPMEAS";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsSpectoccupmeas)));
        }
        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public double? m_value { get { return getDouble(1); } set { setDouble(1, value); } }
        public double? m_stddev { get { return getDouble(2); } set { setDouble(2, value); } }
        public double? m_vmin { get { return getDouble(3); } set { setDouble(3, value); } }
        public double? m_vmmax { get { return getDouble(4); } set { setDouble(4, value); } }
        public double? m_limit { get { return getDouble(5); } set { setDouble(5, value); } }
        public double? m_occupancy { get { return getDouble(6); } set { setDouble(6, value); } }
        public int? m_id_xbs_measurementres { get { return getInt(7); } set { setInt(7, value); } }
    }

    public class YXbsLevelmeasonlres : Yyy
    {
        public YXbsLevelmeasonlres()
        {
            TableName = "XBS_LEVELMEASONLRES";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YXbsLevelmeasonlres)));
        }

        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public double? m_value { get { return getDouble(1); } set { setDouble(1, value); } }
        public int? m_id_xbs_measurementres { get { return getInt(2); } set { setInt(2, value); } }
    }

    public class YTeams : Yyy
    {
        public YTeams()
        {
            TableName = "TEAMS";
            getAllFields.AddRange(Utils.GetAllProps(typeof(YTeams)));
        }

        public int? m_id { get { return getInt(0); } set { setInt(0, value); } }
        public string m_name { get { return getString(1); } set { setString(1, 150, value); } }
    }



    public static class Utils
    {
        public static List<OracleParameter> GetAllProps(Type tp)
        {
            List<OracleParameter> value = new List<OracleParameter>();
            PropertyInfo[] memberInfo;
            memberInfo = tp.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo f in memberInfo)
            {
                OracleDbType oracleDbType = OracleDbType.Object;
                string tps = "";
                if (f.PropertyType.GetGenericArguments().Length > 0)
                    tps = f.PropertyType.GetGenericArguments()[0].ToString();
                else tps = f.PropertyType.Name.ToString();

                switch (tps)
                {
                    case "System.String":
                    case "String":
                        oracleDbType = OracleDbType.Varchar2;
                        break;
                    case "System.Int32":
                    case "Int32":
                        oracleDbType = OracleDbType.Int32;
                        break;
                    case "System.Double":
                    case "Double":
                        oracleDbType = OracleDbType.Double;
                        break;
                    case "System.Float":
                    case "Float":
                        oracleDbType = OracleDbType.Single;
                        break;
                    case "System.DateTime":
                    case "DateTime":
                        oracleDbType = OracleDbType.Date;
                        break;
                    case "System.Byte":
                    case "Byte":
                        oracleDbType = OracleDbType.Byte;
                        break;
                    case "System.Byte[]":
                    case "Byte[]":
                        oracleDbType = OracleDbType.Blob;
                        break;
                    default:
                        throw new Exception();
                        break;
                }
                value.Add(new OracleParameter()
                {
                    SourceColumn = "\"" + f.Name.Replace("m_", "").ToUpper() + "\"",
                    ParameterName = ":" + f.Name.Replace("m_", "").ToUpper(),
                    OracleDbType = oracleDbType,
                    Direction = System.Data.ParameterDirection.Input,
                });
            }
            return value;
        }
    }
}