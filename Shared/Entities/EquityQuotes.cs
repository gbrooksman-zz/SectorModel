using System.Collections.Generic;

namespace SectorModel.Shared.Entities
{
    public class EquityQuotes
    {
        public EquityQuotes()
        {

        }

        public Equity Equity { get; set; }

        public List<Quote> Quotes { get; set; }
    }
}
