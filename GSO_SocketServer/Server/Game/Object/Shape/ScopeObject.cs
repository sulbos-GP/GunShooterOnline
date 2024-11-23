using Collision.Shapes;
using Differ;
using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Object.Shape
{
    public class ScopeObject : GameObject
    {
        private GameObject _owner;

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


       /* public override void OnCollision(GameObject other)
        {
            //base.OnCollision(other);

            if (other.ObjectType == GameObjectType.Player && GetOwner)
            {
                return;
            }

            if (collision.tag == "Player" && _owner.target == null)
            {
                Debug.Log("감지됨");
                _owner.target = collision.gameObject;
                _owner._state.ChangeState(_owner._check);     //idle과 return 상태에서 자동으로 check으로 전환됨
            }

        }

        private void OnTriggerStay2D(Collider2D collision)
        {
           
        }*/
        public override GameObject GetOwner()
        {
            return _owner;
        }


    }
}
