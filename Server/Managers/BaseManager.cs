
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SectorModel.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace SectorModel.Server.Managers
{
    public class BaseManager
    {
        internal readonly IMemoryCache cache;
       // internal readonly string connString;
        internal readonly IConfiguration config;

        public BaseManager(IMemoryCache _cache, IConfiguration _config)
        {
            cache = _cache;
            config = _config;
          
        }
    }
}
