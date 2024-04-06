using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotSpatial.Projections;

namespace GPSJammerLocator
{
    internal class DataSetParser
    {
        const float SenderHeightM = 0; // radio sender height from ground, meters
        private static float GetRadioHorizonRadius(float TxHeightM, float RxHeightM)
        {
            return 4120 * MathF.Sqrt(TxHeightM) + 4120 * MathF.Sqrt(RxHeightM);
        }
        public static float FeetToMeters(float feet)
        {
            return feet * 0.3048F;
        }

        public static List<Circle> Parse(string filepath)
        {
            // Call ParseDaily to get the circles organized by day
            var dailyCircles = ParseDaily(filepath);

            // Initialize the list to hold all circles
            List<Circle> allCircles = new List<Circle>();

            // Go through each day's circles in the order they were added to the dictionary
            // and add them to the allCircles list
            foreach (var dayCircles in dailyCircles.Values)
            {
                allCircles.AddRange(dayCircles);
            }

            return allCircles;
        }
        public static Dictionary<DateTime, List<Circle>> ParseDaily(string filePath)
        {
            var dailyCircles = new Dictionary<DateTime, List<Circle>>();

            // Check if the file exists
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The file doesn't exist. " + filePath);
            }

            using (StreamReader sr = new StreamReader(filePath))
            {
                // Skip the header line
                sr.ReadLine();

                while (sr.ReadLine() is string currentLine)
                {
                    // Split the line into columns based on comma
                    string[] fields = currentLine.Split(',');

                    // Extract timestamp, lat, lon, and alt_geom fields
                    DateTime timestamp = DateTime.ParseExact(fields[0], "yyyy-MM-dd HH:mm:ss", CultureInfo.CurrentCulture).Date; // Normalize to date
                    float lat = float.Parse(fields[2], CultureInfo.InvariantCulture);
                    float lon = float.Parse(fields[3], CultureInfo.InvariantCulture);
                    float altGeom = float.Parse(fields[7], CultureInfo.InvariantCulture);

                    // Convert latitude and longitude to metric coordinates (x, y)
                    (float x, float y) = CoordinateConverter.ConvertLatLonToMetric(lat, lon);

                    // Assume GetRadioHorizonRadius and SenderHeightM are defined elsewhere
                    var circle = new Circle(x, y, GetRadioHorizonRadius(SenderHeightM, FeetToMeters(altGeom)));

                    // Check if the date already exists in the dictionary
                    if (!dailyCircles.ContainsKey(timestamp))
                    {
                        dailyCircles[timestamp] = new List<Circle>();
                    }

                    // Add the circle to the list for the corresponding day
                    dailyCircles[timestamp].Add(circle);
                }
            }

            return dailyCircles;
        }
    }
}
