using Collision.Shapes;
using Differ;
using Google.Protobuf.Compiler;
using Google.Protobuf.Protocol;
using Pipelines.Sockets.Unofficial.Buffers;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace Server.Game.Object.Shape
{
    public class ScopeObject : GameObject
    {
        private GameObject _owner;
        private List<GameObject> hits = new List<GameObject>();

        public ScopeObject()
        {
            ObjectType = GameObjectType.Noneobject;

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

            CellPos = _owner.CellPos; //콜라이더의 위치 업데이트

            //hit의 초기화
            hits.Clear();

            //EnemyAI enemyAI = _owner as EnemyAI;
            //if (enemyAI == null || enemyAI.target != null)
            //{
            //    //적 스크립트가 없거나 해당 적의 타겟이 이미 있다면 안함
            //    return;
            //}

            ////변수 초기화
            //GameObject closestTarget = null;
            //float closestDistance = float.MaxValue;

            ////hit안의 가장 가까운 게임 오브젝트 계산
            //foreach (GameObject gameObject in hits)
            //{
            //    float distance = Vector2.Distance(_owner.CellPos, gameObject.CellPos);

            //    if (distance < closestDistance)
            //    {
            //        closestDistance = distance;
            //        closestTarget = gameObject;
            //    }
            //    //Console.Write($"HiT {gameObject.info.Name}");
            //}

            //hit의 초기화
            //hits.Clear();

            //가까운 타겟이 있다면 타겟 할당
            //if (closestTarget != null)
            //{
            //    enemyAI.target = closestTarget;
            //    Console.WriteLine($"타겟세팅 {closestTarget.Id}\n타깃과의 거리 : {Vector2.Distance(enemyAI.CellPos, closestTarget.CellPos)}");
            //    if (enemyAI._state.CurState.state == FSM.MobState.Idle || enemyAI._state.CurState.state == FSM.MobState.Return)
            //    {
            //        //대기와 귀환 상태일때만 콜라이더로 인한 check전환
            //        enemyAI._state.ChangeState(enemyAI.CheckState); 
            //    }
               
               
            //}            
        }

        public override void OnCollision(GameObject other)
        {
            //base.OnCollision(other);

            if (other.ObjectType != GameObjectType.Player)
            {
                return;
            }

            //Console.WriteLine("Player");

            if (other.GetOwner().Id == OwnerId)
            {
                return;
            }

            if(hits.Contains(other)) { return; }

            hits.Add(other);
        }

        public List<GameObject> Hits
        {
            get
            {
                return hits;
            }
        }

        public GameObject GetNearestObject()
        {
            var nearest = hits
                .Select(gameObject => new
                {
                    GameObject = gameObject,
                    Distance = Math.Sqrt(
                        Math.Pow(gameObject.CellPos.X - _owner.CellPos.X, 2) + 
                        Math.Pow(gameObject.CellPos.Y - _owner.CellPos.Y, 2))
                })
            .OrderBy(result => result.Distance)
            .FirstOrDefault();

            if(nearest == null)
            {
                return null;
            }

            return nearest.GameObject;
        }

        public bool IsScopeInGameObject(GameObject gameObject)
        {
            return hits.Contains(gameObject);
        }

        public override GameObject GetOwner()
        {
            return _owner;
        }
    }
}
