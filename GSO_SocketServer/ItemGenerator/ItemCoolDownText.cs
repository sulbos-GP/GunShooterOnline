using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseGenerator
{
    internal class ItemCoolDownText
    {
        // {0} handler.add
        // {1} Item Delclaration
        public static String context = @"using System.Collections.Generic;

 public class ItemCoolDown
{{
    #region Singleton
    static ItemCoolDown _instance = new ItemCoolDown();
    public static ItemCoolDown Instance {{ get {{ return _instance; }} }}
  #endregion
Dictionary<int, short> _handler = new Dictionary<int, short>();

    public ItemCoolDown()
    {{
        Register();
    }}

    public void Register()
    {{
{0} 
    }}


{1}

    public short GetCoolTime(int id)
    {{
        short cool = -1;
        if(_handler.TryGetValue(id, out cool) == true)
        {{
            return cool;
        }}
        return cool;

    }}

    public void SetCoolTime(int id, short time)
    {{
        if (_handler.ContainsKey(id) == true)
            _handler[id] = time;
    }}


}}";
    }
}
