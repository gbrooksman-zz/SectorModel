using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Serilog;
using System.Threading.Tasks;
using SectorModel.Shared.Entities;

namespace SectorModel.Server.Managers
{
    public class EquityManager : BaseManager
    {
       
        // an equity is an investment vehilce identified by a equity that is included in the api's data

        public EquityManager(IMemoryCache cache, IConfiguration config) : base(cache, config)
        {

        }

        public async Task<int> GetEquitiesInModelsCount(Guid equityId)
        {
            int mgrResult = 0;
            
            try
            {
                //using NpgsqlConnection db = new NpgsqlConnection(connString);
                //mgrResult = await db.QueryFirstOrDefaultAsync<int>(@"SELECT count(*)  
                //                                                        FROM model_equities 
                //                                                        WHERE equity_id = @p1 ",
                //                    new { p1 = equityId }).ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                Log.Error(ex, "EquityManager::GetEquitiesInModelsCount");
                throw;
            } 

            return mgrResult;
        }


        public async Task<List<Equity>> GetAll()
        {
            List<Equity> equityList = new List<Equity>();

            try
            {
                //equityList = await cache.GetOrCreateAsync(CacheKeys.EQUITY_LIST, entry =>
                //{
                //    using NpgsqlConnection db = new NpgsqlConnection(connString);
                //    return Task.FromResult(db.Query<Equity>("SELECT * FROM equities").ToList());
                //}).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                Log.Error(ex, "EquityManager::GetEquitiesInModelsCount");
                throw;
            }
            
            return equityList;
        }


        /// <summary>
        /// handles a comma-delimited list of symbols
        /// </summary>
        /// <param name="symbols"></param>
        /// <returns></returns>
        public async Task<List<Equity>> GetList(string symbols)
        {
            List<Equity> equityMatchList = new List<Equity>();

            List<string> symbolList = new List<string>(symbols.Split(";".ToCharArray()));

            try
            {
                List<Equity> equityList = await GetAllEquities().ConfigureAwait(false);

                foreach (var symbol in symbolList)
                {
                    Equity eq = equityList.Where(e => e.Symbol.ToUpper() == symbol.ToUpper()).FirstOrDefault();

                    if (eq != default(Equity))
                    {
                        equityMatchList.Add(eq);
                    }
                }
            }
            catch (Exception ex)
            {    
                Log.Error("EquityManager::GetList", ex);
                throw;
            }

            return equityMatchList;
        }       

        private async Task<List<Equity>> GetAllEquities()
        {           
            List<Equity> equityList = new List<Equity>();

            try
            {
                //equityList = await cache.GetOrCreateAsync(CacheKeys.EQUITY_LIST, entry =>
                //{
                //    using NpgsqlConnection db = new NpgsqlConnection(connString);
                //    return Task.FromResult(db.Query<Equity>("SELECT * FROM equities").ToList());
                //});              
            }
            catch (Exception ex)
            {
                Log.Error("EquityManager::GetAllEquities", ex);
                throw;
            }

            return equityList;
        }

        public async Task<Equity> Get(Guid equityId)
        {
            Equity mgrResult = new Equity();

            try
            {
                //                using NpgsqlConnection db = new NpgsqlConnection(connString);
                //                mgrResult = await db.QueryFirstOrDefaultAsync<Equity>(@"SELECT * 
                //                                                                        FROM equities 
                //                                                                        WHERE id = @p1 ", new { p1 = equityId }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Error("EquityManager::Get", ex);
                throw;
            }
            
            return mgrResult;
        }

        public async Task<Equity> GetBySymbol(string symbol)
        {
            Equity equity = new Equity();

            try{
           
            //equity = await cache.GetOrCreateAsync<Equity>(CacheKeys.EQUITY + symbol, entry =>
            //{
            //    //using NpgsqlConnection db = new NpgsqlConnection(connString);
            //    //return Task.FromResult(db.QueryFirstOrDefault<Equity>(@"SELECT * 
            //    //                            FROM equities 
            //    //                            WHERE LOWER(symbol) = @p1 ",
            //    //            new { p1 = symbol.ToLower() }));
            //});
            }
            catch(Exception ex)
            {
                Log.Error("EquityManager::GGetBySymbolet",ex);
                throw;
            }

            return equity;
        }
               
        #region CRUD

        public async Task<Equity> Save(Equity equity)
        {
            Equity mgrResult = new Equity();
            
            try
            {
                //if (equity.Id == Guid.Empty)
                //{
                //    using NpgsqlConnection db = new NpgsqlConnection(connString);
                //    await db.InsertAsync(equity).ConfigureAwait(false);
                //}
                //else
                //{
                //    using NpgsqlConnection db = new NpgsqlConnection(connString);
                //    await db.UpdateAsync(equity).ConfigureAwait(false);
                //}           
               // mgrResult = equity;
            }
            catch(Exception ex)
            {               
                Log.Error("EquityManager::Save",ex);
                throw;
            } 

            return equity;
        }

      

        public async Task<bool> Delete(Guid equityId)
        {
            bool mgrResult = false;
            Equity equity = new Equity()
            {
                Id = equityId
            };            
            
            try
            {   
                int count = GetEquitiesInModelsCount(equity.Id).Result;

                if (count == 0)
                {
                    //using NpgsqlConnection db = new NpgsqlConnection(connString);
                    //mgrResult = await db.DeleteAsync(equity).ConfigureAwait(false);
                }
                else
                {
                    throw new APIException( APIException.EQUITY_USED, 
                                                            APIException.EQUITY_USED_MESSAGE);
                }
            }
            catch(Exception ex)
            {               
                Log.Error("EquityManager::Delete",ex);
                throw;
            } 

            return mgrResult;
        }

        #endregion
    }

}