using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using SectorModel.Shared.Entities;
using Serilog;

namespace SectorModel.Server.Managers
{
    public class UserModelManager : BaseManager
    {
        private readonly EquityManager eqMgr;
        private readonly QuoteManager qMgr;

        public UserModelManager(IMemoryCache _cache, IConfiguration _config) : base(_cache, _config)
        {
            eqMgr = new EquityManager(_cache, _config);
            qMgr = new QuoteManager(_cache, _config);
        }

        public async Task<List<UserModel>> GetActiveModelList(User user)
        {
            List<UserModel> mgrResult = new List<UserModel>();
           
            try
            {
                //using NpgsqlConnection db = new NpgsqlConnection(connString);
                //mgrResult = (List<UserModel>)await db.QueryAsync<UserModel>(@" SELECT * 
                //                                    FROM user_models 
                //                                    WHERE user_id = @p1 and active = true",
                //                    new { p1 = user.Id }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UserModelManager::GetActiveModelList");
                throw;
            } 

            return mgrResult;
        }

        public async Task<UserModel> GetModel(Guid modelId, int versionNumber = 0)
        {
            UserModel userModel = new UserModel();
            try
            {
                //using NpgsqlConnection db = new NpgsqlConnection(connString);
                //if (versionNumber == 0)
                //{
                //    versionNumber = await db.QuerySingleAsync<int>(@" SELECT MAX(version) 
                //                                    FROM user_models 
                //                                    WHERE id = @p1 ",
                //                                new { p1 = modelId }).ConfigureAwait(false);
                //}


                //userModel = await db.QuerySingleAsync<UserModel>(@" SELECT * 
                //                                FROM user_models 
                //                                WHERE id = @p1 and version = @p2",
                //                            new { p1 = modelId, p2 = versionNumber }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UserModelManager::GetModel");
                throw;
            }

            return userModel;
        }

       

        public async Task<UserModel> GetModelVersion(Guid modelId, int versionNumber)
        {
            UserModel mgrResult = new UserModel();
            try
            {
                //using NpgsqlConnection db = new NpgsqlConnection(connString);
                //UserModel model = await db.QueryFirstOrDefaultAsync<UserModel>(@" SELECT * 
                //                                    FROM user_models 
                //                                    WHERE id = @p1 and version = @p2",
                //    new { p1 = modelId, p2 = versionNumber }).ConfigureAwait(false);
                //mgrResult.Entity = model;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UserModelManager::GetModelVersion");
                throw;
            }

            return mgrResult;
        }

        public async Task<List<UserModel>> GetModelVersions(Guid modelId)
        {
            List<UserModel> mgrResult;
            try
            {
                mgrResult = await GetModelList(modelId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UserModelManager::GetModelVersions");
                throw;
            }

            return mgrResult;
        }

        private async Task<List<UserModel>> GetModelList(Guid modelId)
        {
            List<UserModel> modelList = new List<UserModel>();

            //using (NpgsqlConnection db = new NpgsqlConnection(connString))
            //{
            //    modelList = (List<UserModel>) await db.QueryAsync<UserModel>(@" SELECT * 
            //                                        FROM user_models 
            //                                        WHERE id = @p1",
            //                                        new { p1 = modelId }).ConfigureAwait(false);
            //}

            return modelList;
        }


        public async Task<bool> CheckDateRange(Guid modelId, DateTime startdate, DateTime stopdate)
        {
            List<UserModel> modelList = await GetModelList(modelId);

            return modelList.Where(m => m.StartDate >= startdate && m.StopDate <= stopdate).Any();
        }


        public async Task<List<ModelEquity>> GetModelEquityListByDate(Guid modelId, DateTime quoteDate, int versionNumber = 0)
        {
            List<ModelEquity> mgrResult = new List<ModelEquity>();
            try
            {  
                UserModel thisModel = GetModel(modelId, versionNumber).Result;

                decimal startValue = thisModel.StartValue;

                //using NpgsqlConnection db = new NpgsqlConnection(connString);
                //var qResult = await db.QueryAsync<ModelEquity>(@" SELECT * 
                //                                    FROM model_equities 
                //                                    WHERE model_id = @p1 and version = @p2",
                //    new { p1 = modelId, p2 = thisModel.Version }).ConfigureAwait(false);

                List<ModelEquity> modelEquityList = new List<ModelEquity>(); //qResult.ToList();

                foreach (ModelEquity modelEquity in modelEquityList)
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

                mgrResult = modelEquityList;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UserModelManager::GetModelEquityListByDate");
                throw;
            }

            return mgrResult;
        }

        public async Task<UserModel> Save(UserModel userModel)
        {            
            try
            {
                if (userModel.Id == Guid.Empty)
                {
                    //using NpgsqlConnection db = new NpgsqlConnection(connString);
                    //{
                    //    await db.InsertAsync(userModel).ConfigureAwait(false);
                    //}
                }
                else
                {
                    //using NpgsqlConnection db = new NpgsqlConnection(connString);
                    //{
                    //    await db.UpdateAsync(userModel).ConfigureAwait(false);
                    //}
                }           
                
            }
            catch(Exception ex)
            {
                Log.Error(ex, "UserModelManager::Save");
                throw;
            } 

            return userModel;
        }

        internal async Task<UserModel> IncrementVersionAndSave(Guid modelId, int currentVersion)
        {
            UserModel thisModel = GetModel(modelId, currentVersion).Result;
            thisModel.Version++;

            thisModel = await Save(thisModel).ConfigureAwait(false);

            return thisModel;
        }

        #region model equities

        public async Task<ModelEquity> GetModelEquity(Guid modelEquityId, int versionNumber = 1)
        {
            ModelEquity mgrResult = new ModelEquity();

            try
            {
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

            return mgrResult;
        }

        public async Task<List<ModelEquity>> GetModelEquityList(Guid modelId, int versionNumber = 1)
        {
            List<ModelEquity> mgrResult = new List<ModelEquity>();
           
            try
            {
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
            catch(Exception ex)
            {
                Log.Error(ex, "UserModelManager::GetModelEquityList");
                throw;
            } 

            return mgrResult;
        }

        #region CRUD

        public async Task<ModelEquity> Save(ModelEquity modelEquity)
        {
            ModelEquity mgrResult = new ModelEquity();           
            
            try
            {  
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

                mgrResult = modelEquity;
            }
            catch(Exception ex)
            {
                Log.Error(ex, "UserModelManager::Save");
                throw;
            } 

            return mgrResult;
        }      

        public async Task<bool> RemoveEquity(Guid modelEquityId)
        {
            bool mgrResult = false;
            
            try
            {   
                ModelEquity modelEquity = new ModelEquity()
                {
                    Id = modelEquityId                    
                };

                //using NpgsqlConnection db = new NpgsqlConnection(connString);
                //{
                //    mgrResult = await db.DeleteAsync(modelEquity).ConfigureAwait(false);
                //}

            }
            catch(Exception ex)
            {
                Log.Error(ex, "UserModelManager::RemoveEquity");
                throw;
            } 

            return mgrResult;
        }

        #endregion

    #endregion
    }
}
