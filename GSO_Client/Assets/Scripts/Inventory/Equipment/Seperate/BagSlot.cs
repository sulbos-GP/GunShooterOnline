using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bag
{
    public int itemCode;
    public int level;
    public double limitWeight;
}

public class BagSize
{
    public static Vector2Int level0 = new Vector2Int(2, 3);
    public static Vector2Int level1 = new Vector2Int(3, 4);
    public static Vector2Int level2 = new Vector2Int(5, 5);
    public static Vector2Int level3 = new Vector2Int(6, 7);
}

public class BagSlot : EquipSlot
{
    public Dictionary<int, Bag> bagDatabase;
    private void Awake()
    {
        Init();
        
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public override void Init()
    {
        slotId = 4;
        allowedItemType = ItemType.Bag;

        SetBagDatabase();
    }

    private void SetBagDatabase()
    {
        bagDatabase = new Dictionary<int, Bag>();
        
        Bag mediBag = new Bag()
        {
            itemCode = 301,
            level = 1,
            limitWeight = 10
        };

        Bag armyBag = new Bag()
        {
            itemCode = 302,
            level = 2,
            limitWeight = 15
        };

        Bag armyDoubleBag = new Bag()
        {
            itemCode = 303,
            level = 3,
            limitWeight = 30
        };

        bagDatabase.Add(mediBag.itemCode, mediBag);
        bagDatabase.Add(armyBag.itemCode, armyBag);
        bagDatabase.Add(armyDoubleBag.itemCode, armyDoubleBag);
    }

    

    public override bool ApplyItemEffects(ItemData item)
    {
        base.ApplyItemEffects(item);
        PlayerInventoryUI playerUI = InventoryController.invenInstance.playerInvenUI;
        //가방일경우 그리드의 데이터 변경
        // 레벨이 커질경우 -> 크기 증가
        // 레벨이 낮아질경우 -> 크기를 줄일수 있는지(아이템칸에 겹치는지) 검사후 가능하면 감소
        // 가방 크기 0 : 2*3 , 1 : 3*4 , 2 : 5*5 , 3 : 7*6
        Debug.Log($"가방 : {item.item_name} 장착");

        //아이템의 데이터 아이디를 통해 가방의 상세 데이터를 불러옴
        Bag targetBag = new Bag();
        bool isSuccess = bagDatabase.TryGetValue(item.itemId, out targetBag);

        if (!isSuccess)
        {
            Debug.Log("해당 아이디의 가방이 존재하지 않음");
            return false;
        }

        //현재 가방의 레벨보다 적용할 가방의 레벨이 높으면 증가
        //가방을 변화시킬때의 요소 플레이어의 그리드의 사이즈와 무게를 바꿀수 있는지 체크후 가능하면 교체후 아이템 재배치

        if (playerUI.bagLv == targetBag.level)
        {
            //레벨이 같은 경우 변화 없음
            return true;
        }
        else
        {
            if(playerUI.instantGrid != null) //인벤토리가 열려있는 상태에서 변화
            {
                Vector2Int newSize = ChangeLevelToVector(targetBag.level);

                if (playerUI.bagLv > targetBag.level)
                {
                    isSuccess = playerUI.instantGrid.CheckAvailableToChange(newSize);
                    if (!isSuccess || targetBag.limitWeight < playerUI.instantGrid.GridWeight)
                    {
                        Debug.Log("해당 가방에 놓을수 없음");
                        return false;
                    }
                }

                playerUI.bagLv = targetBag.level;
                playerUI.instantGrid.limitWeight = targetBag.limitWeight;
                //가방 레벨이 크거나 테스트 통과시 그리드의 사이즈 업데이트
                playerUI.instantGrid.UpdateGridObject(newSize, targetBag.limitWeight);
            }
            else //최초에 스폰되어 장비를 불러올때(인벤토리가 열려있지 않음)
            {
                playerUI.bagLv = targetBag.level;
            }
            
        }

        return true;
    }

    private Vector2Int ChangeLevelToVector(int level)
    {
        switch (level)
        {
            case 0: return BagSize.level0;
            case 1: return BagSize.level1;
            case 2: return BagSize.level2;
            case 3: return BagSize.level3;
            default: Debug.Log($"잘못된 레벨 : {level}"); return new Vector2Int(0,0);
        }
    }

    public override bool RemoveItemEffects(ItemData item)
    {
        base.RemoveItemEffects(item); //무조건 인벤토리가 열린 상태에서만 호출됨

        Debug.Log($"가방 아이템 해제");
        PlayerInventoryUI playerUI = InventoryController.invenInstance.playerInvenUI;

        Vector2Int newSize = ChangeLevelToVector(0);

        bool isSuccess = playerUI.instantGrid.CheckAvailableToChange(newSize);
        if (!isSuccess ||playerUI.instantGrid.GridWeight > 5)
        {
            Debug.Log("해당 가방을 해제 할수 없음");
            return false;
        }
        playerUI.bagLv = 0;
        playerUI.instantGrid.UpdateGridObject(newSize, 5);
        return true;
    }
}
