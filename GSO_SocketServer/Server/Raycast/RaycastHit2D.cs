using Collision.Shapes;
using System.Numerics;
using Vector2 = System.Numerics.Vector2;



public class RaycastHit2D
{
    public int Id;
    public Shape Collider;
    //public GameObject hitObj;
    public float distance;
    //public Vector2 normal;
    public Vector2? hitPoint;
}