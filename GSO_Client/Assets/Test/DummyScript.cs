using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyScript : CreatureController
{
    Rigidbody2D rig2D;

    public List<GameObject> grounds;
    public LayerMask mask;

    public List<GameObject> ttemp;

    public GameObject target;
    private void Awake()
    {
        rig2D = GetComponent<Rigidbody2D>();
    }

    IEnumerator CoStart(Action action, float time)
    {
        yield return new WaitForSeconds(time);
        action.Invoke();
    }

    void Start()
    {
        //StartCoroutine(CoStart(() => { Debug.Log(1); return; },1));
        //StartCoroutine(CoStart(() => { Debug.Log(3); return; },1));
        //StartCoroutine(CoStart(() => { Debug.Log(2); return; }, 2));



        //mask = LayerMask.GetMask("Ground");
    }
    
    

    // Update is called once per frame
    //void Update()
    //{

    //    //Debug.Log(transform.rotation.z);
    //    //Debug.Log(transform.rotation.eulerAngles.z);
    //    CellPos = new Vector3(4, -3.5f, 0);
    //    float dis = (CellPos - transform.position).magnitude;
    //    Speed = 6f;

    //    return;

    //    Vector2 t = (target.transform.position - transform.position).normalized;

    //    GameObject go = Managers.Resource.Instantiate("Creature/Arrow");
    //    go.name = "Arrow";

    //    ArrowController ac = go.GetComponent<ArrowController>();
    //    ac.PosInfo = new Google.Protobuf.Protocol.PositionInfo()
    //    {
    //        DirX = (float)Math.Round(t.x, 2),
    //        DirY = (float)Math.Round(t.y, 2),
    //        CurrentRoomId = 0,
    //        PosX = transform.position.x,
    //        PosY = transform.position.y,
    //        State = Google.Protobuf.Protocol.CreatureState.Moving

    //    };
    //    ac.Stat = new Google.Protobuf.Protocol.StatInfo()
    //    {
    //        Speed = 10f
    //    };
    //    ac.SyncPos();
    //    Debug.Log
    //        ($"{ac.PosInfo.DirX},{ac.PosInfo.DirY},{ac.PosInfo.PosX},{ac.PosInfo.PosY}");

    //    return;
    //    for (int i = 0; i < ttemp.Count; i++)
    //    {
    //        if (i == 0)
    //        {
    //            ttemp[i].transform.Rotate(new Vector3(0, 0, transform.rotation.eulerAngles.z), Space.World);
    //        }
    //        if (i == 1)
    //        {
    //            ttemp[i].transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z);

    //        }
    //        if (i == 2)
    //        {
    //            ttemp[i].transform.Rotate(new Vector3(0, 0, transform.rotation.eulerAngles.z), Space.Self);


    //        }
    //        if (i == 3)
    //        {
    //            ttemp[i].transform.eulerAngles = new Vector3(0, 0, transform.rotation.eulerAngles.z);

    //        }
    //        if (i == 4)
    //        {
    //            //ttemp[i].transform.rotation = new Vector3(0, 0, transform.rotation.z);

    //        }
    //    }







    //    if (Input.GetKeyDown(KeyCode.K))
    //    {
    //        StartCoroutine(Move());
    //    }

    //    Vector2 Dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    //    transform.Translate(Dir.normalized * 3 * Time.fixedDeltaTime, Space.Self);

    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        StartCoroutine("Jump");
    //    }
    //    //Rigidbody.AddForce(10 * Vector2.down);


    //    foreach (Transform tr in transform)
    //    {
    //        if (tr.name == "Ground")
    //        {
    //            RaycastHit2D[] hits = Physics2D.RaycastAll(tr.position, Vector2.down, 0.1f, mask);
    //            foreach (RaycastHit2D hit in hits)
    //            {
    //                if (grounds.Contains(hit.transform.gameObject) == false)
    //                    grounds.Add(hit.transform.gameObject);
    //            }

    //            if (grounds.Count > 0)
    //                return;

    //            return;
    //        }//foreach (Transform tr in transform)

    //    }

    //    return;
    //}



    IEnumerator Move()
    {
        //Debug.Log(13);
        //transform.Translate(Speed * Time.deltaTime * (CellPos - transform.position).normalized, Space.World);
        yield return null;
        StartCoroutine(CoSecondsUpdateFlag());

    }


    int t = 0;
    IEnumerator CoSecondsUpdateFlag()
    {

        while (t < 10)
        {
            Debug.Log("角青");
            yield return new WaitForSeconds(.1f);
            t += 1;
        }


        Debug.Log(t);
    }



    IEnumerator Jump()
    {

        Debug.Log("内风凭 矫累");
        rig2D.AddRelativeForce(Vector2.up * 10, ForceMode2D.Impulse);

        yield return new WaitForSeconds(1f);
        Debug.Log("内风凭 场");

    }



}
