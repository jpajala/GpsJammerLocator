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
        public float CenterX { get; }
        public float CenterY { get; }
        public int HeatCounter { get; set; }

        public HeatCell(int column, int row, float centerX, float centerY)
        {
            Column = column;
            Row = row;
            CenterX = centerX;
            CenterY = centerY;
            HeatCounter = 0;
        }
    }
}
