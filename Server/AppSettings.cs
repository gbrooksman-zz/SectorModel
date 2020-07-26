using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SectorModel.Server
{
    public class AppSettings : IAppSettings
    {
        public AppSettings()
        {

        }

        public string UserName { get; set; }

        public string IEXCloudAPIKey { get; set; }

        public string DBConnectionString { get; set; }

        public Guid UserGuid { get; set; }

        public Guid CoreModelId { get; set; }

        public Guid SPDRModelId { get; set; }

    }

    public interface IAppSettings
    {
        string UserName { get; set; }

        string IEXCloudAPIKey { get; set; }

        string DBConnectionString { get; set; }

        Guid UserGuid { get; set; }

        Guid CoreModelId { get; set; }

        Guid SPDRModelId { get; set; }
    }
}
