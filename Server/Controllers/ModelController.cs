﻿using System.Collections.Generic;
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
            List<Model> modelList = await umMgr.GetModelList(userId);

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
            Model model = await umMgr.GetModel(modelId);

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
        [Route("GetSPDRModel")]
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
            List<Model> result = await umMgr.GetModelVersions(modelId);

            if (result == null)
            {
                return BadRequest(result);
            }
            else
            {
                return Ok(result);
            }
        }

		[HttpPost]
        [Route("Save")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Model>> Save(Model model)
        {
            model = await umMgr.Save(model);

            if (model == null)
            {
                return BadRequest(model);
            }
            else
            {
                return Ok(model);
            }
        }
    }
}