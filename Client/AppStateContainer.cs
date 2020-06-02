using SectorModel.Client.Entities;
using System;
using System.Collections.Generic;
using SectorModel.Shared.Entities;

namespace SectorModel.Client
{
    public class AppStateContainer
    {
        public AppStateContainer()
        {

        }

        public string UserName { get; set; }
        public Guid UserId { get; set; }
        public List<ModelEquity> ModelEquityList { get; set; }
        public List<UserModel> UserModels { get; set; }
        public DateTime LastQuoteDate { get; set; }
        public Guid CoreModelId { get; set; }
        public Guid SPDRModelID { get; set; }
        public List<ContentItem> ContentItemList { get; set; }
        public List<Equity> SelectedEquityList { get; set; }
    }
}
