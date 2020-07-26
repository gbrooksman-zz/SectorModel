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
        private readonly CloudDataManager cdMgr;       

        public CloudDataController(IMemoryCache _cache, IConfiguration _config, AppSettings _appSettings)
        {
           cdMgr = new CloudDataManager(_cache, _config, _appSettings);
        }

        [HttpGet]
        [Route("UpdateQuotes")]       
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> UpdateQuotes(DateTime lastQuoteDate, Guid coreModelId)
        {
            bool isOK = await cdMgr.UpdateQuotes(lastQuoteDate, coreModelId).ConfigureAwait(false);

            return Ok(isOK);
        }
    }
}