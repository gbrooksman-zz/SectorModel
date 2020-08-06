using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SectorModel.Shared.Entities
{
    public class BaseEntity
    {
        public BaseEntity ()
        {
            
        }

        public Guid Id { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
    }
}
