using System;

namespace SectorModel.Shared.Entities
{
    public class ModelComment : BaseEntity
    {
        public ModelComment()
        {

        }

        public Guid ModelId { get; set; }

        public Guid UserId { get; set; }

        public string Comment { get; set; }
    }
}
