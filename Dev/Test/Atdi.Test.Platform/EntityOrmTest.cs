using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Sdrns.Server.Entities;
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

            Test_InsertPatterns(dataLayer);
            //Test_ReferenceFields(dataLayer);
            //Test_Boolean(builder, executor);
        }

        private static void Test_InsertPatterns(IDataLayer<EntityDataOrm> dataLayer)
        {
            //var insert = dataLayer.GetBuilder<ITestRefBook>()
            //    .Insert()
            //    .SetValue(c => c.Name, "Name")
            //    .SetValue(c => c.SUBBOOK1.Code, "code_1")
            //    .SetValue(c => c.SUBBOOK1.SubType, "sub_type_1");

            //using (var scope = dataLayer.CreateScope<SdrnServerDataContext>())
            //{
            //   var pk = scope.Executor.Execute<ITestRefBook_PK>(insert);
            //    var pk2 = scope.Executor.Execute<ITestRefBook_PK>(insert);
            //    var pk3 = scope.Executor.Execute<ITestRefBook_PK>(insert);
            //    Console.WriteLine($"Id = {pk.Id}, Guid = {pk.GuidId}");
            //}

            try
            {
                var insert2 = dataLayer.GetBuilder<ITestEntityAbsSmpProtoEnd>()
                .Insert()
                .SetValue(c => c.AbsField1, "Abs Field 1")
                .SetValue(c => c.AbsField2, "Abs Field 2")
                .SetValue(c => c.AbsField3, "Abs Field 3")

                .SetValue(c => c.SmpField1, "Smp Field 1")
                .SetValue(c => c.SmpField2, "Smp Field 2")
                .SetValue(c => c.SmpField3, "Smp Field 3")

                .SetValue(c => c.Proto0Field1, "Proto 0 Field 1")
                .SetValue(c => c.Proto0Field2, "Proto 0 Field 2")
                .SetValue(c => c.Proto0Field3, "Proto 0 Field 3")

                .SetValue(c => c.Proto1Field1, "Proto 1 Field 1")
                .SetValue(c => c.Proto1Field2, "Proto 1 Field 2")
                .SetValue(c => c.Proto1Field3, "Proto 1 Field 3")

                .SetValue(c => c.ProtoEndField1, "Proto End Field 1")
                .SetValue(c => c.ProtoEndField2, "Proto End Field 2")
                .SetValue(c => c.ProtoEndField3, "Proto End Field 3");

                using (var scope = dataLayer.CreateScope<SdrnServerDataContext>())
                {
                    var pk = scope.Executor.Execute<ITestEntityAbsSmpProtoEnd_PK>(insert2);
                    Console.WriteLine($"AbsPkId1 = {pk.AbsPkId1}, AbsPkId2 = {pk.AbsPkId2}, AbsPkId3 = {pk.AbsPkId3}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
