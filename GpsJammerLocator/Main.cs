//#define ALL_IN_ONE

using GpsJammerLocator;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Reflection;


namespace GPSJammerLocator
{
    class Program
    {        
        static void Main(string[] args)
        {
            string dataset = @"C:\Temp\paste-08b4351a79e12a8c.csv";
            string filePath = @"C:\Temp\output.csv";

            // Set the CultureInfo to InvariantCulture for the current thread
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            Console.WriteLine("parsing...");
#if ALL_IN_ONE
            var data = DataSetParser.Parse(dataset);
#else
            var data = DataSetParser.ParseDaily(dataset);
#endif
#if ALL_IN_ONE                
            ComputeAndOutput(filePath, data);
#else
            int numDates = 0;
            foreach (var key in data.Keys)
            {
                Console.WriteLine(key);
                ComputeAndOutput(AddDateToFilename(filePath, key), data[key]);
                numDates++;
            }
            Console.WriteLine($"\n\nDates: {numDates}");
#endif
            Console.WriteLine("Done. Press enter.");
        }

        private static void ComputeAndOutput(string filePath, List<Circle> circles)
        {
            Console.WriteLine($"Individual circles: {circles.Count}");

            List<Vector2> intersections;
            ComputeIntersections(circles, out intersections);
            HeatMap heat = ComputeHeatMap(intersections);
            var heatCells = heat.GetHeatOrderedCells();
            Console.WriteLine("Output File: " + filePath);
            // Using StreamWriter to open the file for writing
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"lat,lon,heat");
                foreach (var cell in heatCells)
                {
                    (float lat, float lon) = CoordinateConverter.ConvertMetricToLatLon(cell.CenterX, cell.CenterY);
                    CultureInfo ci = CultureInfo.InvariantCulture;

                    // Writing formatted string to file
                    writer.WriteLine($"{lat.ToString(ci)},{lon.ToString(ci)},{cell.HeatCounter}");
                }
            }
        }

        private static HeatMap ComputeHeatMap(List<Vector2> intersections)
        {
            HeatMap heat = new HeatMap(10000);
            ProgressBar progressBar = new ProgressBar(intersections.Count);
            for (int i = 0; i < intersections.Count; i++)
            {
                heat.AddPoint(intersections[i].X, intersections[i].Y);
                progressBar.Update(i+1);
            }

            return heat;
        }

        private static void ComputeIntersections(List<Circle> circles, out List<Vector2> intersections)
        {
            intersections = new List<Vector2>();

            // Choose one circle and find its good intersections with the rest
            Console.WriteLine("Computing intersections");
            ProgressBar pb = new ProgressBar(circles.Count * (circles.Count - 1) / 2);
            int ctr = 0;
            if (circles.Count > 1)
            {
                for (int i = 0; i < circles.Count; i++)
                {
                    for (int j = i + 1; j < circles.Count; j++)
                    {
                        var goodIntersections = circles[i].GetGoodIntersections(circles[j]);
                        intersections.AddRange(goodIntersections);
                        ctr++;
                        pb.Update(ctr);
                    }
                }
            }
            Console.WriteLine($"Num intersections: {intersections.Count}");
        }
        public static string AddDateToFilename(string originalFilename, DateTime date)
        {
            // Split the original filename into prefix and postfix
            var parts = originalFilename.Split(new[] { '.' }, 2); // Splits into two parts at the first period

            // Check if the original filename had a postfix
            if (parts.Length < 2)
            {
                throw new ArgumentException("Filename must have a postfix (e.g., 'prefix.postfix').", nameof(originalFilename));
            }
            // Get the current date in yyyyMMdd format
            string datestr = date.ToString("yyyyMMdd");

            // Reconstruct the filename with the current date inserted
            string newFilename = $"{parts[0]}.{datestr}.{parts[1]}";

            return newFilename;
        }
    }
}