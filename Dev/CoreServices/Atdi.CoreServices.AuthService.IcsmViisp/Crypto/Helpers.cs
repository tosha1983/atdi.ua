using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atdi.CoreServices.AuthService.IcsmViisp
{
    public class Helpers
    {
        public static int DecodeIntegerSize(System.IO.BinaryReader rd)
        {
            byte byteValue;
            int count;

            byteValue = rd.ReadByte();
            if (byteValue != 0x02)        
                return 0;

            byteValue = rd.ReadByte();
            if (byteValue == 0x81)
            {
                count = rd.ReadByte();    
            }
            else if (byteValue == 0x82)
            {
                byte hi = rd.ReadByte();  
                byte lo = rd.ReadByte();
                count = BitConverter.ToUInt16(new[] { lo, hi }, 0);
            }
            else
            {
                count = byteValue;        
            }

            while (rd.ReadByte() == 0x00)
            {
                count -= 1;
            }
            rd.BaseStream.Seek(-1, System.IO.SeekOrigin.Current);

            return count;
        }

        /// <summary>
        /// Парсер URL-строки
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ParseQueryString(String query)
        {
            var queryDict = new Dictionary<string, string>();
            var idx = query.TrimStart().LastIndexOf('?');
            if (idx > 0)
            {
                query = query.Remove(0, idx + 1);
                foreach (String token in query.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] parts = token.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        queryDict[parts[0].Trim()] = parts[1].Trim();
                    }
                    else
                    {
                        queryDict[parts[0].Trim()] = "";
                    }
                }
            }
            return queryDict;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pemString"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static byte[] GetBytesFromPEM(string pemString, PemStringType type)
        {
            string header; string footer;

            switch (type)
            {
                case PemStringType.Certificate:
                    header = "-----BEGIN CERTIFICATE-----";
                    footer = "-----END CERTIFICATE-----";
                    break;
                case PemStringType.RsaPrivateKey:
                    header = "-----BEGIN RSA PRIVATE KEY-----";
                    footer = "-----END RSA PRIVATE KEY-----";
                    break;
                default:
                    return null;
            }

            int start = pemString.IndexOf(header) + header.Length;
            int end = pemString.IndexOf(footer, start) - start;
            return Convert.FromBase64String(pemString.Substring(start, end));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputBytes"></param>
        /// <param name="alignSize"></param>
        /// <returns></returns>
        public static byte[] AlignBytes(byte[] inputBytes, int alignSize)
        {
            int    inputBytesSize = inputBytes.Length;

            if ((alignSize != -1) && (inputBytesSize < alignSize))
            {
                byte[] buf = new byte[alignSize];
                for (int i = 0; i < inputBytesSize; ++i)
                {
                    buf[i + (alignSize - inputBytesSize)] = inputBytes[i];
                }
                return buf;
            }
            else
            {
                return inputBytes;      
            }
        }
    }
}
