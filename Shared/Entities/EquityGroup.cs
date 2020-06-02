using System;


namespace SectorModel.Shared.Entities
{

    public class EquityGroup : BaseEntity
    {
        public EquityGroup()
        {
            
        }


        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }
    }
}