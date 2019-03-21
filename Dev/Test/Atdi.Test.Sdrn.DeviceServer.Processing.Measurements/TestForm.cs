using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Atdi.AppUnits.Sdrn.DeviceServer.Processing.Measurements;
using Atdi.DataModels.Sdrn.DeviceServer.Processing;
using Atdi.DataModels.Sdrns.Device;
using Atdi.DataModels.Sdrn.DeviceServer.Commands.Results;
using Atdi.DataModels.Sdrn.DeviceServer;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;


namespace Atdi.Test.Sdrn.DeviceServer.Processing.Measurements
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            openFileDialog1.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog1.FileName != "")
                {
                    FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open);
                    try
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        var obj = formatter.Deserialize(fs);
                        if (obj != null)
                        {
                            var mesureTraceResult = (obj as MesureTraceResult);

                            Emitting emitting = new Emitting();
                            ReferenceSituation referenceSituation = new ReferenceSituation();
                            MesureTraceDeviceProperties mesureTraceDeviceProperties = new MesureTraceDeviceProperties();



                            ReferenceLevels referenceLevels = CalcReferenceLevels.CalcRefLevels(referenceSituation, mesureTraceResult, mesureTraceDeviceProperties);
                        }
                    }
                    catch (SerializationException eX)
                    {
                        Console.WriteLine("Failed to deserialize. Reason: " + eX.Message);
                    }
                    finally
                    {
                        fs.Close();
                    }
                }
            }

            
        }
    }
}
