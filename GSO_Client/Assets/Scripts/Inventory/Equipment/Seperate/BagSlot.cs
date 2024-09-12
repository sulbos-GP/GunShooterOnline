using Google.Protobuf.Protocol;
using Org.BouncyCastle.Asn1.Crmf;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagSlot : EquipSlot
{
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
    }

    public override bool ApplyItemEffects(ItemData item)
    {
        base.ApplyItemEffects(item);

        // 레벨이 커질경우 -> 크기 증가
        // 레벨이 낮아질경우 -> 크기를 줄일수 있는지(아이템칸에 겹치는지) 검사후 가능하면 감소

        PlayerInventoryUI playerUI = InventoryController.invenInstance.playerInvenUI;
        Debug.Log($"가방 : {item.item_name} 장착");

        //아이템의 데이터 아이디를 통해 가방의 상세 데이터를 불러옴
        BagData targetBag = new BagData();
        bool isSuccess = BagDB.bagDB.TryGetValue(item.itemId, out targetBag);
        if (!isSuccess)
        {
            Debug.Log("해당 아이디의 가방이 존재하지 않음");
            return false;
        }

        

        if (playerUI.instantGrid != null) //인벤토리가 열려있는 상태에서 변화
        {
            if (playerUI.equipBag.item_id == targetBag.item_id) //같은 종류의 가방이라면 변화 없이 성공 처리
            {
                return true;
            }

            Vector2Int newSize = new Vector2Int(targetBag.total_scale_x, targetBag.total_scale_y);

            if (playerUI.equipBag.total_scale_x > newSize.x || playerUI.equipBag.total_scale_y > newSize.y)
            {
                isSuccess = playerUI.instantGrid.CheckAvailableToChange(newSize);
                if (!isSuccess || targetBag.total_weight < playerUI.instantGrid.GridWeight)
                {
                    Debug.Log("해당 가방에 놓을수 없음");
                    return false;
                }
            }

            playerUI.equipBag = targetBag;
            //가방 레벨이 크거나 테스트 통과시 그리드의 사이즈 업데이트
            playerUI.instantGrid.UpdateGridObject(newSize, targetBag.total_weight);
        }
        else //최초에 스폰되어 장비를 불러올때(인벤토리가 열려있지 않음)
        {
            playerUI.equipBag = targetBag;
        }

        return true;
    }

    public override bool RemoveItemEffects(ItemData item)
    {
        base.RemoveItemEffects(item); //무조건 인벤토리가 열린 상태에서만 호출됨

        Debug.Log($"가방 아이템 해제");
        PlayerInventoryUI playerUI = InventoryController.invenInstance.playerInvenUI;

        BagData basicBag = new BagData();
        bool isSuccess = BagDB.bagDB.TryGetValue(-1,out basicBag);
        if (!isSuccess )
        {
            Debug.Log("기본 가방 데이터를 찾지 못함");
            return false;
        }

        if (playerUI.instantGrid.GridWeight > 5)
        {
            Debug.Log("해당 가방을 해제 할수 없음");
            return false;
        }

        playerUI.equipBag = basicBag;
        Vector2Int basicSize = new Vector2Int(basicBag.total_scale_x, basicBag.total_scale_y);
        playerUI.instantGrid.UpdateGridObject(basicSize, basicBag.total_weight);
        return true;
    }
}
