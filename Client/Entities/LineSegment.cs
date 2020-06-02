namespace SectorModel.Client.Entities
{
    public class LineSegment
    {
        public LineSegment()
        {

        }

        public string Color { get; set; }
        public string Name { get; set; }
        public double StartingX { get; set; }
        public double StartingY { get; set; }
        public double EndingX { get; set; }
        public double EndingY { get; set; }

    }
}