using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Contracts.WcfServices.Sdrn.Server;
using Newtonsoft.Json;
using System.ServiceModel;
using System.Threading;


namespace Atdi.Test.Sdrn.SynchroProcess.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Press any key to start SDRN Server WCF Service Client test ...");
            Console.ReadLine();

            var sdrnServer = GetServicByEndpoint("SdrnServerBasicHttpEndpoint");

            var proc = sdrnServer.GetAllDataSynchronizationProcess();
            var proc2 = sdrnServer.GetProtocolsByParameters(4, null, null, null, null, null, null, null, null, null, null, null, null, null);
            //bool isSuccess  = sdrnServer.DeleteRefSpectrum(new long[] { 9});  

            DataSynchronizationBase dataSynchronizationBase = new DataSynchronizationBase();
            dataSynchronizationBase.CreatedBy = "ICSM";
            dataSynchronizationBase.DateCreated = DateTime.Now;
            dataSynchronizationBase.DateStart = new DateTime(2019, 07, 01);
            dataSynchronizationBase.DateEnd = DateTime.Now.AddDays(100);

            RefSpectrum refSpectrum = new RefSpectrum();
            refSpectrum.CreatedBy = "ICSM";
            refSpectrum.DateCreated = DateTime.Now;
            refSpectrum.FileName = "FileName";
            refSpectrum.DataRefSpectrum = new DataRefSpectrum[2];
            refSpectrum.DataRefSpectrum[0] = new DataRefSpectrum();
            refSpectrum.DataRefSpectrum[0].DateMeas = DateTime.Now;
            refSpectrum.DataRefSpectrum[0].DispersionLow = 20.344;
            refSpectrum.DataRefSpectrum[0].DispersionUp = 30.123;
            refSpectrum.DataRefSpectrum[0].Freq_MHz = 421.0455000714;
            refSpectrum.DataRefSpectrum[0].GlobalSID = "SID2000";
            refSpectrum.DataRefSpectrum[0].IdNum = 1;
            refSpectrum.DataRefSpectrum[0].Level_dBm = 5;
            refSpectrum.DataRefSpectrum[0].Percent = 10;
            refSpectrum.DataRefSpectrum[0].SensorId = 1;
            refSpectrum.DataRefSpectrum[0].TableId = 22345;
            refSpectrum.DataRefSpectrum[0].TableName = "MOB_STATION";

            refSpectrum.DataRefSpectrum[1] = new DataRefSpectrum();
            refSpectrum.DataRefSpectrum[1].DateMeas = DateTime.Now;
            refSpectrum.DataRefSpectrum[1].DispersionLow = 10.133;
            refSpectrum.DataRefSpectrum[1].DispersionUp = 20.346;
            refSpectrum.DataRefSpectrum[1].Freq_MHz = 421.0455000715;
            refSpectrum.DataRefSpectrum[1].GlobalSID = "SID2001";
            refSpectrum.DataRefSpectrum[1].IdNum = 2;
            refSpectrum.DataRefSpectrum[1].Level_dBm = 3.5;
            refSpectrum.DataRefSpectrum[1].Percent = 10.34;
            refSpectrum.DataRefSpectrum[1].SensorId = 1;
            refSpectrum.DataRefSpectrum[1].TableId = 22346;
            refSpectrum.DataRefSpectrum[1].TableName = "MOB_STATION";



            long? id = sdrnServer.ImportRefSpectrum(refSpectrum);
            //var refSpectrums = sdrnServer.GetAllRefSpectrum();

            //var refSpectrums = sdrnServer.CurrentDataSynchronizationProcess();


            Area area = new Area();
            area.CreatedBy = "ICSM";
            area.DateCreated = DateTime.Now;
            area.IdentifierFromICSM = 11;
            area.Name = "Київська";
            area.TypeArea = "область";
            area.Location = new DataLocation[3];
            area.Location[0] = new DataLocation();
            area.Location[0].Longitude = 30.509033203125004;
            area.Location[0].Latitude = 50.57626025689928;
            area.Location[1] = new DataLocation();
            area.Location[1].Longitude = 30.3057861328125;
            area.Location[1].Latitude = 50.31565429419651;
            area.Location[2] = new DataLocation();
            area.Location[2].Longitude = 30.758972167968754;
            area.Location[2].Latitude = 50.31039245071915;

            StationExtended stationExtended1 = new StationExtended();
            stationExtended1.Address = "Address 1";
            stationExtended1.BandWidth = 200;
            stationExtended1.DesigEmission = "1E34--";
            stationExtended1.Location = new DataLocation();
            stationExtended1.Location.Longitude= 30.555725097656254;
            stationExtended1.Location.Latitude = 50.41726883571085;
            stationExtended1.OwnerName = "Киевстар";
            stationExtended1.PermissionNumber = "CA-46-65754-53464456";
            stationExtended1.PermissionStart = DateTime.Now;
            stationExtended1.PermissionStop = DateTime.Now.AddDays(30);
            stationExtended1.Province = "Київська";
            stationExtended1.Standard = "GSM-1800";
            stationExtended1.StandardName= "GSM-1800";
            stationExtended1.TableId = 22346;
            stationExtended1.TableName = "MOB_STATION";

            StationExtended stationExtended2 = new StationExtended();
            stationExtended2.Address = "Address 2";
            stationExtended2.BandWidth = 130;
            stationExtended2.DesigEmission = "2E34--";
            stationExtended2.Location = new DataLocation();
            stationExtended2.Location.Longitude = 30.514526367187504;
            stationExtended2.Location.Latitude = 50.44176389056172;
            stationExtended2.OwnerName = "Киевстар";
            stationExtended2.PermissionNumber = "CA-46-65754-53464457";
            stationExtended2.PermissionStart = DateTime.Now;
            stationExtended2.PermissionStop = DateTime.Now.AddDays(30);
            stationExtended2.Province = "Київська";
            stationExtended2.Standard = "GSM-1800";
            stationExtended2.StandardName = "GSM-1800";
            stationExtended2.TableId = 22345;
            stationExtended2.TableName = "MOB_STATION";

            var status = sdrnServer.RunDataSynchronizationProcess(dataSynchronizationBase, new long[] { id.Value }, new long[] { 1 }, new Area[] { area }, new StationExtended[] { stationExtended1, stationExtended2 }  );


            Console.WriteLine($"Test was finished. Press any key to exit.");
            Console.ReadLine();
        }

        static ISdrnsController GetServicByEndpoint(string endpointName)
        {
            var f = new ChannelFactory<ISdrnsController>(endpointName);
            return f.CreateChannel();
        }
    }
}
