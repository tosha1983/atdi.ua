using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Modules.Licensing
{
    [Serializable]
    public class LicenseData
    {

        public string LicenseNumber { get; set; }

        public string LicenseType { get; set; }

        public string Company { get; set; }

        public string Copyright { get; set; }

        public string OwnerName { get; set; }

        public string OwnerId { get; set; }

        public string ProductName {get; set; }

        public string ProductKey { get; set; }

        public DateTime Created { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime StopDate { get; set; }

        public int Count { get; set; }

        public string Instance { get; set; }
    }


    [Flags]
    public enum LicenseLimitationTerms
    {
        TimePeriod = 1,
        Version = 2, 
        Assembly = 4,
        Year = 8,
		/// <summary>
		/// Простая привязка по имени хоста
		/// </summary>
        Host = 16,
		/// <summary>
		/// Привязка к оборудованию
		/// </summary>
        HardwareBinding = 32
	}

    [Serializable]
    public class LicenseData2 : LicenseData
    {
        public LicenseLimitationTerms LimitationTerms { get; set; }

        public string Version { get; set; }

        public ushort Year { get; set; }

        public string AssemblyFullName { get; set; }

        public string HostData { get; set; }
    }

    [Flags]
    public enum LicenseHardwareBinding
	{
		/// <summary>
		/// Привязка к идентификатору CPU, дублируется в VM
		/// </summary>
	    Cpu = 1,
		/// <summary>
		/// Привязка к серийному номеру материнской платы, под VM может быть не определен
		/// </summary>
	    Motherboard = 2,
		/// <summary>
		/// Привязка к серийному номеру системного диска, дублируется в VM
		/// </summary>
		SysDrive = 4,
		/// <summary>
		/// Привязка к имени хоста, дублируется в VM
		/// </summary>
		HostName = 8,
		/// <summary>
		/// Привязка у идентифкатору хоста, вродебы не дублируеться в VM
		/// </summary>
		HostUUID = 16,
		/// <summary>
		/// Приявязка к серийному номеру ОС, дублируется в VM
		/// </summary>
		OperatingSystem = 32,
		/// <summary>
		/// Привязка к сетевым интерфейсам
		/// </summary>
		NetworkInterfaces = 64
	}

	[Serializable]
    public class LicenseData3 : LicenseData2
    {
	    public LicenseHardwareBinding HardwareBinding { get; set; }

		public HostHardwareDescriptor HardwareDescriptor { get; set; }
	}

    [Serializable]
	public class HostHardwareDescriptor
	{
	    public string Cpu;
	    public string Motherboard;
	    public string SysDrive;
	    public string HostName;
	    public string HostUuid;
	    public string OperatingSystem;
	    public HostNetworkInterface[] NetworkInterfaces;

	    public override string ToString()
	    {
			var info = new StringBuilder();
			info.AppendLine($"Host Name: {this.HostName}");
			info.AppendLine($"Host UUID: {this.HostUuid}");
			info.AppendLine($"OS SerialNumber: {this.OperatingSystem}");
			info.AppendLine($"CPU SerialNumber: {this.Cpu}");
			info.AppendLine($"MB SerialNumber: {this.Motherboard}");
			info.AppendLine($"SysDrive SerialNumber: {this.SysDrive}");
			info.AppendLine($"Network Interfaces: ");
			foreach (var ni in NetworkInterfaces)
			{
				info.AppendLine($" - {ni.Id}: {ni.Name}, MAC='{ni.Address}', Type='{ni.InterfaceType}'");
			}
			return info.ToString();
	    }
	}

	[Serializable]
	public struct HostNetworkInterface
    {
	    public string Id;
	    public string Name;
	    public string Address;
	    public string InterfaceType;

    }
}
