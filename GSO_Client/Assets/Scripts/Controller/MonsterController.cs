using System.Collections;
using UnityEngine;

public class MonsterController : CreatureController
{
    //id
    //stat
    public bool CanAttackDirect;

    private bool canMove = true;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.transform.CompareTag("Projectile") && collision.transform.GetComponent<ArrowController>())
        //{
        //    OnDamaged(collision.transform);
        //    Debug.Log("몬스터 공격당함");

        //}
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Projectile") && collision.transform.GetComponent<ArrowController>())
        {
            OnDamaged(collision.transform);
            Debug.Log("몬스터 공격당함-");
        }
    }

    private IEnumerator CoolTimeMove()
    {
        canMove = false;
        yield return new WaitForSeconds(1f);
        canMove = true;
    }

    public override void UpdateMoving()
    {
        //Debug.Log("Cell" + CellPos);
        if (canMove == false)
            return;

        if ((CellPos - transform.position).sqrMagnitude > 10)
            transform.position = CellPos;
        else
            transform.position = Vector3.Lerp(transform.position, CellPos, Time.deltaTime * Speed);
    }
}