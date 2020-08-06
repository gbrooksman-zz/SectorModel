using System;
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

        public decimal StartValue { get; set; }

        public decimal StopValue { get; set; }

	
        public int Version { get; set; }

	
        public bool IsPrivate { get; set; }

        public Guid ModelVersionId { get; set; }
    }
}