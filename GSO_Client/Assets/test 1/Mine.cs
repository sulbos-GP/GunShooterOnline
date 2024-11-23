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
        // 트리거 설정 및 애니메이션 시작
        animator.ResetTrigger(name);
        animator.SetTrigger(name);

        // 상태 정보 가져오기
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // 상태가 업데이트되도록 한 프레임 대기
        yield return null;
        // 애니메이션 상태 검사
        while (stateInfo.normalizedTime < 1.0f)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 상태 업데이트
            yield return null; // 다음 프레임까지 대기
        }

        Destroy(gameObject);

        // Destroy 확인
        yield return null;
    }

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }
}
