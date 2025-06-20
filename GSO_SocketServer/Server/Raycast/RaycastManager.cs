using Collision.Shapes;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vector2 = System.Numerics.Vector2;
using Server.Game;
using ObjectManager = Server.Game.ObjectManager;
using System;
using static Humanizer.On;

namespace Server
{
    public class RaycastManager
    {
        //public static RaycastManager Instance = new RaycastManager();
        static DDA _dda = new DDA(); //안에 맵 데이터 있음
        private static int _index = 0;
        Map map;

        public void Init(Map m)
        {
            //Physics2D.Raycast()
            map = m;
            _dda.Init(m);
        }

        private void OnEnable()
        {
            _index = 0;
        }


        //RaycastHit2D hit = RaycastManager.Raycast(new Vector2(transform.position.x, transform.position.y), dir, 100);
        public static RaycastHit2D Raycast(Vector2 StartPos, Vector2 Dir, float Length,List<GameObject> myObject)
        {

            #region Packet

            #endregion

            _index++;

            //Debug.Log(_index);

            Ray raycast = new Ray(_index, StartPos, Dir, Length);
            Shape[] shapes = ObjectManager.Instance.GetShapeValue();


            RaycastHit2D rayRes = null;

            foreach (Shape shape in shapes)
            {
                RaycastHit2D temp = raycast.Cast(shape);

                if (temp == null || myObject.Contains(temp.Collider.Parent) )
                {
                    continue;

                }
                else
                {
                    //Console.WriteLine($"{shape.Parent.info.Name} is hit with {temp.distance} distance");
                }


                if (rayRes != null)
                {
                    if (rayRes.distance > temp.distance)
                    {
                        rayRes = temp;
                        rayRes.rayID = _index;

                        //hitpos = res.hitPoint;
                    }
                }
                else
                {
                    rayRes = temp;
                    rayRes.rayID = _index;

                    //hitpos = res.hitPoint;
                }
            }

            //return rayRes;  

            if (rayRes == null)
            {
                rayRes = new RaycastHit2D();

                rayRes.hitPoint = raycast.origin + raycast.direction * Length;
            }


            #region DDA
            RaycastHit2D _ddaRes = _dda.RayCast2Map(raycast.origin, rayRes.hitPoint);

            if (_ddaRes == null)
            {
                //dda에서 검출 안되면 기존 결과 리턴
                return rayRes;
            }
            else
            {
                //검출시 벽 위치 보내기
                //Console.WriteLine("벽이야");
                return _ddaRes;
            }
            #endregion

        }

        //Vector2 hitpos;

        private void Update()
        {
            //hitpos = Vector2.Zero;


        }


        void OnDrawGizmos()
        {
            // Gizmos 색상을 붉은색으로 설정
            //Gizmos.color = Color.red;

            // 지정된 위치에 구체를 그리기
            //Gizmos.DrawSphere(new Vector3(hitpos.X, hitpos.Y), 1f);
        }

    }



    /*
        private Dictionary<int, Raycast> raycasts = new Dictionary<int, Raycast>();


        public void Add(int key, Vector3 origin, Vector3 direction, float distance)
        {
            if (!raycasts.ContainsKey(key))
            {
                Raycast newRaycast = new Raycast(origin, direction, distance);
                raycasts.Add(key, newRaycast);
            }
            else
            {
                Debug.LogWarning($"Raycast with key {key} already exists.");
            }
        }

        public void Add(int key, Raycast raycast)
    {
        if (!raycasts.ContainsKey(key))
        {
            raycasts.Add(key, raycast);
        }
        else
        {
            Debug.LogWarning($"Raycast with key {key} already exists.");
        }
    }



    public void Remove(int key)
    {
        if (raycasts.ContainsKey(key))
        {
            raycasts.Remove(key);
        }
        else
        {
            Debug.LogWarning($"Raycast with key {key} does not exist.");
        }
    }

    // 특정 Raycast 가져오는 메서드
    public Raycast GetRaycast(int key)
    {
        if (raycasts.TryGetValue(key, out Raycast raycast))
        {
            return raycast;
        }
        else
        {
            Debug.LogWarning($"Raycast with key {key} does not exist.");
            return null;
        }
    }
    */
}