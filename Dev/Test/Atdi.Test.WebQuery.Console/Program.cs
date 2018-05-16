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
            Console.WriteLine("Press any key to start testing ...");
            Console.ReadLine();
            var u = new Uri("net.tcp://localhost:8734/Atdi/WcfServices/AuthenticationManager/");
            var tcpAuthManager = GetWebQueryByEndpoint("TcpWebQuery");
            var httpAuthManager = GetWebQueryByEndpoint("HttpWebQuery");
            var pipeAuthManager = GetWebQueryByEndpoint("PipeWebQuery");

            while(true)
            {
                TestWebQuery(tcpAuthManager, "tcp");
                TestWebQuery(httpAuthManager, "http");
                TestWebQuery(pipeAuthManager, "pipe");
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
            var changeset = new DataModels.WebQuery.Changeset();
            for (int i = 0; i < 1; i++)
            {
                var tree = webQueryService.GetQueriesTree(userToken);
                var metadata = webQueryService.GetQueryMetadata(userToken, new DataModels.WebQuery.QueryToken());
                var data = webQueryService.ExecuteQuery(userToken, queryToken, fetchOptions);
                var result = webQueryService.SaveChanges(userToken, queryToken, changeset);
            }


            timer.Stop();
            Console.WriteLine($"{context}: {timer.Elapsed.TotalMilliseconds} ms");
        }
    }
}
