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
        private readonly ModelManager umMgr;
        private readonly AppSettings appSettings;

        public ModelController(IMemoryCache _cache, IConfiguration _config, AppSettings _appSettings)
        {
            appSettings = _appSettings;
            umMgr = new ModelManager(_cache, _config, appSettings);
        }

        [HttpGet]
        [Route("GetModelList")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Model>>> GetModelList(Guid userId)
        {
            List<Model> modelList = await umMgr.GetActiveModelList(userId);

            return Ok(modelList);
        }               

        [HttpGet]
        [Route("GetModel")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Model>> GetModel(Guid modelId)
        { 
            Model model = await umMgr.GetModel(modelId).ConfigureAwait(false);

            if (model == null)
            {
                return BadRequest(model);
            }
            else
            {
                return Ok(model);
            }
        }

		[HttpGet]
        [Route("GetCoreModel")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Model>> GetCoreModel()
        { 

			Model model = new Model();

			if (appSettings.CoreModel == null)
			{
            	model = await umMgr.GetModel(appSettings.CoreModelId);
				appSettings.CoreModel = model;
			}
			else
			{
				model = appSettings.CoreModel;
			}

            if (model == null)
            {
                return BadRequest(model);
            }
            else
            {
                return Ok(model);
            }
        }

		[HttpGet]
        [Route("GetSPDReModel")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Model>> GetSPDRModel()
        { 

			Model model = new Model();

			if (appSettings.SPDRModel == null)
			{
            	model = await umMgr.GetModel(appSettings.SPDRModelId);
				appSettings.SPDRModel = model;
			}
			else
			{
				model = appSettings.SPDRModel;
			}

            if (model == null)
            {
                return BadRequest(model);
            }
            else
            {
                return Ok(model);
            }
		}

        [HttpGet]
        [Route("GetModelVersions")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Model>>> GetModelVersions(Guid modelId)
        {
            List<Model> mgrResult = await umMgr.GetModelVersions(modelId);

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