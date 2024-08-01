using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static UnityEditor.Progress;


public class ServerGrid
{
    public static int CreateItemId = 1;

    public GridDataInfo gridData; 
    public ServerInventory ownerInventory; //�׸��带 ������ �κ��丮 (�׳� �κ��丮 ���̵�� �ٲ㵵 ����)
    public int gridId;
    public int[,] gridSlot; //�׸��忡 ä���� �������� �ڵ�

    public float GridWeight
    {
        get => gridWeight;
        set
        {
            gridWeight = value;
        }
    }
    private float gridWeight;

    private void Init()
    {
        gridSlot = new int[gridData.GridSizeX,gridData.GridSizeY];

        if (gridData.ItemList.Count == 0)
        {
            //�������� ����ִٸ� ������ �������� �����ؾ��ϴ��� üũ
            if (gridData.CreateRandomItem)
            {
                for (int i = 0; i < gridData.RandomItemAmount; i++)
                {
                    gridData.ItemList.Add(CreateItemInfo());
                }
                gridData.CreateRandomItem = false;
            }
        }

        foreach (ItemDataInfo items in gridData.ItemList)
        {
            PushItemIntoSlot(items);
        }

    }

    //������ �������� ������ ����. 
    private ItemDataInfo CreateItemInfo()
    {
        //���ο� �������� �����Ͽ� ������ ����Ʈ�� �߰�
        ItemDataInfo newItemInfo = new ItemDataInfo();

        //������ �������� �ڵ带 �̾Ƽ� �� �ڵ�� �����ͺ��̽� ��ȸ�Ͽ� �������� Info �������ְ� ��ȯ
        System.Random rnd = new System.Random();
        int randomCode = rnd.Next(1,6); //1~5������ ������ �ڵ带 ��ȯ
        newItemInfo.ItemId = CreateItemId;
        CreateItemId++;
        newItemInfo.ItemCode = randomCode;

        

        newItemInfo.ItemAmount = 1;
        //newItemInfo.SearchedPlayerId = �̰� new�� �Ҵ�������ϳ�?;



        return newItemInfo;

    }

    private bool ItemPushCheck(ItemDataInfo item, int posX, int posY)
    {
        for (int x = 0; x < item.Width; x++)
        {
            for (int y = 0; y < item.Height; y++)
            {
                //�������� �ش� �ڸ��� ���� �ִ��� üũ. 0(null)�̸� ���� ����
                if(gridSlot[posX + x, posY + y] != 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void PushItemIntoSlot(ItemDataInfo item)
    {
        for (int x = 0; x < item.Width; x++)
        {
            for (int y = 0; y < item.Height; y++)
            {
                //�������� ��ġ�� �������� ũ�� ��ŭ ���Կ� ������ �ڵ�� ä��
                gridSlot[item.ItemPosX + x, item.ItemPosX + y] = item.ItemCode;
            }
        }
    }

    public void PrintInvenContents(int[,] gridSlot)
    {
        string content = gridId+ "\n";

        for (int i = 0; i < gridSlot.GetLength(1); i++)
        {
            for (int j = 0; j < gridSlot.GetLength(0); j++)
            {
                int itemCode = gridSlot[j, i];
                if (itemCode !=  0)
                {
                    content += $"| {itemCode} |";
                }
                else
                {
                    content += $"| Null |";
                }
            }
            content += "\n";
        }

        Debug.Print(content);
    }
}
