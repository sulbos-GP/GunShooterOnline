using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSO_WebServerLibrary
{
    public enum EUserState
    {
        Connect,            //연결중 상태
        Disconnect,         //정상적인 종료중 상태
        Pause,              //애플리케이션에서 잠시 뒤로가서 일시정지된 상태
        Quit,               //게임을 갑자기 끈어버린 상태
        Ready,              //다른 행동을 할 수 있는 준비된 상태
        MatchJoin,          //매칭큐에 들어가 있는 상태
        MatchCancle,        //매칭큐 취소하는 상태
        Game,               //게임 진행중
    }
}
