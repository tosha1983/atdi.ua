﻿using Atdi.DataModels.EntityOrm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.DataModels.Sdrn.CalcServer.Entities
{
	[EntityPrimaryKey]
	public interface IContextStation_PK
	{
		long Id { get; set; }
	}
	[Entity]
	public interface IContextStation : IContextStation_PK
	{
		IClientContext CONTEXT { get; set; }

		DateTimeOffset CreatedDate { get; set; }

        string ExternalCode { get; set; }

        string ExternalSource { get; set; }

        DateTimeOffset? ModifiedDate { get; set; }

        string LicenseGsid { get; set; }

        string RealGsid { get; set; }

        string RegionCode { get; set; }

        string Name { get; set; }

		string CallSign { get; set; }

		byte StateCode { get; set; }

		string StateName { get; set; }

        string Standard { get; set; }

        IContextStationSite SITE { get; set; }

		IContextStationAntenna ANTENNA { get; set; }

		IContextStationTransmitter TRANSMITTER { get; set; }

		IContextStationReceiver RECEIVER { get; set; }

		IContextStationCoordinates COORDINATES { get; set; }
}


	public enum StationStateCode
	{
		/// <summary>
		/// Unknown
		/// </summary>
		U = 0,

		/// <summary>
		/// Active
		/// </summary>
		A = 1,

		/// <summary>
		/// Passive
		/// </summary>
		P = 2, 

		/// <summary>
		/// Interference
		/// </summary>
		I = 3,

		/// <summary>
		/// Victim
		/// </summary>
		V = 4, //Victim
	}
}
