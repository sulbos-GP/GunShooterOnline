using Collision.Shapes;
using Differ;
using Google.Protobuf.Protocol;
using Pipelines.Sockets.Unofficial.Buffers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Object.Shape
{
    public class ScopeObject : GameObject
    {
        private GameObject _owner;
        List<GameObject> hits = new List<GameObject>();

        public ScopeObject()
        {
            ObjectType = GameObjectType.Box;

            float width = 1;
            float left = -0.5f;
            float bottom = -0.5f;
            Polygon rectangle = ShapeManager.CreateCenterSquare(left, bottom, width);
            rectangle.Parent = this;

            currentShape = rectangle;

            info.Box = new BoxInfo()
            {
                X = 6,
                Y = 6,
                Weight = 50,
            };



        }

        public void Init(GameObject owenr)
        {
            _owner = owenr;

           info.Name = owenr.info.Name + "Detect";
           OwnerId = owenr.Id;
           currentShape.Parent = owenr;
           CellPos = owenr.CellPos;
        }

        public override void Update()
        {
            base.Update();

            CellPos = _owner.CellPos;

            GameObject closestTarget = null;
            float closestDistance = float.MaxValue;

            foreach (GameObject gameObject in hits)
            {
                // 현재 오브젝트와 타겟 간의 거리 계산
                float distance = Vector2.Distance(_owner.CellPos, gameObject.CellPos);

                // 가장 가까운 거리 갱신
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = gameObject;
                }
            }

            if (closestTarget != null)
            {
                EnemyAI enemyAI = _owner as EnemyAI;
                if(enemyAI != null)
                {
                    enemyAI.target = closestTarget;
                    enemyAI._state.ChangeState(enemyAI.CheckState); // 상태를 체크로 전환
                }
               
            }


            hits.Clear();
        }

        public override void OnCollision(GameObject other)
        {
            //base.OnCollision(other);

            if (other.ObjectType != GameObjectType.Player && other.GetOwner().Id != OwnerId)
            {
                return;
            }
            hits.Add(other);

           
        }

      
        public override GameObject GetOwner()
        {
            return _owner;
        }


    }
}
