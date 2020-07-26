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
        private readonly ModelManager egMgr;
        private readonly ModelItemManager miMgr;

        public EquityGroupController(IMemoryCache _cache, IConfiguration _config, IAppSettings _appSettings)
        {
            egMgr = new ModelManager(_cache, _config, _appSettings);
            miMgr = new ModelItemManager(_cache, _config, _appSettings);
        }

       /* [HttpGet]
        [Route("GetList")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Model>>> GetList()
        {
            List<Model> modelList = await egMgr.GetActiveModelList();
            if (!modelList.Any())
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
                return Ok(modelList);
            }*/
       // }

        [HttpGet]
        [Route("GetGroupItems")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ModelItem>>> GetGroupItems(Guid modelId)
        {
            List<ModelItem> mgrResult = await miMgr.GetModelEquityList(modelId);
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
        public async Task<ActionResult<int>> GetCount(Guid modelId)
        {
            int mgrResult = await miMgr.GetEquitiesInModelsCount(modelId);
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