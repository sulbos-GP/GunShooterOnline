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
        Debug.Log($"���� : {item.item_name} ����");

        // ���� ������ �ҷ�����
        if (!TryGetBagData(item.itemId, out Data_master_item_backpack targetBag))
        {
            return false;
        }

        // �κ��丮�� ���� �ִ� ���¿��� ���� ����
        if (playerUI.instantGrid != null)
        {
            return EquipBagInOpenInventory(playerUI, targetBag);
        }

        // ���� ���� �� ���� ����
        playerUI.equipBag = targetBag;
        return true;
    }

    public override bool RemoveItemEffects(ItemData item)
    {
        base.RemoveItemEffects(item);
        Debug.Log($"���� ������ ����");

        PlayerInventoryUI playerUI = InventoryController.invenInstance.playerInvenUI;

        // �⺻ ���� ������ �ҷ�����
        if (!TryGetBagData(-1, out Data_master_item_backpack basicBag))
        {
            return false;
        }

        // �⺻ ���� ����
        return EquipBagInOpenInventory(playerUI, basicBag);
    }

    //�κ��丮�� ���������� ���� ��ü ����
    private bool EquipBagInOpenInventory(PlayerInventoryUI playerUI, Data_master_item_backpack targetBag)
    {
        if (IsSameBag(playerUI, targetBag)) // ���� �����̸� ��ȭ ���� ����
        {
            return true;
        }

        Vector2Int newSize = new Vector2Int(targetBag.total_scale_x, targetBag.total_scale_y);

        //������ ���ų� ��ü�� �������� �Ǵ�
        if (IsBagSizeReducing(playerUI.equipBag, newSize) &&
            (!playerUI.instantGrid.CheckAvailableToChange(newSize) || !CompareChangeWeight(playerUI, targetBag)))
        {
            Debug.Log("�ش� ���濡 ���� �� ����");
            return false;
        }

        playerUI.equipBag = targetBag;
        playerUI.instantGrid.UpdateGridObject(newSize, targetBag.total_weight);
        return true;
    }

    // �ٲ� ������ ���԰� ���� ������ �ִ� �����۵��� ���Ը� ���簡���Ѱ�
    private static bool CompareChangeWeight(PlayerInventoryUI playerUI, Data_master_item_backpack targetBag)
    {
        if (targetBag.total_weight < playerUI.instantGrid.GridWeight)
        {
            playerUI.StartBlink();
            return false;
        }
        return true;
    }


    //���� db���� �ش� ���̵�� �˻�. �����ϸ� true
    private bool TryGetBagData(int itemId, out Data_master_item_backpack bagData)
    {
        if(itemId == -1)
        {
            Data_master_item_backpack noBag = new Data_master_item_backpack()
            {
                Key = -1,
                total_scale_x = 3,
                total_scale_y = 2,
                total_weight = 5,
            };
            bagData = noBag;
            return true;
        }

        bagData = Data_master_item_backpack.GetData(itemId);
        if (bagData == null) 
        {
            return false;
        }

        return true;
    }

    // ���� ������ ����� �ٲ� ������ ������ true
    private bool IsSameBag(PlayerInventoryUI playerUI, Data_master_item_backpack targetBag)
    {
        return playerUI.equipBag.Key == targetBag.Key;
    }

    // ������ ����� �پ��� true
    private bool IsBagSizeReducing(Data_master_item_backpack currentBag, Vector2Int newSize)
    {
        return currentBag.total_scale_x > newSize.x || currentBag.total_scale_y > newSize.y;
    }
}
