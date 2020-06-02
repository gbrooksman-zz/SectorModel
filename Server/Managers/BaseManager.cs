
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

    public static class DBTables
    {
        public static readonly string Equities = "equities";

        public static readonly string EquityGroups = "equitygroups";

        public static readonly string Quotes = "quotes";

        public static readonly string EquityItems = "equityitems";

        public static readonly string Models = "models";

        public static readonly string Users = "users";

        public static readonly string UserModels = "usermodels";

    }
}
