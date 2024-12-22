using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.Protocol;
using NPOI.XSSF.UserModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public partial class MyPlayerController : PlayerController
{
    [SerializeField] private List<BaseController> _attackableList = new();

    
    private Vector3 _latestPos;

    //게임
    public PlayerInput playerInput;
    private GameInfoBar _gameInfoBar; // HP+ Exp + Info
    private VirtualJoystick _joystick;
    //public Gun usingGun { get; private set; } //플레이어가 들고있는 총(발사하는 총)
    
    //----------------------------------위: 사용 / 아래:사용안하는듯--------------------------
    //private ButtonSkill _butnSkill;
    //UI
    public UI_Quest _Quest;

    private Vector3 _target;
    private MyPlayerAttack _myPlayerAttack;
    //private Vector2 _skillDir = new();
    private SkillRequst _skillPanel;
    private Coroutine _coskillCoolTime;
    //public int Attack => Stat.Attack;
    

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);

        //Debug.Log($"결과 :{_rig2d.velocity}");
    }

    private void OnCollisionEnter2D(Collision2D collision) //물리적인 충돌
    {
        //콜라이더 끼리 not trigeger
    }

    private void OnTriggerEnter2D(Collider2D collision) //화살 맞으면
    {
        //Debug.Log($"test Hit by {collision.transform.name}");

        ProjectileContoller pc;
        collision.transform.TryGetComponent(out pc);
        if (collision.transform.CompareTag("Projectile") && pc != null && pc.OwnerId != Id)
        {
            Debug.Log($"Hit by {collision.transform.name}");
            OnDamaged(collision.transform);
        }
    }



    private void CheakUpdateInfoBar()
    {
        //hp나 스텟이 바뀌면 호출
        
        float _hpratio = 0f;
        if (Stat.MaxHp > 0)
        {
            _hpratio = Mathf.Max(0, (float)Hp / Stat.MaxHp);
            _gameInfoBar.PlayerHpBarHandle.GetComponent<Image>().fillAmount = _hpratio;
        }
        
        float _expratio = 0f;
        if (Stat.MaxHp > 0)
        {
           // _expratio = Mathf.Max(0, (float)Stat.Exp / Stat.MaxExp);
            _gameInfoBar.PlayerExpBarHandle.GetComponent<Image>().fillAmount = _expratio;
        }
        
    }
    protected override void Init()
    {
      
        InitWeaponQuickSlot();
        

        //base 무시
        base.Init();

        UIManager.Instance.SetReloadBtnListener(usingGun);


    }

    private int leastLevel = 0;
    public void CheakUpdateLevel()
    {
        /*
        if (leastLevel < Stat.Level) //레벨이 오르면
        {
            Debug.Log(($"Level Up : {leastLevel} -> {Stat.Level} "));
             _butnSkill.UpdateBtn(Stat.Level);
        }

        leastLevel = Stat.Level;*/
    }


    private void Movement_performed(InputAction.CallbackContext context)
    {
        //Debug.Log("Movement_performed");

        if (context.performed)
        {
            Debug.Log("Movement_performed");

            var inputMovement = context.ReadValue<Vector2>().normalized;
            Dir = new Vector2(inputMovement.x, inputMovement.y);

            //State = CreatureState.Moving;

            if (Dir == Vector2.zero)
                Dir = _joystick.GetDir();
        }
    }


    private void Movement_Canceled(InputAction.CallbackContext context)
    {
        Dir = Vector2.zero;
        //State = CreatureState.Idle;
    }

    public void UseSkill_Requst(int number, int[] targets = null, Vector2? dir = null)
    {
        //클래스가 100 -> 사용 스킬 100 ~ 110

        if (number <= 10)
            //버튼이면
            number += Stat.Class * 100; //100번 성기사 200번 ... 300번 ...

        //보내야할 스킬 찾기


        if (dir == null)
        {
            var t = (FindEnemyAndAttack() - (Vector2)CellPos).normalized;
            dir = new Vector2((float)Math.Round(t.x, 2), (float)Math.Round(t.y, 2));
        }

        UseSkill(number, dir: dir);
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();
        // 이동 상태로 갈지 확인
        if (Dir == Vector2.zero)
        {
            //State = CreatureState.Moving;
        }
    }

    public override void UpdateMoving()
    {
        //Dir = new Vector2(Dir.x, Dir.y); ;
        //Debug.Log(Dir.ToString("0.000"));

        //transform.Translate(Speed * Time.deltaTime * Dir.normalized, Space.Self);

        //Debug.Log("Dir " + Dir);
        _rig2d.velocity = Dir.normalized * Speed;
        CellPos = transform.position;
        RotationZ = transform.rotation.eulerAngles.z;


        if (Dir == Vector2.zero)
        {
            //Debug.Log(_rig2d.velocity);
            if (_rig2d.velocity.magnitude < 0.1f)
                _rig2d.velocity = Vector2.zero;

            //State = CreatureState.Idle;

            return;
        }

        CheakUpdatedFlag();
    }

    private void AutoUpdatedFlag()
    {
        CellPos = transform.position;
        //CheakUpdatedFlag(true);
        Invoke(nameof(AutoUpdatedFlag), 0.25f);
    }

    protected override void CheakUpdatedFlag(bool isForce = false)
    {
        if (isForce || _latestPos == null || Vector3.Distance(_latestPos, transform.position) > 0.05f)
        {
            _latestPos = transform.position;
            var movePacket = new C_Move();
            movePacket.PositionInfo = PosInfo;
            Managers.Network.Send(movePacket);
        }
    }

    private Vector2 FindEnemyAndAttack() // 근처에 있는 것들중 가까운거
    {
        _attackableList = _myPlayerAttack.GetAttackableList();

        if (_attackableList.Count <= 0)
            return Vector2.zero;

        _attackableList = _attackableList.Where(x => x.OwnerId != Id).ToList();


        var _totalAtackables = new List<RaycastHit2D>();

        foreach (var creature in _attackableList)
        {
            var _no = 0; //초기화
            /*if (State == CreatureState.Dead)
                return Vector2.zero;*/

            Vector2 distance = creature.transform.position - transform.position;
            var direction = distance.normalized;

            var hits = Physics2D.RaycastAll(transform.position, direction, distance.magnitude);

            foreach (var hit in hits)
                if (hit.transform.CompareTag("Object") || hit.transform.CompareTag("Map"))
                {
                    _no = 1; //중간에 막힘
                    break;
                }

            if (_no == 0) //안막혔다면
                _totalAtackables.Add(hits
                    .Where(hit => hit.transform.CompareTag("Enemy") || hit.transform.CompareTag("Monster")).First());
        }


        if (_totalAtackables.Count > 0)
        {
            var ray2d = _totalAtackables.OrderBy(t => t.distance).First();
            if (ray2d.transform != null)
                return ray2d.transform.position;
            Debug.Log("RaycastHit2D 오류");
        }

        Debug.Log("근처 공격 가능한 몬스터 없음");
        return Vector2.zero;
    }

    public Vector2 AttackTarget(Transform target)
    {
        if (target == null)
            return Vector2.zero;

        //쿨타임
        if (_coskillCoolTime == null)
        {
            Vector2 t = (target.transform.position - transform.position).normalized;


            var temp = new int[1] { target.GetComponent<CreatureController>().Id };
            //Debug.Log("ID =========" + target.GetComponent<CreatureController>().Id);

            //  UseSkill(100, targets: temp);

            return new Vector2((float)Math.Round(t.x, 2), (float)Math.Round(t.y, 2));

            //C_Skill skillpacket = new C_Skill();
            //skillpacket.Info = new SkillInfo()
            //{
            //    SkillId = Stat.Class + 10,
            //    DirX = (float)Math.Round(t.x, 2),
            //    DirY = (float)Math.Round(t.y, 2),
            //};


            //Server.Data.Skill skill;
            //DataManager.SkillDict.TryGetValue(skillpacket.Info.SkillId, out skill);


            //Managers.Network.Send(skillpacket);
            ////bullet.GetComponent
            //_coskillCoolTime = StartCoroutine(coInputCoolTime(skill.cooldown)); //Todo : 시간조정

            //페킷 보내기
        }

        return Vector2.zero;
    }

    private IEnumerator coInputCoolTime(float time)
    {
        yield return new WaitForSeconds(time);
        _coskillCoolTime = null;
    }


//ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ현재 들고 있는 총의 변화ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ//

    

    private Button quickSlotBtn1 => UIManager.Instance.MainWeaponBtn;
    private Button quickSlotBtn2 => UIManager.Instance.SubWeaponBtn;
    private void InitWeaponQuickSlot()
    {
        quickSlotBtn1.onClick.RemoveAllListeners();
        quickSlotBtn2.onClick.RemoveAllListeners();

        quickSlotBtn1.onClick.AddListener(() => ChangeUseGun(1));
        quickSlotBtn2.onClick.AddListener(() => ChangeUseGun(2));
    }

    public async void ChangeUseGun(int slotNumber)
    {
        if(slotNumber != 1 && slotNumber != 2) return;
        await Task.Delay(100);

        InventoryController inven = InventoryController.Instance;
        ItemData equipptedItem = inven.GetItemInDictByGearCode(slotNumber);
        if (equipptedItem == null)
        {
            UIManager.Instance.ReloadBtn.interactable = false;
            SendChangeGunPacket(0, slotNumber); //총을 들고있지 않을 경우 0(널값) 전송
            Managers.Object.MyPlayer.usingGun.curGunEquipSlot = 0;
            Managers.Object.MyPlayer.usingGun.ResetGun();
            return;
        }
        else if(equipptedItem.item_type != ItemType.Weapon)
        {
            Debug.Log("잘못된 아이템 참조");
        }
        UIManager.Instance.ReloadBtn.interactable = true;
        usingGun.SetGunStat(equipptedItem);
        usingGun.curGunEquipSlot = slotNumber;
        Debug.Log("usingGun : " + usingGun);

        SendChangeGunPacket(equipptedItem.objectId, slotNumber);
    }

    private static void SendChangeGunPacket(int gunObjectId, int slotNumber)
    {
        C_ChangeAppearance packet = new C_ChangeAppearance()
        {
            ObjectId = Managers.Object.MyPlayer.Id,
            GunType = new PS_GearInfo()
            {
                Item = new PS_ItemInfo()
                {
                    ObjectId = gunObjectId,
                },
            }
        };

        // 승현 : protocol에서 enum으로 switch해서 참조하기
        if (slotNumber == 1)
        {
            packet.GunType.Part = PE_GearPart.MainWeapon;
        }
        else if (slotNumber == 2) 
        {
            packet.GunType.Part = PE_GearPart.SubWeapon;

        }

        Managers.Network.Send(packet);
        Debug.Log($"C_ChangeAppearance 전송 {packet.ObjectId}, {packet.GunType.Part}");
    }

    private Coroutine healCoroutine;
    [ContextMenu("디버그 버프")]
    public void DebugHeal()
    {
        if(healCoroutine == null ) 
        {
            healCoroutine = StartCoroutine(OnBuffed(Data_master_item_use.GetData(404)));
        }
    }

    [ContextMenu("디버그 버프 중단")]
    public void DebugStopHeal()
    {
        if (healCoroutine != null)
        {
            StopCoroutine(healCoroutine);
        }
    }
}