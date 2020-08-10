using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using SectorModel.Shared.Entities;
using System.Net.Http;
using System.Text.Json;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace SectorModel.QuoteLoader
{
    public class CloudDataManager
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

        private readonly AppSettings appSettings;


        public CloudDataManager() 
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                IgnoreNullValues = true
            };

            string settings = File.ReadAllText($"{Directory.GetCurrentDirectory()}/secretsettings.json");

            appSettings = JsonSerializer.Deserialize<AppSettings>(settings, options);

            client = new HttpClient();

            apiToken = appSettings.IEXCloudAPIKey;

            baseURL = "https://cloud.iexapis.com/stable/stock/";
            quoteURL = "/chart/date/";
            tokenURL = $"?chartByDay=true&token={apiToken}";

            client.BaseAddress = new Uri(baseURL);
        }

        private string FormatDateForQuery(DateTime inDate)
        {
            return inDate.ToString("yyyyMMdd");
        }

        public async Task<bool> UpdateQuotes(Guid coreModelId)
        {
            DateTime lastQuoteDate = new DateTime(2015,1,1);

            List<string> datesToLoad = GetDatesToLoad(lastQuoteDate);

            List<Equity> egiList = new List<Equity>();

            var modelItems = await GetModelItems(coreModelId);

            // foreach (ModelItem item in modelItems)
            // {
            //     egiList.Add(await GetEquity(item.EquityId));
            // }

			egiList.Add(new Equity{Symbol="XLRE", SymbolName="The Real Estate Select Sector SPDR Fund",Id= new Guid("e16836ea-056a-4187-9a99-961d77196865")});

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                IgnoreNullValues = true
            };

           
			foreach (Equity e in egiList)
			{
			//	Console.WriteLine($"loading quotes for {e.Symbol} {e.SymbolName}");
				foreach (string quoteDate in datesToLoad)
				{
					string url = $"{baseURL}{e.Symbol}{quoteURL}{quoteDate}{tokenURL}";
					// string url = $"{baseURL}XLF{quoteURL}{quoteDate}{tokenURL}";

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

						await AddQuote(quote);
					}
				}

			}
           
            return true;
        }

        public async Task<Quote> AddQuote(Quote quote)
        {
		
			using var db = new WriteContext(appSettings);
			db.Quotes.Add(quote);
			await db.SaveChangesAsync();
            
            return quote;
        }


        public async Task<Equity> GetEquity(Guid equityId)
        {
            Equity equity = new Equity();

            using var db = new ReadContext(appSettings);
            equity = await db.Equities.Where(i => i.Id == equityId).FirstOrDefaultAsync();

            return equity;
        }

        public async Task<List<ModelItem>> GetModelItems(Guid modelId)
        {
            List<ModelItem> modelItemList = new List<ModelItem>();

                using var db = new ReadContext(appSettings);
                modelItemList = await db.ModelItems
                                    .Where(i => i.ModelId == modelId)
                                    .ToListAsync();            
            return modelItemList;
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

        // internal string Symbol { get; set; }

        public decimal UClose { get; set; }

        public string Date { get; set; }

        public int Volume { get; set; }
    }
}
