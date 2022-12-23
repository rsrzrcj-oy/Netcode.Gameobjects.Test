
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Unity.Services.Relay.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Unity.Services.Relay
{
    /// <summary>
    /// The entry class of the Relay Allocations Service enables clients to connect to relay servers. Once connected, they are able to communicate with each other, via the relay servers, using the bespoke relay binary protocol.
    /// </summary>
    public static class Relay
    {
        private static IRelayServiceSDK service;

        private static readonly Configuration configuration;

        internal static void SetBasePath(string basePath)
        {
            configuration.BasePath = basePath;
        }

        static Relay()
        {
            configuration = new Configuration("https://relay-allocations.cloud.unity3d.com", 10, 4, null);
        }

        /// <summary>
        /// A static instance of the Relay Allocation Client.
        /// </summary>
        public static IRelayServiceSDK Instance
        {
            get
            {
                if (service == null)
                {
                    service = new WrappedRelayService(RelayService.Instance.AllocationsApi, configuration);
                }
                return service;
            }
        }
    }
}