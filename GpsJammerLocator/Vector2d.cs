namespace GPSJammerLocator
{
    public struct Vector2d
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Vector2d(double x, double y)
        {
            X = x;
            Y = y;
        }

        // Operator overloads
        public static Vector2d operator -(Vector2d a, Vector2d b)
        {
            return new Vector2d(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2d operator +(Vector2d a, Vector2d b)
        {
            return new Vector2d(a.X + b.X, a.Y + b.Y);
        }

        // Distance method calculates the Euclidean distance between two vectors
        public static double Distance(Vector2d a, Vector2d b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        // Dot method calculates the dot product of two vectors
        public static double Dot(Vector2d a, Vector2d b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        // Length method calculates the magnitude (length) of the vector
        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y);
        }
    }
}
