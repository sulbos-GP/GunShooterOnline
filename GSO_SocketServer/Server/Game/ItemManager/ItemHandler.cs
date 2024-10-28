using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.ItemManager
{
    internal class ItemHandler
    {
        internal static void HandleItem101(GameObject go)
        {
           /* Skill _skill;
            if (DataManager.SkillDict.TryGetValue(, out _skill) == false)
                return;

            //if(obj.stat.Mp <= 0 || obj.stat.Mp < _skill.)
            #region 검사구역

            if (obj.ObjectType != GameObjectType.Player) //플레이어라면
                return;

            //----------------- 쿨타임 ---------------
            Player p = (Player)obj;
            bool t = p.ApplySkill(, _skill.cooldown);
            Console.WriteLine(t ? $"{obj.info.Name}성공" : $"{obj.info.Name}실패");
            if (t == false) //실패하면
                return;


            //----------------- 코스트 ---------------







            #endregion
            //------------ 통과 --------------


            //---------------- 후처리 --------------


            //------------ 
            Console.WriteLine("Skill____________");*/
        }

        internal static void HandleItem401(GameObject @object)
        {
            throw new NotImplementedException();
        }

        internal static void HandleItem402(GameObject @object)
        {
            throw new NotImplementedException();
        }

        internal static void HandleItem403(GameObject @object)
        {
            throw new NotImplementedException();
        }

        internal static void HandleItem404(GameObject @object)
        {
            throw new NotImplementedException();
        }
    }
}
