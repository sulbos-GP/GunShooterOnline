using Google.Protobuf.Protocol;
using Server.Data;
using Server.Database.Handler;
using Server.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCommonLibrary.Models.MasterDatabase;

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

    internal static void HandleItem401(GameObject obj)
    {
        int id = 401;
        FMasterItemUse use = DatabaseHandler.Context.MasterItemUse.Find(id);

        if (obj.ObjectType != GameObjectType.Player) //플레이어라면
            return;

        if(use == null) 
            return;

        //----------------- 쿨타임 ---------------
        Player p = (Player)obj;
        bool t = p.ApplyItem(id , use.cool_time);
        Console.WriteLine(t ? $"{obj.info.Name}성공" : $"{obj.info.Name}실패");
        if (t == false) //실패하면
            return;


        //----------------- 코스트 ---------------



        //------------ 통과 --------------


        //---------------- 후처리 --------------
        p.OnHealed(p, use.energy);

        Console.WriteLine($"Item ID : {id} Active");


        //------------ 
    }

    internal static void HandleItem402(GameObject obj)
    {
        int id = 402;
        FMasterItemUse use = DatabaseHandler.Context.MasterItemUse.Find(id);

        if (obj.ObjectType != GameObjectType.Player) //플레이어라면
            return;

        if (use == null)
            return;

        //----------------- 쿨타임 ---------------
        Player p = (Player)obj;
        bool t = p.ApplyItem(id, use.cool_time);
        Console.WriteLine(t ? $"{obj.info.Name}성공" : $"{obj.info.Name}실패");
        if (t == false) //실패하면
            return;


        //----------------- 코스트 ---------------



        //------------ 통과 --------------


        //---------------- 후처리 --------------
        p.OnHealed(p, use.energy);

        Console.WriteLine($"Item ID : {id} Active");


        //------------ 
    }

    internal static void HandleItem403(GameObject obj)
    {
        int id = 403;
        FMasterItemUse use = DatabaseHandler.Context.MasterItemUse.Find(id);

        if (obj.ObjectType != GameObjectType.Player) //플레이어라면
            return;

        if (use == null)
            return;

        //----------------- 쿨타임 ---------------
        Player p = (Player)obj;
        bool t = p.ApplyItem(id, use.cool_time);
        Console.WriteLine(t ? $"{obj.info.Name}성공" : $"{obj.info.Name}실패");
        if (t == false) //실패하면
            return;


        //----------------- 코스트 ---------------



        //------------ 통과 --------------


        //---------------- 후처리 --------------
        p.gameRoom.Push(() => p.OnHealed(p, use.energy));

        for (int i = 1; i < use.duration; i++)
        {
            p.gameRoom.PushAfter((int)(i * use.active_time * 1000), () => p.OnHealed(p, use.energy));

        }

        Console.WriteLine($"Item ID : {id} Active");


        //------------ 
    }

    internal static void HandleItem404(GameObject obj)
    {
        int id = 404;
        FMasterItemUse use = DatabaseHandler.Context.MasterItemUse.Find(id);

        if (obj.ObjectType != GameObjectType.Player) //플레이어라면
            return;

        if (use == null)
            return;

        //----------------- 쿨타임 ---------------
        Player p = (Player)obj;
        bool t = p.ApplyItem(id, use.cool_time);
        Console.WriteLine(t ? $"{obj.info.Name}성공" : $"{obj.info.Name}실패");
        if (t == false) //실패하면
            return;


        //----------------- 코스트 ---------------



        //------------ 통과 --------------


        //---------------- 후처리 --------------

        p.gameRoom.Push(() => p.OnHealed(p, use.energy));

        for (int i = 1; i < use.duration; i++)
        {
            p.gameRoom.PushAfter((int)(i * use.active_time * 1000), () => p.OnHealed(p, use.energy));

        }
        Console.WriteLine($"Item ID : {id} Active");


        //------------ 
    }
}

