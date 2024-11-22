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
    public void Enter(); // state���� �ʱ�ȭ �� �ִϸ��̼� ����
    public void Update(); // �� ������ ���� Ȥ�� �ٸ� ���·��� ��ȯ���� üũ
    public void Exit(); // ���� ���� �� ����
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
