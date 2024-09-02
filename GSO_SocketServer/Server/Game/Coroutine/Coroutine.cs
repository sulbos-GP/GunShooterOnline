using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class Coroutine
    {
        private IEnumerator routine;
        private DateTime? waitUntil; // 코루틴이 다시 실행되기까지 대기해야 할 시간

        public bool IsRunning { get; private set; }

        public Coroutine(IEnumerator routine)
        {
            this.routine = routine;
            IsRunning = true;
        }

        public bool MoveNext()
        {
            if (routine == null || !IsRunning) return false;

            // 대기 상태 체크
            if (waitUntil.HasValue)
            {
                if (DateTime.Now < waitUntil.Value)
                {
                    return true; // 아직 대기 중이므로 true 반환
                }
                else
                {
                    waitUntil = null; // 대기 종료
                }
            }

            if (routine.MoveNext())
            {
                if (routine.Current is WaitForSeconds wait)
                {
                    // 대기 시간 설정
                    waitUntil = DateTime.Now.AddSeconds(wait.Seconds);
                }
                else if (routine.Current is IEnumerator nestedRoutine)
                {
                    // 중첩된 코루틴 처리
                    if (new Coroutine(nestedRoutine).MoveNext())
                    {
                        return true;
                    }
                }
                return true;
            }
            else
            {
                IsRunning = false;
                return false;
            }
        }
    }

    // WaitForSeconds는 단순히 대기 시간을 전달하기 위한 클래스
    public class WaitForSeconds
    {
        public float Seconds { get; }

        public WaitForSeconds(float seconds)
        {
            Seconds = seconds;
        }
    }
}
