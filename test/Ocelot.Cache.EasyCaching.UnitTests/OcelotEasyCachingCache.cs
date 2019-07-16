using System;
using System.Collections.Generic;
using System.Text;

namespace Ocelot.Cache.EasyCaching.UnitTests
{
    using global::EasyCaching.Core;
    using global::EasyCaching.InMemory;
    using Microsoft.Extensions.Options;
    using Moq;
    using Ocelot.Cache.EasyCaching;
    using Shouldly;
    using System;
    using TestStack.BDDfy;
    using Xunit;

    public class OcelotEasyCachingCache
    {
        private OcelotEasyCachingCache<string> _ocelotEasyCaching;
        private Mock<IOptions<OcelotEasyCachingOptions>> _mockOptions;
        private Mock<IEasyCachingProviderFactory> _mockProviderFactory;
        private Mock<IEasyCachingProvider> _mockProvider;
        private string _key;
        private string _value;
        private string _resultGet;
        private TimeSpan _ttlSeconds;
        private string _region;

        public OcelotEasyCachingCache()
        {
            _mockOptions = new Mock<IOptions<OcelotEasyCachingOptions>>();
            _mockProviderFactory = new Mock<IEasyCachingProviderFactory>();
            _mockProvider = new Mock<IEasyCachingProvider>();

            _mockOptions.Setup(x => x.Value).Returns(new OcelotEasyCachingOptions()
            {
                EnableHybrid = false,
                ProviderName = "m1",
                Settings = y =>
                {
                    y.UseInMemory("m1");
                }
            });

            _mockProviderFactory.Setup(x => x.GetCachingProvider(It.IsAny<string>())).Returns(_mockProvider.Object);

            _ocelotEasyCaching = new OcelotEasyCachingCache<string>(_mockOptions.Object, _mockProviderFactory.Object);
        }

        [Fact]
        public void should_get_from_cache()
        {
            this.Given(x => x.GivenTheFollowingIsCached("someKey", "someRegion", "someValue"))
                .When(x => x.WhenIGetFromTheCache())
                .Then(x => x.ThenTheResultIs("someValue"))
                .BDDfy();
        }

        [Fact]
        public void should_add_to_cache()
        {
            this.When(x => x.WhenIAddToTheCache("someKey", "someValue", TimeSpan.FromSeconds(1)))
                .Then(x => x.ThenTheCacheIsCalledCorrectly())
                .BDDfy();
        }

        [Fact]
        public void should_delete_key_from_cache()
        {
            this.Given(_ => GivenTheFollowingRegion("fookey"))
                .When(_ => WhenIDeleteTheRegion("fookey"))
                .Then(_ => ThenTheRegionIsDeleted("fookey"))
                .BDDfy();
        }

        private void WhenIDeleteTheRegion(string region)
        {
            _ocelotEasyCaching.ClearRegion(region);
        }

        private void ThenTheRegionIsDeleted(string region)
        {
            _mockProvider
                .Verify(x => x.RemoveByPrefix(region), Times.Once);
        }

        private void GivenTheFollowingRegion(string key)
        {
            _ocelotEasyCaching.Add(key, "doesnt matter", TimeSpan.FromSeconds(10), "region");
        }

        private void WhenIAddToTheCache(string key, string value, TimeSpan ttlSeconds)
        {
            _key = key;
            _value = value;
            _ttlSeconds = ttlSeconds;                        

            _ocelotEasyCaching.Add(_key, _value, _ttlSeconds, "region");
        }

        private void ThenTheCacheIsCalledCorrectly()
        {
            _mockProvider
                .Verify(x => x.Set(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Once);
        }

        private void ThenTheResultIs(string expected)
        {
            _resultGet.ShouldBe(expected);
        }

        private void WhenIGetFromTheCache()
        {
            _resultGet = _ocelotEasyCaching.Get(_key, _region);
        }

        private void GivenTheFollowingIsCached(string key, string region, string value)
        {
            _key = key;
            _value = value;
            _region = region;
            
            _mockProvider.Setup(x => x.Get<string>(It.IsAny<string>())).Returns(new CacheValue<string>(value, true));
        }
    }
}
