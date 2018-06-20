﻿using Atdi.Contracts.WcfServices.Identity;
using Atdi.Contracts.WcfServices.WebQuery;
using Atdi.DataModels.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

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


                System.Console.WriteLine("Press any key to start testing ...");
                System.Console.ReadLine();

                //TestAuthenticationManager("TcpAuthenticationManager");
                SimpleUsingWebQuery2.Run();
                //TestWebQueryAccess("HttpAuthenticationManager", "HttpWebQuery");

            }
        }
        static void TestGetData()
        {
            System.Console.WriteLine("Press any key to start testing ...");
            System.Console.ReadLine();

            var tcpAuthManager = GetWebQueryByEndpoint("TcpWebQuery");
            var httpAuthManager = GetWebQueryByEndpoint("HttpWebQuery");
            var pipeAuthManager = GetWebQueryByEndpoint("PipeWebQuery");

            var result = tcpAuthManager.ExecuteQuery(null, null, null);
        }

        static void BlockTest()
        {
            System.Console.WriteLine("Press any key to start testing ...");
            System.Console.ReadLine();


            var tcpAuthManager = GetWebQueryByEndpoint("TcpWebQuery");
            var httpAuthManager = GetWebQueryByEndpoint("HttpWebQuery");
            var pipeAuthManager = GetWebQueryByEndpoint("PipeWebQuery");

            while (true)
            {
                TestWebQuery(tcpAuthManager, "tcp");
                TestWebQuery(httpAuthManager, "http");
                TestWebQuery(pipeAuthManager, "pipe");
                System.Console.WriteLine("Testing has been done.");
                System.Console.ReadLine();
                System.Console.WriteLine("Repeat ...");
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
            System.Console.WriteLine($"{context}: {timer.Elapsed.TotalMilliseconds} ms");
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
                //  Console.WriteLine(data.Data.Rows[0][0]);
                //  var result = webQueryService.SaveChanges(userToken, queryToken, changeset);
            }


            timer.Stop();
            System.Console.WriteLine($"{context}: {timer.Elapsed.TotalMilliseconds} ms");


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
            System.Console.WriteLine($"{context}: {timer2.Elapsed.TotalMilliseconds} ms");
        }

        static void TestAuthenticationManager(string endpointName)
        {
            var authManager = GetAuthenticationManagerByEndpoint(endpointName);
            var timer = System.Diagnostics.Stopwatch.StartNew();
            var c = new UserCredential()
            {
                UserName = "Andrey",
                Password = "P@ssw0rd"
            };

            for (int i = 0; i < 10; i++)
            {
                var userIdentity = authManager.AuthenticateUser(c);
            }


            timer.Stop();
            System.Console.WriteLine($"AuthenticateUser: {timer.Elapsed.TotalMilliseconds} ms");
        }

        static void TestWebQueryAccess(string authEndpointName, string webQueryEndpointName)
        {
            var authManager = GetAuthenticationManagerByEndpoint(authEndpointName);
            var userCredential = new UserCredential()
            {
                UserName = "ICSM",
                Password = "ICSM"
            };

            var authResult = authManager.AuthenticateUser(userCredential);
            if (authResult.State == DataModels.CommonOperation.OperationState.Fault)
            {
                Console.WriteLine(authResult.FaultCause);
                return;
            }

            var userIdentity = authResult.Data;

            var webQuery = GetWebQueryByEndpoint(webQueryEndpointName);
            var groupsResult = webQuery.GetQueryGroups(userIdentity.UserToken);
            if (groupsResult.State == DataModels.CommonOperation.OperationState.Fault)
            {
                Console.WriteLine(groupsResult.FaultCause);
                return;
            }

            Console.WriteLine($"Count group: {groupsResult.Data.Groups.Length}");
        }

    }
}