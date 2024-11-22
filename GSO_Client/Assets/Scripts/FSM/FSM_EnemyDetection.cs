using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM_EnemyDetection : MonoBehaviour
{
    public FSM_Enemy Owner;
    
    private void Awake()
    {
        Owner = GetComponentInParent<FSM_Enemy>();
        GetComponent<CircleCollider2D>().radius = Owner.detectionRange;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Owner.target != null)
        {
            return;
        }

        if (collision.tag == "Player" && Owner.target == null)
        {
            Debug.Log("감지됨");
            Owner.target = collision.gameObject;
            Owner._state.ChangeState(Owner._check);     //idle과 return 상태에서 자동으로 check으로 전환됨
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == Owner.target)
        {
            Owner.target = null;
        }
    }
}
