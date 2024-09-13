using System;
using System.Collections.Generic;
using System.Text;

namespace WebCommonLibrary.Enum
{
    public enum ETicketState
    {
        None,               //
        Join,               // WebAPI Join처리 중
        Cancle,             // WebAPI Cancle처리 중
        InQueue,            // 현재 대기열에 있는 상태
        NotInQueue,         // 현재 대기열에서 제외된 상태
        WaitingForMatch,    // 매칭중인 상태
        FindMatch,          // 매치가 잡힌 상태
    }
}
