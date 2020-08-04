using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SectorModel.Shared.Entities
{
    public class BaseEntity
    {
        public BaseEntity ()
        {
            
        }

		//[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
    }
}
