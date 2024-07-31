using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    /*
     * �κ��丮 UI�� �۵����δ� �̰����θ�
     */
    public bool isActive = false;

    private void Awake()
    {
        isActive = gameObject.activeSelf;
    }

    public void invenUIControl()
    {
        isActive = !isActive;
        gameObject.SetActive(isActive);
    }
}
