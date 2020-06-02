using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using SectorModel.Shared.Entities;
using Serilog;
using System.Net.Http;
using System.Text.Json;
using System.Globalization;

namespace SectorModel.Server.Managers
{
    public class CloudDataManager : BaseManager
    {
        // this manager holds methods for equity_groups and equity_group_items tables
        // equity groups are collections of equities that are related by some criteria
        // these are different from user models which are collections of equities created
        // by users for whatever purposed

        private readonly string baseURL;
        private readonly string quoteURL;
        private readonly string apiToken;
        private readonly string tokenURL;

        private static HttpClient client;

        private readonly EquityGroupManager egMgr;
        private readonly QuoteManager qMgr;

       public CloudDataManager(IMemoryCache cache, IConfiguration config) : base(cache, config)
        {
            egMgr = new EquityGroupManager(cache, config);
            qMgr = new QuoteManager(cache, config);
           
            client = new HttpClient();

            apiToken = config.GetSection("IEXCloud").GetValue<string>("APIToken");

            baseURL = "https://cloud.iexapis.com/stable/stock/";
            quoteURL = "/chart/date/";
            tokenURL = $"?chartByDay=true&token={apiToken}";

            client.BaseAddress = new Uri(baseURL);
        }

        private string FormatDateForQuery(DateTime inDate)
        {
            return inDate.ToString("yyyyMMdd");
        }       

        public async Task<bool> UpdateQuotes(DateTime lastQuoteDate, Guid coregroupid)
        {
            List<string> datesToLoad = GetDatesToLoad(lastQuoteDate);

            List<Equity> egiList = await egMgr.GetGroupEquities(coregroupid).ConfigureAwait(false);

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                IgnoreNullValues = true
            };

            try
            {
                foreach (Equity e in egiList)
                {
                    foreach (string quoteDate in datesToLoad)
                    {                       
                        string url = $"{baseURL}{e.Symbol}{quoteURL}{quoteDate}{tokenURL}";

                        string response = await client.GetStringAsync(new Uri(url)).ConfigureAwait(false);

                        if (response != "[]")
                        {
                            string trimmedResponse = response.TrimStart("[".ToCharArray()).TrimEnd("]".ToCharArray());

                            IEXQuote iexQuote = JsonSerializer.Deserialize<IEXQuote>(trimmedResponse, options);

                            Quote quote = new Quote()
                            {
                                EquityId = e.Id,
                                Date = DateTime.ParseExact(iexQuote.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                                Price = iexQuote.UClose,
                                Volume = iexQuote.Volume
                            };

                            await qMgr.Add(quote).ConfigureAwait(false);
                        }
                    }

                }
            }
            catch(Exception ex)
            {
                Log.Error(ex, "CloudDataManager::UpdateQuotes");
                throw;
            }
            
            return true;
        }

        private List<string> GetDatesToLoad(DateTime lastQuoteDate)
        {
            List<string> dateList = new List<string>();

            for (var day = lastQuoteDate.Date.AddDays(1); day.Date <= DateTime.Now.Date; day = day.AddDays(1))
            {
                dateList.Add(FormatDateForQuery(day));
            }

            return dateList;
        }

    }



    public class IEXQuote
    {
        public IEXQuote()
        {

        }

        public string Symbol { get; set; }

        public decimal UClose { get; set; }

        public string Date { get; set; }

        public int Volume { get; set; }
    }
}
