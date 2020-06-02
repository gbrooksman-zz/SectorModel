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
    public class EquityGroupManager : BaseManager
    {
        // this manager holds methods for equity_groups and equity_group_items tables
        // equity groups are collections of equities that are related by some criteria
        // these are different from user models which are collections of equities created
        // by users for whatever purposed
        
        public EquityGroupManager(IMemoryCache _cache, IConfiguration _config) : base(_cache, _config)
        {

        }

        public async Task<List<EquityGroup>> GetList()
        {
            List<EquityGroup> equityGroupList;
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
        
        public async Task<List<EquityGroup>> GetActiveList()
        {
            List<EquityGroup> equityGroupList = new List<EquityGroup>();           

            try
            {

                using var db = new SectorModelContext();
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

        private async Task<List<EquityGroup>> GetAllGroups()
        {
            List<EquityGroup> equityGroupList = new List<EquityGroup>();

            using var db = new SectorModelContext();
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

        public async Task<EquityGroup> Save(EquityGroup equityGroup)
        {            
            try
            {               
                if (equityGroup.Id == Guid.Empty)
                {
                    using var db = new SectorModelContext();
                    await db.EquityGroups.AddAsync(equityGroup);
                    await db.SaveChangesAsync();
                    //using NpgsqlConnection db = new NpgsqlConnection(connString);
                    //{
                    //    await db.InsertAsync(equityGroup).ConfigureAwait(false);
                    //}
                }
                else
                {
                    using var db = new SectorModelContext();
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
                using var db = new SectorModelContext();
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
                using var db = new SectorModelContext();
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
                using var db = new SectorModelContext();
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

        public async Task<EquityGroupItem> AddEquity(EquityGroup equityGroup, Guid equityId)
        {
            EquityGroupItem equityGroupItem = new EquityGroupItem();
            
            try
            {   
                equityGroupItem.GroupId = equityGroup.Id;
                equityGroupItem.EquityId = equityId;

                using var db = new SectorModelContext();
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

        public async Task<bool> RemoveEquity(EquityGroup sysmbolGroup, Guid equitylId)
        {
            int x = 0;

            try
            {
                var equityGroupItem = new EquityGroupItem
                {
                    GroupId = sysmbolGroup.Id,
                    EquityId = equitylId
                };

                using var db = new SectorModelContext();
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
