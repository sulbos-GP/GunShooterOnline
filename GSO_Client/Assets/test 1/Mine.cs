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
        // 트리거 설정 및 애니메이션 시작
        sprite.color = new Color(1,1,1,1) ; //투명도 제거
        animator.ResetTrigger(name);
        animator.SetTrigger(name);

        // 상태 정보 가져오기
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        while (!stateInfo.IsName(name))
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return null; // 대기
        }

        // 애니메이션이 끝날 때까지 대기
        while (stateInfo.normalizedTime < 1.0f)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return null; // 대기
        }
        Managers.Object.Remove(id);
    }

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        sprite=  GetComponent<SpriteRenderer>();    
    }
}
