using System;
using System.Collections.Generic;
using System.Text;

namespace WebCommonLibrary.Models.MasterDB
{
    /// <summary>
    /// 어플리케이션 (App or Data)버전 관리
    /// </summary>
    public class DB_Version
    {
        public int major { get; set; } = 0; //내부적으로 대대적인 수정이 있는 경우 (리팩토링 작업 등)
        public int minor { get; set; } = 0; //신규 기능이 추가된 경우
        public int patch { get; set; } = 0; //버그를 수정한 경우

        public override string ToString()
        {
            return $"{major}.{minor}.{patch}";
        }
    }
}
