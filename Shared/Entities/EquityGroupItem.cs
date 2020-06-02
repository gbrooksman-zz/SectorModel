using System;


namespace SectorModel.Shared.Entities
{
    public class EquityGroupItem : BaseEntity
    {
        public EquityGroupItem()
        {
            
        }

        public Guid Id { get; set; }

        public Guid GroupId { get; set; }

        public Guid EquityId { get; set; }
    }
}