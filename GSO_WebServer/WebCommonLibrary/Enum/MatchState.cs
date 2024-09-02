using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCommonLibrary.Enum
{
    public enum EMatchState
    {
        None = 0,       //
        PortAllocation, // 사용가능한 포트 할당중
        Creating,       // 생성중인 상태
        Error,          // 컨테이너 생성 중 에러 발생
        Starting,       // 컨테이너 생성 되었을 경우
        Scheduled,      // 게임 서버 준비중
        Reserved,       // (X)
        RequestReady,   // 준비가 되었다고 알린다
        Ready,          // 게임이 가능한 준비 상태
        Allocated,      // 게임이 이미 할당되었다
        Shutdown,       // 게임 서버가 종료됨
        UnHealthy,      // 게임 서버와 통신이 안될 경우
    }
}
