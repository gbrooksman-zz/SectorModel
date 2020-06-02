using System;
namespace SectorModel.Shared.Entities
{
    public class Quote : BaseEntity
    {
        public Quote()
        {
            
        }

        public DateTime Date { get; set; }

        public Guid EquityId { get; set; }

        public decimal Price { get; set; }

        public int Volume { get; set; }

        public decimal RateOfChange { get; set; }
    }
}
