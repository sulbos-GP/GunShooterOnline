using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Protocol;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public partial class MyPlayerController : PlayerController
{
    [SerializeField] private List<BaseController> _attackableList = new();

    private ButtonSkill _butnSkill;
    private GameInfoBar _gameInfoBar; // HP+ Exp + Info
    
    private Coroutine _coskillCoolTime;
    private VirtualJoystick _joystick;
    private Vector3 _latestPos;

    private MyPlayerAttack _myPlayerAttack;

    //private Vector2 _skillDir = new();
    private SkillRequst _skillPanel;

    private Vector3 _target;
    //게임

    private PlayerInput playerInput;


    public int Attack => Stat.Attack;


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
            _expratio = Mathf.Max(0, (float)Stat.Exp / Stat.MaxExp);
            _gameInfoBar.PlayerExpBarHandle.GetComponent<Image>().fillAmount = _expratio;
        }
        
    }
    protected override void Init()
    {
        //base 무시
        base.Init();
        _gameInfoBar = GameObject.Find("Canvas").transform.GetComponentInChildren<GameInfoBar>();
        ChangeStat = CheakUpdateInfoBar; //초기화 했음
        
        
        // Player 
        var inputActions = new InputActions_Player();
        inputActions.PlayerControls.Enable();
        inputActions.PlayerControls.Movement.performed += Movement_performed;
        inputActions.PlayerControls.Movement.canceled += Movement_Canceled;


        //---------------------------------------------------------------------------
        _myPlayerAttack = GetComponentInChildren<MyPlayerAttack>();
        _myPlayerAttack.transform.GetComponent<CircleCollider2D>().radius = AttackRange;

        //Managers.Object.MyPlayer.RefreshAddtionalStat();


        //----------------------------------------------------------------------------------------
        //_joystick = FindObjectOfType<VirtualJoystick>();
        //if (_joystick == null) _joystick = Managers.Resource.Instantiate("UI/Joystick").GetComponent<VirtualJoystick>();
        //_joystick.StartJoystick(true, this);
        //----------------------------------------------------------------------------------------


        //----------------------------------------------------------------------------------------

        _skillPanel = FindObjectOfType<SkillRequst>();
        if (_skillPanel == null)
            _skillPanel = Managers.Resource.Instantiate("UI/SkillPanel").GetOrAddComponent<SkillRequst>();
        _skillPanel.Init();
        _skillPanel.Player = gameObject;
        //----------------------------------------------------------------------------------------

        //----------------------------------------------------------------------------------------

        _butnSkill = FindObjectOfType<ButtonSkill>();
        if (_butnSkill == null)
            _butnSkill = Managers.Resource.Instantiate("UI/SkillBtns").GetOrAddComponent<ButtonSkill>();
        _butnSkill.Init(Stat);

        //----------------------------------------------------------------------------------------
        AutoUpdatedFlag();
    }

    private int leastLevel = 0;
    public void CheakUpdateLevel()
    {
        if (leastLevel < Stat.Level) //레벨이 오르면
        {
            Debug.Log(($"Level Up : {leastLevel} -> {Stat.Level} "));
             _butnSkill.UpdateBtn(Stat.Level);
        }

        leastLevel = Stat.Level;
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


    /*protected override void UpdateController()
    {
        GetUIKeyInput();

        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    Vector2 _dir = FindEnemyAndAttack();
        //    if (_dir != Vector2.zero)
        //    {
        //        UseSkill(200, dir: _dir);
        //    }
        //}
        //if (Input.GetKeyDown(KeyCode.J))
        //{
        //    UseSkill(301);
        //}
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    UseSkill(102);
        //} 
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    UseSkill(102);
        //}


        if (Dir.x == -1)
            GetComponent<SpriteRenderer>().flipX = true;
        else if (Dir.x == 1)
            GetComponent<SpriteRenderer>().flipX = false;

        base.UpdateController();
    }*/


    private void OnAttack(InputValue value)
    {
        Debug.Log("OnAttack");
    }

    private void GetUIKeyInput()
    {
        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //	UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        //	UI_Inventory invenUI = gameSceneUI.InvenUI;

        //	if (invenUI.gameObject.activeSelf)
        //	{
        //		invenUI.gameObject.SetActive(false);
        //	}
        //	else
        //	{
        //		invenUI.gameObject.SetActive(true);
        //		invenUI.RefreshUI();
        //	}
        //}
        //else if (Input.GetKeyDown(KeyCode.C))
        //{
        //	UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
        //	UI_Stat statUI = gameSceneUI.StatUI;

        //	if (statUI.gameObject.activeSelf)
        //	{
        //		statUI.gameObject.SetActive(false);
        //	}
        //	else
        //	{
        //		statUI.gameObject.SetActive(true);
        //		statUI.RefreshUI();
        //	}
        //}
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


    //IEnumerator CoJumpFlag()
    //{
    //    int t = 0;
    //    while (t < 10)
    //    {
    //        CheakUpdatedFlag(isForce: true);
    //        yield return new WaitForSeconds(.1f);
    //        t += 1;
    //    }

    //    Debug.Log("CoUpdateFlagEnd");
    //}


    private void AutoUpdatedFlag()
    {
        CellPos = transform.position;
        CheakUpdatedFlag(true);
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
}