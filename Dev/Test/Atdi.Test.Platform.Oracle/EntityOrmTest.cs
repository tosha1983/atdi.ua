using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
//using Atdi.DataModels.Sdrns.Server.Entities.Entities;
using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atdi.DataModels.Sdrns.Server.Entities;

namespace Atdi.Test.Platform
{
    class EntityOrmTest
    {
        public static void Run(IServicesResolver servicesResolver)
        {
         
            var dataLayer = servicesResolver.Resolve<IDataLayer<EntityDataOrm>>();
            //var builder = dataLayer.GetBuilder<ITestDataType>();
            var executor = dataLayer.Executor<SdrnServerDataContext>();

            Test_ReferenceFields(executor, dataLayer);
            //Test_Boolean(builder, executor);
            //Test_InsertPatternsNew(dataLayer);
        }

        private static void Test_InsertPatternsNew(IDataLayer<EntityDataOrm> dataLayer)
        {

            try
            {
                var insert2 = dataLayer.GetBuilder<ITestEntityAbsSmpProtoEnd>()
                .Insert()
                // поля из абстрактной сущности
                .SetValue(c => c.AbsField1, "Abs Field1")
                .SetValue(c => c.AbsField2, "Abs Field2")
                .SetValue(c => c.AbsField3, "Abs Field3")
                // поля сущности Simple
                .SetValue(c => c.SmpField1, "Smp Field1")
                .SetValue(c => c.SmpField2, "Smp Field2")
                .SetValue(c => c.SmpField3, "Smp Field3")
                // поля из расширения сущности Simple = Requered
                .SetValue(c => c.SMP_EXT1.Ext1SmpField1, "1SmpField1")
                .SetValue(c => c.SMP_EXT1.Ext1SmpField2, "1SmpField2")
                .SetValue(c => c.SMP_EXT1.Ext1SmpField3, "1SmpField3")
                // поля из расширения сущности Simple = Not Requered
                .SetValue(c => c.SMP_EXT2.Ext2SmpField1, "2SmpField1")
                .SetValue(c => c.SMP_EXT2.Ext2SmpField2, "2.Ext2SmpField2")
                .SetValue(c => c.SMP_EXT2.Ext2SmpField3, "2.Ext2SmpField3")


                .SetValue(c => c.Proto0Field1, "Proto0F1")
                .SetValue(c => c.Proto0Field2, "Proto0F2")
                .SetValue(c => c.Proto0Field3, "Proto0F3")

                .SetValue(c => c.PRT0_EXT1.Ext1Proto0Field1, "P0_E1.0F1")
                .SetValue(c => c.PRT0_EXT1.Ext1Proto0Field2, "P0_E1.0F2")
                .SetValue(c => c.PRT0_EXT1.Ext1Proto0Field3, "P0_E1.0F3")

                .SetValue(c => c.PRT0_EXT2.Ext2Proto0Field1, "P0_E2.2F1")
                .SetValue(c => c.PRT0_EXT2.Ext2Proto0Field2, "P0_E2.2F2")
                .SetValue(c => c.PRT0_EXT2.Ext2Proto0Field3, "P0_E2.2F3")

                .SetValue(c => c.Proto1Field1, "P1Fi 1")
                .SetValue(c => c.Proto1Field2, "P1Fi 2")
                .SetValue(c => c.Proto1Field3, "P1Fi 3")

                .SetValue(c => c.PRT1_EXT1.Ext1Proto1Field1, "P0_T1.t111")
                .SetValue(c => c.PRT1_EXT1.Ext1Proto1Field2, "P0_T1.t112")
                .SetValue(c => c.PRT1_EXT1.Ext1Proto1Field3, "P0_T1.t113")

                .SetValue(c => c.PRT1_EXT2.Ext2Proto1Field1, "P0_T2.t111")
                .SetValue(c => c.PRT1_EXT2.Ext2Proto1Field2, "P0_T2.t112")
                .SetValue(c => c.PRT1_EXT2.Ext2Proto1Field3, "P0_T1.t113")

                .SetValue(c => c.ProtoEndField1, "PEndF1")
                .SetValue(c => c.ProtoEndField2, "PEndF2")
                .SetValue(c => c.ProtoEndField3, "PEndF3")

                .SetValue(c => c.PRTEND_EXT1.Ext1ProtoEndField1, "PRTEND1_1")
                .SetValue(c => c.PRTEND_EXT1.Ext1ProtoEndField2, "PRTEND1_3")
                .SetValue(c => c.PRTEND_EXT1.Ext1ProtoEndField3, "PRTEND1_4")

                .SetValue(c => c.PRTEND_EXT2.Ext2ProtoEndField1, "PRTEND2_1")
                .SetValue(c => c.PRTEND_EXT2.Ext2ProtoEndField2, "PRTEND2_2")
                .SetValue(c => c.PRTEND_EXT2.Ext2ProtoEndField3, "PRTEND2_3");

                using (var scope = dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    var pk = scope.Executor.Execute<ITestEntityAbsSmp_PK>(insert2);
                    Console.WriteLine($"AbsPkId1 = {pk.AbsPkId1}, AbsPkId2 = {pk.AbsPkId2}, AbsPkId3 = {pk.AbsPkId3}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private static void Test_ReferenceFields(IQueryExecutor executor, IDataLayer<EntityDataOrm> dataLayer)
        {

            /*
            var query = dataLayer.GetBuilder<ISensor>()
            .From()
            .Select(
                c => c.Id,
                c => c.Agl,
                c => c.CreatedBy
            )
            .OrderByAsc(c => c.Id)
            .Where(c => c.Id > 0)
            .Distinct();
            */
            var queryExecuter = dataLayer.Executor<SdrnServerDataContext>();

            var query3 = dataLayer.GetBuilder<ISensor>()
           .Delete()
           .Where(c => c.Id, DataModels.DataConstraint.ConditionOperator.GreaterEqual, 0);

            var query1 = dataLayer.GetBuilder<ISensor>()
            .Insert()
            .Select(
                c => c.Id,
                c => c.CreatedBy
            )
            .SetValue(c => c.Id, 2)
            .SetValue(c => c.Name, "TEST90801")
            .SetValue(c => c.TechId, "TECH1078")
            .SetValue(c => c.CreatedBy,"ICSM67");

            var query2 = dataLayer.GetBuilder<ITestEntityAbs>()
            .Insert()
            .Select(
                c => c.AbsField1,
                c => c.AbsField2,
                c => c.AbsField3
            )
            .SetValue(c => c.AbsPkId1, 2)
            .SetValue(c => c.AbsPkId2, Guid.NewGuid())
            .SetValue(c => c.AbsPkId3, new DateTimeOffset(DateTime.Now))
            .SetValue(c => c.AbsField1, "F1");


            //dataLayer.Executor<SdrnServerDataContext>().Execute(query); // create new connaction #1, execute statement without trasaction, close connaction
            //dataLayer.Executor<SdrnServerDataContext>().Execute(query3); // create new connaction #2, execute statement without trasaction, close connaction

           
            //queryExecuter.Execute(query3);


            using (var scope = dataLayer.CreateScope<SdrnServerDataContext>()) // try to open new connection #3
            {
                //scope.Executor.Execute(query); // connaction #3: execute statement without trasaction

                scope.BeginTran(); // connaction #3: scope has transaction

                //scope.Executor.Execute(queryNoTyped); // connaction #3: execute statement with trasaction
                var x1 = scope.Executor.Execute<ISensor_PK>(query1); // connaction #3: execute statement with trasaction
                //var x2 = scope.Executor.Execute<ITestEntityAbs_PK>(query2); // connaction #3: execute statement with trasaction
                //scope.Executor.Execute(query2); // connaction #3: execute statement with trasaction
                //scope.Executor.Execute(query); // connaction #3: execute statement with trasaction

                scope.Commit(); // connaction #3:  commit trasaction
                // connaction #3: scope has no transaction

                //scope.Executor.Execute(query); // connaction #3: execute statement without trasaction
            } // close connection #3
            



            /*
            using (var scope = dataLayer.CreateScope<SdrnServerDataContext>()) // try to open new connection #3
            {
                scope.Executor.ExecuteAndFetch(query, reader =>
                {
                    reader.Read();
                    var x = reader.GetValue(c => c.Id);
                    return true;
                });
            }
            */
                /*
                dataLayer.Executor<SdrnServerDataContext>().Execute(query); // create new connaction #4

                using (var scope = dataLayer.BeginScope<SdrnServerDataContext>()) // try to open new connection #5
                {
                    scope.Executor.Execute(query); // connaction #5: execute statement without trasaction
                    scope.Executor.Execute(query); // connaction #5: execute statement without trasaction
                }
                using (var scope = dataLayer.BeginScope<SdrnServerDataContext>()) // try to open new connection #6
                {
                    scope.Executor.Execute(query); // connaction #6: execute statement without trasaction
                    scope.Executor.Execute(query); // connaction #6: execute statement without trasaction
                }


                using (var scope = dataLayer.BeginScope<SdrnServerDataContext>()) // try to open new connection #1
                {
                    dataLayer.Executor<SdrnServerDataContext>().Execute(query); // create new connaction #2, execute statement without trasaction, close transaction
                }

                using (var scope = dataLayer.BeginScope<SdrnServerDataContext>()) // try to open new connection #7
                {
                    scope.Executor.Execute(query); // connaction #7: execute statement without trasaction
                }
                dataLayer.Executor<SdrnServerDataContext>().Execute(query); // create new connaction #7, execute statement without trasaction, close transaction

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
               */
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
