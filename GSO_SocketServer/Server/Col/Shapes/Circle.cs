using System;
using System.Numerics;

namespace Collision.Shapes
{
    public class Circle : Shape
    {
        private float _radius;
        public float radius { get { return _radius; } set { _radius = value; name = "circle " + radius; } }
        
        
        public bool isEmpty =>
            //bounds.Width == 0 || bounds.Height == 0
            radius <= 0;
        
        public float transformedRadius => radius * scaleX;

        
        public Circle (float x, float y, float radius) : base(x, y)
        {
            //bounds = radius;
            this.radius = radius;
            Type = ShapeType.CIRCLE;
            name = "Circle";
        }

        public override ShapeCollision test (Shape shape)
        {
            return shape.testCircle(this, true);
        }

        public override ShapeCollision testCircle (Circle circle, bool flip = false)
        {
            return Sat2D.testCircleVsCircle(this, circle, flip);
        }

        public override ShapeCollision testPolygon (Polygon polygon, bool flip = false)
        {
            return Sat2D.testCircleVsPolygon( this, polygon, flip );
        }

        // For insertion
        public bool ContainedBy(Rectangle rect)
        {
            return rect.Left <= position.x - radius &&
                   rect.Bottom <= position.y - radius &&
                   rect.Left + rect.Width >= position.x+ radius &&
                   rect.Bottom + rect.Height >= position.y + radius;
        }

        // The rest is for query 
        public bool Contains(Rectangle rect)
        {
            // The distance to the furthest corner is less than the radius 
            return new Vector2(
                Math.Max(Math.Abs(position.x - rect.Left), Math.Abs(position.x - (rect.Left + rect.Width))),
                Math.Max(Math.Abs(position.y - rect.Bottom), Math.Abs(position.y - (rect.Bottom + rect.Height)))
            ).LengthSquared() < radius * radius;
        }

        // https://stackoverflow.com/questions/401847/circle-rectangle-collision-detection-intersection/1879223#1879223
        public bool IntersectsWith(Rectangle rect)
        {
            return Vector2.DistanceSquared(position, new Vector2(
                Math.Clamp(position.x, rect.Left, rect.Left + rect.Width),
                Math.Clamp(position.y, rect.Bottom, rect.Bottom + rect.Height)
            )) < radius * radius;
        }

        public bool IntersectsWith(Circle circle)
        {
            return Vector2.DistanceSquared(position, circle.position) <
                   (radius + circle.radius) * (radius + circle.radius);
        }
    }
}