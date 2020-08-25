using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SectorModel.Shared.Entities
{

    public class Model : BaseEntity
    {
        public Model()
        {
            
        }
		
        public Guid UserId { get; set; }
	
		[Required]	
    	[StringLength(50, ErrorMessage = "The name is too long (50 character limit).")]	
        public string Name { get; set; }

		[Required]		
        public bool IsActive { get; set; }

		[Required]		
        public DateTime StartDate { get; set; }

        public DateTime StopDate { get; set; }

		[Required]
		[Range(100,10000000, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public decimal StartValue { get; set; }

		[NotMapped]
        public decimal LatestValue { get; set; }	

		[NotMapped]
		public List<ModelItem> ItemList { get; set; }
    }
}