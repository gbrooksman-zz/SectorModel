using Microsoft.AspNetCore.Components;
using SectorModel.Client.Entities;
using SectorModel.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


namespace SectorModel.Client.Services
{
    public class EquityService
    {

        private readonly AppSettings state;
        private readonly HttpClient httpClient;

        public EquityService(HttpClient _httpClient, AppSettings _state)
        {
            httpClient = _httpClient;
            state = _state;
        }

        public int GetInterval(DateTime startDate, DateTime stopDate)
        {
            int ret = 0;

            int daysInPeriod = (stopDate - startDate).Days;

            if ((daysInPeriod >= 0) && (daysInPeriod <= 31))
            {
                ret = 1;
            }
            else if ((daysInPeriod > 31) && (daysInPeriod <= 100))
            {
                ret = 3;
            }
            else if ((daysInPeriod > 101) && (daysInPeriod <= 365))
            {
                ret = 5;
            }
            else if (daysInPeriod > 365)
            {
                ret = 10;
            }

            return ret;
        }



        //public async Task<List<EquityQuotes>> GetEquitiesWithQuotes(string EquityList, DateTime StartDate, DateTime StopDate, int quoteInterval)
        //{
        //    List<Quote> allQuotesList = new List<Quote>();

        //    List<EquityQuotes> equitiesWithQuotes = new List<EquityQuotes>();

        //    List<string> quotes = new List<string>(EquityList.Split(";".ToCharArray()));

        //    allQuotesList = await httpClient.GetFromJsonAsync<List<Quote>>($@"quote/GetRangeForList
        //                                                        ?symbols={EquityList}
        //                                                        &startdate={StartDate}
        //                                                        &stopdate={StopDate}");

        //    List<Equity> equities = await httpClient.GetFromJsonAsync<List<Equity>>($@"equity/GetList?symbols={EquityList}");

        //    foreach (Equity equity in equities)
        //    {
        //        List<Quote> quoteList = allQuotesList.Where(q => q.EquityId == equity.Id)
        //                                                .OrderByDescending(q => q.Date)
        //                                                .ToList();

        //        List<Quote> quoteListSkipped = new List<Quote>();

        //        for (int u = 0; u < quoteList.Count; u += quoteInterval)
        //        {
        //            quoteListSkipped.Add(quoteList.Skip(quoteInterval).Take(1).FirstOrDefault());
        //        }

        //        equitiesWithQuotes.Add(new EquityQuotes
        //        {
        //            Equity = equity,
        //            Quotes = quoteListSkipped
        //        });

        //    }

        //    return equitiesWithQuotes;
        //}
    }
}
