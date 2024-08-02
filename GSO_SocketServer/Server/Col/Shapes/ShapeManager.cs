
using System;
using System.Collections.Generic;
using Collision.Shapes;
using Utils;

public class ShapeManager
{
   
    private static Polygon create(float x, float y, int sides, float radius)
    {
        if (sides < 3)
        {
            throw new ArgumentException("A polygon must have a least 3 sides.");
        }

        float rotation = (float)(System.Math.PI * 2) / sides;
        float angle;
        Vector vector;
        IList<Vector> vertices = new List<Vector>();

        for (var i = 0; i < sides; i++)
        {
            angle = (float)((i * rotation) + ((System.Math.PI - rotation) * 0.5));
            vector = new Vector();
            vector.x = (float)System.Math.Cos(angle) * radius;
            vector.y = (float)System.Math.Sin(angle) * radius;
            vertices.Add(vector);
        }

        return new Polygon(x, y, vertices);
    }


    public static Polygon CreateSquare(float left, float top, float width)
    {
        return new Rectangle(left, top, width, width);
    }
    public static Polygon CreateCenterSquare(float x, float y, float width)
    {
        return Rectangle.rectangle(x, y, width, width, true);
    }

 
    public static Polygon CreateTriangle(float x, float y, float radius)
    {
        return create(x, y, 3, radius);
    }

    public static Circle CreateCircle(float x, float y, float r)
    {
        return new Circle(x, y, r);
    }

    public static Polygon CreateArcPoly(float x, float y, float r, float angle)
    {
        return new ArcPoly(x, y, r, angle);
    }
}
