#define ALL_IN_ONE

using GpsJammerLocator;
using System.Collections.Concurrent;
using System.Globalization;


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
            ComputeAndOutput(filePath, data);
            Console.WriteLine("Done. Press enter.");
        }
#if ALL_IN_ONE

        private static void ComputeAndOutput(string filePath, List<Circle> circles)
#else
        private static void ComputeAndOutput(string filePath, Dictionary<DateTime, List<Circle>> dailydata)
#endif
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.Write($"lat,lon,heat");
#if ALL_IN_ONE
                    writer.WriteLine();
#else
                writer.WriteLine(",date");
                foreach (var day in dailydata.Keys)
                {
                    Console.WriteLine(day);
                    List<Circle> circles = dailydata[day];
#endif
                    Console.WriteLine($"Individual circles: {circles.Count}");

                    List<Vector2d> intersections;
                    ComputeIntersections(circles, out intersections);
                    HeatMap heat = ComputeHeatMap(intersections);
                    var heatCells = heat.GetHeatOrderedCells();
                    Console.WriteLine("Output File: " + filePath);
                    // Using StreamWriter to open the file for writing
                    foreach (var cell in heatCells)
                    {
                        (double lat, double lon) = CoordinateConverter.ConvertMetricToLatLon(cell.CenterX, cell.CenterY);
                        CultureInfo ci = CultureInfo.InvariantCulture;

                        // Writing formatted string to file
                        writer.Write($"{lat.ToString(ci)},{lon.ToString(ci)},{cell.HeatCounter}");
#if ALL_IN_ONE
                        writer.WriteLine();
#else
                        writer.WriteLine(","+day.ToString("ddMMyyyy"));
                    }
#endif
                }
            }
        }
        private static HeatMap ComputeHeatMap(List<Vector2d> intersections)
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

        private static void ComputeIntersections(List<Circle> circles, out List<Vector2d> intersections)
        {
            ConcurrentBag<Vector2d> tempIntersections = new ConcurrentBag<Vector2d>();
            int totalOperations = circles.Count * (circles.Count - 1) / 2;
            Console.WriteLine("Computing intersections");
            ProgressBar pb = new ProgressBar(totalOperations);
            int ctr = 0;

            if (circles.Count > 1)
            {
                Parallel.For(0, circles.Count, i =>
                {
                    for (int j = i + 1; j < circles.Count; j++)
                    {
                        var goodIntersections = circles[i].GetGoodIntersections(circles[j]);
                        foreach (var intersection in goodIntersections)
                        {
                            tempIntersections.Add(intersection);
                        }

                        int updatedCtr = Interlocked.Increment(ref ctr);
                        // Updating the progress bar for every increment might be too frequent and cause performance issues
                        // Consider updating the progress bar less frequently or outside the parallel loop
                        // Update the progress bar after completing the parallel operation
                        pb.Update(ctr);
                    }
                });

            }

            // If order matters or further processing is needed, handle it here
            intersections = new List<Vector2d>(tempIntersections);

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