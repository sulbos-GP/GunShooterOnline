using WebCommonLibrary.Enum;

namespace WebCommonLibrary.Models.Match
{

    public class MatchStatus
    {
        public string       name {  get; set; } = string.Empty;             // 컨테이너 이름
        public string       world { get; set; } = string.Empty;             // 맵 이름
        public EMatchState state {  get; set; } = EMatchState.None;        // 상태
        public string       host_ip { get; set; } = string.Empty;           // 서버 아이피
        public int          container_port {  get; set; } = 0;              // 컨테이너 포트
        public int          host_port { get; set; } = 0;                    // 게임 서버 포트
        public int          retries { get; set; } = 0;                      // Health 체크 타임 아웃 횟수
        public long         age { get; set; } = 0;                          // 컨테이너가 생성되고 난 이후 시간
    }
}
