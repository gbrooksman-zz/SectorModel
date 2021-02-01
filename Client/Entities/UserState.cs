using SectorModel.Shared.Entities;
using System.Collections.Generic;

namespace SectorModel.Client.Entities
{
    public class UserState
    {
        public UserState() { }

		public User CurrentUser { get; set; }		
		public Model Model { get; set; }
        public List<Model> ModelList { get; set; }
		//public List<HelpTopic> HelpTopics { get; set; }
    }
}
