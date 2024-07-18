using System;
using Server.Data;
using UnityEngine;

public class ArrowController : ProjectileContoller
{
    private bool _first;
    private Vector3 _temp = Vector3.zero;
    private float _time;


    //Vector3 last = Vector3.zero;
    //Vector3 _dir;

    //public override void UpdateMoving()
    //{
    //    if (CellPos != last)  //새로 갱신
    //    {
    //        //-------------------------속도 디버깅-------------------------
    //        last = CellPos;
    //        GameObject go = Managers.Resource.Instantiate("Objects/Pos");
    //        GameObject pso = Managers.Resource.Instantiate("Objects/Pos");
    //        pso.GetComponent<SpriteRenderer>().color = Color.red;
    //        go.transform.position = CellPos;
    //        pso.transform.position = transform.position;
    //        //transform


    //        _dir = (CellPos - transform.position).normalized;
    //        Debug.Log(_dir.ToString("00.000000"));
    //    }


    //        if ((CellPos - transform.position).magnitude > 10)
    //            transform.position = CellPos;
    //        else
    //        {
    //            rig.MovePosition(transform.position + Speed * Time.deltaTime * _dir);


    //        }


    //}


    private Vector3 last = Vector3.zero;
    private Rigidbody2D rig;
    private Skill skill;

    private void Start()
    {
        //Debug.Log(Dir.x.ToString("0.00000") +  Dir.y.ToString("0.00000"));
        Debug.Log("시작 :" + Environment.TickCount);

        rig = GetComponent<Rigidbody2D>();
        //Destroy(gameObject,5f);

        var rot_z = Mathf.Atan2(Dir.y, Dir.x) * Mathf.Rad2Deg;

        if (float.IsNaN(rot_z))
        {
            Destroy(gameObject);
            return;
        }

        transform.rotation = Quaternion.Euler(0f, 0f, rot_z);


        if (DataManager.SkillDict.TryGetValue(SkillId, out skill))
        {
        }
        else
        {
            Debug.LogError("스킬 딕션어리 오류");
            transform.position = CellPos;
            //if ((CellPos - transform.position).magnitude > 20)
            //    transform.position = CellPos;
            //else
            //    transform.position = Vector3.Lerp(transform.position, CellPos, Time.deltaTime * 10);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile") || collision.CompareTag("Player") || collision.CompareTag("Map"))
            return;

        //Debug.Log(collision.name);
        //if (collision.CompareTag("Monster") || collision.CompareTag("Enemy"))
        //{
        //    CreatureController cc = collision.GetComponent<CreatureController>();
        //    if (cc == null)
        //    {
        //        return;
        //    }

        //    C_Hit hitPacket = new C_Hit()
        //    {
        //        AttackId = Id,
        //        HitId = cc.Id,
        //    };

        //    Managers.Network.Send(hitPacket);
        //}

        //Destroy(gameObject);
    }

    public override void UpdateMoving()
    {
        if (_first == false) // 위치로 순간이동
        {
            transform.position = CellPos;
            //Debug.Log("처음 갱신 :" + transform.position + "/" + CellPos + "/" + System.Environment.TickCount);
            // Debug.Log(Dir);
            _first = true;
            return;
        }

        if (CellPos != last) //새로 갱신
        {
            //Debug.Log("새로 갱신 :" + transform.position + "/" + CellPos + "/" + System.Environment.TickCount);

            last = CellPos;

            if (_time != Time.time)
                //Debug.Log(Time.time - _time);
                _time = Time.time;
            if (_temp != transform.position)
                //Debug.Log(_temp - transform.position);
                //Debug.Log((_temp - transform.position).magnitude);
                _temp = transform.position;

            //-------------------------속도 디버깅-------------------------

            //Todo : s나중에 하기
            var go = Managers.Resource.Instantiate("Objects/Pos");
            var pso = Managers.Resource.Instantiate("Objects/Pos");
            pso.GetComponent<SpriteRenderer>().color = Color.red;
            go.transform.position = CellPos;
            pso.transform.position = transform.position;

            //Debug.Log("Change = "+transform.position);
        }


        if ((CellPos - transform.position).magnitude > 5)
            transform.position = CellPos;
        else
            transform.position = Vector3.MoveTowards(transform.position, CellPos, Speed * Time.deltaTime);
    }
}