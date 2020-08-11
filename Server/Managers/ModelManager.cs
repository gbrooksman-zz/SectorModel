using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using SectorModel.Shared.Entities;
using Serilog;
using Microsoft.EntityFrameworkCore;

namespace SectorModel.Server.Managers
{
    public class ModelManager : BaseManager
    {
        private readonly EquityManager eqMgr;
        private readonly QuoteManager qMgr;
        private readonly IConfiguration config;
        private readonly AppSettings appSettings;

        public ModelManager(IMemoryCache _cache, IConfiguration _config, AppSettings _appSettings) : base(_cache, _config)
        {
            appSettings = _appSettings;
            config = _config;

            eqMgr = new EquityManager(_cache, config, appSettings);
            qMgr = new QuoteManager(_cache, config, appSettings);            
        }

        public async Task<List<Model>> GetModelList(Guid userId, bool activeOnly = false, bool privateOnly = false)
        {
            List<Model> modelList = new List<Model>();
			List<Model> pricedModelList = new List<Model>();
           
            try
            {
                using var db = new ReadContext(appSettings);
                modelList = await db.Models
                                    .Where(i => i.UserId == userId)
                                    .ToListAsync(); 
				if (activeOnly)
				{
					modelList = modelList.Where( i => i.IsActive == true).ToList();
				} 
				if (privateOnly)
				{
					modelList = modelList.Where( i => i.IsPrivate == true).ToList();
				} 

				foreach (Model model in modelList)
				{
					Model pricedModel = new Model();

					if (model.StopDate > DateTime.Now)
					{
						pricedModel = await GetModel(model.Id, DateTime.Now);
					}
					else
					{
						pricedModel = await GetModel(model.Id, model.StopDate);
					}
					pricedModelList.Add(pricedModel);
				}				
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ModelManager::GetModelList");
                throw;
            } 

            return pricedModelList;
        }

        public async Task<Model> GetModel(Guid modelId, DateTime quoteDate)
        {
            Model model = new Model();

            try
            {
                using (var db = new ReadContext(appSettings))
                {
                    model = await db.Models.FindAsync(modelId);

					model.ItemList = await db.ModelItems
										.Where( m => m.ModelId == modelId)
										.ToListAsync();

					model.ItemList = await qMgr.GetModelItemsWithPrices(model, quoteDate);

                    model.StopValue = model.ItemList.Sum(m => m.CurrentValue);				
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ModelManager::GetModel");
                throw;
            }

            return model;    
        }   

		public async Task<Model> Get(Guid modelId)
        {
			using (var db = new ReadContext(appSettings))
			return await db.Models.FindAsync(modelId);  
        }   
	
        public async Task<Model> Save(Model model)
        {            
            try
            {
                using (var db = new WriteContext(appSettings))
                {
                    if (model.Id == Guid.Empty)
                    {
                        db.Models.Add(model);
                    }
                    else
                    {
                        db.Models.Update(model);
                    }
                    await db.SaveChangesAsync();
                }
                
            }
            catch(Exception ex)
            {
                Log.Error(ex, "ModelManager::Save");
                throw;
            } 

            return model;
        }

    }
}

