using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SectorModel.Server.Managers;
using System.Threading.Tasks;
using SectorModel.Shared.Entities;
using System.Linq;
using SectorModel.Shared;

namespace SectorModel.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class QuoteController : ControllerBase
    {
        private readonly QuoteManager qMgr;
        private readonly EquityManager eMgr;
        private readonly ModelManager mMgr;
        private readonly ModelItemManager miMgr;

        private readonly AppSettings appSettings;

        public QuoteController(IMemoryCache _cache, IConfiguration _config, AppSettings _appSettings)
        {
            appSettings = _appSettings;

            qMgr = new QuoteManager(_cache, _config, appSettings);
            eMgr = new EquityManager(_cache, _config, appSettings);
            mMgr = new ModelManager(_cache, _config, appSettings);
            miMgr = new ModelItemManager(_cache, _config, appSettings);
        }

        [HttpGet]
        [Route("GetRange")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Quote>>> GetRange(string symbol, DateTime startdate, DateTime stopdate)
        {
            List<Quote> quoteList = new List<Quote>();
            Equity equity = await eMgr.GetBySymbol(symbol);

            if (equity == default(Equity))
            {               
                return BadRequest(quoteList);
            }
            else
            {
                quoteList = await qMgr.GetByEquityIdAndDateRange(equity.Id, startdate, stopdate);
                if (!quoteList.Any())
                {
                    return BadRequest(quoteList);
                }
            }
            return Ok(quoteList);
        }

        [HttpGet]
        [Route("GetRangeForList")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Quote>>> GetRangeForList(string symbols, DateTime startdate, DateTime stopdate)
        {
            List<string> symbolList = new List<string>(symbols.Split(","));

            List<Quote> mrQuoteList = await qMgr.GetBySymbolListAndDateRange(symbolList, startdate, stopdate);

            if (mrQuoteList == null)
            {
                return BadRequest(mrQuoteList);
            }
            else
            {
                return Ok(mrQuoteList);
            }
        }

        [HttpGet]
        [Route("GetDate")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Quote>> GetDate(string symbol, DateTime date)
        {
            Equity equity = await eMgr.GetBySymbol(symbol);

            Quote quoteList = new Quote();

            if (equity == default(Equity))
            {               
                return BadRequest(quoteList);
            }
            else
            {               
                quoteList = await qMgr.GetByEquityIdAndDate(equity.Id, date);
                if (quoteList == null)
                {
                    return BadRequest(quoteList);
                }
            }
            return Ok(quoteList);
        }
      

        [HttpGet]
        [Route("GetLast")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Quote>> GetLast(Guid equityid)
        {
            Quote mrQuoteList = await qMgr.GetLast(equityid);
            if (mrQuoteList == null)
            {
                return BadRequest(mrQuoteList);            
            }
            return Ok(mrQuoteList);
        }

		[HttpGet]
        [Route("GetModelItemPrices")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ModelItem>>> GetModelItemPrices(Guid modelId, DateTime quoteDate)
        {
			Model model = await mMgr.GetModelFull(modelId);

			List<ModelItem> items = new List<ModelItem>();

			foreach (ModelItem mi in model.ItemList)
			{
				Quote quote = await qMgr.GetLast(mi.EquityId);
				mi.LastPrice = quote.Price;
				mi.LastPriceDate = quote.Date;
				mi.Equity = await eMgr.Get(mi.EquityId);
				mi.CurrentValue = mi.Shares * mi.LastPrice;				
				items.Add(mi);	
			}
            
            return Ok(items);
        }

        [HttpGet]
        [Route("GetLastQuoteDate")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<DateTime>> GetLastQuoteDate()
        {
            DateTime lastQuoteDate = await qMgr.GetLastQuoteDate();

			appSettings.LastQuoteDate = lastQuoteDate;

            if (lastQuoteDate == null)
            {
                return BadRequest(lastQuoteDate);
            }
			
            return Ok(lastQuoteDate);
        }



    }
}