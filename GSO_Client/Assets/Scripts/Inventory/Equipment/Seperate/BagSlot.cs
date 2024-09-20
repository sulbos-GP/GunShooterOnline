using Google.Protobuf.Protocol;
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

        PlayerInventoryUI playerUI = InventoryController.invenInstance.playerInvenUI;
        Debug.Log($"가방 : {item.item_name} 장착");

        // 가방 데이터 불러오기
        if (!TryGetBagData(item.itemId, out BagData targetBag))
        {
            return false;
        }

        // 인벤토리가 열려 있는 상태에서 가방 장착
        if (playerUI.instantGrid != null)
        {
            return EquipBagInOpenInventory(playerUI, targetBag);
        }

        // 최초 스폰 시 가방 장착
        playerUI.equipBag = targetBag;
        return true;
    }

    public override bool RemoveItemEffects(ItemData item)
    {
        base.RemoveItemEffects(item);
        Debug.Log($"가방 아이템 해제");

        PlayerInventoryUI playerUI = InventoryController.invenInstance.playerInvenUI;

        // 기본 가방 데이터 불러오기
        if (!TryGetBagData(-1, out BagData basicBag))
        {
            return false;
        }

        // 기본 가방 장착
        return EquipBagInOpenInventory(playerUI, basicBag);
    }

    //인벤토리가 열려있을때 가방 교체 로직
    private bool EquipBagInOpenInventory(PlayerInventoryUI playerUI, BagData targetBag)
    {
        if (IsSameBag(playerUI, targetBag)) // 같은 가방이면 변화 없이 성공
        {
            return true;
        }

        Vector2Int newSize = new Vector2Int(targetBag.total_scale_x, targetBag.total_scale_y);

        //가방을 빼거나 교체가 가능한지 판단
        if (IsBagSizeReducing(playerUI.equipBag, newSize) &&
            (!playerUI.instantGrid.CheckAvailableToChange(newSize) || !CompareChangeWeight(playerUI, targetBag)))
        {
            Debug.Log("해당 가방에 놓을 수 없음");
            return false;
        }

        playerUI.equipBag = targetBag;
        playerUI.instantGrid.UpdateGridObject(newSize, targetBag.total_weight);
        return true;
    }

    // 바꿀 가방의 무게가 현재 가지고 있는 아이템들의 무게를 감당가능한가
    private static bool CompareChangeWeight(PlayerInventoryUI playerUI, BagData targetBag)
    {
        if (targetBag.total_weight < playerUI.instantGrid.GridWeight)
        {
            playerUI.StartBlink();
            return false;
        }
        return true;
    }


    //가방 db에서 해당 아이디로 검색. 존재하면 true
    private bool TryGetBagData(int itemId, out BagData bagData)
    {
        if (!BagDB.bagDB.TryGetValue(itemId, out bagData))
        {
            Debug.Log($"해당 아이디({itemId})의 가방이 존재하지 않음");
            return false;
        }
        return true;
    }

    // 현재 장착한 가방과 바꿀 가방이 같으면 true
    private bool IsSameBag(PlayerInventoryUI playerUI, BagData targetBag)
    {
        return playerUI.equipBag.item_id == targetBag.item_id;
    }

    // 가방의 사이즈가 줄어들면 true
    private bool IsBagSizeReducing(BagData currentBag, Vector2Int newSize)
    {
        return currentBag.total_scale_x > newSize.x || currentBag.total_scale_y > newSize.y;
    }
}
