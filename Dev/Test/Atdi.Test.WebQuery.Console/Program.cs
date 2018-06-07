using Atdi.Contracts.WcfServices.Identity;
using Atdi.Contracts.WcfServices.WebQuery;
using Atdi.DataModels.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Atdi.AppServices.WebQuery;
using Atdi.LegacyServices.Icsm;

namespace Atdi.Test.WebQuery
{
    class Program
    {
        //private static ChannelFactory<IAuthenticationManager> _tcpChannelFactory = null;
        //private static ChannelFactory<IAuthenticationManager> _httpChannelFactory = null;
        //private static ChannelFactory<IAuthenticationManager> _pipeChannelFactory = null;

        static void Main(string[] args)
        {
            while (true)
            {


                Console.WriteLine("Press any key to start testing ...");
                Console.ReadLine();

                TestAuthenticationManager("HttpAuthenticationManager");
                //BlockTest();

            }
            //BlockTest();

            //TestGetData();


            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
        }
        static void TestGetData()
        {
            Console.WriteLine("Press any key to start testing ...");
            Console.ReadLine();

            var tcpAuthManager = GetWebQueryByEndpoint("TcpWebQuery");
            var httpAuthManager = GetWebQueryByEndpoint("HttpWebQuery");
            var pipeAuthManager = GetWebQueryByEndpoint("PipeWebQuery");

            var result = tcpAuthManager.ExecuteQuery(null, null, null);
        }
        static void BlockTest()
        {
            Console.WriteLine("Press any key to start testing ...");
            Console.ReadLine();


            //var tcpAuthManager = GetWebQueryByEndpoint("TcpWebQuery");
            var httpAuthManager = GetWebQueryByEndpoint("HttpWebQuery");
            //var pipeAuthManager = GetWebQueryByEndpoint("PipeWebQuery");

            while (true)
            {
                //TestWebQuery(tcpAuthManager, "tcp");
                TestWebQuery(httpAuthManager, "http");
                //TestWebQuery(pipeAuthManager, "pipe");
                Console.WriteLine("Testing has been done.");
                Console.ReadLine();
                Console.WriteLine("Repeat ...");
            }
        }

        static IWebQuery GetWebQueryByEndpoint(string endpointName)
        {
            var f = new ChannelFactory<IWebQuery>(endpointName);
            return f.CreateChannel();
        }

        static IAuthenticationManager GetAuthenticationManagerByEndpoint(string endpointName)
        {
            var f = new ChannelFactory<IAuthenticationManager>(endpointName);
            return f.CreateChannel();
        }

        static void TestService(IAuthenticationManager authManager, string context)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            var c = new UserCredential();
            for (int i = 0; i < 1000; i++)
            {
                var tcpToken = authManager.AuthenticateUser(c);
            }


            timer.Stop();
            Console.WriteLine($"{context}: {timer.Elapsed.TotalMilliseconds} ms");
        }

        static void TestWebQuery(IWebQuery webQueryService, string context)
        {
            var timer = System.Diagnostics.Stopwatch.StartNew();
            var c = new UserCredential();
            var userToken = new UserToken();
            var queryToken = new DataModels.WebQuery.QueryToken();
            var fetchOptions = new DataModels.WebQuery.FetchOptions();
            var changeset = new DataModels.Changeset();
            for (int i = 0; i < 1; i++)
            {
                // var tree = webQueryService.GetQueriesTree(userToken);
                //  var metadata = webQueryService.GetQueryMetadata(userToken, new DataModels.WebQuery.QueryToken());
                var data = webQueryService.ExecuteQuery(null, queryToken, fetchOptions);



                ///
                /*
                var authManager = GetAuthenticationManagerByEndpoint("HttpAuthenticationManager");
                var c1 = new UserCredential()
                {
                    UserName = "Andrey",
                    Password = "P@ssw0rd"
                };

             
                var userIdentity = authManager.AuthenticateUser(c1);
                userIdentity.Data
                */


                //webQueryService.GetQueryGroups(userIdentity);
                //  Console.WriteLine(data.Data.Rows[0][0]);
                //  var result = webQueryService.SaveChanges(userToken, queryToken, changeset);
            }


            timer.Stop();
            Console.WriteLine($"{context}: {timer.Elapsed.TotalMilliseconds} ms");


            var timer2 = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < 1; i++)
            {
                // var tree = webQueryService.GetQueriesTree(userToken);
                //  var metadata = webQueryService.GetQueryMetadata(userToken, new DataModels.WebQuery.QueryToken());
                var data = webQueryService.ExecuteQuery(userToken, queryToken, fetchOptions);
                //  Console.WriteLine(data.Data.RowsAsString[0][0]);
                //  var result = webQueryService.SaveChanges(userToken, queryToken, changeset);
            }


            timer2.Stop();
            Console.WriteLine($"{context}: {timer2.Elapsed.TotalMilliseconds} ms");
        }

        static void TestAuthenticationManager(string endpointName)
        {


            var authManager = GetAuthenticationManagerByEndpoint(endpointName);
            var timer = System.Diagnostics.Stopwatch.StartNew();
            var c = new UserCredential()
            {
                UserName = "ICSM",
                Password = "ICSM"
            };


           var userIdentity = authManager.AuthenticateUser(c);

            //ReaderQueryDescriptor red = new ReaderQueryDescriptor();
            //red.GetAllXWebQuery();


            timer.Stop();
            Console.WriteLine($"AuthenticateUser: {timer.Elapsed.TotalMilliseconds} ms");
        }

    }
}
