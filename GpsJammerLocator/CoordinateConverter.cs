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


        static public (double X, double Y) ConvertLatLonToMetric(double latitude, double longitude)
        {
            var transformed = _toMetricTransform.MathTransform.Transform(new double[] { longitude, latitude });
            return ((double)transformed[0], (double)transformed[1]);
        }

        static public (double Latitude, double Longitude) ConvertMetricToLatLon(double x, double y)
        {
            var transformed = _toLatLonTransform.MathTransform.Transform(new double[] { x, y });
            return ((double)transformed[1], (double)transformed[0]);
        }
    }
}
