using System;
using System.Collections.Generic;

namespace SectorModel.Shared.Entities
{
    public class AppSettings
    {
        public AppSettings()
        {

        }
       
	    public DateTime LastQuoteDate { get; set; }  
        public Guid CoreModelId { get; } = Guid.Parse("FAC8A666-74D8-4531-B3AD-DA7B95360462");
		public Model CoreModel { get; set; }  
        public Guid SPDRModelId { get; } = Guid.Parse("4237E32A-5C60-4141-8658-FA357C28EF28");  
		public Model SPDRModel { get; set; }       
        public string DBConnectionString { get; set; } 
        public string IEXCloudAPIKey { get; set; }		
		public List<Equity> AllEquities { get; set; }
		public List<Quote> LatestQuotes { get; set; }


    }
}