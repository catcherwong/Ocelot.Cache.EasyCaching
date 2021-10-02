namespace Ocelot.Cache.EasyCaching
{
    using global::EasyCaching.Core;
    using Microsoft.Extensions.Options;
    using System;

    public class OcelotEasyCachingCache<T> : IOcelotCache<T>
    {
        private readonly OcelotEasyCachingOptions _options;
        private readonly IEasyCachingProvider _provider;
        private readonly IHybridCachingProvider _hybridProvider;

        public OcelotEasyCachingCache(
            IOptions<OcelotEasyCachingOptions> optionsAccs, 
            IEasyCachingProviderFactory providerFactory, 
            IHybridProviderFactory hybridFactory = null)
        {
            _options = optionsAccs.Value;

            if (!_options.EnableHybrid)
            {
                _provider = providerFactory.GetCachingProvider(_options.ProviderName);
            }
            else
            {
                _hybridProvider = hybridFactory.GetHybridCachingProvider(_options.HybridName);
            }            
        }

        public void Add(string key, T value, TimeSpan ttl, string region)
        {
            var cacheKey = $"{region}:{key}";

            if (!_options.EnableHybrid)
            {
                _provider.Set(cacheKey, value, ttl);                
            }
            else
            {
                _hybridProvider.Set(cacheKey, value, ttl);
            }
        }

        public void AddAndDelete(string key, T value, TimeSpan ttl, string region)
        {
            var cacheKey = $"{region}:{key}";

            if (!_options.EnableHybrid)
            {                
                _provider.Set(cacheKey, value, ttl);
            }
            else
            {
                _hybridProvider.Set(cacheKey, value, ttl);
            }
        }

        public void ClearRegion(string region)
        {
            if (!_options.EnableHybrid)
            {
                _provider.RemoveByPrefix(region);
            }
            else
            {
                _hybridProvider.RemoveByPrefix(region);
            }
        }

        public T Get(string key, string region)
        {
            var cacheKey = $"{region}:{key}";

            if (!_options.EnableHybrid)
            {
                var res = _provider.Get<T>(cacheKey);
                return res.Value;
            }
            else
            {
                var res = _hybridProvider.Get<T>(cacheKey);
                return res.Value;
            }          
        }
    }
}
