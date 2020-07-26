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
    public class ModelCommentManager : BaseManager
    {
        private readonly IConfiguration config;
        private readonly AppSettings appSettings;

        public ModelCommentManager(IMemoryCache _cache, IConfiguration _config, AppSettings _appSettings) : base(_cache, _config)
        {
            config = _config;
            appSettings = _appSettings;
        }


        public async Task<bool> Add(ModelComment modelComment)
        {
            int x = 0;
            try
            {
                using var db = new WriteContext(appSettings);               
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
            List<ModelComment> modelCommentList = new List<ModelComment>();

            try
            {
                using var db = new ReadContext(appSettings);
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
