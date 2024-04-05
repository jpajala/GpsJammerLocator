using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;

namespace GPSJammerLocator
{
    class Program
    {        
        static void Main(string[] args)
        {
            var circles = DataSetParser.Parse();

//            TestCoordConv();

            Console.WriteLine($"Individual circles: {circles.Count}");

            var intersections = new List<Vector2>();

            // Choose one circle and find its good intersections with the rest
            if (circles.Count > 1)
            {
                for (int i = 0; i < circles.Count; i++)
                {
                    for (int j = i + 1; j < circles.Count; j++)
                    {
                        var goodIntersections = circles[i].GetGoodIntersections(circles[j]);
                        intersections.AddRange(goodIntersections);
                    }
                }
            }

            Console.WriteLine($"Num intersections: {intersections.Count}");

            HeatMap heat = new HeatMap(10000);

            foreach (var vector in intersections)
            {
                heat.AddPoint(vector.X, vector.Y);
            }

            var heatCells = heat.GetHeatOrderedCells();
            Console.WriteLine($"lat, lon, heat");
            foreach (var cell in heatCells)
            {
                (float lat, float lon) = CoordinateConverter.ConvertMetricToLatLon(cell.CenterX, cell.CenterY);
                CultureInfo ci = CultureInfo.InvariantCulture;
                Console.WriteLine($"{lat.ToString(ci)},{lon.ToString(ci)},{cell.HeatCounter}");
            }

            Console.ReadLine();
        }
    }
}