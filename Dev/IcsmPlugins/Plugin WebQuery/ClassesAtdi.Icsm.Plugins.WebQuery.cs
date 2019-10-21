//-------------------------------------------------------------------------
//FILE GENERATED BY ICS MANAGER FROM SCHEMA 'Atdi.Icsm.Plugins.WebQuery' DON'T MODIFY IT;
//-------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace OrmCs
{
	public class YXwebquery : Yyy
	{
		public YXwebquery() { construct("XWEBQUERY",null); }
		public YXwebquery(OrmLinker lk) { construct("XWEBQUERY",lk); }
		public int m_id { get { return getInt(0); } set { setInt(0,value); } }
			public DatPtr z_id { get { return new DatPtr(this,0); } }
		public string m_name { get { return getString(1); } set { setString(1,50,value); } }
			public DatPtr z_name { get { return new DatPtr(this,1); } }
		public string m_comments { get { return getString(2); } set { setString(2,250,value); } }
			public DatPtr z_comments { get { return new DatPtr(this,2); } }
		public string m_identuser { get { return getString(3); } set { setString(3,250,value); } }
			public DatPtr z_identuser { get { return new DatPtr(this,3); } }
		public string m_code { get { return getString(4); } set { setString(4,50,value); } }
			public DatPtr z_code { get { return new DatPtr(this,4); } }
		public string m_taskforcegroup { get { return getString(5); } set { setString(5,100,value); } }
			public DatPtr z_taskforcegroup { get { return new DatPtr(this,5); } }
		public string m_viewcolumns { get { return getString(6); } set { setString(6,4000,value); } }
			public DatPtr z_viewcolumns { get { return new DatPtr(this,6); } }
		public string m_addcolumns { get { return getString(7); } set { setString(7,4000,value); } }
			public DatPtr z_addcolumns { get { return new DatPtr(this,7); } }
		public string m_editcolumns { get { return getString(8); } set { setString(8,4000,value); } }
			public DatPtr z_editcolumns { get { return new DatPtr(this,8); } }
		public string m_tablecolumns { get { return getString(9); } set { setString(9,4000,value); } }
			public DatPtr z_tablecolumns { get { return new DatPtr(this,9); } }
	}

	public class YXwebqueryattributes : Yyy
	{
		public YXwebqueryattributes() { construct("XWEBQUERYATTRIBUTES",null); }
		public YXwebqueryattributes(OrmLinker lk) { construct("XWEBQUERYATTRIBUTES",lk); }
		public int m_id { get { return getInt(0); } set { setInt(0,value); } }
			public DatPtr z_id { get { return new DatPtr(this,0); } }
		public int m_webqueryid { get { return getInt(1); } set { setInt(1,value); } }
			public DatPtr z_webqueryid { get { return new DatPtr(this,1); } }
		public string m_path { get { return getString(2); } set { setString(2,250,value); } }
			public DatPtr z_path { get { return new DatPtr(this,2); } }
		public int m_readonly { get { return getInt(3); } set { setInt(3,value); } }
			public DatPtr z_readonly { get { return new DatPtr(this,3); } }
		public int m_notchangeadd { get { return getInt(4); } set { setInt(4,value); } }
			public DatPtr z_notchangeadd { get { return new DatPtr(this,4); } }
		public int m_notchangeedit { get { return getInt(5); } set { setInt(5,value); } }
			public DatPtr z_notchangeedit { get { return new DatPtr(this,5); } }
		public YXwebquery m_JoinWebQuery { get { return (YXwebquery)getYyy(6); }  set { setYyy(6,value); } }
	}

	public class YXwebcoverage : Yyy
	{
		public YXwebcoverage() { construct("XWEBCOVERAGE",null); }
		public YXwebcoverage(OrmLinker lk) { construct("XWEBCOVERAGE",lk); }
		public double m_id { get { return getDouble(0); } set { setDouble(0,value); } }
			public DatPtr z_id { get { return new DatPtr(this,0); } }
		public string m_file_name { get { return getString(1); } set { setString(1,50,value); } }
			public DatPtr z_file_name { get { return new DatPtr(this,1); } }
		public DateTime m_date_created { get { return getDateTime(2); } set { setDateTime(2,value); } }
			public DatPtr z_date_created { get { return new DatPtr(this,2); } }
		public string m_province { get { return getString(3); } set { setString(3,50,value); } }
			public DatPtr z_province { get { return new DatPtr(this,3); } }
	}

	public class YXwebconstraint : Yyy
	{
		public YXwebconstraint() { construct("XWEBCONSTRAINT",null); }
		public YXwebconstraint(OrmLinker lk) { construct("XWEBCONSTRAINT",lk); }
		public int m_id { get { return getInt(0); } set { setInt(0,value); } }
			public DatPtr z_id { get { return new DatPtr(this,0); } }
		public int m_webqueryid { get { return getInt(1); } set { setInt(1,value); } }
			public DatPtr z_webqueryid { get { return new DatPtr(this,1); } }
		public string m_name { get { return getString(2); } set { setString(2,50,value); } }
			public DatPtr z_name { get { return new DatPtr(this,2); } }
		public string m_path { get { return getString(3); } set { setString(3,250,value); } }
			public DatPtr z_path { get { return new DatPtr(this,3); } }
		public double m_min { get { return getDouble(4); } set { setDouble(4,value); } }
			public DatPtr z_min { get { return new DatPtr(this,4); } }
		public double m_max { get { return getDouble(5); } set { setDouble(5,value); } }
			public DatPtr z_max { get { return new DatPtr(this,5); } }
		public string m_strvalue { get { return getString(6); } set { setString(6,4000,value); } }
			public DatPtr z_strvalue { get { return new DatPtr(this,6); } }
		public DateTime m_datevaluemin { get { return getDateTime(7); } set { setDateTime(7,value); } }
			public DatPtr z_datevaluemin { get { return new DatPtr(this,7); } }
		public int m_include { get { return getInt(8); } set { setInt(8,value); } }
			public DatPtr z_include { get { return new DatPtr(this,8); } }
		public DateTime m_datevaluemax { get { return getDateTime(9); } set { setDateTime(9,value); } }
			public DatPtr z_datevaluemax { get { return new DatPtr(this,9); } }
		public string m_descrcondition { get { return getString(10); } set { setString(10,2000,value); } }
			public DatPtr z_descrcondition { get { return new DatPtr(this,10); } }
		public string m_typecondition { get { return getString(11); } set { setString(11,50,value); } }
			public DatPtr z_typecondition { get { return new DatPtr(this,11); } }
		public string m_opercondition { get { return getString(12); } set { setString(12,50,value); } }
			public DatPtr z_opercondition { get { return new DatPtr(this,12); } }
		public string m_messagenotvalid { get { return getString(13); } set { setString(13,1000,value); } }
			public DatPtr z_messagenotvalid { get { return new DatPtr(this,13); } }
		public string m_defaultvalue { get { return getString(14); } set { setString(14,200,value); } }
			public DatPtr z_defaultvalue { get { return new DatPtr(this,14); } }
		public string m_momentofuse { get { return getString(15); } set { setString(15,50,value); } }
			public DatPtr z_momentofuse { get { return new DatPtr(this,15); } }
		public string m_strvalueto { get { return getString(16); } set { setString(16,4000,value); } }
			public DatPtr z_strvalueto { get { return new DatPtr(this,16); } }
		public YXwebquery m_JoinWebQuery { get { return (YXwebquery)getYyy(17); }  set { setYyy(17,value); } }
	}

	public class YXupdateobjects : Yyy
	{
		public YXupdateobjects() { construct("XUPDATEOBJECTS",null); }
		public YXupdateobjects(OrmLinker lk) { construct("XUPDATEOBJECTS",lk); }
		public int m_id { get { return getInt(0); } set { setInt(0,value); } }
			public DatPtr z_id { get { return new DatPtr(this,0); } }
		public string m_objtable { get { return getString(1); } set { setString(1,50,value); } }
			public DatPtr z_objtable { get { return new DatPtr(this,1); } }
		public DateTime m_datemodified { get { return getDateTime(2); } set { setDateTime(2,value); } }
			public DatPtr z_datemodified { get { return new DatPtr(this,2); } }
	}

	public class YXwebqueryorders : Yyy
	{
		public YXwebqueryorders() { construct("XWEBQUERYORDERS",null); }
		public YXwebqueryorders(OrmLinker lk) { construct("XWEBQUERYORDERS",lk); }
		public int m_id { get { return getInt(0); } set { setInt(0,value); } }
			public DatPtr z_id { get { return new DatPtr(this,0); } }
		public int m_webqueryid { get { return getInt(1); } set { setInt(1,value); } }
			public DatPtr z_webqueryid { get { return new DatPtr(this,1); } }
		public string m_path { get { return getString(2); } set { setString(2,250,value); } }
			public DatPtr z_path { get { return new DatPtr(this,2); } }
		public int m_order { get { return getInt(3); } set { setInt(3,value); } }
			public DatPtr z_order { get { return new DatPtr(this,3); } }
		public YXwebquery m_JoinWebQuery { get { return (YXwebquery)getYyy(4); }  set { setYyy(4,value); } }
	}

}
