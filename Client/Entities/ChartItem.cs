namespace SectorModel.Client.Entities
{  
  	public class ChartItem
    {
        public ChartItem() { }

        public decimal StartPrice { get; set; }

        public decimal HighPrice { get; set; }

        public decimal LowPrice { get; set; }

        public string ChartPath { get; set; }

        public string LegendText { get; set; }

        public int ColorIndex { get; set; }

    }
}