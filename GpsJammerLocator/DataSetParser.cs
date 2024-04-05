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
        public static List<Circle> Parse()
        {
            string filePath = @"C:\Temp\Dataset.txt";
            List<Circle> result = new List<Circle>();

            // Check if the file exists
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The file doesn't exist. " + filePath);
            }

            using (StreamReader sr = new StreamReader(filePath))
            {
                string currentLine;
                // Skip the header line
                sr.ReadLine();

                while ((currentLine = sr.ReadLine()) != null)
                {
                    // Split the line into columns based on comma
                    string[] fields = currentLine.Split(',');

                    // Extract lat, lon, and alt_geom fields
                    string lat = fields[2];
                    string lon = fields[3];
                    string altGeom = fields[7];

                    (float x, float y) = CoordinateConverter.ConvertLatLonToMetric(float.Parse(lat, CultureInfo.InvariantCulture), float.Parse(lon, CultureInfo.InvariantCulture));

                    result.Add(new Circle(x, y, GetRadioHorizonRadius(SenderHeightM, FeetToMeters(float.Parse(altGeom, CultureInfo.InvariantCulture)))));

                    // Output or process the extracted values
                    //Console.WriteLine($"Lat: {lat}, Lon: {lon}, Alt_Geom: {altGeom}, radius: {result.Last().R}");
                }
            }
            return result;
        }
    }
}
