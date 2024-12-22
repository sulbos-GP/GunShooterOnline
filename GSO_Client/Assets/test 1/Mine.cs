using Org.BouncyCastle.Crypto.Tls;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer sprite;

    public void Explosion(int id)
    {
        StartCoroutine(Explo("explosion", id));
    }

    IEnumerator Explo(string name, int id)
    {
        // Ʈ���� ���� �� �ִϸ��̼� ����
        sprite.color = new Color(1,1,1,1) ; //���� ����
        animator.ResetTrigger(name);
        animator.SetTrigger(name);
        AudioManager.instance.PlaySound("Explosion",gameObject.GetComponent<AudioSource>());

        // ���� ���� ��������
        //AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //while (!stateInfo.IsName(name))
        //{
        //    stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        //    yield return null; // ���
        //}

        //// �ִϸ��̼��� ���� ������ ���
        //while (stateInfo.normalizedTime < 1.0f)
        //{
        //    stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        //    yield return null; // ���
        //}

        sprite.enabled = false;
        while (gameObject.GetComponent<AudioSource>().isPlaying)
        {
            yield return null;
        }
        Managers.Object.Remove(id);
    }

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        sprite=  GetComponent<SpriteRenderer>();    
    }
}
