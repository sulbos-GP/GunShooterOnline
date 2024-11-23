using Org.BouncyCastle.Crypto.Tls;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Mine : MonoBehaviour
{
    private Animator animator;
    public void Explosion(int id)
    {
        StartCoroutine(Explo("explosion"));
        Managers.Object.Remove(id);
        Destroy(gameObject);
    }

    IEnumerator Explo(string name)
    {
        // Ʈ���� ���� �� �ִϸ��̼� ����
        animator.ResetTrigger(name);
        animator.SetTrigger(name);

        // ���� ���� ��������
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // ���°� ������Ʈ�ǵ��� �� ������ ���
        yield return null;
        // �ִϸ��̼� ���� �˻�
        while (stateInfo.normalizedTime < 1.0f)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0); // ���� ������Ʈ
            yield return null; // ���� �����ӱ��� ���
        }

        Destroy(gameObject);

        // Destroy Ȯ��
        yield return null;
    }

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }
}
