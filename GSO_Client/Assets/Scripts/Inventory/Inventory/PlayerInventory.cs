using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : Inventory
{
    //슬롯이 나오면 그때 개발
    public int bagLv = 0;

    private void Awake()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
    }

    public void ChangeInventory()
    {
        //가방슬롯을 바꿨을때 인벤토리 내의 그리드의 크기와 아이템 슬롯의 크기를 증가

    }
}
