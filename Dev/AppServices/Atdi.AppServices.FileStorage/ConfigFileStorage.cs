using System;
using Atdi.Platform.AppComponent;
using System.Collections.Generic;



namespace Atdi.AppServices.FileStorage
{
    public class ConfigFileStorage
    {
        /// <summary>
        /// 
        /// </summary>
        [ComponentConfigProperty("Storage.WorkFolder")]
        public string WorkFolder { get; set; }
    
    }
}
