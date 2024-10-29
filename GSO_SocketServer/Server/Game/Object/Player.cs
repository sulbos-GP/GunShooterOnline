using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Net.Http;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Collision.Shapes;
using Google.Protobuf.Protocol;
using Humanizer.DateTimeHumanizeStrategy;
using Server.Database.Handler;
using Server.Game.Object.Gear;
using WebCommonLibrary.Models.GameDB;
using WebCommonLibrary.Models.MasterDatabase;

namespace Server.Game;

public class Player : CreatureObj
{
    public SkillCoolDown SkillCoolDown = new();
    public ItemCoolDown ItemCoolDown = new();
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

        /*var diePacket = new S_Die();
        diePacket.ObjectId = Id;
        diePacket.AttackerId = attacker.Id;

        gameRoom.BroadCast(diePacket);

        var room = gameRoom;*/

        base.OnDead(attacker);

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
    /// <summary>
    /// 등록된 아이템을 사용. 이건 아이템의 기능이 나와야할듯
    /// </summary>
    public void UseQuickSlot(int sourceObjectId, int deleteItemId)
    {
        
        PS_ItemInfo deleteInfo;
        ItemObject deleteItem = gameRoom.FindAndDeleteItem(this , sourceObjectId, deleteItemId, out deleteInfo);

        if (deleteItem == null)
        {
            // 삭제 실패 시 패킷 전송
            S_DeleteItem packet = new S_DeleteItem
            {
                IsSuccess = false,
                DeleteItem = deleteInfo,
                SourceObjectId = sourceObjectId
            };
            Session.Send(packet);

            return;
        }
        
        // 삭제 성공 시 데이터베이스 처리 및 결과 전송
        gameRoom.HandleDeleteItemResult(this, sourceObjectId, deleteItem, deleteInfo);

        //아이템이 있고 사용할 수 있는 상태


        if (deleteItem == null)
        {
            Console.WriteLine("아이템이 등록되어있지 않음");
            return;
        }

        ItemManager.Instance.UseIteme(this, deleteItem.ItemId);


      
    }


    public bool ApplyItem(int id, double CoolDown)
    {
        short ItemCool = ItemCoolDown.GetCoolTime(id);
        short current = (short)(DateTime.Now.Second + DateTime.Now.Minute * 60);
        //최소 : 0 최대 : 3660
        //Console.WriteLine(skillCool);
        //Console.WriteLine(currnt);
        var t = ItemCool + CoolDown;
        if (current >= (t >= 3599 ? t - 3599 : t))
        {
            ItemCoolDown.SetCoolTime(id, current);
            return true;
        }

        return false;
    }

    /*// 아이템을 사용할 수 있는지 확인
    public bool CanUseItem(FMasterItemUse item)
    {
        if (!itemCooldowns.ContainsKey(itemId))
        {
            itemCooldowns.Add(itemId, DateTime.Now);
            return true;
        }

        DateTime lastUsedTime = itemCooldowns[itemId];

        TimeSpan timeSinceLastUse = DateTime.Now - lastUsedTime;

        if (timeSinceLastUse.TotalSeconds >= defaultCooldownDuration)
        {
            return true; // 쿨타임이 끝남
        }
        else
        {
            Console.WriteLine($"아이템 {itemId} 사용 불가. 남은 쿨타임: {defaultCooldownDuration - timeSinceLastUse.TotalSeconds:F2}초");
            return false; // 쿨타임이 아직 남아 있음
        }
    }


    public bool UseConsume(FMasterItemUse consume)
    {
        if (!isReady)
        {
            Console.WriteLine("현재 비활성화 상태임");
            return false;
        }

        if (cooltimer != null)
        {
            Console.WriteLine("쿨타임이 돌아가는 중");
            return false;
        }

        if (consume.effect == EEffect.immediate)
        {
            isReady = false;
            OnHealed(this,consume.energy);
            cooltimer = StartCooltime(consume.cool_time);
        }
        else if (consume.effect == EEffect.buff)
        {
            isReady = false;
            myPlayer.OnHealed(consume.energy);
            StartBuff(myPlayer, consume);
        }

        return true;
    }

    private void StartBuff(MyPlayerController target, DataMasterItemUse consume)
    {
        Task.Run(async () =>
        {
            float elapsedTime = 0f;

            while (elapsedTime < consume.duration)
            {
                if (target.Hp < target.MaxHp)
                {
                    target.OnHealed(consume.energy);
                }

                await Task.Delay(TimeSpan.FromSeconds(consume.active_time));

                elapsedTime += consume.active_time;
            }

            cooltimer = StartCooltime(consume.cool_time);
        });
    }
*/

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