namespace SectorModel.Client.Entities
{
    public class PieSlice
    {

        public PieSlice()
        {

        }

        decimal percent;

        public decimal Percent { get { return percent; } set { percent = (value / 100); } }
        public string Color { get; set; }
        public string Name { get; set; }
        public double StartingX { get; set; }
        public double StartingY { get; set; }
        public double EndingX { get; set; }
        public double EndingY { get; set; }

    }
}