using System;
using System.Collections.Generic;

namespace GPSJammerLocator
{

    using System;
    using System.Collections.Generic;
    using System.Linq;

    class HeatMap
    {
        private Dictionary<(int, int), HeatCell> cells = new Dictionary<(int, int), HeatCell>();

        private double CellWidth;

        public HeatMap(double cellwidth)
        {
            this.CellWidth = cellwidth;
        }

        /// <summary>
        /// Increase existing heat cell value, or if not existing add new heat cell
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void AddPoint(double x, double y)
        {
            (int column, int row) = GetCellColRow(x, y);

            (int, int) key = (column, row);

            if (!cells.TryGetValue(key, out HeatCell cell))
            {
                // Calculate the center of the cell for storage and possible rendering
                double centerX;
                double centerY;
                (centerX, centerY) = CellCenterByColRow(column, row);
                
                // This is just debugging 
                (int column2, int row2) = GetCellColRow((double)centerX, (double)centerY);
                if(row2 != row || column2 != column)
                {
                    // we should never come here
                    Console.WriteLine("Wrong center!");
                }


                cell = new HeatCell(column, row, (double)centerX, (double)centerY);
                cells[key] = cell;
                //Console.WriteLine($"NEW : {cell.Row} {cell.Column} allocated heatmap cells: {cells.Count + 1}");
            }
            else
            {
            //    Console.WriteLine($"Found: {cell.Row} {cell.Column}  heat: {cell.HeatCounter}");
            }

            cell.HeatCounter += 1;
        }

        private (int, int) GetCellColRow(double x, double y)
        {
            // normalize grid cells
            int slotX = (int)(x / CellWidth);
            int slotY = (int)(y / CellWidth);

            return (slotX, slotY);
        }
        private (double, double) CellCenterByColRow(int slotX, int slotY)
        {
            // Calculate the x coordinate
            double x = ((double)slotX) * CellWidth;
            double y = ((double)slotY) * CellWidth;

            return (x, y);
        }

        public List<HeatCell> GetHeatOrderedCells()
        {
            List<HeatCell> orderedCells = cells.Values.OrderByDescending(cell => cell.HeatCounter).ToList();

            return orderedCells;
        }
    }
}
