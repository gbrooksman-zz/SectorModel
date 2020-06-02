using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using SectorModel.Shared.Entities;
using SectorModel.Server.Managers;
using System.Linq;

namespace SectorModel.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EquityGroupController : ControllerBase
    {
        private readonly EquityGroupManager egMgr;

        public EquityGroupController(IMemoryCache _cache, IConfiguration _config)
        {
            egMgr = new EquityGroupManager(_cache, _config);
        }

        [HttpGet]
        [Route("GetList")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<EquityGroup>>> GetList()
        {
            List<EquityGroup> mgrResult = await egMgr.GetList();
            if (!mgrResult.Any())
            {
                return Problem(
                    detail:"",
                    instance: "EquityGroupController/GetList",
                    statusCode: 500,
                    title: "Failed to get List of equity groups",
                    type: ""
                );
            }
            else
            {
                return Ok(mgrResult);
            }
        }

        [HttpGet]
        [Route("GetGroupItems")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<EquityGroupItem>>> GetGroupItems(Guid equityGroupid)
        {
            List<EquityGroupItem> mgrResult = await egMgr.GetGroupItemsList(equityGroupid);
            if (!mgrResult.Any())
            {
                return BadRequest(mgrResult);
            }
            else
            {
                return Ok(mgrResult);
            }
        }

        [HttpGet]
        [Route("GetCount")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> GetCount(Guid equityGroupid)
        {
            int mgrResult = await egMgr.GetItemsCount(equityGroupid);
            if (mgrResult == 0)
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