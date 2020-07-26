using System;
using System.Collections.Generic;

namespace SectorModel.Shared.Entities
{
    public class AppSettings
    {
        public AppSettings()
        {

        }

        public string UserName { get; } = "geoff";
        public Guid UserId { get; } = Guid.Parse("4F3CBB0D-14B5-4C75-AB29-65543CF4CAA5");
        public List<ModelItem> ModelEquityList { get; set; }
        public List<Model> UserModels { get; set; }
        public DateTime LastQuoteDate { get; set; }
        public Guid CoreModelId { get; } = Guid.Parse("FAC8A666-74D8-4531-B3AD-DA7B95360462");
        public Guid SPDRModelId { get; } = Guid.Parse("4237E32A-5C60-4141-8658-FA357C28EF28");
        public List<Equity> SelectedEquityList { get; set; }
        public string DBConnectionString { get; } = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=SectorModel;Integrated Security=True";
        public string IEXCloudAPIKey { get; set; }
    }
}