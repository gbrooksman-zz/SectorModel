using SectorModel.Shared.Entities;
using System;
using System.Collections.Generic;

namespace SectorModel.Client.Entities
{
    public class AppState
    {
        public AppState()
        {

        }

		public User CurrentUser { get; set; }		
        public List<ModelItem> CurrentModelItems { get; set; }
		public Model CurrentModel { get; set; }
        public List<Model> AllUserModels { get; set; }
    }
}
