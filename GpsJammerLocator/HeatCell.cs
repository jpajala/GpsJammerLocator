using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPSJammerLocator
{
    class HeatCell
    {
        public int Column { get; }
        public int Row { get; }
        public double CenterX { get; }
        public double CenterY { get; }
        public int HeatCounter { get; set; }

        public HeatCell(int column, int row, double centerX, double centerY)
        {
            Column = column;
            Row = row;
            CenterX = centerX;
            CenterY = centerY;
            HeatCounter = 0;
        }
    }
}
