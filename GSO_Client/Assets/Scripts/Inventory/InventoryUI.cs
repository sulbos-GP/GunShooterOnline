using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    /*
     * 인벤토리 UI의 작동여부는 이것으로만
     */
    public bool isActive = false;

    public void invenUIControl()
    {
        isActive = !isActive;
        gameObject.SetActive(isActive);
    }
}
