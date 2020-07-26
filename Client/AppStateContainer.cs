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
        public List<ModelItem> ModelEquityList { get; set; }
        public List<Model> UserModels { get; set; }
        public DateTime LastQuoteDate { get; set; }
        public Guid CoreModelId { get; set; }
        public Guid SPDRModelID { get; set; }
        public List<ContentItem> ContentItemList { get; set; }
        public List<Equity> SelectedEquityList { get; set; }
    }

    public interface IAppStateContainer
    {
        string UserName { get; set; }
        Guid UserId { get; set; }
        List<ModelItem> ModelEquityList { get; set; }
        List<Model> UserModels { get; set; }
        DateTime LastQuoteDate { get; set; }
        Guid CoreModelId { get; set; }
        Guid SPDRModelID { get; set; }
        List<ContentItem> ContentItemList { get; set; }
        List<Equity> SelectedEquityList { get; set; }
    }
}
