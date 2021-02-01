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
    public class EquityController : ControllerBase
    {
        private readonly EquityManager eqMgr;

        public EquityController()
        {
            eqMgr = new EquityManager();
        }

        [HttpGet]
        [Route("GetAll")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Equity>>> GetAll()
        {
            List<Equity> mgrResult = await eqMgr.GetAll();
           
            return Ok(mgrResult);
        }

        [HttpGet]
        [Route("GetList")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Equity>>> GetList(string symbols)
        {
            List<Equity> mgrResult = await eqMgr.GetList(symbols);

            return Ok(mgrResult);
        }


        [HttpGet]
        [Route("GetBySymbol")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Equity>> GetBySymbol(string symbol)
        {
            Equity mgrResult =  await eqMgr.GetBySymbol(symbol);

            if (mgrResult == default(Equity))
            {               
                return BadRequest(mgrResult);
            }
            else
            {
                return Ok(mgrResult);
            }
        }       
    }
}