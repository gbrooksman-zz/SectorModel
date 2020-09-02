using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SectorModel.Client.Entities
{
    public class Point
    {
        public Point()
        {

        }

        public decimal X { get; set; }

        public decimal Y { get; set; }

    }


    public class Line
    {
        public Line()
        {

        }

        public List<Point> Points { get; set; }

        public int ColorIndex { get; set; }  

        public Guid EquityId { get; set; }

        public string Title { get; set; }

        public string Path { get; set; }

        public string LengendPath { get; set; }


    }

    public class LineSet
    {
        public LineSet()
        {

        }

        public List<Line> Lines { get; set; }

        public string Title { get; set; }

        public decimal StartX { get; set; }

        public decimal StartY { get; set; }

    }
}
