using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using SectorModel.Shared.Entities;
using Serilog;
using Microsoft.EntityFrameworkCore;
using SectorModel.Server.Data;

namespace SectorModel.Server.Managers
{
	public interface IModelManager
	{		Task<Model> Get(Guid modelId);
		Task<List<Quote>> GetDateRangeWithInterval(Guid modelId, DateTime startdate, DateTime stopdate, int quoteInterval);
		Task<Model> GetModel(Guid modelId, DateTime quoteDate);
		Task<ModelItem> GetModelItem(Guid modelEquityId);
		Task<List<ModelItem>> GetModelItems(Guid modelId);
		Task<List<Model>> GetModelList(Guid userId);
		Task<Model> RebalanceItems(Model model);
		Task<bool> RemoveModelItem(Guid modelItemId);
		Task<Model> Save(Model model);
		Task<ModelItem> SaveItem(ModelItem modelItem);
	}

	public class ModelManager : IModelManager
	{
		private readonly QuoteManager qMgr;
		/* private readonly IConfiguration config;
		 private readonly AppSettings appSettings;*/

		public ModelManager()
		{
			qMgr = new QuoteManager();
		}

		#region Models

		public async Task<List<Model>> GetModelList(Guid userId)
		{
			List<Model> modelList = new List<Model>();
			List<Model> pricedModelList = new List<Model>();

			try
			{
				using var db = new AppDBContext();
				modelList = await db.Models
									.Where(i => i.UserId == userId)
									.ToListAsync();

				foreach (Model model in modelList)
				{
					Model pricedModel = new Model();

					pricedModel = await GetModel(model.Id, DateTime.Now);

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
			Model model = new();

			try
			{
				using (var db = new AppDBContext())
				{
					model = await db.Models.Where(m => m.Id == modelId).FirstOrDefaultAsync();

					model.ItemList = await db.ModelItems
										.Where(m => m.ModelId == modelId)
										.ToListAsync();

					model.ItemList = await qMgr.GetModelItemsWithPrices(model, quoteDate);

					model.LatestValue = model.ItemList.Sum(m => m.CurrentValue);
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex, "ModelManager::GetModel");
				throw;
			}

			return model;
		}

		public async Task<List<Quote>> GetDateRangeWithInterval(Guid modelId, DateTime startdate, DateTime stopdate, int quoteInterval)
		{
			List<Quote> quoteList = new List<Quote>();
			List<Quote> finalQuoteList = new List<Quote>();

			try
			{
				using (var db = new AppDBContext())
				{
					Model model = await db.Models.FindAsync(modelId);

					model.ItemList = await db.ModelItems
										.Where(m => m.ModelId == modelId)
										.ToListAsync();

					List<Guid> equityGuids = model.ItemList.Select(e => e.EquityId).ToList();

					var allQuotes = await db.Quotes.Where(q => q.Date >= model.StartDate && q.Date <= stopdate).ToListAsync();

					quoteList = (from q in allQuotes
								 join e in equityGuids
								 on q.EquityId equals e
								 select q
									 ).ToList();

					int daysInPeriod = quoteList.Select(q => q.Date).Distinct().Count();

					int skipLoops = daysInPeriod / quoteInterval;

					for (int x = 0; x <= skipLoops; x += quoteInterval)
					{
						equityGuids.ForEach(e =>
							{
								Quote q = quoteList.Where(q => q.EquityId == e).Skip(x).Take(1).FirstOrDefault();
								if (q != default) finalQuoteList.Add(q);
							});
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex, "ModelManager::GetDateRangeWithInterval");
				throw;
			}

			return finalQuoteList;
		}


		public async Task<Model> Get(Guid modelId)
		{
			using (var db = new AppDBContext())
				return await db.Models.FindAsync(modelId);
		}

		#region CRUD 

		public async Task<Model> Save(Model model)
		{
			try
			{
				model.StartDate = DateTime.Now; //whenever a model is edited reset the 'start' date

				using var db = new AppDBContext();
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
			catch (Exception ex)
			{
				Log.Error(ex, "ModelManager::Save");
				throw;
			}

			return model;
		}

		#endregion

		#endregion

		#region ModelItems

		public async Task<ModelItem> GetModelItem(Guid modelEquityId)
		{
			ModelItem modelItem = new();

			try
			{
				using var db = new AppDBContext();
				modelItem = await db.ModelItems
									.Where(i => i.Id == modelEquityId)
									.FirstOrDefaultAsync();
			}
			catch (Exception ex)
			{
				Log.Error(ex, "ModelItemManager::GetModelItem");
				throw;
			}

			return modelItem;
		}

		public async Task<List<ModelItem>> GetModelItems(Guid modelId)
		{
			List<ModelItem> modelItemList = new();

			try
			{
				using var db = new AppDBContext();
				modelItemList = await db.ModelItems
									.Where(i => i.ModelId == modelId)
									.ToListAsync();

				if (modelItemList == null)
				{
					modelItemList = new List<ModelItem>();
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex, "ModelItemManager::GetModelItems");
				throw;
			}

			return modelItemList;
		}


		#region CRUD

		public async Task<ModelItem> SaveItem(ModelItem modelItem)
		{
			try
			{
				using var db = new AppDBContext();
				{
					if (modelItem.Id == Guid.Empty)
					{
						db.Add(modelItem);
					}
					else
					{
						db.Update(modelItem);
					}
					await db.SaveChangesAsync();
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex, "ModelItemManager::Save");
				throw;
			}

			return modelItem;
		}


		public async Task<bool> RemoveModelItem(Guid modelItemId)
		{
			int x = 0;

			try
			{
				using var dbR = new AppDBContext();
				ModelItem mi = dbR.ModelItems.Find(modelItemId);

				using var dbW = new AppDBContext();
				dbW.ModelItems.Remove(mi);
				x = await dbW.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				Log.Error(ex, "ModelItemManager::RemoveEquity");
				throw;
			}

			return x > 0;
		}

		public async Task<Model> RebalanceItems(Model model)
		{
			List<ModelItem> newItems = new();
			try
			{
				model.ItemList = await qMgr.GetModelItemsWithPrices(model, DateTime.Now);

				foreach (ModelItem item in model.ItemList)
				{
					item.Cost = model.StartValue * (item.Percentage / 100);
					item.Shares = item.Cost / item.LastPrice;
					item.CurrentValue = item.Cost;
					item.CreatedAt = DateTime.Now;
					await SaveItem(item);
					newItems.Add(item);
				}

				model.ItemList = newItems;
				await Save(model);

			}
			catch (Exception ex)
			{
				Log.Error(ex, "ModelManager::Rebalance");
				throw;
			}

			return model;
		}

		#endregion

		#endregion

	}
}

