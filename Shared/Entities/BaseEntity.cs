using System;

namespace SectorModel.Shared.Entities
{
    public class BaseEntity
    {
        public BaseEntity ()
        {
            
        }

        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
