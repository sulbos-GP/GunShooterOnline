using System;
using System.Collections.Generic;
using Utils;

namespace Collision.Shapes
{
    public class Polygon : Shape
    {
        /** The vertices of this shape */
        public IList<Vector> vertices {
            get => _vertices;

            set {
                _vertices = value;
                
                if(vertices != null)
                {
                    name = "polygon(sides:" + _vertices.Count + ")";
                }
            }
        }
        private IList<Vector> _vertices { get; set; }
        
        
        public IList<Vector> transformedVertices {
            get {

                if (!_transformed) {
                    _transformedVertices = new List<Vector>();
                    _transformed = true;

                    var _count = vertices.Count;

                    for (var i = 0; i < _count; i++) {
                        _transformedVertices.Add( vertices[i].clone().transform( _transformMatrix ) );
                    }
                }

                return _transformedVertices;
            }
        }
        

        private IList<Vector> _transformedVertices;
        
        
        
        public Polygon (float x, float y, IList<Vector> vertices) : base(x, y)
        {
            _transformedVertices = new List<Vector>();
            Type = ShapeType.POLYGON;
            this.vertices = vertices;
            updateBounds();
        }

        public override ShapeCollision test (Shape shape)
        {
            return shape.testPolygon(this, true);
        }

        public override ShapeCollision testCircle (Circle circle, bool flip = false)
        {
            return Sat2D.testCircleVsPolygon(circle, this, !flip);
        }

        public override ShapeCollision testPolygon (Polygon polygon, bool flip = false)
        {
            return Sat2D.testPolygonVsPolygon(this, polygon, flip);
        }

        /** Helper to create an Ngon at x,y with given number of sides, and radius.
            A default radius of 100 if unspecified. Returns a ready made `Polygon` collision `Shape` */
        private static Polygon create(float x, float y, int sides, float radius) {
            if (sides < 3) {
                throw new ArgumentException("A polygon must have a least 3 sides.");
            }

            float rotation = (float)(System.Math.PI * 2) / sides;
            float angle;
            Vector vector;
            IList<Vector> vertices = new List<Vector>();

            for (var i = 0; i < sides; i++) {
                angle = (float)((i * rotation) + ((System.Math.PI - rotation) * 0.5));
                vector = new Vector();
                vector.x = (float)System.Math.Cos(angle) * radius;
                vector.y = (float)System.Math.Sin(angle) * radius;
                vertices.Add(vector);
            }

            return new Polygon(x, y, vertices);
        }
        
        

        /** Helper generate a square at x,y with a given width/height with given centered state.
            Centered by default. Returns a ready made `Polygon` collision `Shape` */
        public static Polygon square(float 
            x, float y, float width) {
            return new Rectangle(x, y, width, width);
        }

        /** Helper generate a triangle at x,y with a given radius.
            Returns a ready made `Polygon` collision `Shape` */
        public static Polygon triangle(float x, float y, float radius) {
            return create(x, y, 3, radius);
        }


        protected void updateBounds()
        {
            if (_vertices is not null && _vertices.Count > 0)
            {
                //QuadTree�� ����
                float max = 0f;
                foreach (var vertex in _vertices)
                {
                    float t = vertex.lengthsq;
                    if (t > max) max = t;
                }
                //bounds = MathF.Sqrt(max);
            }
        }



    }
    
    
}

