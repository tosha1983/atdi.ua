using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Common.Helpers
{
    public static class Compressor
    {
        //private static void CopyTo(Stream src, Stream dest)
        //{
        //    byte[] bytes = new byte[4096];

        //    int cnt;

        //    while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
        //    {
        //        dest.Write(bytes, 0, cnt);
        //    }
        //}

        public static byte[] Compress(byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }
            if (bytes.Length == 0)
            {
                return new byte[] { };
            }

            //using (var msi = new MemoryStream(bytes))
            using (var compressedStream = new MemoryStream())
            {
                using (var gs = new GZipStream(compressedStream, CompressionMode.Compress))
                {
					gs.Write(bytes, 0, bytes.Length);
	                //CopyTo(msi, gs);
                }

                return compressedStream.ToArray();
            }
        }

        public static string Compress(string source)
        {
            var bytes = Encoding.UTF8.GetBytes(source);

            //using (var msi = new MemoryStream(bytes))
            using (var compressedStream = new MemoryStream())
            {
                using (var gs = new GZipStream(compressedStream, CompressionMode.Compress))
                {
	                gs.Write(bytes, 0, bytes.Length);
					//CopyTo(msi, gs);
                }

                return Convert.ToBase64String(compressedStream.ToArray());
            }
        }

        public static byte[] Decompress(byte[] bytes)
        {

            using (var compressedStream = new MemoryStream(bytes))
            using (var resultStream = new MemoryStream())
            {
                using (var gzipStream  = new GZipStream(compressedStream, CompressionMode.Decompress))
                {
	                gzipStream.CopyTo(resultStream);
				}

                return resultStream.ToArray();
            }
        }

        public static string Decompress(string source)
        {
            var bytes = Convert.FromBase64String(source);
            using (var compressedStream = new MemoryStream(bytes))
            using (var resultStream = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                {
					gzipStream.CopyTo(resultStream);
				}

                return Encoding.UTF8.GetString(resultStream.ToArray());
            }
        }
    }
}
