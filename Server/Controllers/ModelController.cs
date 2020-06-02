using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SectorModel.Server.Managers;
using System.Threading.Tasks;
using Serilog;
using System;
using SectorModel.Shared.Entities;

namespace SectorModel.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ModelController : ControllerBase
    {
        private readonly UserModelManager umMgr;

        public ModelController(IMemoryCache _cache, IConfiguration _config)
        {
            umMgr = new UserModelManager(_cache, _config);
        }

        [HttpGet]
        [Route("GetModelList")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserModel>>> GetModelList(User user)
        {
            List<UserModel> mgrResult = await umMgr.GetActiveModelList(user);

            return Ok(mgrResult);
        }               

        [HttpGet]
        [Route("GetModel")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserModel>> GetModel(Guid modelId)
        { 
            UserModel mgrResult = await umMgr.GetModel(modelId);

            if (mgrResult == null)
            {
                return BadRequest(mgrResult);
            }
            else
            {
                return Ok(mgrResult);
            }
        }

        [HttpGet]
        [Route("GetModelVersions")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserModel>>> GetModelVersions(Guid modelId)
        {
            List<UserModel> mgrResult = await umMgr.GetModelVersions(modelId);

            if (mgrResult == null)
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