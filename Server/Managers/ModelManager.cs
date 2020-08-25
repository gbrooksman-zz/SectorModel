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

        public async Task<List<Model>> GetModelList(Guid userId)
        {
            List<Model> modelList = new List<Model>();
			List<Model> pricedModelList = new List<Model>();
           
            try
            {
                using var db = new ReadContext(appSettings);
                modelList = await db.Models
                                    .Where(i => i.UserId == userId  && i.IsActive == true)
                                    .ToListAsync(); 

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
                    model = await db.Models.Where(m => m.Id == modelId && m.IsActive == true).FirstOrDefaultAsync();

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

		public async Task<List<Quote>> GetDateRangeWithInterval(Guid modelId, DateTime startdate, DateTime stopdate, int quoteInterval )
        {			
            List<Quote> quoteList = new List<Quote>();
			List<Quote> finalQuoteList = new List<Quote>();

            try
            {
				using (var db = new ReadContext(appSettings))
				{	
					Model  model = await db.Models.FindAsync(modelId);

					model.ItemList = await db.ModelItems
										.Where( m => m.ModelId == modelId)
										.ToListAsync();

					List<Guid> equityGuids = model.ItemList.Select(e => e.EquityId).ToList();

					var allQuotes = await db.Quotes.ToListAsync();

					quoteList = (from q in allQuotes
								join l in equityGuids on q.EquityId equals l select q).ToList();

					quoteList = quoteList.Where (q => q.Date >= model.StartDate	&& q.Date <= stopdate).ToList();

					int skipLoops = quoteList.Count/quoteInterval;

					for ( int x = 0 ; x <= skipLoops ; x = x + quoteInterval)
					{
						finalQuoteList.Add(quoteList.Skip(x).Take(1).FirstOrDefault());
					}


				}               
            }
            catch(Exception ex)
            {
                Log.Error(ex, "ModelManager::GetDateRangeWithInterval");
                throw;
            }

            return finalQuoteList;
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
                model.StartDate = DateTime.Now; //whenever a model is edited reset the 'start' date

                using var db = new WriteContext(appSettings);
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
            catch(Exception ex)
            {
                Log.Error(ex, "ModelManager::Save");
                throw;
            } 

            return model;
        }      
    }
}

