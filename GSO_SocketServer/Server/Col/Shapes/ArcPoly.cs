using Collision.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Define;
using System.Xml.Linq;
using Utils;

namespace Collision.Shapes
{
    public class ArcPoly : Polygon
    {
        private float _radius;
        public float radius { get { return _radius; } set { _radius = value; name = "circle " + radius; } }

        public float angle;
        //public float startAngle = 0; //시작 엥글
        //public float endAngle = math.PI; // 마지막 앵글


        public bool isEmpty =>
            //bounds.Width == 0 || bounds.Height == 0
            radius <= 0;

        public float transformedRadius => radius * scaleX;


        public ArcPoly(float x, float y, float radius, float angle) : base(x, y, null)
        {
            this.angle = angle;
            const int polygon = 19; // 얼마나 정확한 원인지 기본은 20 클라와 동기화 해야함
            this.radius = radius;
            //bounds = radius;
            Type = ShapeType.ARCPOLY;
            name = "ArcPoly";

            //vertices = new Collection<Vector>();

            vertices = new Vector[polygon + 2];

            vertices[0] = new Vector(0, 0);
            for (int i = 1; i <= polygon + 1; i++)
            {
                float _angle = (-(i - 1) * (angle) / polygon) + rotation;

                vertices[i] = (new Vector
                    (MathF.Cos(_angle) * radius,
                     MathF.Sin(_angle) * radius));
            }

            //bounds = radius;
        }

        // public override ShapeCollision test (Shape shape)
        // {
        //     return shape.testCircle(this, true);
        // }
        //
        // public override ShapeCollision testCircle (Circle circle, bool flip = false)
        // {
        //     return Sat2D.testCircleVsCircle(this, circle, flip);
        // }
        //
        // public override ShapeCollision testPolygon (Polygon polygon, bool flip = false)
        // {
        //     return Sat2D.testCircleVsPolygon( this, polygon, flip );
        // }
        //
        // // For insertion
        // public bool ContainedBy(Rectangle rect)
        // {
        //     return rect.Left <= Position.X - radius &&
        //            rect.Top <= Position.Y - radius &&
        //            rect.Left + rect.Width >= Position.X + radius &&
        //            rect.Top + rect.Height >= Position.Y + radius;
        // }

        // // The rest is for query 
        // public bool Contains(Rectangle rect)
        // {
        //     // The distance to the furthest corner is less than the radius 
        //     return new Vector2(
        //         Math.Max(Math.Abs(Position.X - rect.Left), Math.Abs(Position.X - (rect.Left + rect.Width))),
        //         Math.Max(Math.Abs(Position.Y - rect.Top), Math.Abs(Position.Y - (rect.Top + rect.Height)))
        //     ).LengthSquared() < radius * radius;
        // }
        //
        // // https://stackoverflow.com/questions/401847/circle-rectangle-collision-detection-intersection/1879223#1879223
        // public bool IntersectsWith(Rectangle rect)
        // {
        //     return Vector2.DistanceSquared(Position, new Vector2(
        //         Math.Clamp(Position.X, rect.Left, rect.Left + rect.Width),
        //         Math.Clamp(Position.Y, rect.Top, rect.Top + rect.Height)
        //     )) < radius * radius;
        // }
        //
        // public bool IntersectsWith(Circle circle)
        // {
        //     return Vector2.DistanceSquared(Position, circle.Position) <
        //            (radius + circle.radius) * (radius + circle.radius);
        // }

    }
}
