using Collision.Shapes;
using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Object
{
    public class RootableObject : GameObject
    {
        protected bool destroyed = false;
        protected bool active = true;
        public Inventory inventory;

        public RootableObject()
        {
            ObjectType = GameObjectType.Box;
        }

        public void Init()
        {
            inventory = new Inventory(Id);
        }



      


        
    }
}
