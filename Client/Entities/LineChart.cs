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

        public string Symbol { get; set; }

        public string Title { get; set; }

        public string Path { get; set; }

    }
}
