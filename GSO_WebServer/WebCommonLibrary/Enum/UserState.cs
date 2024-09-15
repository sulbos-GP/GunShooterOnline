
namespace WebCommonLibrary.Enum
{
    public enum EUserState
    {
        None,               //초기 상태

        Connect,            // 연결중 상태
        Pause,              // 백그라운드 상태
        Disconnect,         // 정상적인 종료중 상태

        Matching,           // 매칭큐에 들어가 있는 상태
        InGame,             // 게임 진행중인 상태
    }
}
