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

        public QuoteController(IMemoryCache _cache, IConfiguration _config)
        {
            qMgr = new QuoteManager(_cache, _config);
            eMgr = new EquityManager(_cache, _config);
            mMgr = new ModelManager(_cache, _config);
            miMgr = new ModelItemManager(_cache, _config);
        }

        [HttpGet]
        [Route("GetRange")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Quote>>> GetRange(string symbol, DateTime startdate, DateTime stopdate)
        {
            List<Quote> mrQuoteList = new List<Quote>();
            Equity equity = await eMgr.GetBySymbol(symbol);

            if (equity == default(Equity))
            {               
                return BadRequest(mrQuoteList);
            }
            else
            {
                mrQuoteList = await qMgr.GetByEquityIdAndDateRange(equity.Id, startdate, stopdate);
                if (!mrQuoteList.Any())
                {
                    return BadRequest(mrQuoteList);
                }
            }
            return Ok(mrQuoteList);
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

            Quote mrQuoteList = new Quote();

            if (equity == default(Equity))
            {               
                return BadRequest(mrQuoteList);
            }
            else
            {               
                mrQuoteList = await qMgr.GetByEquityIdAndDate(equity.Id, date);
                if (mrQuoteList == null)
                {
                    return BadRequest(mrQuoteList);
                }
            }
            return Ok(mrQuoteList);
        }

        [HttpGet]
        [Route("GetModelByDate")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ModelItem>>> GetModelByDate(Guid modelId, DateTime date, int versionNumber)
        {
            List<ModelItem> modelEquityList = await miMgr.GetModelEquityListByDate(modelId, date, versionNumber);

            if (modelEquityList == null)
            {
                return BadRequest(modelEquityList);
            }
           
            return Ok(modelEquityList);
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
        [Route("GetLastQuoteDate")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<DateTime>> GetLastQuoteDate()
        {
            DateTime mrLastQuoteDate = await qMgr.GetLastQuoteDate();
            if (mrLastQuoteDate == null)
            {
                return BadRequest(mrLastQuoteDate);
            }
            return Ok(mrLastQuoteDate);
        }



    }
}