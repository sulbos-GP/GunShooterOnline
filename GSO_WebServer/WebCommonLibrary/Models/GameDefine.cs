using System;
using System.Collections.Generic;
using System.Text;

namespace WebCommonLibrary.Models
{
    public static class GameDefine
    {
        public const int MAX_TICKET = 10;
        public const int WAIT_TICKET_MINUTE = 10;
        public const int WAIT_TICKET_SECOND = (60 * 1000) * WAIT_TICKET_MINUTE;
    }
}
