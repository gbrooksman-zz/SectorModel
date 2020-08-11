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
    //a quote is the price of an equity on a given day
    public class QuoteManager : BaseManager
    {
        private readonly EquityManager eqMgr;
        private readonly IConfiguration config;
        private readonly AppSettings appSettings;

        public QuoteManager(IMemoryCache cache, IConfiguration _config, AppSettings _appSettings) : base(cache, _config)
        {
            appSettings = _appSettings;
            config = _config;

            eqMgr = new EquityManager(cache, config, appSettings);
            
        }
      
        public async Task<List<Quote>> GetByEquityIdAndDateRange(Guid equityId, DateTime startdate, DateTime stopdate)
        {
            List<Quote> quotes = new List<Quote>();

            try
            {
                List<Quote> quoteList = await GetByEquityId(equityId).ConfigureAwait(false);

                quotes = quoteList.Where(q => q.Date >= startdate && q.Date <= stopdate).ToList();            

            }
            catch (Exception ex)
            {
                Log.Error(ex, "QuoteManager::GetByEquityIdAndDateRange");
                throw;
            }
            return quotes;
        }

        public async Task<List<Quote>> GetBySymbolListAndDateRange(List<string> symbols, DateTime startdate, DateTime stopdate)
        {
            List<Quote> quotes = new List<Quote>();

            try
            {   
                foreach (string symbol in symbols)
                {
                    Equity equity = await eqMgr.GetBySymbol(symbol).ConfigureAwait(false);

                    List<Quote> quoteList = await GetByEquityId(equity.Id).ConfigureAwait(false);

                    quotes.AddRange(quoteList.Where(q => q.Date >= startdate && q.Date <= stopdate).ToList());
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex, "QuoteManager::GetBySymbolListAndDateRange");
                throw;
            }

            return quotes;
        }

        public async Task<List<Quote>> GetBySymbolListDateRangeWithInterval(List<string> symbols, DateTime startdate, DateTime stopdate, int quoteInterval )
        {
            List<Quote> quoteListSkipped = new List<Quote>();

            try
            {
                foreach (string symbol in symbols)
                {
                    Equity equity = await eqMgr.GetBySymbol(symbol);

                    List<Quote> quoteList = await GetByEquityId(equity.Id);

                    var tempquoteList = quoteList.Where(q => q.Date >= startdate && q.Date <= stopdate).ToList();

                    for (int u = 0; u < tempquoteList.Count; u += quoteInterval)
                    {
                        quoteListSkipped.Add(tempquoteList.Skip(u).Take(1).FirstOrDefault());
                    }               
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex, "QuoteManager::GetBySymbolListDateRangeWithInterval");
                throw;
            }

            return quoteListSkipped;
        }

		public async Task<List<ModelItem>> GetModelItemsWithPrices(Model model, DateTime quoteDate)
		{
			List<ModelItem> items = new List<ModelItem>();

			quoteDate = await GetNearestQuoteDate(quoteDate);

            List<Quote> quoteList = await GetForDate(quoteDate);
            List<Equity> equityList = await eqMgr.GetAll();

			foreach (ModelItem mi in model.ItemList)
			{
                    Quote quote = quoteList.Where( q => q.EquityId == mi.EquityId).FirstOrDefault();
                    if (quote != null)
                    {
                        mi.LastPrice = quote.Price;
                        mi.LastPriceDate = quote.Date;
                        mi.CurrentValue = mi.Shares * mi.LastPrice;
                    }

                mi.Equity = equityList.Where(e => e.Id == mi.EquityId).FirstOrDefault();
				mi.ModelId = model.Id;										
				items.Add(mi);	
			}

			return items;
		}

        public async Task <List<Quote>> GetForDate(DateTime quoteDate)
        {
            using (var db = new ReadContext(appSettings))
            return  await db.Quotes.Where(q => q.Date == quoteDate).ToListAsync();
          }

        public async Task<Quote> GetLast(Guid equityid)
        {
            Quote quote = new Quote();

            try
            {
                List<Quote> quoteList = await GetByEquityId(equityid);

                quote = quoteList.OrderByDescending(q => q.Date).FirstOrDefault();  
            }
            catch(Exception ex)
            {
                Log.Error(ex, "QuoteManager::GetLast");
                throw;
            }        

            return quote;
        }

  		public async Task<List<Quote>> GetList(DateTime quoteDate)
        {
            List<Quote> quoteList = new List<Quote>();

            quoteDate = await GetNearestQuoteDate(quoteDate);

            try
            {
                using (var db = new ReadContext(appSettings))
                {  
					quoteList = await db.Quotes
                                        .Where(i => i.Date == quoteDate)
                                        .ToListAsync(); 
				}                
            }
            catch(Exception ex)
            {
                Log.Error(ex, "QuoteManager::GetList");
                throw;
            }        

            return quoteList;
        }


        public async Task<DateTime> GetLastQuoteDate()
        {
            DateTime lastDate = new DateTime(2015, 1, 1);

            using var db = new ReadContext(appSettings);
            if (db.Quotes.Count() > 0)
            {
                lastDate = await db.Quotes.MaxAsync(q => q.Date);
            }

            return lastDate;
        }
      
        public async Task<Quote> GetByEquityIdAndDate(Guid equityId, DateTime date)
        {
            Quote quote = new Quote();

            try
            {
                DateTime tradeDate = await GetNearestQuoteDate(date);

                var quoteList = await GetByEquityId(equityId);

                quote = quoteList.Where(q => q.Date == tradeDate).FirstOrDefault();
            }
            catch(Exception ex)
            {
                Log.Error(ex, "GetByEquityIdAndDate");
                throw;
            }        

            return quote;
        }

        internal async Task<DateTime> GetNearestQuoteDate(DateTime inDate)
        {
            DateTime nearestDate = new DateTime();
            DateTime lookDate = inDate;

            try
            {
                List<DateTime> tradeDates = new List<DateTime>();

                using (var db = new ReadContext(appSettings))
                tradeDates = await db.Quotes.Select(q => q.Date).Distinct().ToListAsync();

                bool foundIt = false;

                while (!foundIt)
                {
                    if (tradeDates.Where(d => d.ToShortDateString() == lookDate.ToShortDateString()).Any())
                    {
                        nearestDate = lookDate;
                        foundIt = true;
                    }
                    else
                    {
                        lookDate = lookDate.AddDays(-1);
                    }
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex, "GetByEquityIdAndDate");
                throw;
            }

            return nearestDate.Date;
        }

        internal async Task<List<Quote>> GetByEquityId(Guid equityId)
        {
            using var db = new ReadContext(appSettings);
            return await db.Quotes.Where(q => q.EquityId == equityId)
                                    .OrderBy(q => q.Date)
                                    .ToListAsync();
        }


        #region crud

        public async Task<Quote> Add(Quote quote)
        { 
            try
            {
                using var db = new WriteContext(appSettings);
                db.Quotes.Add(quote);
                await db.SaveChangesAsync();

            }
            catch(Exception ex)
            {
                Log.Error("QuoteManager::Add",ex);
                throw;
            }
             
            return quote;
        }


       public async Task<bool> Delete(Quote quote)
       {
            int x = 0;
            try
            {
                using var db = new WriteContext(appSettings);
                db.Quotes.Remove(quote);
                x = await db.SaveChangesAsync();
            }
            catch(Exception ex)
            {               
                Log.Error("QuoteManager::Delete",ex);
                throw;
            }
             
            return (x > 0);
        }

        #endregion
    }
}
