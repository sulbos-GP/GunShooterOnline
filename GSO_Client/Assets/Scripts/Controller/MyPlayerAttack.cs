using System.Collections.Generic;
using UnityEngine;

public class MyPlayerAttack : MonoBehaviour
{
    [SerializeField] private List<BaseController> _attackableList = new();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Monster"))
        {
            var bc = collision.GetComponent<BaseController>();
            if (_attackableList.Contains(bc) == false)
                _attackableList.Add(bc);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Monster"))
        {
            var bc = collision.GetComponent<BaseController>();
            if (_attackableList.Contains(bc))
                _attackableList.Remove(bc);
        }
    }

    public List<BaseController> GetAttackableList()
    {
        return _attackableList;
    }
}