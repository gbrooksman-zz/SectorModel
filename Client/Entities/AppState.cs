using SectorModel.Shared.Entities;
using System.Collections.Generic;

namespace SectorModel.Client.Entities
{
    public class AppState
    {
        public AppState() { }

		public User CurrentUser { get; set; }		
		public Model Model { get; set; }
        public List<Model> ModelList { get; set; }
		public List<HelpTopic> HelpTopics { get; set; }
    }
}
