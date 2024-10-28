using Server.Game;
using Server.Game.ItemManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ItemManager
{
    #region Singleton
    static ItemManager _instance = new ItemManager();
    public static ItemManager Instance { get { return _instance; } }
    #endregion

    public ItemManager()
    {
        Register();
    }
    Dictionary<int, Action<GameObject>> _handler = new Dictionary<int, Action<GameObject>>();

    public void Register()
    {
		_handler.Add(401, ItemHandler.HandleItem401);
		_handler.Add(402, ItemHandler.HandleItem402);
		_handler.Add(403, ItemHandler.HandleItem403);
		_handler.Add(404, ItemHandler.HandleItem404);

    }


    public void UseIteme(GameObject obj, int id)
    {
        Action<GameObject> action = null;
        _handler.TryGetValue(id, out action);
        if (action != null)
            action.Invoke(obj);
    }


}