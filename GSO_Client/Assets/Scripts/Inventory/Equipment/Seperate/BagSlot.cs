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
        //�����ϰ�� �׸����� ������ ����
        // ������ Ŀ����� -> ũ�� ����
        // ������ ��������� -> ũ�⸦ ���ϼ� �ִ���(������ĭ�� ��ġ����) �˻��� �����ϸ� ����
        // ���� ũ�� 0 : 2*3 , 1 : 3*4 , 2 : 5*5 , 3 : 7*6
        Debug.Log($"���� : {item.item_name} ����");

        //�������� ������ ���̵� ���� ������ �� �����͸� �ҷ���
        Bag targetBag = new Bag();
        bool isSuccess = bagDatabase.TryGetValue(item.itemId, out targetBag);

        if (!isSuccess)
        {
            Debug.Log("�ش� ���̵��� ������ �������� ����");
            return false;
        }

        //���� ������ �������� ������ ������ ������ ������ ����
        //������ ��ȭ��ų���� ��� �÷��̾��� �׸����� ������� ���Ը� �ٲܼ� �ִ��� üũ�� �����ϸ� ��ü�� ������ ���ġ

        if (playerUI.bagLv == targetBag.level)
        {
            //������ ���� ��� ��ȭ ����
            return true;
        }
        else
        {
            if(playerUI.instantGrid != null) //�κ��丮�� �����ִ� ���¿��� ��ȭ
            {
                Vector2Int newSize = ChangeLevelToVector(targetBag.level);

                if (playerUI.bagLv > targetBag.level)
                {
                    isSuccess = playerUI.instantGrid.CheckAvailableToChange(newSize);
                    if (!isSuccess || targetBag.limitWeight < playerUI.instantGrid.GridWeight)
                    {
                        Debug.Log("�ش� ���濡 ������ ����");
                        return false;
                    }
                }

                playerUI.bagLv = targetBag.level;
                playerUI.instantGrid.limitWeight = targetBag.limitWeight;
                //���� ������ ũ�ų� �׽�Ʈ ����� �׸����� ������ ������Ʈ
                playerUI.instantGrid.UpdateGridObject(newSize, targetBag.limitWeight);
            }
            else //���ʿ� �����Ǿ� ��� �ҷ��ö�(�κ��丮�� �������� ����)
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
            default: Debug.Log($"�߸��� ���� : {level}"); return new Vector2Int(0,0);
        }
    }

    public override bool RemoveItemEffects(ItemData item)
    {
        base.RemoveItemEffects(item); //������ �κ��丮�� ���� ���¿����� ȣ���

        Debug.Log($"���� ������ ����");
        PlayerInventoryUI playerUI = InventoryController.invenInstance.playerInvenUI;

        Vector2Int newSize = ChangeLevelToVector(0);

        bool isSuccess = playerUI.instantGrid.CheckAvailableToChange(newSize);
        if (!isSuccess ||playerUI.instantGrid.GridWeight > 5)
        {
            Debug.Log("�ش� ������ ���� �Ҽ� ����");
            return false;
        }
        playerUI.bagLv = 0;
        playerUI.instantGrid.UpdateGridObject(newSize, 5);
        return true;
    }
}
