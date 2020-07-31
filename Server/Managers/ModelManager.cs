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
    public class ModelManager : BaseManager
    {
        private readonly EquityManager eqMgr;
        private readonly QuoteManager qMgr;
        private readonly IConfiguration config;
        private readonly AppSettings appSettings;

        public ModelManager(IMemoryCache _cache, IConfiguration _config, AppSettings _appSettings
) : base(_cache, _config)
        {
            appSettings = _appSettings;
            config = _config;

            eqMgr = new EquityManager(_cache, config, appSettings);
            qMgr = new QuoteManager(_cache, config, appSettings);
            
        }

        public async Task<List<Model>> GetActiveModelList(User user)
        {
            List<Model> modelList = new List<Model>();
           
            try
            {
                using var db = new ReadContext(appSettings);
                modelList = await db.Models
                                    .Where(i => i.UserId == user.Id)
                                    .ToListAsync();  
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ModelManager::GetActiveModelList");
                throw;
            } 

            return modelList;
        }

        public async Task<Model> GetModel(Guid modelId, int versionNumber = 0)
        {
            Model model = new Model();
            try
            {
                using (var db = new ReadContext(appSettings))
                {
                    if (versionNumber == 0)
                    {
                        var models = await db.Models
                                            .Where(i => i.Id == modelId)
                                            .ToListAsync();

                        versionNumber = models.Select(i => i.Version).Max();
                    }

                    model = await db.Models
                                        .Where(i => i.Id == modelId
                                        && i.Version == versionNumber)
                                        .FirstOrDefaultAsync();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ModelManager::GetModel");
                throw;
            }

            return model;
        }

       

        public async Task<Model> GetModelVersion(Guid modelId, int versionNumber)
        {
            Model model = new Model();
            try
            {
                using var db = new ReadContext(appSettings);
                model = await db.Models
                    .Where(i => i.Id == modelId
                    && i.Version == versionNumber)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ModelManager::GetModelVersion");
                throw;
            }

            return model;
        }

        public async Task<List<Model>> GetModelVersions(Guid modelId)
        {
            List<Model> modelList;
            try
            {
                modelList = await GetModelList(modelId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ModelManager::GetModelVersions");
                throw;
            }

            return modelList;
        }

        private async Task<List<Model>> GetModelList(Guid modelId)
        {
            using var db = new ReadContext(appSettings);
            return await db.Models
                .Where(i => i.Id == modelId)
                .ToListAsync();
        }


        public async Task<bool> CheckDateRange(Guid modelId, DateTime startdate, DateTime stopdate)
        {
            List<Model> modelList = await GetModelList(modelId);

            return modelList.Where(m => m.StartDate >= startdate && m.StopDate <= stopdate).Any();
        }     

        public async Task<Model> Save(Model model)
        {            
            try
            {
                using (var db = new WriteContext(appSettings))
                {
                    if (model.Id == Guid.Empty)
                    {
                        db.Models.Add(model);
                    }
                    else
                    {
                        db.Models.Update(model);
                    }
                    await db.SaveChangesAsync();
                }
                
            }
            catch(Exception ex)
            {
                Log.Error(ex, "ModelManager::Save");
                throw;
            } 

            return model;
        }

        internal async Task<Model> IncrementVersionAndSave(Guid modelId, int currentVersion)
        {
            Model thisModel = GetModel(modelId, currentVersion).Result;
            thisModel.Version++;

            thisModel = await Save(thisModel);

            return thisModel;
        }
    }
}


/*using System;
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
    public class EquityGroupManager : BaseManager
    {
        // this manager holds methods for equity_groups and equity_group_items tables
        // equity groups are collections of equities that are related by some criteria
        // these are different from user models which are collections of equities created
        // by users for whatever purposed
        
        public EquityGroupManager(IMemoryCache _cache, IConfiguration _config) : base(_cache, _config)
        {

        }

        public async Task<List<Model>> GetList()
        {
            List<Model> equityGroupList;
            try
            {
                equityGroupList = await GetAllGroups().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "EquityGroupManager::GetList");
                throw;
            }

            return equityGroupList;
        }
        
        public async Task<List<Model>> GetActiveList()
        {
            List<Model> equityGroupList = new List<Model>();           

            try
            {

                using var db = new WriteContext();
                equityGroupList = await db.EquityGroups.AsNoTracking()
                                        .Where(eg => eg.Active == true)
                                        .ToListAsync();


                // List<EquityGroup> equityGroupList =  await GetAllGroups().ConfigureAwait(false);
                // mgrResult = equityGroupList.Where(e => e.Active == true).ToList();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "EquityGroupManager::GetActiveList");
                throw;
            }

            return equityGroupList;
        }

        private async Task<List<Model>> GetAllGroups()
        {
            List<Model> equityGroupList = new List<Model>();

            using var db = new WriteContext();
            equityGroupList = await db.EquityGroups.AsNoTracking().ToListAsync();

            return equityGroupList;

            //return await cache.GetOrCreateAsync<List<EquityGroup>>(CacheKeys.EQUITY_GROUP_LIST, entry =>
            //{
            //    using NpgsqlConnection db = new NpgsqlConnection(connString);
            //    {
            //        return Task.FromResult(db.Query<EquityGroup>("SELECT * FROM equity_groups").ToList());
            //    }
            //});
        }

        #region CRUD

        public async Task<Model> Save(Model equityGroup)
        {            
            try
            {               
                if (equityGroup.Id == Guid.Empty)
                {
                    using var db = new WriteContext();
                    await db.EquityGroups.AddAsync(equityGroup);
                    await db.SaveChangesAsync();
                    //using NpgsqlConnection db = new NpgsqlConnection(connString);
                    //{
                    //    await db.InsertAsync(equityGroup).ConfigureAwait(false);
                    //}
                }
                else
                {
                    using var db = new WriteContext();
                    db.EquityGroups.Update(equityGroup);
                    await db.SaveChangesAsync();

                    //using NpgsqlConnection db = new NpgsqlConnection(connString);
                    //{
                    //    await db.UpdateAsync(equityGroup).ConfigureAwait(false);
                    //}
                }           
            }
            catch(Exception ex)
            {
                Log.Error(ex, "EquityGroupManager::Save");
                throw;
            } 

            return equityGroup;
        }

        #endregion

        #region equity group items

        public async Task<List<EquityGroupItem>> GetGroupItemsList(Guid equityGroupId)
        {           
           List<EquityGroupItem> equityGroupItems = new List<EquityGroupItem>();
     
            try
            {
                using var db = new WriteContext();
                {

                    equityGroupItems = await db.EquityGroupItems.AsNoTracking()
                                                .Where(eg => eg.GroupId == equityGroupId)
                                                .ToListAsync();
                }

                //equityGroupItems = await cache.GetOrCreateAsync(equityGroupId, entry =>
                //{
                //    using NpgsqlConnection db = new NpgsqlConnection(connString);
                //    {
                //        return Task.FromResult(db.Query<EquityGroupItem>(@"SELECT * 
                //                                        FROM equity_group_items 
                //                                        WHERE group_id = @p1 ", 
                //                                        new { p1 = equityGroupId }).ToList());
                //    }
                //}).ConfigureAwait(false);

            }
            catch(Exception ex)
            {
                Log.Error(ex, "EquityGroupManager::GetGroupItemsList");
                throw;
            } 

            return equityGroupItems;
        }

        public async Task<List<Equity>> GetGroupEquities(Guid equityGroupId)
        {
            List<Equity> equityItems = new List<Equity>();
            try
            {
                using var db = new WriteContext();
                {
                    equityItems = await (from e in db.Equities.AsNoTracking()
                                   join egi in db.EquityGroupItems.AsNoTracking()
                                   on e.Id equals egi.EquityId
                                   join g in db.EquityGroups.AsNoTracking()
                                   on egi.GroupId equals g.Id
                                   where g.Id == equityGroupId
                                   select e).ToListAsync();
                }


                    //equityItems = await cache.GetOrCreateAsync($"EQUITIES-{equityGroupId}", entry =>
                    //{
                    //    string sql = @" SELECT e.* 
                    //                    FROM equities e, equity_group_items i, equity_groups g
                    //                    WHERE e.id = i.equity_id 
                    //                    AND i.group_id = g.id
                    //                    AND g.id = @p1";

                    //    using NpgsqlConnection db = new NpgsqlConnection(connString);
                    //    {
                    //        return Task.FromResult(db.Query<Equity>(sql, new { p1 = equityGroupId }).ToList());
                    //    }
                    //}).ConfigureAwait(false);
                }
            catch (Exception ex)
            {
                Log.Error(ex, "EquityGroupManager::GetGroupEquities");
                throw;
            }

            return equityItems;
        }

        public async Task<int> GetItemsCount(Guid equityGroupId)
        {
            int itemCount = 0;
            
            try
            {
                using var db = new WriteContext();
                {
                    itemCount = await db.EquityGroupItems.AsNoTracking()
                                        .Where(i => i.GroupId == equityGroupId)
                                        .CountAsync();
                }

                    //using NpgsqlConnection db = new NpgsqlConnection(connString);
                    //mgrResult = await db.ExecuteScalarAsync<int>(@"SELECT count(*) 
                    //                                        FROM equity_group_items 
                    //                                        WHERE group_id = @p1 ",
                    //                    new { p1 = equityGroupId }).ConfigureAwait(false);
                }
            catch (Exception ex)
            {
                Log.Error(ex, "EquityGroupManager::GetItemsCount");
                throw;
            }

            return itemCount;
        }

        #region CRUD

        public async Task<EquityGroupItem> AddEquity(Model equityGroup, Guid equityId)
        {
            EquityGroupItem equityGroupItem = new EquityGroupItem();
            
            try
            {   
                equityGroupItem.GroupId = equityGroup.Id;
                equityGroupItem.EquityId = equityId;

                using var db = new WriteContext();
                {
                    db.EquityGroupItems.Add(equityGroupItem);
                    await db.SaveChangesAsync();

                }
                    //using NpgsqlConnection db = new NpgsqlConnection(connString);
                    //await db.InsertAsync(equityGroupItem).ConfigureAwait(false);
                }
            catch(Exception ex)
            {
                Log.Error(ex, "EquityGroupManager::AddEquity");
                throw;
            } 

            return equityGroupItem;
        }

        public async Task<bool> RemoveEquity(Model sysmbolGroup, Guid equitylId)
        {
            int x = 0;

            try
            {
                var equityGroupItem = new EquityGroupItem
                {
                    GroupId = sysmbolGroup.Id,
                    EquityId = equitylId
                };

                using var db = new WriteContext();
                {
                    db.EquityGroupItems.Remove(equityGroupItem);
                    x = await db.SaveChangesAsync();
                }
                //using NpgsqlConnection db = new NpgsqlConnection(connString);
                //{
                //    mgrResult = await db.DeleteAsync(equityGroupItem).ConfigureAwait(false);
                //}
            }
            catch(Exception ex)
            {
                Log.Error(ex, "EquityGroupManager::RemoveEquity");
                throw;
            } 

            return (x > 0);
        }
        #endregion

        #endregion
    }
}
*/
