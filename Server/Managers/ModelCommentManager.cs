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

    public interface IModelCommentManager
	{
        Task<bool> Add(ModelComment modelComment);
        Task<List<ModelComment>> GetModelComments(Guid modelId);
    }

    public class ModelCommentManager : IModelCommentManager
    {
        public ModelCommentManager() 
        {

        }

        public async Task<bool> Add(ModelComment modelComment)
        {
            int x = 0;
            try
            {
                using var db = new AppDBContext();               
                db.Add(modelComment);                 
                x = await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ModelCommentManager::Add");
                throw;
            }

            return x > 0;
        }

        public async Task<List<ModelComment>> GetModelComments(Guid modelId)
        {
            List<ModelComment> modelCommentList = new();

            try
            {
                using var db = new AppDBContext();
                modelCommentList = await db.ModelComments
                                    .Where(i => i.ModelId == modelId)                                    
                                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ModelCommentManager::GetModelComments");
                throw;
            }

            return modelCommentList;
        }
    }
}
