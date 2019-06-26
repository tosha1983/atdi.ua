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

            var query = dataLayer.GetBuilder<ISensor>()
            .Insert()
            .Select(
                c => c.Id,
                c => c.CreatedBy
            )
            .SetValue(c => c.Id, 2)
            .SetValue(c => c.Name, "TEST9080")
            .SetValue(c => c.TechId, "TECH1078")
            .SetValue(c => c.CreatedBy,"ICSM67");

            //dataLayer.Executor<SdrnServerDataContext>().Execute(query); // create new connaction #1, execute statement without trasaction, close connaction
            //dataLayer.Executor<SdrnServerDataContext>().Execute(query); // create new connaction #2, execute statement without trasaction, close connaction

            using (var scope = dataLayer.CreateScope<SdrnServerDataContext>()) // try to open new connection #3
            {
                //scope.Executor.Execute(query); // connaction #3: execute statement without trasaction

                scope.BeginTran(); // connaction #3: scope has transaction

                //scope.Executor.Execute(queryNoTyped); // connaction #3: execute statement with trasaction
                var x = scope.Executor.Execute<ISensor_PK>(query); // connaction #3: execute statement with trasaction
                //scope.Executor.Execute(query); // connaction #3: execute statement with trasaction

                scope.Commit(); // connaction #3:  commit trasaction
                // connaction #3: scope has no transaction

                //scope.Executor.Execute(query); // connaction #3: execute statement without trasaction
            } // close connection #3

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
