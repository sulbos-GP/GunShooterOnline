using System.Collections;
using Google.Protobuf.Protocol;
using MathNet.Numerics;
using UnityEngine;

public class PlayerController : CreatureController
{
    protected Coroutine _coSkill;
    protected bool _rangedSkill = false;

    private Transform prevTrn;
    private float Timer = 0.0f;
    private float moveInterval = 0.5f;
    public AudioSource AudioSource;
   

    private void FixedUpdate()
    {
        Timer += Time.deltaTime;
    }
    

    protected override void Init()
    {
        base.Init();
        Debug.Log("init");
        animator = transform.GetChild(1).GetComponent<Animator>();
        characterSprite = transform.GetChild(1).GetComponent<SpriteRenderer>();
        AudioSource = transform.GetComponent<AudioSource>();
    }

    protected override void UpdateController()
    {
        base.UpdateController();
    }

    public override void UpdateMoving()
    {
        //플레이어 위치 변경
        //if ((CellPos - transform.position).sqrMagnitude > 10)
        //    transform.position = CellPos;
        //else
        //    transform.position = Vector3.Lerp(transform.position, CellPos, Time.deltaTime * 10);

        //Move
        Rigidbody2D rig = gameObject.GetComponent<Rigidbody2D>();
        Vector2 newVec2 = Dir * 5.0f * Time.fixedDeltaTime;
        AudioManager.instance.PlaySound("Move",gameObject.GetComponent<AudioSource>());

        //rig.MovePosition(rig.position + newVec2);

        //float angle = Mathf.Atan2(Dir.y, Dir.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
    }

    protected override void UpdateIdle()
    {
        AudioManager.instance.StopSound(gameObject.GetComponent<AudioSource>());
    }



    protected virtual void CheakUpdatedFlag(bool isForce = false)
    {
    }


    public override void OnDamaged(Transform attacker)
    {
        base.OnDamaged(attacker);
        //Debug.Log("Player HIT !");
    }

    public override void OnDead(int attackerId = -1)
    {
        if (attackerId == -1)
            return;

        IsDead = true;

        if (Managers.Object.MyPlayer.Hp == 0)
            UIManager.Instance.SetDieMessage(Managers.Object.FindById(attackerId).name);
        Debug.Log(transform.name + "Dead");

        Managers.Object.RemoveWithoutDestroy(Id);

        animator.SetTrigger("IsDie");
        AudioSource.Stop();

        //사망 후 3초뒤 파괴?
        Invoke("DestoryPlayer", 3);
    }

    private void DestoryPlayer()
    {
        Managers.Resource.Destroy(gameObject);
    }
}