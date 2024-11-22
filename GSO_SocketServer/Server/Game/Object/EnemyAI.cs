using Collision.Shapes;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Object
{
    public class EnemyAI : CreatureObj
    {

        private IJob _job;


        public EnemyAI()
        {
            ObjectType = GameObjectType.Enemyai;

            stat.MergeFrom(new StatInfo()
            {
                //Attack = 3,
                Hp = 20,
                MaxHp = 20,
            });

            float width = 0.5f;
            float left = -0.5f;
            float bottom = -0.5f;
            Polygon rectangle = ShapeManager.CreateCenterSquare(left, bottom, width);
            rectangle.Parent = this;

            currentShape = rectangle;
            Console.WriteLine("생성자 테스트");
            info.Name = "Dog";

        }

        public override void Update()
        {
            if (gameRoom != null) 
                _job = gameRoom.PushAfter(Program.ServerIntervalTick, Update);



            //업데이트 코드
        }

        public override void OnCollision(GameObject other)
        {
            base.OnCollision(other);
        }

    

        public override void OnDead(GameObject attacker)
        {
            base.OnDead(attacker);
        }

        public override void OnDamaged(GameObject attacker, int damage)
        {
            base.OnDamaged(attacker, damage);
        }

        public override void OnHealed(GameObject healer, int heal)
        {
            base.OnHealed(healer, heal);
        }
    }
}
