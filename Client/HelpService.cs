using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using SectorModel.Client.Entities;

namespace SectorModel.Client
{
	public class HelpService
    {	
		public HelpService()
		{
			
		}

		public List<HelpTopic> LoadTopics()
		{
			var helpTopics = new List<HelpTopic>();

            var topic1 = new HelpTopic
            {
                Id = 1,
                Text =  (MarkupString) "This is Topic 1"
            };
            helpTopics.Add(topic1);

			var topic2 = new HelpTopic
			{
				Id = 2,
				Text = (MarkupString)"This is Topic 2"
			};
			helpTopics.Add(topic2);

			var topic3 = new HelpTopic
			{
				Id = 3,
				Text = (MarkupString) "This is Topic 3"
			};
			helpTopics.Add(topic3);


			return helpTopics;
		}
	}
}