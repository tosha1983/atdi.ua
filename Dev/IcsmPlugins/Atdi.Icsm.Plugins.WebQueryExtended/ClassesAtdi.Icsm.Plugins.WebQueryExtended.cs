//-------------------------------------------------------------------------
//FILE GENERATED BY ICS MANAGER FROM SCHEMA 'Atdi.Icsm.Plugins.WebQueryExtended' DON'T MODIFY IT;
//-------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace OrmCs
{
	public class YXvWebBc : Yyy
	{
		public YXvWebBc() { construct("XV_WEB_BC",null); }
		public YXvWebBc(OrmLinker lk) { construct("XV_WEB_BC",lk); }
		public int m_id { get { return getInt(0); } set { setInt(0,value); } }
			public DatPtr z_id { get { return new DatPtr(this,0); } }
		public YMobStationT m_WebQuery_BS_3 { get { return (YMobStationT)getYyy(1); }  set { setYyy(1,value); } }
		public string m_licence { get { return getString(2); } set { setString(2,200,value); } }
			public DatPtr z_licence { get { return new DatPtr(this,2); } }
		public int m_lic_id { get { return getInt(3); } set { setInt(3,value); } }
			public DatPtr z_lic_id { get { return new DatPtr(this,3); } }
		public string m_radiotech { get { return getString(4); } set { setString(4,10,value); } }
			public DatPtr z_radiotech { get { return new DatPtr(this,4); } }
		public string m_conc_number { get { return getString(5); } set { setString(5,200,value); } }
			public DatPtr z_conc_number { get { return new DatPtr(this,5); } }
		public DateTime m_conc_date_from { get { return getDateTime(6); } set { setDateTime(6,value); } }
			public DatPtr z_conc_date_from { get { return new DatPtr(this,6); } }
		public string m_dozv_number { get { return getString(7); } set { setString(7,200,value); } }
			public DatPtr z_dozv_number { get { return new DatPtr(this,7); } }
		public DateTime m_dozv_date_from { get { return getDateTime(8); } set { setDateTime(8,value); } }
			public DatPtr z_dozv_date_from { get { return new DatPtr(this,8); } }
		public string m_equip_name { get { return getString(9); } set { setString(9,50,value); } }
			public DatPtr z_equip_name { get { return new DatPtr(this,9); } }
		public int m_equip_id { get { return getInt(10); } set { setInt(10,value); } }
			public DatPtr z_equip_id { get { return new DatPtr(this,10); } }
		public string m_modulation { get { return getString(11); } set { setString(11,20,value); } }
			public DatPtr z_modulation { get { return new DatPtr(this,11); } }
		public string m_address { get { return getString(12); } set { setString(12,4000,value); } }
			public DatPtr z_address { get { return new DatPtr(this,12); } }
		public int m_pos_id { get { return getInt(13); } set { setInt(13,value); } }
			public DatPtr z_pos_id { get { return new DatPtr(this,13); } }
		public double m_longitude { get { return getDouble(14); } set { setDouble(14,value); } }
			public DatPtr z_longitude { get { return new DatPtr(this,14); } }
		public double m_latitude { get { return getDouble(15); } set { setDouble(15,value); } }
			public DatPtr z_latitude { get { return new DatPtr(this,15); } }
		public string m_province { get { return getString(16); } set { setString(16,50,value); } }
			public DatPtr z_province { get { return new DatPtr(this,16); } }
		public double m_power { get { return getDouble(17); } set { setDouble(17,value); } }
			public DatPtr z_power { get { return new DatPtr(this,17); } }
		public string m_antenna_name { get { return getString(18); } set { setString(18,50,value); } }
			public DatPtr z_antenna_name { get { return new DatPtr(this,18); } }
		public int m_antenna_id { get { return getInt(19); } set { setInt(19,value); } }
			public DatPtr z_antenna_id { get { return new DatPtr(this,19); } }
		public double m_gain { get { return getDouble(20); } set { setDouble(20,value); } }
			public DatPtr z_gain { get { return new DatPtr(this,20); } }
		public double m_agl { get { return getDouble(21); } set { setDouble(21,value); } }
			public DatPtr z_agl { get { return new DatPtr(this,21); } }
		public double m_angle_elev { get { return getDouble(22); } set { setDouble(22,value); } }
			public DatPtr z_angle_elev { get { return new DatPtr(this,22); } }
		public string m_diag { get { return getString(23); } set { setString(23,4000,value); } }
			public DatPtr z_diag { get { return new DatPtr(this,23); } }
		public string m_polarization { get { return getString(24); } set { setString(24,4,value); } }
			public DatPtr z_polarization { get { return new DatPtr(this,24); } }
		public string m_azimuth { get { return getString(25); } set { setString(25,4000,value); } }
			public DatPtr z_azimuth { get { return new DatPtr(this,25); } }
		public string m_tx_freq { get { return getString(26); } set { setString(26,200,value); } }
			public DatPtr z_tx_freq { get { return new DatPtr(this,26); } }
		public string m_rx_freq { get { return getString(27); } set { setString(27,200,value); } }
			public DatPtr z_rx_freq { get { return new DatPtr(this,27); } }
		public string m_des_emission { get { return getString(28); } set { setString(28,9,value); } }
			public DatPtr z_des_emission { get { return new DatPtr(this,28); } }
		public string m_sector_number { get { return getString(29); } set { setString(29,1,value); } }
			public DatPtr z_sector_number { get { return new DatPtr(this,29); } }
		public string m_edrpou { get { return getString(30); } set { setString(30,50,value); } }
			public DatPtr z_edrpou { get { return new DatPtr(this,30); } }
	}

}