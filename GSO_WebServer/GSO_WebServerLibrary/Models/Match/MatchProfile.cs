using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSO_WebServerLibrary.Models.Match
{
    public class MatchProfile
    {
        public string container_id { get; set; } = string.Empty;    //매칭 아이디
        public string world { get; set; } = string.Empty;           //맵 이름
        public string host_ip { get; set; } = string.Empty;         //호스트 주소
        public int container_port { get; set; } = 0;                //컨테이너 포트
        public int host_port { get; set; } = 0;                     //호스트 포트
    }
}
