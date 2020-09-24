using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;

namespace Atdi.Test.Sdrn.DeviceServer.DecodeMessage
{
    class Program
    {
        static void Main(string[] args)
        {
            var loadMessages = new LoadMessages<TaskParameters>();
            string additionalParams = @"c:\TEMP\Dir";
            string fileNameFinded = "";
            var taskParameters = loadMessages.GetMessage(additionalParams, out fileNameFinded);
        }
    }
}
