namespace Ocelot.Cache.EasyCaching.UnitTests
{
    using global::EasyCaching.Core;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Moq;
    using Ocelot.Cache.EasyCaching;
    using Shouldly;
    using System;
    using TestStack.BDDfy;
    using Xunit;

    public class OcelotEasyCachingCacheHybrid
    {
        private readonly OcelotEasyCachingCache<string> _ocelotEasyCaching;
        private readonly Mock<IHybridCachingProvider> _mockHybridProvider;
        private string _key;
        private string _value;
        private string _resultGet;
        private TimeSpan _ttlSeconds;
        private string _region;
        
        public OcelotEasyCachingCacheHybrid()
        {
            var mockOptions = new Mock<IOptions<OcelotEasyCachingOptions>>();
            var mockProviderFactory = new Mock<IEasyCachingProviderFactory>();
            var mockHybridProviderFactory = new Mock<IHybridProviderFactory>();
            var mockMemoryProvider = new Mock<IEasyCachingProvider>();
            var mockDistributedProvider = new Mock<IEasyCachingProvider>();
            
            _mockHybridProvider = new Mock<IHybridCachingProvider>();

            mockOptions.Setup(x => x.Value).Returns(new OcelotEasyCachingOptions
            {
                EnableHybrid = true,
                ProviderName = "m1",
                HybridName = "h1",
                Settings = y =>
                {
                    y.UseInMemory("m1");
                    y.UseInMemory("d1");
                    y.UseHybrid(z =>
                    {
                        z.LocalCacheProviderName = "m1";
                        z.DistributedCacheProviderName = "d1";
                    },"h1");
                }
            });

            mockProviderFactory.Setup(x => x.GetCachingProvider(It.Is<string>(key => string.Equals("m1", key)))).Returns(mockMemoryProvider.Object);
            mockProviderFactory.Setup(x => x.GetCachingProvider(It.Is<string>(key => string.Equals("d1", key)))).Returns(mockDistributedProvider.Object);
            mockHybridProviderFactory.Setup(x => x.GetHybridCachingProvider(It.Is<string>(key => string.Equals("h1", key)))).Returns(_mockHybridProvider.Object);

            mockDistributedProvider.Setup(x => x.IsDistributedCache).Returns(true);

            _ocelotEasyCaching = new OcelotEasyCachingCache<string>(mockOptions.Object, mockProviderFactory.Object, mockHybridProviderFactory.Object);
        }

        [Fact]
        public void should_get_from_cache()
        {
            this.Given(x => x.GivenTheFollowingIsCachedInDistributedCache("someKey", "someRegion", "someValue"))
                .When(x => x.WhenIGetFromTheCache())
                .Then(x => x.ThenTheResultIs("someValue"))
                .BDDfy();
        }

        [Fact]
        public void should_add_to_cache()
        {
            this.When(x => x.WhenIAddToTheCache("someKey", "someValue", TimeSpan.FromSeconds(1)))
                .Then(x => x.ThenTheCacheIsCalledCorrectly("someKey", "someValue", TimeSpan.FromSeconds(1)))
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
            _mockHybridProvider
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

        private void ThenTheCacheIsCalledCorrectly(string key, string value, TimeSpan ttlSeconds)
        {
            _mockHybridProvider
                .Verify(x => x.Set(It.Is<string>(k => k == $"region:{key}"),It.Is<string>(v => v == value), It.Is<TimeSpan>(ttl => ttl == ttlSeconds)), Times.Once);
        }

        private void ThenTheResultIs(string expected)
        {
            _resultGet.ShouldBe(expected);
        }

        private void WhenIGetFromTheCache()
        {
            _resultGet = _ocelotEasyCaching.Get(_key, _region);
        }

        private void GivenTheFollowingIsCachedInDistributedCache(string key, string region, string value)
        {
            _key = key;
            _value = value;
            _region = region;

            _mockHybridProvider.Setup(x => x.Get<string>(It.Is<string>(k => k == $"{region}:{key}"))).Returns(new CacheValue<string>(value, true));
        }
    }
}
