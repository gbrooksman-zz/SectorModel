using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SectorModel.Server.Managers;
using System.Threading.Tasks;
using System.Linq;
using System;
using SectorModel.Shared.Entities;

namespace SectorModel.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ModelController : ControllerBase
    {
        private readonly ModelManager mMgr;
		private readonly EquityManager eqMgr;

        public ModelController()
        {
            mMgr = new ModelManager();
			eqMgr = new EquityManager();
        }

        [HttpGet]
        [Route("GetList")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Model>>> GetList(Guid userId)
        {
            List<Model> modelList = await mMgr.GetModelList(userId);

            return Ok(modelList);
        }               

        [HttpGet]
        [Route("Get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Model>> Get(Guid modelId, DateTime quoteDate)
        {
            Model model = await mMgr.GetModel(modelId, quoteDate);           
            return Ok(model);
        }		
      
	    [HttpGet]
        [Route("GetSimple")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Model>> GetSimple(Guid modelId)
        {
            Model model = await mMgr.Get(modelId);           
            return Ok(model);
        }	

		[HttpGet]
        [Route("GetQuotesForDateRange")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<Quote>>> GetQuotesForDateRange(Guid modelId, DateTime startdate, DateTime stopdate, int interval)
        {
            List<Quote> quoteList = await mMgr.GetDateRangeWithInterval(modelId, startdate, stopdate,interval);
            return Ok(quoteList);
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
        [Route("Copy")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Model>> Copy(Model model)
        {
            model = await mMgr.Save(model);
            List<ModelItem> newItemList = new();

            foreach (ModelItem item in model.ItemList)
            {
                item.ModelId = model.Id;
                ModelItem newItem = await mMgr.SaveItem(item);
                newItemList.Add(newItem);
            }

            model.ItemList = newItemList;
            return Ok(model);
        }

        [HttpPost]
        [Route("SaveItem")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ModelItem>> SaveItem(ModelItem modelItem)
        {
            modelItem.Equity = await eqMgr.Get(modelItem.EquityId);
			modelItem = await mMgr.SaveItem(modelItem);			 

            if (modelItem == null)
            {
                return BadRequest(modelItem);
            }
            else
            {
                return Ok(modelItem);
            }
        }

        [HttpDelete]
        [Route("RemoveModelItem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> RemoveModelItem(Guid modelItemId)
        {
            bool deleted = await mMgr.RemoveModelItem(modelItemId);
            return Ok(deleted);

        }

		[HttpPut]
        [Route("RebalanceItems")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Model>> RebalanceItems(Model model)
        {
			Model newModel = await mMgr.RebalanceItems(model); 
           	return Ok(newModel);
        }
    }
}