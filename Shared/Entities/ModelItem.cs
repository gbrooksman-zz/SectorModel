using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SectorModel.Shared.Entities
{
    public class ModelItem : BaseEntity
    {
        public ModelItem()
        {
            
        }

        public Guid ModelId { get; set; }

        public Guid EquityID { get; set; }      

        public decimal Percentage { get; set; }

        public decimal Cost { get; set; }

        public decimal Shares { get; set; }

        public int Version { get; set; }

        [NotMapped]
        public Equity Equity { get; set; }

        [NotMapped]
        public decimal LastPrice { get; set; }

        [NotMapped]
        public DateTime LastPriceDate { get; set; }

        [NotMapped]
        public decimal CurrentValue { get; set; }


    }
}
