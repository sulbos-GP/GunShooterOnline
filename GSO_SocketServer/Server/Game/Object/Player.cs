using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Net.Http;
using System.Numerics;
using System.Security.Cryptography;
using Collision.Shapes;
using Google.Protobuf.Protocol;
using Server.Game.Object.Gear;
using WebCommonLibrary.Models.GameDB;

namespace Server.Game;

public class Player : CreatureObj
{
    public SkillCoolDown SkillCoolDown = new();
    public Inventory inventory;
    public Gear gear;
    public Gun gun = new();
    public CredentiaInfo credential;

    private float SpawnTime = 0;


    public Player()
    {
        Console.WriteLine("Player Init");

        ObjectType = GameObjectType.Player;

        //바꾼 부분(패킷핸들러의 C_EnterGameHandler에서 플레이어 설정)
        //inventory = new Inventory(Id,6, 7);
        Vision = new VisionRegion(this);

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


        gun.Init(this);

        SpawnTime = System.Environment.TickCount;

    }

    public ClientSession Session { get; set; }
    public VisionRegion Vision { get; set; }


    public Vector2 SkillDir { get; set; }
    public List<GameObject> Targets { get; set; } = new(); //스킬 공격


    public override void OnDamaged(GameObject attacker, int damage)
    {
        if (DamageReflexAction != null)
            DamageReflexAction(attacker);
        else
            base.OnDamaged(attacker, damage);
    }

    public override void OnDead(GameObject attacker)
    {
        if (gameRoom == null)
            return;

        var diePacket = new S_Die();
        diePacket.ObjectId = Id;
        diePacket.AttackerId = attacker.Id;

        gameRoom.BroadCast(diePacket);

        var room = gameRoom;
        //room.Push(room.LeaveGame, Id);
    }


    public void OnEscaped()
    {
        MatchOutcome myInfo;
        if (gameRoom.MatchInfo.TryGetValue(UID, out myInfo) == true)
        {
            myInfo.survival_time =  (int)(System.Environment.TickCount - SpawnTime / 1000) ;
            myInfo.escape = 1;
        }
        else{
            Console.WriteLine("MatchInfo Error");
        }
    }


    #region InGames



    #endregion


    #region Skill

    public bool ApplySkill(int id, float CoolDown)
    {
        int skillCool = SkillCoolDown.GetCoolTime(id);
        var currnt = (short)(DateTime.Now.Second + DateTime.Now.Minute * 60);
        //최소 : 0 최대 : 3660
        //Console.WriteLine(skillCool);
        //Console.WriteLine(currnt);
        var t = skillCool + CoolDown;
        if (currnt >= (t >= 3599 ? t - 3599 : t))
        {
            SkillCoolDown.SetCoolTime(id, currnt);
            return true;
        }

        return false;
    }

   

    public bool CheakSkill(int id, float CoolDown) //Todo : 사용하기
    {
        int skillCool = SkillCoolDown.GetCoolTime(id);
        var currnt = (short)(DateTime.Now.Second + DateTime.Now.Minute * 60);
        //최소 : 0 최대 : 3660
        //Console.WriteLine(skillCool);
        //Console.WriteLine(currnt);
        var t = skillCool + CoolDown;
        if (currnt >= (t >= 3599 ? t - 3599 : t))
        {
            SkillCoolDown.SetCoolTime(id, currnt);
            return true;
        }

        return false;
    }

    #endregion
}