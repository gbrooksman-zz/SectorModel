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

        public QuoteManager(IMemoryCache cache, IConfiguration config) : base(cache, config)
        {            
            eqMgr = new EquityManager(cache, config);
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

        public async Task<Quote> GetLast(Guid equityid)
        {
            Quote quote = new Quote();

            try
            {
                List<Quote> quoteList = await GetByEquityId(equityid).ConfigureAwait(false);

                quote = quoteList.OrderByDescending(q => q.Date).FirstOrDefault();  
            }
            catch(Exception ex)
            {
                Log.Error(ex, "QuoteManager::GetLast");
                throw;
            }        

            return quote;
        }

        public async Task<DateTime> GetLastQuoteDate()
        {
            DateTime maxDate = DateTime.MinValue;

            using (var db = new WriteContext())
            {
                maxDate = await db.Quotes.AsNoTracking().MaxAsync(q => q.Date);
            }

            return maxDate;
        }
      
        public async Task<Quote> GetByEquityIdAndDate(Guid equityId, DateTime date)
        {
            Quote quote = new Quote();

            try
            {
                DateTime tradeDate = await GetNearestQuoteDate(date);

                var quoteList = await GetByEquityId(equityId).ConfigureAwait(false);

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

                using (var db = new WriteContext())
                {
                    var quotes = db.Quotes.AsNoTracking().GroupBy(q => q.Date).Distinct();
                    await quotes.ForEachAsync( q => { tradeDates.Add(q.Key); });
                }

                    //List<DateTime> tradeDates = cache.GetOrCreate<List<DateTime>>(CacheKeys.TRADING_DATES, entry =>
                    //{
                    //    using NpgsqlConnection db = new NpgsqlConnection(connString);
                    //    return db.Query<DateTime>(@"SELECT DISTINCT(date) FROM quotes ORDER BY 1 DESC").ToList();
                    //});

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
            //return await cache.GetOrCreateAsync<List<Quote>>(CacheKeys.QUOTE_LIST + equityId, entry =>
            // {

            List<Quote> quotes = new List<Quote>();

            using (var db = new WriteContext())
            {
                quotes = await db.Quotes.AsNoTracking().Where(q => q.EquityId == equityId)
                                        .OrderBy(q => q.Date)
                                        .ToListAsync();
            }
            return quotes;

                //using NpgsqlConnection db = new NpgsqlConnection(connString);
                //return Task.FromResult(db.Query<Quote>(@"   SELECT * 
                //                                             FROM quotes 
                //                                             WHERE equity_id = @p1",
                //            new { p1 = equityId })
                //            .OrderBy(q => q.Date)
                //            .ToList());
              //  });
        }


        #region crud

        public async Task<Quote> Add(Quote quote)
        { 
            try
            {
                using var db = new WriteContext();
                await db.Quotes.AddAsync(quote);
                await db.SaveChangesAsync();

                //using NpgsqlConnection db = new NpgsqlConnection(base.connString);
                //{
                //    string sql = @"INSERT INTO QUOTES 
                //                (   DATE, PRICE, VOLUME,
                //                    CREATED_AT, UPDATED_AT, 
                //                    EQUITY_ID, RATE_OF_CHANGE
                //                )  
                //                VALUES ( @p1 , @p2, @p3, @p4, @p5, @p6, @p7 )";

                //    await db.ExecuteAsync(sql,
                //        new
                //        {
                //            p1 = quote.Date,
                //            p2 = quote.Price,
                //            p3 = quote.Volume,
                //            p4 = DateTime.Now,
                //            p5 = DateTime.Now,
                //            p6 = quote.EquityId,
                //            p7 = quote.RateOfChange
                //        }).ConfigureAwait(false);
                //}
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
                using var db = new WriteContext();
                db.Quotes.Remove(quote);
                x = await db.SaveChangesAsync();

                //if (eqMgr.Get(quote.EquityId).Result != default(Equity))
                //{
                //    using NpgsqlConnection db = new NpgsqlConnection(base.connString);
                //    await db.DeleteAsync(quote).ConfigureAwait(false);
                //}

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
