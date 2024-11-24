using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Collision;
using Collision.Shapes;
using Server.Game;
using Vector2 = System.Numerics.Vector2;


namespace QuadTree
{
    public class QuadTreeManager 
    {
        //lock을 걸어야 할까? => logic안에서 돌아간다면 상관 x, 분리한다면 o
        
        private QuadTree<GameObject> _quadDynamic = new QuadTree<GameObject>(-100,-100,200,200);

        private GameObject[] _totalObjects
        {
            get
            {
                /*var total = new List<GameObject>((_players == null ? 0 : _players.Count)  + (_objects == null ? 0 : _objects.Count));
                if (_players != null)
                    total.AddRange(_players);
                if (_objects != null)
                    total.AddRange(_objects);
                return total;*/


                return ObjectManager.Instance.GetAllShapes();
            }
        }

        private List<GameObject> _players;
        private List<GameObject> _objects;
        


        public void Init(List<Player> p, List<GameObject> o)
        {
            _players = p.Cast<GameObject>().ToList();
            _objects = o;
        }

       /* private void Insert(GameObject go)
        {
            if (_totalObjects.Contains(go) == false)
            {
                _totalObjects.Add(go);
                _quadDynamic.Insert(go,GetBounds(go));
            }
        }*/
        
        Circle GetBounds(GameObject obj) 
        {
            return new Circle(obj.PosInfo.PosX,obj.PosInfo.PosY, obj.currentShape.GetapproximateRadius());
        }
        
       
       /* public bool RemoveObject(GameObject obj)
        {
            return _totalObjects.Remove(obj);
        }*/

        /// <summary>
        /// Gets the nearest GameObjects to the given GameObject.
        /// </summary>
        /// <param name="obj">GameObject which will be the origin of the check.</param>
        /// <returns>List of GameObjects. Null if GameObject is not present in the QuadTree.</returns>
        public List<GameObject> GetNearestObjects(GameObject obj)
        {
            return GetNearestObjects(GetBounds(obj));
        }
        public List<GameObject> GetNearestObjects(Circle circle)
        {
            var nearest = new List<GameObject>();

            foreach (var obj in _quadDynamic.GetNodesInside(circle))
            {
                nearest.Add(obj);
            }

            return nearest;
        }
        
        public void Update()
        {
            //var objectsCopy = new List<GameObject>(_totalObjects);
            UpdateQuadTree();
            ColUpdatebySAT();


          /*  foreach (var obj in objectsCopy)
            {
                UpdateCollision(obj);
            }*/
        }



        public void ColUpdatebySAT()
        {
   
            foreach (GameObject g1 in _totalObjects)
            {
                Shape s1 = g1.currentShape;
                ShapeCollision result;


                foreach (GameObject g2 in GetNearestObjects(g1))
                {
                    if (g1 == g2)
                        continue;
               

                    Shape s2 = g2.currentShape;

                    result = s1.test(s2);
                    if (result != null && (MathF.Abs(result.overlap) > 0))
                    {
                       /* if (g1.ObjectType == Google.Protobuf.Protocol.GameObjectType.Noneobject && g2.ObjectType == Google.Protobuf.Protocol.GameObjectType.Player)
                        {
                            Console.WriteLine("Scope가 Player 충돌");
                        }*/
                        //Debug.Log("충돌함");
                        g1.OnCollision(g2);
                    }
                }

            }


        }



        private void UpdateCollision(GameObject gameObject)
        {
            Shape mainCol = gameObject.currentShape;
            List<GameObject> hitList = new List<GameObject>();
            ShapeCollision temp = null;
            
            if (mainCol.Type == ShapeType.CIRCLE)
            {
                foreach (GameObject go in GetNearestObjects(gameObject))
                {
                    var otherCol = go.currentShape;
                    if (otherCol.Type == ShapeType.CIRCLE)
                    {
                        temp = Sat2D.testCircleVsCircle((Circle)mainCol, (Circle)otherCol);

                    }
                    else if (otherCol.Type == ShapeType.POLYGON || otherCol.Type == ShapeType.RECTANGLE)
                    {  
                        temp = Sat2D.testCircleVsPolygon((Circle)mainCol, (Polygon)otherCol);
                    }
                    
                    if (temp != null && (temp.overlap > 0 || temp.overlap < 0))
                        hitList.Add(go);
                }
            }
            else if (mainCol.Type == ShapeType.POLYGON || mainCol.Type == ShapeType.RECTANGLE)
            {
                foreach (GameObject go in GetNearestObjects(gameObject))
                {
                    var otherCol = go.currentShape;
                    if (otherCol.Type == ShapeType.CIRCLE)
                    {
                        temp = Sat2D.testCircleVsPolygon(polygon:(Polygon)mainCol,circle:(Circle)otherCol);
                    }
                    else if (otherCol.Type == ShapeType.POLYGON || otherCol.Type == ShapeType.RECTANGLE)
                    {
                        temp = Sat2D.testPolygonVsPolygon((Polygon)mainCol, (Polygon)otherCol);
                    }

                    if (temp != null && (temp.overlap > 0 || temp.overlap < 0))
                        hitList.Add(go);
                }

            }



            //gameObject.OnCollisionList(hitList.ToArray());

            foreach (var go in hitList)
            {
                gameObject.OnCollision(go);
            }


        }
        
        
        private void UpdateQuadTree()
        {
            _quadDynamic.Clear();
            foreach (var obj in _totalObjects)
            {
                //obj.transform.position += new Vector3(Random.(-3, 4), Random.Range(-3, 4), 0);
                _quadDynamic.Insert(obj,GetBounds(obj));
            }
        }
    }
}