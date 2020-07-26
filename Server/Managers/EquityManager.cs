using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Serilog;
using System.Threading.Tasks;
using SectorModel.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace SectorModel.Server.Managers
{
    public class EquityManager : BaseManager
    {

        // an equity is an investment vehicle identified by a symbol 
        // that is included in the api's data

        // private readonly ModelItemManager miMgr;

        private readonly IConfiguration config;
        private readonly AppSettings appSettings;

        public EquityManager(IMemoryCache cache, IConfiguration _config, AppSettings _appSettings) : base(cache, _config)
        {
            config = _config;
            appSettings = _appSettings;
        }

      


        public async Task<List<Equity>> GetAll()
        {
            List<Equity> equityList = new List<Equity>();

            try
            {
                using var db = new ReadContext(appSettings);
                equityList = await db.Equities.ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "EquityManager::GetEquitiesInModelsCount");
                throw;
            }
            
            return equityList;
        }

        public async Task<List<Equity>> GetList(string symbols)
        {
            List<Equity> equityMatchList = new List<Equity>();

            List<string> symbolList = new List<string>(symbols.Split(";".ToCharArray()));

            try
            {
                List<Equity> equityList = await GetAll();

                foreach (var symbol in symbolList)
                {
                    Equity eq = equityList.Where(e => e.Symbol.ToUpper() == symbol.ToUpper()).FirstOrDefault();

                    if (eq != null)
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

        public async Task<Equity> Get(Guid equityId)
        {
            Equity equity = new Equity();

            try
            {
                using var db = new ReadContext(appSettings);
                equity = await db.Equities.Where(i => i.Id == equityId).FirstOrDefaultAsync(); 
            }
            catch (Exception ex)
            {
                Log.Error("EquityManager::Get", ex);
                throw;
            }
            
            return equity;
        }

        public async Task<Equity> GetBySymbol(string symbol)
        {
            Equity equity = new Equity();

            try
            {
                using var db = new ReadContext(appSettings);
                equity = await db.Equities.Where(i => i.Symbol.ToUpper() == symbol.ToUpper()).FirstOrDefaultAsync();

            }
            catch(Exception ex)
            {
                Log.Error("EquityManager::GetBySymbol",ex);
                throw;
            }

            return equity;
        }
               
        #region CRUD

        public async Task<Equity> Save(Equity equity)
        {            
            try
            {
                using var db = new WriteContext(appSettings);
                {
                    if (equity.Id != Guid.Empty)
                    {
                        db.Add(equity);
                    }
                    else
                    {
                        db.Update(equity);
                    }
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {               
                Log.Error("EquityManager::Save",ex);
                throw;
            } 

            return equity;
        }


        public async Task<bool> IsEquityInAnyModel(Guid equityId)
        {
            using var db = new ReadContext(appSettings);
            return await db.ModelItems.Where(m => m.EquityID == equityId).AnyAsync();
        }

        public async Task<bool> Delete(Guid equityId)
        {
            int x = 0;

            Equity equity = new Equity()
            {
                Id = equityId
            };            
            
            try
            {
                bool isUsed = await IsEquityInAnyModel(equityId);

                if (!isUsed)
                {
                    using var db = new WriteContext(appSettings);
                    db.Remove(equity);
                    x = await db.SaveChangesAsync();
                }
                else
                {
                    throw new APIException(APIException.EQUITY_USED,
                                                            APIException.EQUITY_USED_MESSAGE);
                }
            }
            catch(Exception ex)
            {               
                Log.Error("EquityManager::Delete",ex);
                throw;
            } 

            return x > 0 ;
        }

        #endregion
    }

}