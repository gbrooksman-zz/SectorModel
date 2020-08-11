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
        public Guid CoreModelId { get; set; } 
		public Model CoreModel { get; set; }  
        public Guid SPDRModelId { get; set;}  
		public Model SPDRModel { get; set; }       
        public string DBConnectionString { get; set; } 
        public string IEXCloudAPIKey { get; set; }		
		public List<Equity> AllEquities { get; set; }
		public List<Quote> LatestQuotes { get; set; }


    }
}