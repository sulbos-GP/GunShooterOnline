using System;
using System.Collections.Generic;
using System.Text;

namespace WebCommonLibrary.Model.GameDB
{
    public class ClientCredential
    {
        public int uid { get; set; } = 0;

        public string access_token { get; set; } = string.Empty;

        public long expires_in { get; set; } = 0;

        public string scope { get; set; } = string.Empty;

        public string token_type { get; set; } = string.Empty;
    }
}
