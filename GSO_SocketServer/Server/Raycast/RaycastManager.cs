using Collision.Shapes;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Vector2 = System.Numerics.Vector2;
using Server.Game;
using ObjectManager = Server.Game.ObjectManager;

namespace Server
{
    public class RaycastManager
    {
        public static RaycastManager Instance;
        static DDA _dda = new DDA(); //�ȿ� �� ������ ����
        private static int _index = 0;

        private void Awake()
        {
            //Physics2D.Raycast()
            Instance = this;
            _dda.Init();
        }

        private void OnEnable()
        {
            _index = 0;
        }



        public static RaycastHit2D Raycast(Vector2 StartPos, Vector2 Dir, float Length)
        {

            #region Packet

            #endregion

            _index++;

            //Debug.Log(_index);

            Ray raycast = new Ray(_index, StartPos, Dir, Length);
            Shape[] shapes = ObjectManager.Instance.GetValue();


            RaycastHit2D rayRes = null;

            foreach (Shape shape in shapes)
            {
                RaycastHit2D temp = raycast.Cast(shape);

                if (temp == null)
                    continue;

                if (rayRes != null)
                {
                    if (rayRes.distance > temp.distance)
                    {
                        rayRes = temp;
                        rayRes.Id = _index;

                        //hitpos = res.hitPoint;
                    }
                }
                else
                {
                    rayRes = temp;
                    rayRes.Id = _index;

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
                //dda���� ���� �ȵǸ� ���� ��� ����
                return rayRes;
            }
            else
            {
                //����� �� ��ġ ������
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
            // Gizmos ������ ���������� ����
            //Gizmos.color = Color.red;

            // ������ ��ġ�� ��ü�� �׸���
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

    // Ư�� Raycast �������� �޼���
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