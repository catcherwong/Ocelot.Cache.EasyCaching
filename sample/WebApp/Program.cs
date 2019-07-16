namespace WebApp
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Ocelot.DependencyInjection;
    using Ocelot.Middleware;
    using Ocelot.Cache.EasyCaching;
    using EasyCaching.InMemory;
    using EasyCaching.CSRedis;
    using EasyCaching.Bus.CSRedis;
    using EasyCaching.HybridCache;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
             .UseUrls("http://*:9000")
             .ConfigureAppConfiguration((hostingContext, config) =>
             {
                 config
                     .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                     .AddJsonFile("ocelot.json")
                     .AddEnvironmentVariables();
             })
            .ConfigureServices(services =>
            {
                services.AddOcelot()
                            .AddEasyCaching(x =>
                            {
                                // single
                                x.EnableHybrid = false;
                                x.ProviderName = "m1";
                                x.Settings = y =>
                                {
                                    y.UseInMemory("m1");
                                };

                                //// hybrid
                                //x.IsHybird = true;
                                //x.ProviderName = "";
                                //x.Action = y => 
                                //{
                                //    y.UseInMemory("m2");
                                //    y.UseCSRedis(z => 
                                //    {
                                //        z.DBConfig = new CSRedisDBOptions
                                //        {
                                //            ConnectionStrings = new System.Collections.Generic.List<string>
                                //            {
                                //                "127.0.0.1:6379,defaultDatabase=11,poolsize=10"
                                //            }
                                //        };
                                //    }, "r2");

                                //    y.UseHybrid(z => 
                                //    {
                                //        z.LocalCacheProviderName = "m2";
                                //        z.DistributedCacheProviderName = "r2";
                                //        z.TopicName = "caching-bus";                                        
                                //    });

                                //    y.WithCSRedisBus(z => 
                                //    {
                                //        z.ConnectionStrings = new System.Collections.Generic.List<string>
                                //        {
                                //            "127.0.0.1:6379,defaultDatabase=10,poolsize=10"
                                //        };
                                //    });
                                //};
                            });
            })
            .Configure(app =>
            {
                app.UseOcelot().Wait();
            });
    }
}
