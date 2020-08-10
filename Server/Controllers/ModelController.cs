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
        private readonly ModelManager mMgr;
		private readonly ModelItemManager miMgr;
        private readonly AppSettings appSettings;

        public ModelController(IMemoryCache _cache, IConfiguration _config, AppSettings _appSettings)
        {
            appSettings = _appSettings;
            mMgr = new ModelManager(_cache, _config, appSettings);
			miMgr = new ModelItemManager(_cache, _config, appSettings);
        }

        [HttpGet]
        [Route("GetModelList")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Model>>> GetModelList(Guid userId)
        {
            List<Model> modelList = await mMgr.GetModelList(userId);

            return Ok(modelList);
        }               

        [HttpGet]
        [Route("GetModel")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Model>> GetModel(Guid modelId)
        {
            return await Get(modelId);
        }

        [HttpGet]
        [Route("Get")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Model>> Get(Guid modelId)
        {
            Model model = await mMgr.GetModel(modelId);

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
        [Route("GetWithItems")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Model>> GetWithItems(Guid modelId)
        {
            Model model = await mMgr.GetModelFull(modelId);

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
        [Route("GetModelFullWithPrices")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Model>> GetModelFullWithPrices(Guid modelId, DateTime quoteDate)
        {
            Model model = await mMgr.GetModelFullWithPrices(modelId, quoteDate);

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
        [Route("GetItems")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ModelItem>>> GetItems(Guid modelId)
        {
            List<ModelItem> modelItems = await miMgr.GetModelItems(modelId);
            return Ok(modelItems);
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
            	model = await mMgr.GetModel(appSettings.CoreModelId);
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
        [Route("GetSPDRModel")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Model>> GetSPDRModel()
        { 

			Model model = new Model();

			if (appSettings.SPDRModel == null)
			{
            	model = await mMgr.GetModel(appSettings.SPDRModelId);
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
       
		[HttpPost]
        [Route("Save")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Model>> Save(Model model)
        {
            model = await mMgr.Save(model);

            if (model == null)
            {
                return BadRequest(model);
            }
            else
            {
                return Ok(model);
            }
        }

		[HttpPost]
        [Route("SaveItem")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Model>> SaveItem(ModelItem modelItem)
        {
            modelItem = await miMgr.Save(modelItem);

            if (modelItem == null)
            {
                return BadRequest(modelItem);
            }
            else
            {
                return Ok(modelItem);
            }
        }
    }
}