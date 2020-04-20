using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeepServices.Gis;
using Atdi.DataModels.Sdrn.DeepServices.RadioSystem.Stations;

namespace Atdi.DataModels.Sdrn.CalcServer.Internal.Clients
{
	public class ClientContextStation
	{
		public long Id;

		public long ContextId;

		public DateTimeOffset CreatedDate;

		public string Name;

		public string CallSign;

		public ClientContextStationType Type;

		public StationAntenna Antenna;

		public Wgs84Site Site;

		public AtdiCoordinate Coordinate;

		public StationTransmitter Transmitter;

	}

	public enum ClientContextStationType
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
