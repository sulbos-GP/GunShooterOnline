using System.Collections.Generic;
using Utils;

namespace Collision.Shapes;

public class Rectangle : Polygon
{
    public float Bottom;
    public float Left;
    public float Width;
    public float Height;

    //Todo base 고치기!!!!!!
    public Rectangle(float left, float bottom, float width, float height) : base(left+width/2,bottom+height/2,null)
    {
        Left = left;
        Bottom = bottom;
        Width = width;
        Height = height;

        Type = ShapeType.RECTANGLE;

        updateBounds();
    }

    public Rectangle(float x, float y, float width, float height, IList<Vector> vertices) : base(x, y, vertices)
    {
        Left = x - width / 2;
        Bottom = y - height / 2;
        Width = width;
        Height = height;

        Type = ShapeType.RECTANGLE;

        updateBounds();
    }




    /** Helper generate a rectangle at x,y with a given width/height and centered state.
            Centered by default. Returns a ready made `Polygon` collision `Shape` */
    public static Rectangle rectangle(float x, float y, float width, float height, bool centered = true) {

        var vertices = new List<Vector>();

        if (centered) {

            vertices.Add( new Vector( -width / 2, -height / 2) );
            vertices.Add( new Vector(  width / 2, -height / 2) );
            vertices.Add( new Vector(  width / 2,  height / 2) );
            vertices.Add( new Vector( -width / 2,  height / 2) );

        } else {

            vertices.Add( new Vector( 0, 0 ) );
            vertices.Add( new Vector( width, 0 ) );
            vertices.Add( new Vector( width, height) );
            vertices.Add( new Vector( 0, height) );

        }

        return new Rectangle(x,y, width, height, vertices);
        //return new Polygon(x,y,vertices);


    }
}