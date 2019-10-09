using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels;
using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.Platform
{
    class DataLayerTest
    {
        public static void Run(IServicesResolver servicesResolver)
        {
            Console.WriteLine("Press any key to start the Data Layer Test");
            Console.ReadLine();

            var dataLayer = servicesResolver.Resolve<IDataLayer>();
            var engine = dataLayer.GetDataEngine<SdrnServerDataContext>();

            for (int k = 0; k < 10; k++)
            {
                var insertCommand = BuildInsertCommand();
                try
                {
                    var res = engine.Execute(insertCommand);
                }
                catch (Exception e)
                {

                    Debug.WriteLine(e.ToString());
                }

                var command = new EngineCommand
                {
                    Text = "SELECT * FROM ICST.TEST_DATATYPES_SQL2"
                };

                try
                {
                    engine.Execute(command, (IEngineDataReader reader) =>
                    {

                        //var r = reader.NextResult();
                        int recId = 0;
                        while (reader.Read())
                        {
                            ++recId;
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var type = reader.GetFieldType(i);
                                var value = reader.GetValue(i, type);
                                var alias = reader.GetAlias(i);
                                var ordinalByAlias = reader.GetOrdinalByAlias(alias);
                                //var path = reader.GetPath(i);
                                //var ordinalByPath = reader.GetOrdinalByAlias(path);
                                var state = reader.ToString();
                                var len = 0;


                                //var type = reader.GetFieldType(i);
                                //var value = reader.GetValue(i, type);

                               

                                //var name = reader.GetName(i);
                                //if (name== "VAL_AS_UNIQUEIDENTIFIER")
                                //{

                                //}
                                //if (name == "VAL_AS_DATETIMEOFFSET")
                                //{

                                //}
                                //if (name == "VAL_AS_IMAGE")
                                //{
                                    //var stream = reader.GetStream(i);
                                //}
                                //if (name == "VAL_AS_XML")
                                //{
                                    //var stream = reader.GetXmlReader(i);
                                //}

                                


                                //var ordinal = reader.GetOrdinal(name);
                                //var state = reader.ToString();
                                //var valIsNull = false;
                                //var len = 0;
                                if (reader.IsDBNull(i))
                                {
                                }
                                else if (type == typeof(long))
                                {
                                    var val = reader.GetInt64(i);
                                }
                                else if (type == typeof(int))
                                {
                                    var val = reader.GetInt32(i);
                                }
                                else if (type == typeof(short))
                                {
                                    var val = reader.GetInt16(i);
                                }
                                else if (type == typeof(bool))
                                {
                                    var val = reader.GetBool(i);
                                }
                                else if (type == typeof(byte))
                                {
                                    var val = reader.GetByte(i);
                                }
                                else if (type == typeof(byte[]))
                                {
                                    var val = reader.GetBytes(i);
                                    len = val.Length;
                                }
                                else if (type == typeof(float))
                                {
                                    var val = reader.GetFloat(i);
                                }
                                else if (type == typeof(double))
                                {
                                    var val = reader.GetDouble(i);
                                }
                                else if (type == typeof(decimal))
                                {
                                    var val = reader.GetDecimal(i);
                                }
                                else if (type == typeof(string))
                                {
                                    var val = reader.GetString(i);
                                    len = val.Length;
                                }
                                else if (type == typeof(DateTime))
                                {
                                    var val = reader.GetDateTime(i);
                                }
                                else if (type == typeof(TimeSpan))
                                {
                                    var val = reader.GetTimeSpan(i);
                                }
                                else if (type == typeof(DateTimeOffset))
                                {
                                    var val = reader.GetDateTimeOffset(i);
                                }
                                else if (type == typeof(Guid))
                                {
                                    var val = reader.GetGuid(i);
                                }
                                else if (type == typeof(object))
                                {
                                    var val = reader.GetValue(i);
                                }
                                else
                                {
                                    throw new InvalidProgramException("Ухты, что то не то");
                                }

                                //Debug.WriteLine($"Record #{recId} : Alias = '{alias}', Path = '{path}', Len = {len}, OrdinalByA = #{ordinalByAlias}, Type = {type.Name}, IsNull = {valIsNull} ");
                            }
                        }

                    });
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            

            Console.ReadLine();

            //var count = 10000;

            //for (int j = 0; j < 10; j++)
            //{
            //    var timer = System.Diagnostics.Stopwatch.StartNew();

            //    for (int i = 0; i < count; i++)
            //    {
            //        var result = engine.ExecuteScalar(command);
            //    }

            //    timer.Stop();

            //    Debug.WriteLine($"OLD Way: {timer.ElapsedMilliseconds}");
            //    Console.WriteLine($"OLD Way: {timer.ElapsedMilliseconds}");

            //    timer = System.Diagnostics.Stopwatch.StartNew();

            //    for (int i = 0; i < count; i++)
            //    {
            //        var result = engine.ExecuteScalar(command);
            //    }
            //    timer.Stop();

            //    Debug.WriteLine($"NEW Way: {timer.ElapsedMilliseconds}");
            //    Console.WriteLine($"NEW Way: {timer.ElapsedMilliseconds}");
            //}
            

            Console.WriteLine("Press any key to start the Data Layer Test");
            Console.ReadLine();
        }

        private static EngineCommand BuildInsertCommand()
        {
            var sql = new StringBuilder();
            //sql.AppendLine("INSERT INTO [dbo].[TEST_DATATYPES_SQL] (");


            sql.AppendLine("INSERT INTO ICST.TEST_DATATYPES_SQL2 (");
            sql.AppendLine("val_as_bigint,"); //NUMBER(19)
            sql.AppendLine("val_as_float,"); //BINARY_DOUBLE
            sql.AppendLine("val_as_real,"); //BINARY_FLOAT
            sql.AppendLine("val_as_datetimeoffset,");//TIMESTAMP(7) WITH TIME ZONE
            sql.AppendLine("val_as_image,");//BLOB
            sql.AppendLine("val_as_bit,");
            sql.AppendLine("val_as_smallint,");
            sql.AppendLine("val_as_int,");
            sql.AppendLine("val_as_char_1,");
            sql.AppendLine("val_as_char_250,");

            sql.AppendLine("val_as_varchar_1,");
            sql.AppendLine("val_as_varchar_250,");
            sql.AppendLine("val_as_varchar_max,"); //CLOB
            sql.AppendLine("val_as_text,");

            sql.AppendLine("val_as_nchar_1,");
            sql.AppendLine("val_as_nchar_250,");
            sql.AppendLine("val_as_nvarchar_1,");
            sql.AppendLine("val_as_nvarchar_250,");
            sql.AppendLine("val_as_nvarchar_max,");
            sql.AppendLine("val_as_ntext,");

            sql.AppendLine("val_as_decimal,"); //NUMBER(38,10)
            sql.AppendLine("val_as_numeric,"); //NUMBER(38,10)
            sql.AppendLine("val_as_xml,");//NCLOB

            sql.AppendLine("val_as_datetime,");
            sql.AppendLine("val_as_smalldatetime,");
            sql.AppendLine("val_as_money,");
            sql.AppendLine("val_as_smallmoney,");
            sql.AppendLine("val_as_sql_variant,");
            sql.AppendLine("val_as_tinyint,");// NUMBER(3)
            sql.AppendLine("val_as_binary_1,"); // NUMBER(3)
            sql.AppendLine("val_as_binary_250,"); //BLOB, LONG RAW, RAW(250
            sql.AppendLine("val_as_varbinary_1,");
            sql.AppendLine("val_as_time,");
            sql.AppendLine("val_as_uniqueidentifier,"); //NCLOB
            sql.AppendLine("VAL_AS_DATETIMEOFFSET_LTZ"); //NCLOB
            





            sql.AppendLine(")");
            sql.AppendLine("VALUES(");
            sql.AppendLine(":val_as_bigint,");
            sql.AppendLine(":val_as_float,");
            sql.AppendLine(":val_as_real,");
            sql.AppendLine(":val_as_datetimeoffset,");
            sql.AppendLine(":val_as_image,");
            sql.AppendLine(":val_as_bit,");
            sql.AppendLine(":val_as_smallint,");
            sql.AppendLine(":val_as_int,");
            sql.AppendLine(":val_as_char_1,");
            sql.AppendLine(":val_as_char_250,");

            sql.AppendLine(":val_as_varchar_1,");
            sql.AppendLine(":val_as_varchar_250,");
            sql.AppendLine(":val_as_varchar_max,");
            sql.AppendLine(":val_as_text,");

            sql.AppendLine(":val_as_nchar_1,");
            sql.AppendLine(":val_as_nchar_250,");
            sql.AppendLine(":val_as_nvarchar_1,");
            sql.AppendLine(":val_as_nvarchar_250,");
            sql.AppendLine(":val_as_nvarchar_max,");
            sql.AppendLine(":val_as_ntext,");

            sql.AppendLine(":val_as_decimal,");
            sql.AppendLine(":val_as_numeric,");
            sql.AppendLine(":val_as_xml,");

            sql.AppendLine(":val_as_datetime,");
            sql.AppendLine(":val_as_smalldatetime,");
            sql.AppendLine(":val_as_money,");
            sql.AppendLine(":val_as_smallmoney,");
            sql.AppendLine(":val_as_sql_variant,");
            sql.AppendLine(":val_as_tinyint,");
            sql.AppendLine(":val_as_binary_1,");
            sql.AppendLine(":val_as_binary_250,");
            sql.AppendLine(":val_as_varbinary_1,");
            sql.AppendLine(":val_as_time,");
            sql.AppendLine(":val_as_uniqueidentifier,");
            sql.AppendLine(":VAL_AS_DATETIMEOFFSET_LTZ");
            




            sql.AppendLine(")");

            var insertCommand = new EngineCommand
            {
                Text = sql.ToString()
            };
            insertCommand.AddParameter("val_as_bigint", DataType.Long, long.MaxValue);
            insertCommand.AddParameter("val_as_float", DataType.Double, double.MaxValue);
            insertCommand.AddParameter("val_as_real", DataType.Float, float.MaxValue);
            insertCommand.AddParameter("val_as_datetimeoffset", DataType.DateTimeOffset, DateTimeOffset.Now);
            insertCommand.AddParameter("val_as_image", DataType.Bytes, BuildBytes(1024 * 1024));
            insertCommand.AddParameter("val_as_bit", DataType.Boolean, true);
            
            insertCommand.AddParameter("val_as_smallint", DataType.Short, short.MaxValue);
            insertCommand.AddParameter("val_as_int", DataType.Integer, 353464);
            insertCommand.AddParameter("val_as_char_1", DataType.Char, 'W');
            insertCommand.AddParameter("val_as_char_250", DataType.Chars, BuildChars(250));
            insertCommand.AddParameter("val_as_varchar_1", DataType.String, "W");
            insertCommand.AddParameter("val_as_varchar_250", DataType.String, BuildStringA(250));
            insertCommand.AddParameter("val_as_varchar_max", DataType.String, BuildStringA(4000));
            insertCommand.AddParameter("val_as_text", DataType.String, BuildString(1024 * 1024));

            insertCommand.AddParameter("val_as_nchar_1", DataType.Char, 'S');
            insertCommand.AddParameter("val_as_nchar_250", DataType.Chars, BuildChars(250));
            insertCommand.AddParameter("val_as_nvarchar_1", DataType.String, "A");
            insertCommand.AddParameter("val_as_nvarchar_250", DataType.String, BuildString(250));
            insertCommand.AddParameter("val_as_nvarchar_max", DataType.String, BuildString(4000));
            insertCommand.AddParameter("val_as_ntext", DataType.String, BuildString(1024 * 1024));

            insertCommand.AddParameter("val_as_decimal", DataType.Decimal, decimal.MaxValue / 10000000000); //  1234567890.0123456789m);//decimal.MaxValue);
            insertCommand.AddParameter("val_as_numeric", DataType.Decimal, decimal.MinValue / 10000000000);//decimal.MaxValue);
            insertCommand.AddParameter("val_as_xml", DataType.Xml, BuildXml());

            insertCommand.AddParameter("val_as_datetime", DataType.DateTime, DateTime.Now);
            insertCommand.AddParameter("val_as_smalldatetime", DataType.DateTime, DateTime.Now);
            insertCommand.AddParameter("val_as_money", DataType.Decimal, 922337203685477.5807M);
            insertCommand.AddParameter("val_as_smallmoney", DataType.Decimal, 214748.3647M);
            insertCommand.AddParameter("val_as_sql_variant", DataType.String, "sql_variant");
            insertCommand.AddParameter("val_as_tinyint", DataType.Byte, byte.MaxValue);
            insertCommand.AddParameter("val_as_binary_1", DataType.SignedByte, sbyte.MaxValue);
            insertCommand.AddParameter("val_as_binary_250", DataType.Bytes, BuildBytes(250));
            insertCommand.AddParameter("val_as_varbinary_1", DataType.Bytes, BuildBytes(2 * 1024));
            insertCommand.AddParameter("val_as_time", DataType.Time, TimeSpan.FromMinutes(10));

            insertCommand.AddParameter("val_as_uniqueidentifier", DataType.Guid,  Guid.NewGuid());
            insertCommand.AddParameter("VAL_AS_DATETIMEOFFSET_LTZ", DataType.DateTimeOffset, DateTimeOffset.Now);
            

            return insertCommand;
       
        }

        private static string BuildXml()
        {
            var xml = new StringBuilder();
            xml.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8"" ?>");
            xml.AppendLine(@"<DataType>");
            xml.AppendLine(@"</DataType>");
            var result = xml.ToString();
            
            return result;
        }
        private static byte[] BuildBytes(int count)
        {
            var result = new byte[count];
            var r = new Random();
            r.NextBytes(result);
            return result;
        }

        private static char[] BuildChars(int count)
        {
            var result = new char[count];
            var b = new byte[count];
            var r = new Random();
            r.NextBytes(b);
            for (int i = 0; i < count; i++)
            {
                result[i] = Convert.ToChar(b[i]);
            }
            return result;
        }

        private static string BuildString(int count)
        {
            var result = new char[count];
            var b = new byte[count];
            var r = new Random();
            r.NextBytes(b);
            for (int i = 0; i < count; i++)
            {
                result[i] = Convert.ToChar(b[i]);
            }
            var v  = string.Join("", result);
            v = new String(result);
            return v;
        }

        private static string BuildStringA(int count)
        {
            var result = new char[count];
            var b = new byte[count];
            var r = new Random();
            r.NextBytes(b);
            for (int i = 0; i < count; i++)
            {
                result[i] = 'V';
            }
            var v = string.Join("", result);
            v = new String(result);
            return v;
        }
    }
}
