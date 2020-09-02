using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace SectorModel.Client.Entities
{
	public class HelpTopic
	{
		public HelpTopic()
		{
			
		}

		public int Id {get; set;}

		public MarkupString Text { get; set; }
	}
}

