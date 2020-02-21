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




            DataSynchronizationBase dataSynchronizationBase = new DataSynchronizationBase();
            dataSynchronizationBase.CreatedBy = "ICSM";
            dataSynchronizationBase.DateCreated = DateTime.Now;
            dataSynchronizationBase.DateStart = DateTime.Now;
            dataSynchronizationBase.DateEnd = DateTime.Now;

            RefSpectrum refSpectrum = new RefSpectrum();
            refSpectrum.CreatedBy = "ICSM";
            refSpectrum.DateCreated = DateTime.Now;
            refSpectrum.FileName = "FileName";
            refSpectrum.DataRefSpectrum = new DataRefSpectrum[2];
            refSpectrum.DataRefSpectrum[0] = new DataRefSpectrum();
            refSpectrum.DataRefSpectrum[0].DateMeas = DateTime.Now;
            refSpectrum.DataRefSpectrum[0].DispersionLow = 20.344;
            refSpectrum.DataRefSpectrum[0].DispersionUp = 30.123;
            refSpectrum.DataRefSpectrum[0].Freq_MHz = 2000;
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
            refSpectrum.DataRefSpectrum[1].Freq_MHz = 3000.4533;
            refSpectrum.DataRefSpectrum[1].GlobalSID = "SID2001";
            refSpectrum.DataRefSpectrum[1].IdNum = 2;
            refSpectrum.DataRefSpectrum[1].Level_dBm = 3.5;
            refSpectrum.DataRefSpectrum[1].Percent = 10.34;
            refSpectrum.DataRefSpectrum[1].SensorId = 1;
            refSpectrum.DataRefSpectrum[1].TableId = 22346;
            refSpectrum.DataRefSpectrum[1].TableName = "MOB_STATION";



            //long? id = sdrnServer.ImportRefSpectrum(refSpectrum);
            var refSpectrums = sdrnServer.GetAllRefSpectrum();
            // var status = sdrnServer.RunDataSynchronizationProcess(dataSynchronizationBase, );


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
