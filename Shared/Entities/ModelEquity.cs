using System;

namespace SectorModel.Shared.Entities
{
    public class ModelEquity : BaseEntity
    {
        public ModelEquity()
        {
            
        }

        public Guid Id { get; set; }

        public Guid ModelId { get; set; }

        public Guid EquityID { get; set; }      

        public decimal Percent { get; set; }

        public decimal Cost { get; set; }

        public decimal Shares { get; set; }

        public int Version { get; set; }

        public Equity Equity { get; set; }

        public decimal LastPrice { get; set; }

        public DateTime LastPriceDate { get; set; }

        public decimal CurrentValue { get; set; }


    }
}
