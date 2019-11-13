using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using System.Text;
using System.IO;
using Atdi.AppUnits.Icsm.CoverageEstimation.Models;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;



namespace Atdi.AppUnits.Icsm.CoverageEstimation.Handlers
{
    public class CheckOperation
    {
        private string _protocolFileName { get; set; }
        public CheckOperation(string protocolFileName)
        {
            this._protocolFileName = protocolFileName;
        }

        public bool Save(CurrentOperation currentFailedOperation)
        {
            IFormatter formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, currentFailedOperation);
                File.WriteAllBytes(this._protocolFileName, stream.ToArray());
            }
            return true;
        }

        public CurrentOperation Load()
        {
            var currentOperation = new CurrentOperation();
            if (System.IO.File.Exists(this._protocolFileName))
            {
                var body = File.ReadAllBytes(this._protocolFileName);
                using (var memoryStream = new MemoryStream(body))
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Binder = new LocalBinder();
                    currentOperation = (CurrentOperation)formatter.Deserialize(memoryStream);
                }
            }
            else
            {
                currentOperation = null;
            }
            return currentOperation;
        }

        public CurrentOperation isNotNullFailedOperation()
        {
            return Load();
        }

        public bool isFindOperation(CurrentOperation operation)
        {
            bool isFindOperation = false;
            var currentOperation = Load();
            if (currentOperation != null)
            {
                if ((currentOperation.CurrICSTelecomProjectDir == operation.CurrICSTelecomProjectDir)
                        && (currentOperation.Standard == operation.Standard))
                {
                    isFindOperation = true;
                }
            }
            return isFindOperation;
        }

        public void DeleteProtocolFile()
        {
            if (System.IO.File.Exists(this._protocolFileName))
            {
                System.IO.File.Delete(this._protocolFileName);
            }
        }
    }
}
