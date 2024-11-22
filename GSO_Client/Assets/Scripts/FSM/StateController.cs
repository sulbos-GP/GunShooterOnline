using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MobState
{
    Idle,
    Check,
    Chase,
    Attack,
    Return,
    Stun,
    Dead
}

public interface IState
{
    public void Enter(); // state변수 초기화 및 애니메이션 실행
    public void Update(); // 매 프레임 실행 혹은 다른 상태로의 전환조건 체크
    public void Exit(); // 상태 종료 시 실행
}

public class StateController : MonoBehaviour
{
    private IState curState;

    public void ChangeState(IState newState)
    {
        curState?.Exit();
        curState = newState;
        curState.Enter();
    }

    public void Update()
    {
        curState?.Update();
    }
}
