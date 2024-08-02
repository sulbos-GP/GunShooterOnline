using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using UnityEditor.Build;
using UnityEngine.PlayerLoop;


public class ServerItem
{
    public const int maxItemMergeAmount = 64;
    public ItemDataInfo itemDataInfo; //������(������ �ڵ�, �̸�, ��ȸ�ð�, ũ�� , �̹���)
    public int ItemRotate
    {
        get => itemDataInfo.ItemRotate;
        set
        {
            itemDataInfo.ItemRotate = value;
            itemDataInfo.Width = Width;
            itemDataInfo.Height = Height;
        }
    }
    //ȸ�����¿� ���� �ʺ�� ���̸� ������ �̰ɷ�
    public int Width
    {
        get
        {
            if (itemDataInfo.ItemRotate % 2 == 0)
            {
                return itemDataInfo.Width;
            }
            return itemDataInfo.Height;
        }
    }
    public int Height
    {
        get
        {
            if (itemDataInfo.ItemRotate % 2 == 0)
            {
                return itemDataInfo.Height;
            }
            return itemDataInfo.Width;
        }
    }

    private void Init()
    {
        if(itemDataInfo == null)
        {
            //�ش� ������Ʈ���� ������ ������ info�� �־����
            return;
        }
    }

    /// <summary>
    /// ���� �������ڵ��� �������� ���� ��ġ�� �ξ������ 
    /// targetItem = ������ ��ġ�� ���� ��ġ�� �����ϴ� ���� �ڵ��� ������
    /// ���� �����۰� Ÿ�پ������� ������ ��ģ���� �ִ��� 64�� ���� ������ -> 64-Ÿ�پ������� ���� Ÿ�ٿ� ���ϰ� ��������ۿ� �� -> ���� �������� ����ġ��
    /// ���ų� ������ Ÿ�پ������� ������ ���� �������� ������ ���ϰ� ���� �������� ����
    /// </summary>
    public void MergeItem(ServerItem targetItem)
    {
        //���� ������Ʈ�� ��ġ���� ������Ʈ�� ������
        if (itemDataInfo.ItemCode != targetItem.itemDataInfo.ItemCode)
        {
            return;
        }

        if(itemDataInfo.ItemAmount+ targetItem.itemDataInfo.ItemAmount > maxItemMergeAmount)
        {   
            //�ΰ��� �������� ��ģ ������ 64������ Ŭ��� Ÿ�� �������� 64���� ����� ���� �������� ���� ������ ������ ���� ��ġ�� ��ȯ

            //Ÿ�� �������� ������ �ִ밡 �Ǳ����� ���� ����
            int indexAmount = maxItemMergeAmount - targetItem.itemDataInfo.ItemAmount;

            targetItem.itemDataInfo.ItemAmount += indexAmount;
            itemDataInfo.ItemAmount -= indexAmount;
            return;
        }
        else
        {
            targetItem.itemDataInfo.ItemAmount += itemDataInfo.ItemAmount;
      
            DestroyItem();
        }
    }

    public void DestroyItem()
    {
        //�ش� ������ ������Ʈ ����
        
    }

    
}
