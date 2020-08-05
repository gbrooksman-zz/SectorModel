using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using SectorModel.Shared.Entities;
using Serilog;
using Microsoft.EntityFrameworkCore;

namespace SectorModel.Server.Managers
{
    public class ModelItemManager : BaseManager
    {
        private readonly EquityManager eqMgr;
        private readonly QuoteManager qMgr;
        private readonly ModelManager mMgr;
        private readonly IConfiguration config;
        private readonly AppSettings appSettings;

        public ModelItemManager(IMemoryCache _cache, IConfiguration _config, AppSettings _appSettings
) : base(_cache, _config)
        {
            config = _config;
            appSettings = _appSettings;

            eqMgr = new EquityManager(_cache, config, appSettings);
            qMgr = new QuoteManager(_cache, config, appSettings);
            mMgr = new ModelManager(_cache, config, appSettings);            
        }

        public async Task<ModelItem> GetModelEquity(Guid modelEquityId, int versionNumber = 1)
        {
            ModelItem modelItem = new ModelItem();

            try
            {
                using var db = new ReadContext(appSettings);
                modelItem = await db.ModelItems
                                    .Where(i => i.Id == modelEquityId 
                                    && i.Version == versionNumber)
                                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ModelItemManager::GetModelEquity");
                throw;
            }

            return modelItem;
        }

        public async Task<List<ModelItem>> GetModelEquityListByDate(Guid modelId, DateTime quoteDate, int versionNumber = 0)
        {
            List<ModelItem> modelItemList = new List<ModelItem>();
			
            try
            {
                Model thisModel = await mMgr.GetModel(modelId, versionNumber);

                List<ModelItem> modelItems = await GetModelEquityList(modelId, versionNumber);

                foreach (ModelItem modelEquity in modelItems)
                {
                    modelEquity.Equity = await eqMgr.Get(modelEquity.EquityID);
                    Quote quote = await qMgr.GetByEquityIdAndDate(modelEquity.EquityID, quoteDate);
                    if (quote != null)
                    {
                        modelEquity.LastPrice = quote.Price;
                        modelEquity.LastPriceDate = quote.Date;
                        modelEquity.CurrentValue = Math.Round((modelEquity.Shares * quote.Price), 2);
                    }
                }

                modelItemList = modelItems;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ModelItemManager::GetModelEquityListByDate");
                throw;
            }

            return modelItemList;
        }

        public async Task<List<ModelItem>> GetModelEquityList(Guid modelId, int versionNumber = 1)
        {
            List<ModelItem> modelItemList = new List<ModelItem>();

            try
            {
                using var db = new ReadContext(appSettings);
                modelItemList = await db.ModelItems
                                    .Where(i => i.ModelId == modelId
                                    && i.Version == versionNumber)
                                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ModelItemManager::GetModelEquityList");
                throw;
            }

            return modelItemList;
        }

        public async Task<int> GetEquitiesInModelsCount(Guid equityId)
        {
            int itemCount = 0;

            try
            {
                using var db = new ReadContext(appSettings);
                itemCount = await db.ModelItems
                                    .Where(i => i.EquityID == equityId)
                                    .CountAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ModelItemManager::GetEquitiesInModelsCount");
                throw;
            }

            return itemCount;
        }

        #region CRUD

        public async Task<ModelItem> Save(ModelItem modelItem)
        {
            try
            {
                using var db = new WriteContext(appSettings);
                {
                    if (modelItem.Id != Guid.Empty)
                    {
                        db.Add(modelItem);
                    }
                    else
                    {
                        db.Update(modelItem);                        
                    }
                    await db.SaveChangesAsync();
                }               
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ModelItemManager::Save");
                throw;
            }

            return modelItem;
        }

        public async Task<bool> RemoveEquity(Guid modelEquityId)
        {
            int x = 0;

            try
            {
                ModelItem modelEquity = new ModelItem()
                {
                    Id = modelEquityId
                };

                using var db = new WriteContext(appSettings);
                db.Remove(modelEquity);
                x = await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ModelItemManager::RemoveEquity");
                throw;
            }

            return x > 0;
        }

        #endregion

    }
}
