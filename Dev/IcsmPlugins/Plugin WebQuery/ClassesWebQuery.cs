//-------------------------------------------------------------------------
//FILE GENERATED BY ICS MANAGER FROM SCHEMA 'WebQuery' DON'T MODIFY IT;
//-------------------------------------------------------------------------
using System;
using System.Collections.Generic;

namespace OrmCs
{
	public class YXwebquery : Yyy
	{
		public YXwebquery() { construct("XWebQuery",null); }
		public YXwebquery(OrmLinker lk) { construct("XWebQuery",lk); }
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
	}

	public class YXwebconstraint : Yyy
	{
		public YXwebconstraint() { construct("XWebConstraint",null); }
		public YXwebconstraint(OrmLinker lk) { construct("XWebConstraint",lk); }
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
		public string m_strvalue { get { return getString(6); } set { setString(6,250,value); } }
			public DatPtr z_strvalue { get { return new DatPtr(this,6); } }
		public DateTime m_datevaluemin { get { return getDateTime(7); } set { setDateTime(7,value); } }
			public DatPtr z_datevaluemin { get { return new DatPtr(this,7); } }
		public int m_include { get { return getInt(8); } set { setInt(8,value); } }
			public DatPtr z_include { get { return new DatPtr(this,8); } }
		public DateTime m_datevaluemax { get { return getDateTime(9); } set { setDateTime(9,value); } }
			public DatPtr z_datevaluemax { get { return new DatPtr(this,9); } }
		public YXwebquery m_JoinWebQuery { get { return (YXwebquery)getYyy(10); }  set { setYyy(10,value); } }
	}

}
