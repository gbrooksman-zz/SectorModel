using SectorModel.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SectorModel.Client.Entities
{
    public class CoreEquitySelector
    {
        public CoreEquitySelector()
        {

        }

        public DateTime StartDate { get; set; }

        public DateTime StoptDate { get; set; }

        public List<Equity> EquityList { get; set; }

    }
}
