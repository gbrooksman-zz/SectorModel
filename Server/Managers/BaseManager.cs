

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;


namespace SectorModel.Server.Managers
{
    public class BaseManager
    {
        private readonly IMemoryCache cache;
        private readonly IConfiguration config;

        public BaseManager(IMemoryCache _cache, IConfiguration _config)
        {
            cache = _cache;
            config = _config;        
		}
    }
}
