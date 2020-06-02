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

        public ModelItemManager(IMemoryCache _cache, IConfiguration _config) : base(_cache, _config)
        {
            eqMgr = new EquityManager(_cache, _config);
            qMgr = new QuoteManager(_cache, _config);
            mMgr = new ModelManager(_cache, _config);
        }

        public async Task<ModelItem> GetModelEquity(Guid modelEquityId, int versionNumber = 1)
        {
            ModelItem modelItem = new ModelItem();

            try
            {
                using var db = new ReadContext();
                modelItem = await db.ModelItems
                                    .Where(i => i.Id == modelEquityId 
                                    && i.Version == 1)
                                    .FirstOrDefaultAsync();

                //using NpgsqlConnection db = new NpgsqlConnection(connString);
                //{
                //    mgrResult = await db.QueryFirstOrDefaultAsync<ModelEquity>(@" SELECT * 
                //                                            FROM model_equities 
                //                                            WHERE id = @p1 and version = @p2",
                //                                            new { p1 = modelEquityId, p2 = versionNumber }).ConfigureAwait(false);
                //}
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UserModelManager::GetModelEquity");
                throw;
            }

            return modelItem;
        }

        public async Task<List<ModelItem>> GetModelEquityListByDate(Guid modelId, DateTime quoteDate, int versionNumber = 0)
        {
            List<ModelItem> modelItemList = new List<ModelItem>();
            try
            {
                Model thisModel = mMgr.GetModel(modelId, versionNumber).Result;

                decimal startValue = thisModel.StartValue;

                // add a > 0 check here?
                using var db = new ReadContext();
                List<ModelItem> modelEquityList = await db.ModelItems
                    .Where(i => i.Id == modelId
                    && i.Version == versionNumber)
                    .ToListAsync();

                //using NpgsqlConnection db = new NpgsqlConnection(connString);
                //var qResult = await db.QueryAsync<ModelEquity>(@" SELECT * 
                //                                    FROM model_equities 
                //                                    WHERE model_id = @p1 and version = @p2",
                //    new { p1 = modelId, p2 = thisModel.Version }).ConfigureAwait(false);

                //List<ModelItem> modelEquityList = new List<ModelItem>(); //qResult.ToList();

                foreach (ModelItem modelEquity in modelEquityList)
                {
                    modelEquity.Equity = eqMgr.Get(modelEquity.EquityID).Result;
                    Quote quote = qMgr.GetByEquityIdAndDate(modelEquity.EquityID, quoteDate).Result;
                    if (quote != null)
                    {
                        modelEquity.LastPrice = quote.Price;
                        modelEquity.LastPriceDate = quote.Date;
                        modelEquity.CurrentValue = Math.Round((modelEquity.Shares * quote.Price), 2);
                    }
                }

                modelItemList = modelEquityList;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UserModelManager::GetModelEquityListByDate");
                throw;
            }

            return modelItemList;
        }

        public async Task<List<ModelItem>> GetModelEquityList(Guid modelId, int versionNumber = 1)
        {
            List<ModelItem> modelItemList = new List<ModelItem>();

            try
            {
                using var db = new ReadContext();
                modelItemList = await db.ModelItems
                                    .Where(i => i.Id == modelId
                                    && i.Version == versionNumber)
                                    .ToListAsync();

                //using NpgsqlConnection db = new NpgsqlConnection(connString);
                //{
                //    var qResult = await db.QueryAsync<ModelEquity>(@"SELECT * 
                //                                            FROM model_equities 
                //                                            WHERE model = @p1 
                //                                            AND version = @p2", 
                //                                            new { p1 = modelId, p2 = versionNumber }).ConfigureAwait(false);

                //    mgrResult  = qResult.ToList();
                //}
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UserModelManager::GetModelEquityList");
                throw;
            }

            return modelItemList;
        }

        public async Task<int> GetEquitiesInModelsCount(Guid equityId)
        {
            int itemCount = 0;

            try
            {
                using var db = new ReadContext();
                {
                    itemCount = await db.ModelItems
                                        .Where(i => i.EquityID == equityId)
                                        .CountAsync();
                }

                //using NpgsqlConnection db = new NpgsqlConnection(connString);
                //mgrResult = await db.QueryFirstOrDefaultAsync<int>(@"SELECT count(*)  
                //                                                        FROM model_equities 
                //                                                        WHERE equity_id = @p1 ",
                //                    new { p1 = equityId }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "EquityManager::GetEquitiesInModelsCount");
                throw;
            }

            return itemCount;
        }

        #region CRUD

        public async Task<ModelItem> Save(ModelItem modelItem)
        {
            try
            {
                using var db = new WriteContext();
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


                //using (NpgsqlConnection db = new NpgsqlConnection(connString))
                //{
                //    if (modelEquity.Id != default)
                //    {
                //        await db.UpdateAsync(modelEquity).ConfigureAwait(false);
                //    }
                //    else
                //    { 
                //        await db.InsertAsync(modelEquity).ConfigureAwait(false);
                //    }
                //} 

               
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UserModelManager::Save");
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

                using var db = new WriteContext();
                db.Remove(modelEquity);
                x = await db.SaveChangesAsync();

                //using NpgsqlConnection db = new NpgsqlConnection(connString);
                //{
                //    mgrResult = await db.DeleteAsync(modelEquity).ConfigureAwait(false);
                //}

            }
            catch (Exception ex)
            {
                Log.Error(ex, "UserModelManager::RemoveEquity");
                throw;
            }

            return x > 0;
        }

        #endregion

    }
}
