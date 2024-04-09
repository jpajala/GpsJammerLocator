using System.Numerics;

namespace GPSJammerLocator
{
    public class Circle
    {
        public Vector2d Center { get; private set; }
        public double R { get; private set; }

        public Circle(double x, double y, double r)
        {
            Center = new Vector2d(x, y);
            R = r;
        }

        public List<Vector2d> GetGoodIntersections(Circle other)
        {
            //List<Vector2d> intersections = new List<Vector2d>();
            List<Vector2d> ToRet = new List<Vector2d>();

            List<Vector2d> intersections = GetIntersections(other);

            foreach (var point in intersections)
            {
                Vector2d vectorFromThis = point - this.Center;
                Vector2d vectorFromOther = point - other.Center;

                double angle = Vector2Angle(vectorFromThis, vectorFromOther);
                const double minAcceptedAngle = 50;
                if (angle >= minAcceptedAngle && angle <= 180-minAcceptedAngle)
                {
                    ToRet.Add(point);
                }
            }

            return ToRet;
        }

        private List<Vector2d> GetIntersections(Circle otherCircle)
        {
            // Intersection calculation logic...
            List<Vector2d> intersections = new List<Vector2d>();

            // Calculate distance between the centers
            double dx = otherCircle.Center.X - this.Center.X;
            double dy = otherCircle.Center.Y - this.Center.Y;
            double distance = Vector2d.Distance(this.Center, otherCircle.Center);

            // Check if circles are too far apart or one circle is completely inside the other
            if (distance > this.R + otherCircle.R || distance < Math.Abs(this.R - otherCircle.R) || distance == 0)
            {
                return intersections; // No intersections
            }

            // Find a and h
            double a = (this.R * this.R - otherCircle.R * otherCircle.R + distance * distance) / (2 * distance);
            double h = (double)Math.Sqrt(this.R * this.R - a * a);

            // Find P2
            Vector2d P0 = new Vector2d(this.Center.X + a * (dx) / distance,
                                     this.Center.Y + a * (dy) / distance);

            // Find intersection points, P3 and P4
            double x3 = P0.X + h * (dy) / distance;
            double y3 = P0.Y - h * (dx) / distance;
            double x4 = P0.X - h * (dy) / distance;
            double y4 = P0.Y + h * (dx) / distance;

            Vector2d intersection1 = new Vector2d(x3, y3);
            Vector2d intersection2 = new Vector2d(x4, y4);

            intersections.Add(intersection1);
            intersections.Add(intersection2);
            return intersections;
        }

        private double Vector2Angle(Vector2d a, Vector2d b)
        {
            double dotProduct = Vector2d.Dot(a, b);
            double magnitudeA = a.Length();
            double magnitudeB = b.Length();
            double angleRadians = (double)Math.Acos(dotProduct / (magnitudeA * magnitudeB));
            return angleRadians * (180f / (double)Math.PI); // Convert to degrees
        }
    }
}