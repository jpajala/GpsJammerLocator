using System.Numerics;

namespace GPSJammerLocator
{
    public class Circle
    {
        public Vector2 Center { get; private set; }
        public float R { get; private set; }

        public Circle(float x, float y, float r)
        {
            Center = new Vector2(x, y);
            R = r;
        }

        public List<Vector2> GetGoodIntersections(Circle other)
        {
            //List<Vector2> intersections = new List<Vector2>();
            List<Vector2> ToRet = new List<Vector2>();

            List<Vector2> intersections = GetIntersections(other);

            foreach (var point in intersections)
            {
                Vector2 vectorFromThis = point - this.Center;
                Vector2 vectorFromOther = point - other.Center;

                float angle = Vector2Angle(vectorFromThis, vectorFromOther);

                if (angle >= 25 && angle <= 90)
                {
                    ToRet.Add(point);
                }
            }

            return ToRet;
        }

        private List<Vector2> GetIntersections(Circle otherCircle)
        {
            // Intersection calculation logic...
            List<Vector2> intersections = new List<Vector2>();

            // Calculate distance between the centers
            float dx = otherCircle.Center.X - this.Center.X;
            float dy = otherCircle.Center.Y - this.Center.Y;
            float distance = Vector2.Distance(this.Center, otherCircle.Center);

            // Check if circles are too far apart or one circle is completely inside the other
            if (distance > this.R + otherCircle.R || distance < Math.Abs(this.R - otherCircle.R) || distance == 0)
            {
                return intersections; // No intersections
            }

            // Find a and h
            float a = (this.R * this.R - otherCircle.R * otherCircle.R + distance * distance) / (2 * distance);
            float h = (float)Math.Sqrt(this.R * this.R - a * a);

            // Find P2
            Vector2 P0 = new Vector2(this.Center.X + a * (dx) / distance,
                                     this.Center.Y + a * (dy) / distance);

            // Find intersection points, P3 and P4
            float x3 = P0.X + h * (dy) / distance;
            float y3 = P0.Y - h * (dx) / distance;
            float x4 = P0.X - h * (dy) / distance;
            float y4 = P0.Y + h * (dx) / distance;

            Vector2 intersection1 = new Vector2(x3, y3);
            Vector2 intersection2 = new Vector2(x4, y4);

            intersections.Add(intersection1);
            intersections.Add(intersection2);
            return intersections;
        }

        private float Vector2Angle(Vector2 a, Vector2 b)
        {
            float dotProduct = Vector2.Dot(a, b);
            float magnitudeA = a.Length();
            float magnitudeB = b.Length();
            float angleRadians = (float)Math.Acos(dotProduct / (magnitudeA * magnitudeB));
            return angleRadians * (180f / (float)Math.PI); // Convert to degrees
        }
    }
}