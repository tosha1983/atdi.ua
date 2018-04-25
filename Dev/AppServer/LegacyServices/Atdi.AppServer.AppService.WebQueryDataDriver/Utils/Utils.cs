using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Configuration;
using System.Runtime.Serialization;
using System.Data;
using System.Collections;
using System.Security.Cryptography;
using System.Globalization;
using System.Windows.Forms;
using Atdi.AppServer.AppService.WebQueryDataDriver;
using DatalayerCs;
using OrmCs;



namespace Atdi.AppServer.AppService.WebQueryDataDriver.ICSMUtilities
{


    public sealed class MassPoints
    {
        public double Deg { get; set; }
        public double Value { get; set; }

        public int Min { get; set; }
        public int Max { get; set; }
        public int Zero { get; set; }
        public int MiddleValue { get; set; }
        public bool FlagRemove { get; set; }
    }

    static public class ICSMUtils
    {
        /// <summary>
        /// Генерация хэш-функции по алгоритму SHA-256
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static public string SHA256(string input)
        {
            byte[] inputbytes = UTF8Encoding.UTF8.GetBytes(input);
            SHA256 sha = new SHA256CryptoServiceProvider();
            byte[] hash = sha.ComputeHash(inputbytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++) sb.Append(hash[i].ToString("X2"));
            return sb.ToString();
        }

     
       

      

    }
}
