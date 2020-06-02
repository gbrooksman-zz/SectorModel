using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Serilog;
using SectorModel.Server.Managers;
using SectorModel.Shared.Entities;
using System;
using System.Linq;

namespace SectorModel.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CloudDataController : ControllerBase
    {
       // private readonly EquityManager eqMgr;
       // private readonly QuoteManager qMgr;
        private readonly CloudDataManager cdMgr;       

        public CloudDataController(IMemoryCache cache, IConfiguration config)
        {
          //  eqMgr = new EquityManager(cache, config);
           // qMgr = new QuoteManager(cache, config);

           cdMgr = new CloudDataManager(cache, config);
        }

        [HttpGet]
        [Route("UpdateQuotes")]       
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> UpdateQuotes(DateTime lastQuoteDate, Guid coregroupid)
        {
            bool isOK = await cdMgr.UpdateQuotes(lastQuoteDate, coregroupid).ConfigureAwait(false);

            return Ok(isOK);
        }
    }
}