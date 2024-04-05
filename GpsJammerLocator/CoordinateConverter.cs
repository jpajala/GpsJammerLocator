using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    using System;
    using ProjNet.CoordinateSystems;
    using ProjNet.CoordinateSystems.Transformations;

namespace GPSJammerLocator
{

    static public class CoordinateConverter
    {

        static CoordinateTransformationFactory coordinateTransformationFactory = new CoordinateTransformationFactory();

        static private ICoordinateTransformation _toMetricTransform = coordinateTransformationFactory.CreateFromCoordinateSystems(GeographicCoordinateSystem.WGS84, ProjectedCoordinateSystem.WGS84_UTM(33, true));
        static private ICoordinateTransformation _toLatLonTransform = coordinateTransformationFactory.CreateFromCoordinateSystems(ProjectedCoordinateSystem.WGS84_UTM(33, true), GeographicCoordinateSystem.WGS84);


        static public (float X, float Y) ConvertLatLonToMetric(double latitude, double longitude)
        {
            var transformed = _toMetricTransform.MathTransform.Transform(new double[] { longitude, latitude });
            return ((float)transformed[0], (float)transformed[1]);
        }

        static public (float Latitude, float Longitude) ConvertMetricToLatLon(float x, float y)
        {
            var transformed = _toLatLonTransform.MathTransform.Transform(new double[] { x, y });
            return ((float)transformed[1], (float)transformed[0]);
        }
    }
}
