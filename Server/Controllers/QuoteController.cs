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
        private readonly AppSettings appSettings;

        public QuoteController(IMemoryCache _cache, IConfiguration _config, AppSettings _appSettings)
        {
            appSettings = _appSettings;

            qMgr = new QuoteManager();
            eMgr = new EquityManager();
            mMgr = new ModelManager();
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
        [Route("GetList")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Quote>>> GetList(DateTime quoteDate)
        {
            List<Quote> quoteList = await qMgr.GetList(quoteDate);

            if (quoteList == null)
            {
                return BadRequest(quoteList);            
            }
            return Ok(quoteList);
        }


        [HttpGet]
        [Route("GetLastQuoteDate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<DateTime>> GetLastQuoteDate()
        {
            DateTime lastQuoteDate = await qMgr.GetLastQuoteDate();
			appSettings.LastQuoteDate = lastQuoteDate;		
            return Ok(lastQuoteDate);
        }
    }
}