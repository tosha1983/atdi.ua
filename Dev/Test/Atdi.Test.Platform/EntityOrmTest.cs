using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Sdrns.Server.Entities;
using Atdi.DataModels.Sdrns.Server.Entities.Monitoring;
using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atdi.Test.Platform
{
    class EntityOrmTest
    {
        public static void Run(IServicesResolver servicesResolver)
        {
            var dataLayer = servicesResolver.Resolve<IDataLayer<EntityDataOrm>>();
            // var builder = dataLayer.GetBuilder<ITestDataType>();
            // var executor = dataLayer.Executor<SdrnServerDataContext>();
            //for (int i = 0; i < 100; i++)
            //{
            //    Test_InsertPatterns(dataLayer);
            //}
            //Test_AssembliesContext(dataLayer);
            Test_InsertPatterns(dataLayer);
            //Test_SelectPatterns(dataLayer);
            //Test_UpdatePatterns(dataLayer);
            //Test_DeletePatterns(dataLayer);

            //Test_ReferenceFields(dataLayer);
            //Test_Boolean(builder, executor);
        }

        private static void Test_AssembliesContext(IDataLayer<EntityDataOrm> dataLayer)
        {
            var logQuery = dataLayer.GetBuilder<ILogEvent>()
                .From()
                .Select(c => c.Id, c => c.LevelCode, c => c.LevelName, c => c.Text)
                ;

            using (var scope = dataLayer.CreateScope(new SimpleDataContext("Platform")))
            {
                try
                {
                    var result = scope.Executor.ExecuteAndFetch(logQuery, reader => 
                    {
                        while(reader.Read())
                        {
                            try
                            {
                                var id = reader.GetValue(c => c.Id);
                                var levelCode = reader.GetValue(c => c.LevelCode);
                                var levelName = reader.GetValue(c => c.LevelName);
                                var text = reader.GetValue(c => c.Text);
                                Console.WriteLine($"Id = '{id}'; LevelCode = '{levelCode}'; LevelName = '{levelName}'; Text = '{text}';");
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                Console.ReadLine();
                            }
                            
                        }
                        Console.ReadLine();
                        return true;
                    });
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.ReadLine();
                }
            }

        }
        private static void Test_DeletePatterns(IDataLayer<EntityDataOrm> dataLayer)
        {
            var delete = dataLayer.GetBuilder<ITestEntityAbsSmpProtoEndExt1>()
                .Delete()
                .Where(c => c.AbsPkId1 == 123)
                .Where(c => c.Ext1ProtoEndField1 == "VVVV");

            using (var scope = dataLayer.CreateScope<SdrnServerDataContext>())
            {
                try
                {
                    var result = scope.Executor.Execute(delete);
                    Console.WriteLine($"Updeted count = {result}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.ReadLine();
                }

            }

            Console.ReadLine();

            var delete2 = dataLayer.GetBuilder<ITestEntityRef1>()
                .Delete()
                .Where(c => c.Ref1PkId1 == 123)
                .Where(c => c.REFTO2.Ref2Field1 == "VVVV2")
                .Where(c => c.REFTO2.REFTO3.Ref3Field1 == "VVVV3");

            using (var scope = dataLayer.CreateScope<SdrnServerDataContext>())
            {
                try
                {
                    var result = scope.Executor.Execute(delete2);
                    Console.WriteLine($"Updeted count = {result}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.ReadLine();
                }

            }

            Console.ReadLine();
        }
        private static void Test_UpdatePatterns(IDataLayer<EntityDataOrm> dataLayer)
        {
            var update = dataLayer.GetBuilder<ITestEntityAbsSmpProtoEnd>()
                .Update()
                //.SetValue(c => c.AbsField3, "AbsField3 (updated " + DateTime.Now + ")")
                //.SetValue(c => c.SmpField3, "SmpField3 (updated " + DateTime.Now + ")");
                //.SetValue(c => c.SMP_EXT1.Ext1SmpField3, "Ext1SmpField3 (updated)")
                //.SetValue(c => c.SMP_EXT2.Ext2SmpField3, "Ext2SmpField3 (updated)")
                //.SetValue(c => c.Proto0Field3, "Proto0Field3 (updated)")
                //.SetValue(c => c.PRT0_EXT1.Ext1Proto0Field3, "Ext1Proto0Field3 (updated)")
                //.SetValue(c => c.PRT0_EXT2.Ext2Proto0Field3, "Ext2Proto0Field3 (updated)")
                //.SetValue(c => c.PRT1_EXT1.Ext1Proto1Field3, "Ext1Proto1Field3 (updated)")
                //.SetValue(c => c.PRT1_EXT2.Ext2Proto1Field3, "Ext2Proto1Field3 (updated)")
                //.SetValue(c => c.Proto1Field3, "Proto1Field3 (updated)")
                //.SetValue(c => c.ProtoEndField3, "ProtoEndField3 (updated)")
                //.SetValue(c => c.PRTEND_EXT1.Ext1ProtoEndField3, "Ext1ProtoEndField3 (updated)")
                .SetValue(c => c.PRTEND_EXT2.Ext2ProtoEndField1, "Ext2ProtoEndField1 (updated " + DateTime.Now + ")")
                .SetValue(c => c.PRTEND_EXT2.Ext2ProtoEndField2, "Ext2ProtoEndField2 (updated " + DateTime.Now + ")")
                .SetValue(c => c.PRTEND_EXT2.Ext2ProtoEndField3, "Ext2ProtoEndField3 (updated " + DateTime.Now + ")")
                .Where(c => c.REFTO1N.Ref1Field1 == "Test_N")
                .Where(c => c.REFTO1R.Ref1Field1 == "Test_R");

            using (var scope = dataLayer.CreateScope<SdrnServerDataContext>())
            {
                try
                {
                    var result = scope.Executor.Execute(update);
                    Console.WriteLine($"Updeted count = {result}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.ReadLine();
                }

            }

            var update2 = dataLayer.GetBuilder<ITestEntityAbsSmpProtoEndExt1>()
                .Update()
                .SetValue(c => c.Ext1ProtoEndField1, "Field1 (updated " + DateTime.Now + ")")
                .SetValue(c => c.Ext1ProtoEndField2, "Field2 (updated " + DateTime.Now + ")")
                .SetValue(c => c.Ext1ProtoEndField3, "Field3 (updated " + DateTime.Now + ")");
            using (var scope = dataLayer.CreateScope<SdrnServerDataContext>())
            {
                try
                {
                    var result = scope.Executor.Execute(update2);
                    Console.WriteLine($"Updeted count = {result}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.ReadLine();
                }

            }

            Console.ReadLine();

        }
        private static void Test_SelectPatterns(IDataLayer<EntityDataOrm> dataLayer)
        {
            var refQuery = dataLayer.GetBuilder<ITestEntityRef1>()
                .From()
                .Select(c => c.REFTO2.Ref2Field2)
                .Select(c => c.REFTO2.REFTO3.Ref3Field3)
                .Select(c => c.REFTO2.REFTO3.REFPRTEND.AbsField1)
                .Select(c => c.REFTO2.REFTO3.REFPRTEND.SmpField1)
                .Select(c => c.REFTO2.REFTO3.REFPRTEND.Proto0Field1)
                .Select(c => c.REFTO2.REFTO3.REFPRTEND.Proto1Field1)
                .Select(c => c.REFTO2.REFTO3.REFPRTEND.ProtoEndField1)
                .Select(c => c.REFTO2.REFTO3.REFPRTEND.REFTO1N.Ref1Field1)
                .Select(c => c.REFTO2.REFTO3.REFPRTEND.REFTO1R.Ref1Field1)
                .Select(c => c.REFTO2.REFTO3.REFPRTEND.SMP_EXT1.Ext1SmpField1)
                .Select(c => c.REFTO2.REFTO3.REFPRTEND.PRT0_EXT1.Ext1Proto0Field1)
                .Select(c => c.REFTO2.REFTO3.REFPRTEND.PRT1_EXT1.Ext1Proto1Field1)
                .Select(c => c.REFTO2.REFTO3.REFPRTEND.PRTEND_EXT1.Ext1ProtoEndField1);

            using (var scope = dataLayer.CreateScope<SdrnServerDataContext>())
            {
                try
                {
                    var pk = scope.Executor.ExecuteAndFetch(refQuery, reader =>
                    {
                        while (reader.Read())
                        {
                            var cv1 = reader.GetValue(c => c.REFTO2.Ref2Field2);
                            var cv2 = reader.GetValue(c => c.REFTO2.REFTO3.Ref3Field3);
                            var cv3 = reader.GetValue(c => c.REFTO2.REFTO3.REFPRTEND.AbsField1);
                            var cv4 = reader.GetValue(c => c.REFTO2.REFTO3.REFPRTEND.SmpField1);
                            var cv5 = reader.GetValue(c => c.REFTO2.REFTO3.REFPRTEND.Proto0Field1);
                            var cv6 = reader.GetValue(c => c.REFTO2.REFTO3.REFPRTEND.Proto1Field1);
                            var cv7 = reader.GetValue(c => c.REFTO2.REFTO3.REFPRTEND.ProtoEndField1);

                            var cv8 = reader.GetValue(c => c.REFTO2.REFTO3.REFPRTEND.REFTO1N.Ref1Field1);
                            var cv9 = reader.GetValue(c => c.REFTO2.REFTO3.REFPRTEND.REFTO1R.Ref1Field1);
                            var cv10 = reader.GetValue(c => c.REFTO2.REFTO3.REFPRTEND.SMP_EXT1.Ext1SmpField1);
                            var cv11 = reader.GetValue(c => c.REFTO2.REFTO3.REFPRTEND.PRT0_EXT1.Ext1Proto0Field1);
                            var cv12 = reader.GetValue(c => c.REFTO2.REFTO3.REFPRTEND.PRT1_EXT1.Ext1Proto1Field1);
                            var cv13 = reader.GetValue(c => c.REFTO2.REFTO3.REFPRTEND.PRTEND_EXT1.Ext1ProtoEndField1);
                        }
                        return 0;
                    });
                    Console.WriteLine($"Id = {pk}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.ReadLine();
                }
                
            }

            Console.ReadLine();
            var selectArray = dataLayer.GetBuilder<ITestRefBook>()
                .From()
                .Select(c => c.Id)
                .Select(c => c.GuidId)
                .Select(c => c.Name)
                .Select(c => c.ArrayDouble)
                .OrderByDesc(c => c.Id);

            using (var scope = dataLayer.CreateScope<SdrnServerDataContext>())
            {
                var pk = scope.Executor.ExecuteAndFetch(selectArray, reader =>
                {
                    while (reader.Read())
                    {
                        var a = reader.GetValue(c => c.ArrayDouble);
                        //Console.WriteLine($"{a.Length}");
                    }
                    return 0;
                });
                Console.WriteLine($"Id = {pk}");
            }

            var select = dataLayer.GetBuilder<ITestEntityAbsSmpProtoEnd>()
                .From()
                .Distinct()
                .OnPercentTop(45)
                .Select(
                    c => c.AbsField1,
                    c => c.AbsField2,
                    c => c.AbsField3,

                    c => c.SmpField1,
                    c => c.SmpField2,
                    c => c.SmpField3,

                    c => c.SMP_EXT1.Ext1SmpField1,
                    c => c.SMP_EXT1.Ext1SmpField2,
                    c => c.SMP_EXT1.Ext1SmpField3,

                    c => c.SMP_EXT2.Ext2SmpField1,
                    c => c.SMP_EXT2.Ext2SmpField2,
                    c => c.SMP_EXT2.Ext2SmpField3,

                    c => c.Proto0Field1,
                    c => c.Proto0Field2,
                    c => c.Proto0Field3,

                    c => c.PRT0_EXT1.Ext1Proto0Field1,
                    c => c.PRT0_EXT1.Ext1Proto0Field2,
                    c => c.PRT0_EXT1.Ext1Proto0Field3,

                    c => c.PRT0_EXT2.Ext2Proto0Field1,
                    c => c.PRT0_EXT2.Ext2Proto0Field2,
                    c => c.PRT0_EXT2.Ext2Proto0Field3,

                    c => c.Proto1Field1,
                    c => c.Proto1Field2,
                    c => c.Proto1Field3,

                    c => c.PRT1_EXT1.Ext1Proto1Field1,
                    c => c.PRT1_EXT1.Ext1Proto1Field2,
                    c => c.PRT1_EXT1.Ext1Proto1Field3,

                    c => c.PRT1_EXT2.Ext2Proto1Field1,
                    c => c.PRT1_EXT2.Ext2Proto1Field2,
                    c => c.PRT1_EXT2.Ext2Proto1Field3,

                    c => c.ProtoEndField1,
                    c => c.ProtoEndField2,
                    c => c.ProtoEndField3,

                    c => c.PRTEND_EXT1.Ext1ProtoEndField1,
                    c => c.PRTEND_EXT1.Ext1ProtoEndField2,
                    c => c.PRTEND_EXT1.Ext1ProtoEndField3,

                    c => c.PRTEND_EXT2.Ext2ProtoEndField1,
                    c => c.PRTEND_EXT2.Ext2ProtoEndField2,
                    c => c.PRTEND_EXT2.Ext2ProtoEndField3
                )
                .OrderByAsc(c => c.ProtoEndField1)
                .OrderByDesc(c => c.Proto1Field3)
                .OrderByAsc(c =>c.Proto0Field3)
                .OrderByDesc(c => c.AbsField3);

            //select.Where(c => c.AbsPkId1 == 1 || c.AbsPkId1 == 2 || c.AbsPkId1 == 3);
            //select.Where(c => c.AbsField1 == "Test");
            //select.Where(c => c.SmpField1 == "Test");
            //select.Where(c => c.Proto0Field1 == "Test");
            //select.Where(c => c.Proto1Field1 == "Test");
            //select.Where(c => c.ProtoEndField1 == "Test");

            //select.Where(c => c.SMP_EXT1.Ext1SmpField2 == "Test" || c.SMP_EXT2.Ext2SmpField2 == "Test");
            //select.Where(c => c.PRT0_EXT1.Ext1Proto0Field2 == "Ext1Proto0Field2" || c.PRT0_EXT2.Ext2Proto0Field2 == "Ext2Proto0Field2");
            //select.Where(c => c.PRT1_EXT1.Ext1Proto1Field2 == "Ext1Proto1Field2" || c.PRT1_EXT2.Ext2Proto1Field2 == "Ext2Proto1Field2");
            //select.Where(c => c.PRTEND_EXT1.Ext1ProtoEndField2 == "Ext1ProtoEndField2" || c.PRTEND_EXT2.Ext2ProtoEndField2 == "Ext2ProtoEndField2");


            using (var scope = dataLayer.CreateScope<SdrnServerDataContext>())
            {
                var pk = scope.Executor.ExecuteAndFetch(select, reader => 
                {
                    while(reader.Read())
                    {
                        var s = string.Empty;
                        s = reader.GetValue(c => c.AbsField1);
                        s = reader.GetValue(c => c.AbsField2);
                        s = reader.GetValue(c => c.AbsField3);
                        s = reader.GetValue(c => c.SmpField1);
                        s = reader.GetValue(c => c.SmpField2);
                        s = reader.GetValue(c => c.SmpField3);
                        s = reader.GetValue(c => c.SMP_EXT1.Ext1SmpField1);
                        s = reader.GetValue(c => c.SMP_EXT1.Ext1SmpField2);
                        s = reader.GetValue(c => c.SMP_EXT1.Ext1SmpField3);
                        s = reader.GetValue(c => c.SMP_EXT2.Ext2SmpField1);
                        s = reader.GetValue(c => c.SMP_EXT2.Ext2SmpField2);
                        s = reader.GetValue(c => c.SMP_EXT2.Ext2SmpField3);


                    }
                    return 0;
                });
                Console.WriteLine($"Id = {pk}");
            }
        }

        private static void Test_InsertPatterns(IDataLayer<EntityDataOrm> dataLayer)
        {
            try
            {
                var insertToRef3 = dataLayer.GetBuilder<ITestEntityRef3>()
                .Insert()
                .SetValue(c => c.Ref3Field1, $"Ref 3: Test value {Guid.NewGuid()}")
                .SetValue(c => c.Ref3Field2, $"Ref 3: Test value {Guid.NewGuid()}")
                .SetValue(c => c.Ref3Field3, $"Ref 3: Test value {Guid.NewGuid()}");

                var ref3Pk = default(ITestEntityRef3_PK);
                using (var scope = dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    ref3Pk = scope.Executor.Execute<ITestEntityRef3_PK>(insertToRef3);
                }

                var insertToRef2 = dataLayer.GetBuilder<ITestEntityRef2>()
                    .Insert()
                    .SetValue(c => c.Ref2Field1, $"Ref 2: Test value {Guid.NewGuid()}")
                    .SetValue(c => c.Ref2Field2, $"Ref 2: Test value {Guid.NewGuid()}")
                    .SetValue(c => c.Ref2Field3, $"Ref 2: Test value {Guid.NewGuid()}");
                insertToRef2.SetValue(c => c.REFTO3.Ref3PkId1, ref3Pk.Ref3PkId1);
                insertToRef2.SetValue(c => c.REFTO3.Ref3PkId2, ref3Pk.Ref3PkId2);
                insertToRef2.SetValue(c => c.REFTO3.Ref3PkId3, ref3Pk.Ref3PkId3);

                var ref2Pk = default(ITestEntityRef2_PK);
                using (var scope = dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    ref2Pk = scope.Executor.Execute<ITestEntityRef2_PK>(insertToRef2);
                }

                var insertToRef1 = dataLayer.GetBuilder<ITestEntityRef1>()
                    .Insert()
                    .SetValue(c => c.Ref1Field1, $"Ref 1: Test value {Guid.NewGuid()}")
                    .SetValue(c => c.Ref1Field2, $"Ref 1: Test value {Guid.NewGuid()}")
                    .SetValue(c => c.Ref1Field3, $"Ref 1: Test value {Guid.NewGuid()}")
                    .SetValue(c => c.REFTO2.Ref2PkId1, ref2Pk.Ref2PkId1)
                    .SetValue(c => c.REFTO2.Ref2PkId2, ref2Pk.Ref2PkId2)
                    .SetValue(c => c.REFTO2.Ref2PkId3, ref2Pk.Ref2PkId3);

                var ref1Pk = default(ITestEntityRef1_PK);
                using (var scope = dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    ref1Pk = scope.Executor.Execute<ITestEntityRef1_PK>(insertToRef1);
                }

                var insert2 = dataLayer.GetBuilder<ITestEntityAbsSmpProtoEnd>()
                .Insert()
                // поля из абстрактной сущности
                .SetValue(c => c.AbsField1, "Abs Field 1")
                .SetValue(c => c.AbsField2, "Abs Field 2")
                .SetValue(c => c.AbsField3, "Abs Field 3")

                // ссілки
                .SetValue(c => c.REFTO1N.Ref1PkId1, ref2Pk.Ref2PkId1)
                .SetValue(c => c.REFTO1N.Ref1PkId2, ref2Pk.Ref2PkId2)
                .SetValue(c => c.REFTO1N.Ref1PkId3, ref2Pk.Ref2PkId3)

                .SetValue(c => c.REFTO1R.Ref1PkId1, ref2Pk.Ref2PkId1)
                .SetValue(c => c.REFTO1R.Ref1PkId2, ref2Pk.Ref2PkId2)
                .SetValue(c => c.REFTO1R.Ref1PkId3, ref2Pk.Ref2PkId3)

                // поля сущности Simple
                .SetValue(c => c.SmpField1, "Smp Field 1")
                .SetValue(c => c.SmpField2, "Smp Field 2")
                .SetValue(c => c.SmpField3, "Smp Field 3")
                // поля из расширения сущности Simple = Requered
                .SetValue(c => c.SMP_EXT1.Ext1SmpField1, "SMP_EXT1.Ext1SmpField1")
                .SetValue(c => c.SMP_EXT1.Ext1SmpField2, "SMP_EXT1.Ext1SmpField2")
                .SetValue(c => c.SMP_EXT1.Ext1SmpField3, "SMP_EXT1.Ext1SmpField3")
                // поля из расширения сущности Simple = Not Requered
                .SetValue(c => c.SMP_EXT2.Ext2SmpField1, "SMP_EXT2.Ext2SmpField1")
                .SetValue(c => c.SMP_EXT2.Ext2SmpField2, "SMP_EXT2.Ext2SmpField2")
                .SetValue(c => c.SMP_EXT2.Ext2SmpField3, "SMP_EXT2.Ext2SmpField3")


                .SetValue(c => c.Proto0Field1, "Proto 0 Field 1")
                .SetValue(c => c.Proto0Field2, "Proto 0 Field 2")
                .SetValue(c => c.Proto0Field3, "Proto 0 Field 3")

                .SetValue(c => c.PRT0_EXT1.Ext1Proto0Field1, "PRT0_EXT1.Ext1Proto0Field1")
                .SetValue(c => c.PRT0_EXT1.Ext1Proto0Field2, "PRT0_EXT1.Ext1Proto0Field2")
                .SetValue(c => c.PRT0_EXT1.Ext1Proto0Field3, "PRT0_EXT1.Ext1Proto0Field3")

                .SetValue(c => c.PRT0_EXT2.Ext2Proto0Field1, "PRT0_EXT2.Ext2Proto0Field1")
                .SetValue(c => c.PRT0_EXT2.Ext2Proto0Field2, "PRT0_EXT2.Ext2Proto0Field2")
                .SetValue(c => c.PRT0_EXT2.Ext2Proto0Field3, "PRT0_EXT2.Ext2Proto0Field3")

                .SetValue(c => c.Proto1Field1, "Proto 1 Field 1")
                .SetValue(c => c.Proto1Field2, "Proto 1 Field 2")
                .SetValue(c => c.Proto1Field3, "Proto 1 Field 3")

                .SetValue(c => c.PRT1_EXT1.Ext1Proto1Field1, "PRT0_EXT1.Ext1Proto1Field1")
                .SetValue(c => c.PRT1_EXT1.Ext1Proto1Field2, "PRT0_EXT1.Ext1Proto1Field2")
                .SetValue(c => c.PRT1_EXT1.Ext1Proto1Field3, "PRT0_EXT1.Ext1Proto1Field3")

                .SetValue(c => c.PRT1_EXT2.Ext2Proto1Field1, "PRT0_EXT2.Ext2Proto1Field1")
                .SetValue(c => c.PRT1_EXT2.Ext2Proto1Field2, "PRT0_EXT2.Ext2Proto1Field2")
                .SetValue(c => c.PRT1_EXT2.Ext2Proto1Field3, "PRT0_EXT2.Ext2Proto1Field3")

                .SetValue(c => c.ProtoEndField1, "Proto End Field 1")
                .SetValue(c => c.ProtoEndField2, "Proto End Field 2")
                .SetValue(c => c.ProtoEndField3, "Proto End Field 3")

                .SetValue(c => c.PRTEND_EXT1.Ext1ProtoEndField1, "PRTEND_EXT1.Ext1ProtoEndField1")
                .SetValue(c => c.PRTEND_EXT1.Ext1ProtoEndField2, "PRTEND_EXT1.Ext1ProtoEndField2")
                .SetValue(c => c.PRTEND_EXT1.Ext1ProtoEndField3, "PRTEND_EXT1.Ext1ProtoEndField3")

                .SetValue(c => c.PRTEND_EXT2.Ext2ProtoEndField1, "PRTEND_EXT2.Ext2ProtoEndField1")
                .SetValue(c => c.PRTEND_EXT2.Ext2ProtoEndField2, "PRTEND_EXT2.Ext2ProtoEndField2")
                .SetValue(c => c.PRTEND_EXT2.Ext2ProtoEndField3, "PRTEND_EXT2.Ext2ProtoEndField3");

                var protoEndPk = default(ITestEntityAbsSmpProtoEnd_PK);

                using (var scope = dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    try
                    {
                        protoEndPk = scope.Executor.Execute<ITestEntityAbsSmpProtoEnd_PK>(insert2);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Console.ReadLine();
                    }
                    
                    Console.WriteLine($"AbsPkId1 = {protoEndPk.AbsPkId1}, AbsPkId2 = {protoEndPk.AbsPkId2}, AbsPkId3 = {protoEndPk.AbsPkId3}");
                }


                //var insert = dataLayer.GetBuilder<ITestRefBook>()
                //.Insert()
                //.SetValue(c => c.Name, "Name")
                //.SetValue(c => c.SUBBOOK1.Code, "code_1")
                //.SetValue(c => c.SUBBOOK1.SubType, "sub_type_1");
                //insert.SetValue(c => c.ArrayDouble, new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });

                //using (var scope = dataLayer.CreateScope<SdrnServerDataContext>())
                //{
                //    var pk = scope.Executor.Execute<ITestRefBook_PK>(insert);
                //    //var pk2 = scope.Executor.Execute<ITestRefBook_PK>(insert);
                //    //var pk3 = scope.Executor.Execute<ITestRefBook_PK>(insert);
                //    Console.WriteLine($"Id = {pk.Id}, Guid = {pk.GuidId}");
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadLine();
                throw;
            }
            
        }

        private static void Test_ReferenceFields(IDataLayer<EntityDataOrm> dataLayer)
        {
            //var queryNoTyped = dataLayer.Builder
            //        .From("TestRefRoot")
            //        .Select("BOOK1.SUBBOOK1.EXT2.Prop1");

            var query = dataLayer.GetBuilder<ITestRefRoot>()
            .From()
            .Select(
                c => c.Id,
                c => c.BOOK1.Id,
                c => c.BOOK1.Name,
                c => c.BOOK1.SUBBOOK1.Code,
                c => c.BOOK1.SUBBOOK1.SubType,
                c => c.BOOK1.SUBBOOK1.Name,
                c => c.BOOK1.SUBBOOK1.EXT1.Prop1,
                c => c.BOOK1.SUBBOOK1.EXT1.Prop2,
                c => c.BOOK1.SUBBOOK1.EXT1.Prop3,
                c => c.BOOK1.SUBBOOK1.EXT2.Prop1,
                c => c.BOOK1.SUBBOOK1.EXT2.Prop2,
                c => c.BOOK1.SUBBOOK1.EXT2.Prop3,
                c => c.BOOK2.Id,
                c => c.BOOK2.Name,
                c => c.BOOK2.SUBBOOK1.Code,
                c => c.BOOK2.SUBBOOK1.SubType,
                c => c.BOOK2.SUBBOOK1.Name,
                c => c.BOOK2.SUBBOOK1.EXT1.Prop1,
                c => c.BOOK2.SUBBOOK1.EXT1.Prop2,
                c => c.BOOK2.SUBBOOK1.EXT1.Prop3,
                c => c.BOOK2.SUBBOOK1.EXT2.Prop1,
                c => c.BOOK2.SUBBOOK1.EXT2.Prop2,
                c => c.BOOK2.SUBBOOK1.EXT2.Prop3
            )
            .OrderByAsc(c => c.Id)
            .OrderByDesc(c => c.SecondBookId)
            .Where(c => c.Id > 0)
            .Where(c => c.BOOK2.Id > 0)
            .OnPercentTop(100)
            .Distinct();

            using (var scope = dataLayer.CreateScope<SdrnServerDataContext>())
            {
                var res = scope.Executor.Execute(query);
            }

            //dataLayer.Executor<SdrnServerDataContext>().Execute(query); // create new connaction #1, execute statement without trasaction, close connaction
            //dataLayer.Executor<SdrnServerDataContext>().Execute(query); // create new connaction #2, execute statement without trasaction, close connaction

            //using (var scope = dataLayer.CreateScope<SdrnServerDataContext>()) // try to open new connection #3
            //{
            //    scope.Executor.Execute(query); // connaction #3: execute statement without trasaction

            //    scope.BeginTran(); // connaction #3: scope has transaction

            //    scope.Executor.Execute(queryNoTyped); // connaction #3: execute statement with trasaction
            //    scope.Executor.Execute(query); // connaction #3: execute statement with trasaction


            //    scope.Commit(); // connaction #3:  commit trasaction
            //    // connaction #3: scope has no transaction

            //    scope.Executor.Execute(query); // connaction #3: execute statement without trasaction
            //} // close connection #3

            //dataLayer.Executor<SdrnServerDataContext>().Execute(query); // create new connaction #4

            //using (var scope = dataLayer.CreateScope<SdrnServerDataContext>()) // try to open new connection #5
            //{
            //    scope.Executor.Execute(query); // connaction #5: execute statement without trasaction
            //    scope.Executor.Execute(query); // connaction #5: execute statement without trasaction
            //}
            //using (var scope = dataLayer.CreateScope<SdrnServerDataContext>()) // try to open new connection #6
            //{
            //    scope.Executor.Execute(query); // connaction #6: execute statement without trasaction
            //    scope.Executor.Execute(query); // connaction #6: execute statement without trasaction
            //}


            //using (var scope = dataLayer.CreateScope<SdrnServerDataContext>()) // try to open new connection #1
            //{
            //    dataLayer.Executor<SdrnServerDataContext>().Execute(query); // create new connaction #2, execute statement without trasaction, close transaction
            //}

            //using (var scope = dataLayer.CreateScope<SdrnServerDataContext>()) // try to open new connection #7
            //{
            //    scope.Executor.Execute(query); // connaction #7: execute statement without trasaction
            //}
            //dataLayer.Executor<SdrnServerDataContext>().Execute(query); // create new connaction #7, execute statement without trasaction, close transaction

            //SqlConnection connection = new SqlConnection();

            //var tran2 = connection.BeginTransaction(System.Data.IsolationLevel.Serializable);
            //new SqlCommand()


            //var insertStatement = dataLayer.GetBuilder<ITestRefSubBook>()
            //    .InitProc()
            //    .SetParam()
            //    .Insert()
            //        .InsertChild()
                    
            //    .SetValue(c => c.Name, "Test")
            //    .Select(c => c.Code,);

            //var pk = dataLayer.Executor<SdrnServerDataContext>().Execute<ITestRefSubBook_PK>(insertStatement);
            

           //dataLayer.Executor<SdrnServerDataContext>().ExecuteAndFetch(insertStatement, ())
        }

        private static void Test_Boolean(IQueryBuilder<ITestDataType> builder, IQueryExecutor executor)
        {
            var insertQuery = builder
                .Insert()
                .SetValue(c => c.Boolean_BOOL_Bit, true)
                .SetValue(c => c.Boolean_BOOL_Byte, true)
                .SetValue(c => c.Boolean_BOOL_Char, true)
                .SetValue(c => c.Boolean_BOOL_Integer, true)
                .SetValue(c => c.Boolean_BOOL_Nchar, true)
                .SetValue(c => c.Boolean_BOOL_Nvarchar, true)
                .SetValue(c => c.Boolean_BOOL_Tinyint, true)
                .SetValue(c => c.Boolean_BOOL_Varchar, true);

            executor.Execute(insertQuery);

            insertQuery = builder
                .Insert()
                .SetValue(c => c.Boolean_BOOL_Bit, false)
                .SetValue(c => c.Boolean_BOOL_Byte, false)
                .SetValue(c => c.Boolean_BOOL_Char, false)
                .SetValue(c => c.Boolean_BOOL_Integer, false)
                .SetValue(c => c.Boolean_BOOL_Nchar, false)
                .SetValue(c => c.Boolean_BOOL_Nvarchar, false)
                .SetValue(c => c.Boolean_BOOL_Tinyint, false)
                .SetValue(c => c.Boolean_BOOL_Varchar, false);

            executor.Execute(insertQuery);

            var query = builder
                .From()
                .Select(
                    c => c.Id, 
                    c => c.Boolean_BOOL_Bit,
                    c => c.Boolean_BOOL_Byte,
                    c => c.Boolean_BOOL_Char,
                    c => c.Boolean_BOOL_Integer,
                    c => c.Boolean_BOOL_Nchar,
                    c => c.Boolean_BOOL_Nvarchar,
                    c => c.Boolean_BOOL_Tinyint,
                    c => c.Boolean_BOOL_Varchar
                );

            executor.Fetch(query, reader =>
            {
                while (reader.Read())
                {
                    var bool_bit = reader.GetValue(c => c.Boolean_BOOL_Bit);
                    var bool_byte = reader.GetValue(c => c.Boolean_BOOL_Byte);
                    var bool_char = reader.GetValue(c => c.Boolean_BOOL_Char);
                    var bool_int = reader.GetValue(c => c.Boolean_BOOL_Integer);
                    var bool_nchar = reader.GetValue(c => c.Boolean_BOOL_Nchar);
                    var bool_nvarchar = reader.GetValue(c => c.Boolean_BOOL_Nvarchar);
                    var bool_tityint = reader.GetValue(c => c.Boolean_BOOL_Tinyint);
                    var bool_varchar = reader.GetValue(c => c.Boolean_BOOL_Varchar);

                }
                return true;
            });
        }

        
    }
}
