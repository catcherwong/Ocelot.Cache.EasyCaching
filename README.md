# Ocelot.Cache.EasyCaching

[![nuget](https://img.shields.io/nuget/v/Ocelot.Cache.EasyCaching.svg)](https://www.nuget.org/packages/Ocelot.Cache.EasyCaching/)  [![GitHub license](https://img.shields.io/github/license/catcherwong/Ocelot.Cache.EasyCaching.svg)](https://github.com/catcherwong/Ocelot.Cache.EasyCaching/blob/master/LICENSE)

This package adds [EasyCaching](https://github.com/dotnetcore/EasyCaching) support to Ocelot.

## How to install

Install it via Nuget.

`Install-Package Ocelot.Cache.EasyCaching`

Or via the .NET Core CLI:

`dotnet add package Ocelot.Cache.EasyCaching`


## How to use

Config `EasyCaching` in  `ConfigureServices`

```cs
public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseUrls("http://*:9000")
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
            });        
```
