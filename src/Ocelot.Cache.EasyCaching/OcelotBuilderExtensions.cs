namespace Ocelot.Cache.EasyCaching
{
    using Configuration;
    using Configuration.File;
    using DependencyInjection;
    using global::EasyCaching.Core;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using System;

    public static class OcelotBuilderExtensions
    {
        public static IOcelotBuilder AddEasyCaching(this IOcelotBuilder builder, Action<OcelotEasyCachingOptions> setupAction)
        {
            builder.Services.RemoveAll(typeof(IOcelotCache<CachedResponse>));          
            builder.Services.RemoveAll(typeof(IOcelotCache<IInternalConfiguration>));
            builder.Services.RemoveAll(typeof(IOcelotCache<FileConfiguration>));

            var options = new OcelotEasyCachingOptions();
            setupAction(options);
            builder.Services.Configure(setupAction);
            builder.Services.AddEasyCaching(options.Settings);

            builder.Services.AddSingleton(typeof(IOcelotCache<>), typeof(OcelotEasyCachingCache<>));

            builder.Services.RemoveAll(typeof(ICacheKeyGenerator));
            builder.Services.AddSingleton<ICacheKeyGenerator, CacheKeyGenerator>();

            return builder;
        }
    }

   
}
