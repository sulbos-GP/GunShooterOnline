using System;
using System.Numerics;
using Google.Protobuf.Protocol;
using ServerCore;
using WebCommonLibrary.Models.GameDB;
using static System.Net.Mime.MediaTypeNames;

namespace Server.Game;

public class CreatureObj : GameObject
{

    public CreatureObj()
    {
        info.StatInfo = stat;
    }

    public Action<GameObject> DamageReflexAction;


    public StatInfo stat { get; private set; } = new();


   

    public virtual void OnDead(GameObject attacker)
    {
        if (gameRoom == null)
            return;

        Player player = attacker as Player;
        if(player != null)
        {
            EventBus.Publish(EEventBusType.Combat, player, "PLAYER");
        }


        var room = gameRoom;


        room.Push(room.LeaveGame, Id);

    


        var diePacket = new S_Die();
        diePacket.ObjectId = Id;
        diePacket.AttackerId = attacker.Id;

        room.BroadCast(diePacket);
        //room.Push(room.BroadCast , diePacket);

        Console.WriteLine("Die Pakcet Send");

        if (gameRoom.MatchInfo.TryGetValue(UID, out MatchOutcome outcome) == true)
        {
            outcome.death += 1;
        }
        else
        {
            Console.WriteLine("outcome is null");
        }
        gameRoom.PostPlayerStats(Id);


        /*room.Push(new Job(() =>
        {
            stat.Hp = stat.MaxHp;
            //PosInfo.State = CreatureState.Idle;
        }));
        room.PushAfter(10, room.EnterGame, this);*/
    }
   



    public float AttackRange
    {
        get => stat.AttackRange;
        set => stat.AttackRange = value;
    }


    public int Hp
    {
        get => stat.Hp;
        set => stat.Hp = Math.Clamp(value, 0, stat.MaxHp);
    }


  


   
    /// <summary>
    /// 
    /// </summary>
    /// <param name="attacker">스킬 or 장판 or 발사체</param>
    /// <param name="damage"></param>
    public override void OnDamaged(GameObject attacker, int damage) //할일 TODO : 데미지 말고 영향(class damaage effect)등 고려
    {
        if (gameRoom == null)
            return;

        GameObject rootAttacker = attacker;

        if (attacker.ObjectType == GameObjectType.Monster || attacker.ObjectType == GameObjectType.Player)
        {

        }
        else
        {
            rootAttacker = attacker.GetOwner();
        }
        //할일 TODO : 만약 주인이 또 스킬이거나 하는 경우!!
        //TODO : 진짜 딜량 구하기 지승현 241019
        Player p = this as Player;
        if (p != null)
        {
            damage = Math.Max(damage - p.gear.GetPartItem(WebCommonLibrary.Enum.EGearPart.Armor).Data.amount, 0);

        }
        else
        {
            damage = Math.Max(damage, 0);

        }





        stat.Hp = Math.Max(stat.Hp - damage, 0);

        Console.WriteLine($" attacker :{rootAttacker.Id} Damage : {damage}  stat.Hp : {stat.Hp}");


        MatchOutcome attackerInfo;
        if(gameRoom.MatchInfo.TryGetValue(attacker.UID, out attackerInfo) == true)
        {
            attackerInfo.damage += damage;
        }




        var ChangePacket = new S_ChangeHp();
        ChangePacket.ObjectId = Id;
        ChangePacket.Hp = stat.Hp;
        gameRoom.BroadCast(ChangePacket);

        if (stat.Hp <= 0)
        {
            if (attackerInfo != null)
            {
                attackerInfo.kills += 1;
            }
           


                stat.Hp = 0;
            OnDead(rootAttacker);
        }
    }

    public override void OnHealed(GameObject healer, int heal)
    {
        if (gameRoom == null)
            return;

        //TODO : 진짜 힐량 구하기  지승현 241019
        heal = Math.Max(heal, 0);
        stat.Hp = Math.Max(stat.Hp + heal, stat.MaxHp);

        Console.WriteLine($" healer :{healer.Id} heal : {heal}  stat.Hp : {stat.Hp}");

/*
        MatchOutcome attackerInfo;
        if (gameRoom.MatchInfo.TryGetValue(attacker.UID, out attackerInfo) == true)
        {
            attackerInfo.damage += damage;
        }
*/

        var ChangePacket = new S_ChangeHp();
        ChangePacket.ObjectId = Id;
        ChangePacket.Hp = stat.Hp;
        gameRoom.BroadCast(ChangePacket);


    }




}