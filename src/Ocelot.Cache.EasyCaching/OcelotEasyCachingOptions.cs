namespace Ocelot.Cache.EasyCaching
{
    using global::EasyCaching.Core.Configurations;
    using System;

    public class OcelotEasyCachingOptions
    {
        /// <summary>
        /// Enable hybrid caching provider or not
        /// </summary>
        public bool EnableHybrid { get; set; } = false;

        /// <summary>
        /// Specify the caching provider name when <see cref="EnableHybrid"/> is false
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// Specify the hybrid provider name when <see cref="EnableHybrid"/> is true
        /// </summary>
        public string HybridName { get; set; }

        /// <summary>
        /// Settings of EasyCaching 
        /// </summary>
        public Action<EasyCachingOptions> Settings { get; set; }
    }
}
