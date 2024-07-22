using Server.Game;
using System;
using System.Collections.Generic;
using System.Text;


public class SkillManager
{
    #region Singleton
    static SkillManager _instance = new SkillManager();
    public static SkillManager Instance { get { return _instance; } }
    #endregion

    public SkillManager()
    {
        Register();
    }
    Dictionary<int, Action<GameObject>> _handler = new Dictionary<int, Action<GameObject>>();

    public void Register()
    {
		

    }


    public void UseSkill(GameObject obj,int id)
    { 
        Action<GameObject> action = null;
        _handler.TryGetValue(id, out action);
        if(action != null)
            action.Invoke(obj);
    }


}



