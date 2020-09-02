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
			var topic = new HelpTopic();

			topic.Id = 1;
			topic.Text = (MarkupString) "This is Topic 1";
			helpTopics.Add(topic);

			topic.Id = 2;
			topic.Text = (MarkupString) "This is Topic 2";
			helpTopics.Add(topic);

			topic.Id = 3;
			topic.Text = (MarkupString) "This is Topic 3";
			helpTopics.Add(topic);

			return helpTopics;
		}
	}
}