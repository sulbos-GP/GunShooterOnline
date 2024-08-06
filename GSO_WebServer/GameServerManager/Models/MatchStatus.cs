namespace GameServerManager.Models
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

    public class MatchStatus
    {
        public string       name {  get; set; } = string.Empty;             // 컨테이너 이름
        public string       world { get; set; } = string.Empty;             // 맵 이름
        public EMatchState  state {  get; set; } = EMatchState.None;        // 상태
        public string       host_ip { get; set; } = string.Empty;           // 서버 아이피
        public int          container_port {  get; set; } = 0;              // 컨테이너 포트
        public int          host_port { get; set; } = 0;                    // 게임 서버 포트
        public int          retries { get; set; } = 0;                      // Health 체크 타임 아웃 횟수
        public long         age { get; set; } = 0;                          // 컨테이너가 생성되고 난 이후 시간
    }
}
