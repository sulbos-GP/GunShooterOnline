using System.Collections.Generic;

 public class ItemCoolDown
{
    
    Dictionary<int, short> _handler = new Dictionary<int, short>();

    public ItemCoolDown()
    {
        Register();
    }

    public void Register()
    {
		 _handler.Add(401, cool401);
		 _handler.Add(402, cool402);
		 _handler.Add(403, cool403);
		 _handler.Add(404, cool404);
 
    }


	short cool401= 0;
	short cool402= 0;
	short cool403= 0;
	short cool404= 0;


    public short GetCoolTime(int id)
    {
        short cool = -1;
        if(_handler.TryGetValue(id, out cool) == true)
        {
            return cool;
        }
        return cool;

    }

    public void SetCoolTime(int id, short time)
    {
        if (_handler.ContainsKey(id) == true)
            _handler[id] = time;
    }


}