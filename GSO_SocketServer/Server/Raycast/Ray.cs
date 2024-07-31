using Collision.Shapes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using Vector2 = System.Numerics.Vector2;


public class Ray
{
    public int id;
    public Vector2 origin;
    public Vector2 direction;
    public float distance; //todo 설정하기


    public Ray(int id, Vector2 origin, Vector2 direction, float distance)
    {
        this.id = id;
        this.origin = origin;
        this.direction = Vector2.Normalize(direction);
        this.distance = distance;
    }


    public void SetDir(Vector2 direction)
    {
        this.direction = Vector2.Normalize(direction);
    }

    


    public RaycastHit2D Cast(Shape shape)
    {

        RaycastHit2D res = null;
        switch (shape.Type)
        {
            case ShapeType.SHAPE:

                break;

            case ShapeType.CIRCLE:
                res = Cast2Circle(shape);
                break;
            case ShapeType.RECTANGLE:
                res = Cast2Polygon(shape);

                break;
            case ShapeType.POLYGON:
                res = Cast2Polygon(shape);

                break;
            case ShapeType.ARCPOLY:
                res = Cast2CArcPoly(shape);
                break;
            default:
                break;
        }

        return res;
    }

    //if (shape.Type == ShapeType.POLYGON || shape.Type == ShapeType.RECTANGLE || shape.Type == ShapeType.ARCPOLY)



    public RaycastHit2D Cast2CArcPoly(Shape shape)
    {

        ArcPoly arc = (ArcPoly)shape;

        Vector2 s = new Vector2(origin.X - arc.position.x, origin.Y - arc.position.y);
        float b = Vector2.Dot(s, direction);
        float c = Vector2.Dot(s, s) - arc.radius * arc.radius;

        double h = b * b - c;
        if (h < 0.0f)
        {
            return null;
        }

        //intersection

        h = Math.Sqrt(h);
        double t = -b - h;
        //far pos : t = -b + h;

        Vector2 pos = origin + Math.Max((float)t, 0) * direction;
        //요기까지가 원 안에 잇으면

        

        Vector2 center =  new Vector2( pos.X - arc.position.x, pos.Y - arc.position.y );

        Vector2 left = new Vector2(arc.transformedVertices[1].x - arc.position.x, arc.transformedVertices[1].y - arc.position.y);
        Vector2 right = new Vector2(arc.transformedVertices[arc.transformedVertices.Count - 1].x - arc.position.x, arc.transformedVertices[arc.transformedVertices.Count - 1].y - arc.position.y);


        if(Vector2.Dot(left, center) > 0 && Vector2.Dot(center, right) > 0)
        {
            RaycastHit2D hit = new RaycastHit2D();

            hit.Collider = shape;
            hit.distance = (float)t;
            hit.hitPoint = (Vector2)pos;

            return hit;
        }
        else
        {
            Vector2? t1 = CastLine(arc.position, arc.transformedVertices[1]);

            Vector2? t2 = CastLine(arc.position, arc.transformedVertices[arc.transformedVertices.Count - 1]);

            if (t1 == null && t2 == null)
            {
                return null;
            }
            else if (t1 == null)
            {
                pos = (Vector2)t2;
            }
            else if (t2 == null)
            {
                pos = (Vector2)t1;
            }
            else
            {
                pos = Vector2.Distance(origin, t1.Value) > Vector2.Distance(origin, t2.Value) ? (Vector2)t2 : (Vector2)t1;
            }


            RaycastHit2D hit = new RaycastHit2D();

            hit.Collider = shape;
            hit.distance = (float)t;
            hit.hitPoint = (Vector2)pos;

            return hit;
        }

        
    }







    public RaycastHit2D Cast2Circle(Shape shape)
    {

        Circle circle = (Circle)shape;

        Vector2 s = new Vector2(origin.X - circle.position.x, origin.Y - circle.position.y);
        float b = Vector2.Dot(s, direction);
        float c = Vector2.Dot(s, s) -  circle.radius * circle.radius;

        double h = b * b - c;
        if (h < 0.0f)
        {
            return null;
        }

        //intersection

        h = Math.Sqrt(h);
        double t = -b - h;
        //far pos : t = -b + h;

        Vector2 pos = origin + Math.Max((float)t, 0) * direction;

        RaycastHit2D hit = new RaycastHit2D();

        hit.Collider = shape;
        hit.distance = (float)t;
        hit.hitPoint = (Vector2)pos;

        return hit;
    }




    public RaycastHit2D Cast2Polygon(Shape shape)
    {
        Polygon polygon = (Polygon)shape;
        RaycastHit2D hit = null;
        Vector2? closestPos = null;
        float minDistance = float.MaxValue;

        for (int i = 0; i < polygon.vertices.Count; i++)
        {
            Vector2 t1 = new Vector2(polygon.transformedVertices[i].x, polygon.transformedVertices[i].y);
            Vector2 t2 = new Vector2(polygon.transformedVertices[(i + 1) % polygon.vertices.Count].x, polygon.transformedVertices[(i + 1) % polygon.vertices.Count].y);
            Vector2? pos = CastLine(t1, t2);

            if (pos != null)
            {
                float distance = Vector2.Distance(origin, pos.Value);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPos = pos;
                }

            }
        }

        if (closestPos != null)
        {
            // 충돌 위치 처리
            hit = new RaycastHit2D();

            hit.Collider = shape;
            hit.distance = minDistance;
            hit.hitPoint = (Vector2)closestPos;
        }


        return hit;
    }




    public Vector2? CastLine(Vector2 start, Vector2 end)
    {
        float x1 = start.X;
        float y1 = start.Y;
        float x2 = end.X;
        float y2 = end.Y;

        float x3 = origin.X;
        float y3 = origin.Y;
        float x4 = origin.X + direction.X;
        float y4 = origin.Y + direction.Y;

        float den = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

        if (den == 0)
            return null;

        float t = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / den;
        float u = -((x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3)) / den;


        if (t > 0 && t < 1 && u > 0)
            return new Vector2(x1 + t * (x2 - x1), y1 + t * (y2 - y1));
        else
            return null;





    }
}
