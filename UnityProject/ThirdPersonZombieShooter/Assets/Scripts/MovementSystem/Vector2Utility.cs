using UnityEngine;

public static class Vector2Utility
{
    public static Vector2 CalculateLinePointClosestToOrigin(Vector2 lineOrigin, Vector2 lineDirection)
    {
        if (lineDirection == Vector2.zero)
            return lineOrigin;

        // Look at an edge case in which lineDirection points straight up or down
        if (lineDirection.x == 0)
        {
            return new Vector2(lineOrigin.x, 0);
        }

        // Transform the line into a function of the form m * x + b
        var m = lineDirection.y / lineDirection.x;

        var b = lineOrigin.y - m * lineOrigin.x;

        // Use the approach explained in this video: https://www.youtube.com/watch?v=-CsEPYeSBsg
        // d*d = x*x + y*y = x*x + (m * x + b)*(m * x + b) = x*x + m*m * x*x + 2 * m * x * b + b*b
        // => (d*d)' = 2 * x + 2 * x * m*m + 2 * m * b = x * (2 + 2 * m*m) + 2 * m * b
        // => The line is closest to the origin at the point (-2 * m * b) / (2 + 2 * m*m)
        var x = -(m * b) / (1 + m * m);

        return new Vector2(x, m * x + b);
    }


    public static void CalculatePointsOnLineWithDistanceToOrigin(Vector2 lineOrigin, Vector2 lineDirection, float distance, out Vector2 p1, out Vector2 p2)
    {
        // Eliminate an edge case in which lineDirection.x = 0.
        if (lineDirection.x == 0)
        {
            // The line is parallel to the y axis.

            // The distance of a point (lineOrigin.x, x) to the origin is sqrt(lineOrigin.x * lineOrigin.x + x * x)
            // Create the equation distance = sqrt(lineOrigin.x * lineOrigin.x + x * x) and use the quadratic formula to get solutions for x.
            // https://en.wikipedia.org/wiki/Quadratic_formula
            // a = 1
            // b = 0
            var c = lineOrigin.x * lineOrigin.x - distance * distance;

            var discriminantRoot = Mathf.Sqrt(-4 * c);

            var x1 = discriminantRoot / 2;
            var x2 = -discriminantRoot / 2;

            p1 = new Vector2(lineOrigin.x, x1);
            p2 = new Vector2(lineOrigin.x, x2);

        }
        else
        {
            // Transform the line into a linear function of the form m * x + k.
            var m = lineDirection.y / lineDirection.x;

            // m * origin.x + k = origin.y => k = origin.y - m * origin.x
            var k = lineOrigin.y - m * lineOrigin.x;

            // The distance of a point (x, m*x + k) to the origin is sqrt(x*x + (m*x + k) * (m*x + k)) = sqrt(x*x + m*m*x*x + 2*m*x*k + k*k) = sqrt(x*x*(1 + m*m) + x*2*m*k + k*k)
            // Create the equation distance = sqrt(x*x*(1 + m*m) + x*2*m*k + k*k) and use the quadratic formula to get solutions for x.
            // https://en.wikipedia.org/wiki/Quadratic_formula
            var a = 1 + m * m;
            var b = 2 * m * k;
            var c = k * k - distance * distance;

            var discriminantRoot = Mathf.Sqrt(b * b - 4 * a * c);

            var x1 = (-b + discriminantRoot) / (2 * a);
            var x2 = (-b - discriminantRoot) / (2 * a);

            p1 = new Vector2(x1, m * x1 + k);
            p2 = new Vector2(x2, m * x2 + k);
        }
    }
}
