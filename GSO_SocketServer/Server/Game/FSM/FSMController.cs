using Server.Game.Object;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.FSM
{
    public enum MobState
    {
        None,
        Idle,
        Round,
        Check,
        Chase,
        Attack,
        Return,
        Stun,
        Dead
    }

    public interface IState
    {
        public MobState state { get; }
        public void Enter(); // state변수 초기화 및 애니메이션 실행
        public void Update(); // 매 프레임 실행 혹은 다른 상태로의 전환조건 체크
        public void Exit(); // 상태 종료 시 실행
    }

    public class FSMController
    {
        private EnemyAI owner;
        private IState curState;

        public FSMController(EnemyAI _owner)
        {
            owner = _owner; 
        }

        public IState CurState
        {
            get 
            { 
                return curState; 
            }
        }

        public void ChangeState(IState newState)
        {
            if (owner == null)
            {
                return;
            }

            if (curState == null)
            {
                curState = owner.IdleState;
                curState.Enter();
                return;
            }

            if (curState == newState)
            {
                return;
            }

            IState oldState = curState;
            oldState.Exit();

            curState = newState;
            curState.Enter();

            Console.WriteLine($"[{owner.info.Name}={owner.Id}] {oldState.state} -->> {newState.state}상태로 전환");
        }

        public void Update()
        {
            if(curState != null)
            {
                curState.Update();
            }
        }
    }
}
