using Collision.Shapes;
using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class ExitZone : GameObject
    {

        
        public ExitZone()
        {
            ObjectType = GameObjectType.Exitzone;
            float width = 2;
            float left = 2;
            float top = 2;


            Polygon rectangle = ShapeManager.CreateCenterSquare(left, top, width);
            rectangle.Parent = this;
            currentShape = rectangle;

            Console.WriteLine(currentShape.position.x+ ": "+ currentShape.position.y);
        }

        public void Update(Player player)
        {
            if (IsPlayerInZone(player))
            {
                OnPlayerExit(player);
            }
        }

        private bool IsPlayerInZone(Player player)
        {
            //return Bounds.IntersectsWith(player.Bounds);
            return true;
        }

        private void OnPlayerExit(Player player)
        {
            // 게임을 종료하거나 다음 단계로 넘어가는 로직을 여기에 작성
            Console.WriteLine("Player has exited the zone!");

            // 예를 들어, 게임 종료 시
            // Game.End();

            // 또는 다음 단계로 넘어가기
            // Game.LoadNextLevel();
        }
    }
}
