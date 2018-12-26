using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.Modules.MonitoringProcess.ProcessSignal;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Atdi.Modules.MonitoringProcess.Measurement
{
    public class IQStreem
    {
        public ReceivedIQStream receivedIQStream;
        public IQStreem(ISDR SDR, TaskParameters taskParameters)
        {
            receivedIQStream = new ReceivedIQStream();
            bool done = SDR.GetIQStream(ref receivedIQStream , taskParameters.ReceivedIQStreemDuration_sec);
            if (done == false)
            {
                receivedIQStream = null;
            }
            // просто получили поток пока не ясно что с ним делать для тестов запись
            //SerializeObject("C:\\projects\\Monitoring projects\\SDR\\DataSignal\\GSM_900_2.bin", receivedIQStream);

        }
        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="File"></param>
        /// <param name="obj"></param>
        private void SerializeObject(string File, object obj)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = new FileStream(File, FileMode.OpenOrCreate);
            formatter.Serialize(fs, obj);
            fs.Close();
            fs.Dispose();
        }
    }
}
