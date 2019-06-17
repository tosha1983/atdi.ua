using Atdi.Contracts.CoreServices.DataLayer;
using Atdi.Contracts.CoreServices.EntityOrm;
using Atdi.Contracts.Sdrn.Server;
using Atdi.DataModels.Sdrns.Server.Entities.Entities;
using Atdi.Platform.DependencyInjection;
using System;
using System.Collections.Generic;
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
            var builder = dataLayer.GetBuilder<ITestDataType>();
            var executor = dataLayer.Executor<SdrnServerDataContext>();

            Test_ReferenceFields(executor, dataLayer);
            //Test_Boolean(builder, executor);
        }

        private static void Test_ReferenceFields(IQueryExecutor executor, IDataLayer<EntityDataOrm> dataLayer)
        {
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
