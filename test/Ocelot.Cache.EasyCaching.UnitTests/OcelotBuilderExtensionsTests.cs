namespace Ocelot.Cache.EasyCaching.UnitTests
{
    using Cache;
    using DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Shouldly;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TestStack.BDDfy;
    using Xunit;

    public class OcelotBuilderExtensionsTests
    {
        private readonly IServiceCollection _services;
        private readonly IConfiguration _configRoot;
        private IOcelotBuilder _ocelotBuilder;
        private Exception _ex;

        public OcelotBuilderExtensionsTests()
        {
            _configRoot = new ConfigurationRoot(new List<IConfigurationProvider>());
            _services = new ServiceCollection();
            _services.AddSingleton(_configRoot);
        }

        [Fact]
        public void should_set_up_cache_manager()
        {
            this.Given(x => WhenISetUpOcelotServices())
                .When(x => WhenISetUpEasyCaching())
                .Then(x => ThenAnExceptionIsntThrown())
                .And(x => OnlyOneVersionOfCacheIsRegistered())
                .BDDfy();
        }

        private void OnlyOneVersionOfCacheIsRegistered()
        {
            var ocelotCache = _services.Single(x => x.ServiceType == typeof(IOcelotCache<>));

            ocelotCache.ShouldNotBeNull();
        }

        private void WhenISetUpOcelotServices()
        {
            try
            {
                _ocelotBuilder = _services.AddOcelot(_configRoot);
            }
            catch (Exception e)
            {
                _ex = e;
            }
        }

        private void WhenISetUpEasyCaching()
        {
            try
            {
                _ocelotBuilder.AddEasyCaching(x => {
                    x.EnableHybrid = false;
                    x.ProviderName = "m1";
                    x.Settings = y =>
                    {
                        y.UseInMemory("m1");
                    };
                });
            }
            catch (Exception e)
            {
                _ex = e;
            }
        }

        private void ThenAnExceptionIsntThrown()
        {
            _ex.ShouldBeNull();
        }
    }
}
