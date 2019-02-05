using System;
using Atdi.Contracts.Api.Sdrn.MessageBus;


namespace Atdi.AppUnits.Sdrn.ControlA.Bus
{
   
    public class BusConfig
    {
        public static IFormatProvider CultureEnUs = new System.Globalization.CultureInfo("en-US");
        private  Platform.AppComponent.IComponentConfig _configParameters { get; set; }
        public IBusGateConfig GetBusConfig(IBusGateFactory busGateFactory, Platform.AppComponent.IComponentConfig configParameters)
        {
           _configParameters = configParameters;
            if (configParameters != null)
            {
                string periodSendActivitySensor = configParameters["PeriodSendActivitySensor"].ToString();
                ConfigParameters.PeriodSendActivitySensor = Int32.Parse(periodSendActivitySensor);
                double.TryParse(configParameters["LonDelta"].ToString(), System.Globalization.NumberStyles.Float, CultureEnUs, out ConfigParameters.LonDelta);
                double.TryParse(configParameters["LatDelta"].ToString(), System.Globalization.NumberStyles.Float, CultureEnUs, out ConfigParameters.LatDelta);
                int.TryParse(configParameters["TimeUpdateSensorLocation"].ToString(), System.Globalization.NumberStyles.Float, CultureEnUs, out ConfigParameters.TimeUpdateSensorLocation);
                double.TryParse(configParameters["MeasSdrParamRbw"].ToString(), System.Globalization.NumberStyles.Float, CultureEnUs, out ConfigParameters.MeasSdrParamRbw);
                double.TryParse(configParameters["MeasSdrParamVbw"].ToString(), System.Globalization.NumberStyles.Float, CultureEnUs, out ConfigParameters.MeasSdrParamVbw);
                double.TryParse(configParameters["MeasSdrParamRefLeveldbm"].ToString(), System.Globalization.NumberStyles.Float, CultureEnUs, out ConfigParameters.MeasSdrParamRefLeveldbm);
                double.TryParse(configParameters["MeasSdrParamTimeOfm"].ToString().Trim(), System.Globalization.NumberStyles.Float, CultureEnUs, out ConfigParameters.MeasSdrParamTimeOfm);
                ConfigParameters.MEASTypeOfm = configParameters["MeasTypeOfm"].ToString();
                ConfigParameters.MeasTypeFunction = configParameters["MeasTypeFunction"].ToString();
                int.TryParse(configParameters["MEASSwTime"].ToString(), System.Globalization.NumberStyles.Float, CultureEnUs, out ConfigParameters.MEASSwTime);
                int.TryParse(configParameters["TimeArchiveResult"].ToString(), System.Globalization.NumberStyles.Float, CultureEnUs, out ConfigParameters.TimeArchiveResult);
            }
            return CreateConfig(busGateFactory);
        }

        IBusGateConfig CreateConfig(IBusGateFactory gateFactory)
        {
            var config = gateFactory.CreateConfig();
            config["License.FileName"] = _configParameters["License.FileName"];
            config["License.OwnerId"] = _configParameters["License.OwnerId"];
            config["License.ProductKey"] = _configParameters["License.ProductKey"];
            config["RabbitMQ.Host"] = _configParameters["RabbitMQ.Host"];
            config["RabbitMQ.User"] = _configParameters["RabbitMQ.User"]; 
            config["RabbitMQ.Password"] = _configParameters["RabbitMQ.Password"];
            config["RabbitMQ.Port"] = _configParameters["RabbitMQ.Port"];
            config["RabbitMQ.VirtualHost"] = _configParameters["RabbitMQ.VirtualHost"];
            config["SDRN.Device.SensorTechId"] = _configParameters["SDRN.Device.SensorTechId"]; 
            config["SDRN.ApiVersion"] = _configParameters["SDRN.ApiVersion"];
            config["SDRN.Server.Instance"] = _configParameters["SDRN.Server.Instance"]; 
            config["SDRN.Server.QueueNamePart"] = _configParameters["SDRN.Server.QueueNamePart"]; 
            config["SDRN.Device.Exchange"] = _configParameters["SDRN.Device.Exchange"]; 
            config["SDRN.Device.QueueNamePart"] = _configParameters["SDRN.Device.QueueNamePart"]; 
            config["SDRN.Device.MessagesBindings"] = _configParameters["SDRN.Device.MessagesBindings"];
            config["SDRN.MessageConvertor.UseEncryption"] = _configParameters["SDRN.Device.UseEncryption"];
            config["SDRN.MessageConvertor.UseСompression"] = _configParameters["SDRN.MessageConvertor.UseСompression"];
            return config;
        }
        /*
        IBusGateConfig CreateConfigTest(IBusGateFactory gateFactory)
        {
            var config = gateFactory.CreateConfig();
            config["License.FileName"] = "LIC-DBD12-A00-722.SENSOR-DBD12-A00-1692.lic";
            config["License.OwnerId"] = "OID-BD12-A00-N00";
            config["License.ProductKey"] = "BE1D-RLNN-S0S6-EN42-0028";
            config["RabbitMQ.Host"] = "10.1.1.131";
            config["RabbitMQ.User"] = "SDR_Client";
            config["RabbitMQ.Password"] = "32Xr567";
            config["RabbitMQ.Port"] = "5672";
            config["RabbitMQ.VirtualHost"] = "/";
            config["SDRN.Device.SensorTechId"] = "BB60C_";
            config["SDRN.ApiVersion"] = "2.0";
            config["SDRN.Server.Instance"] = "ServerSDRN01";
            config["SDRN.Server.QueueNamePart"] = "Q.SDRN.Server";
            config["SDRN.Device.Exchange"] = "EX.SDRN.Device";
            config["SDRN.Device.QueueNamePart"] = "Q.SDRN.Device";
            config["SDRN.Device.MessagesBindings"] = "{messageType=RegisterSensor, routingKey=#01};{messageType=SendCommandResult, routingKey=#02};{messageType=SendMeasResults, routingKey=#03};{messageType=SendEntity, routingKey=#04};{messageType=SendEntityPart, routingKey=#05};{messageType=UpdateSensor, routingKey=#06};{messageType=SendActivitySensor, routingKey=#07};{messageType=SendActivitySensorResult, routingKey=#08};{messageType=SendMeasSdrResults, routingKey=#09};{messageType=SendMeasSdrTask, routingKey=#10};{messageType=UpdateSensorLocation, routingKey=#11};{messageType=StopMeasSdrTask, routingKey=#12};{messageType=UpdateSensorLocationResult, routingKey=#13};";
            config["SDRN.MessageConvertor.UseEncryption"] = "false";
            config["SDRN.MessageConvertor.UseСompression"] = "false";
            return config;
        }
        */
    }

   


}
