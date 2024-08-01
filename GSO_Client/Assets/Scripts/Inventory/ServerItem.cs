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
    public ItemDataInfo itemDataInfo; //데이터(아이템 코드, 이름, 조회시간, 크기 , 이미지)
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
    //회전상태에 따른 너비와 높이를 보려면 이걸로
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
            //해당 오브젝트에는 아이템 데이터 info가 있어야함
            return;
        }
    }

    /// <summary>
    /// 같은 아이템코드의 아이템을 같은 위치에 두었을경우 
    /// targetItem = 아이템 배치에 놓은 위치에 존재하는 같은 코드의 아이템
    /// 현재 아이템과 타겟아이템의 개수를 합친것이 최대인 64개 보다 높으면 -> 64-타겟아이템의 양을 타겟에 더하고 현재아이템에 뺌 -> 현재 아이템을 원위치로
    /// 같거나 낮으면 타겟아이템의 개수에 현재 아이템의 개수를 더하고 현재 아이템은 없앰
    /// </summary>
    public void MergeItem(ServerItem targetItem)
    {
        //현재 오브젝트와 합치려는 오브젝트와 같으면
        if (itemDataInfo.ItemCode != targetItem.itemDataInfo.ItemCode)
        {
            return;
        }

        if(itemDataInfo.ItemAmount+ targetItem.itemDataInfo.ItemAmount > maxItemMergeAmount)
        {   
            //두개의 아이템을 합친 개수가 64개보다 클경우 타겟 아이템은 64개로 만들고 기존 아이템은 남은 개수를 가지고 원래 위치로 귀환

            //타깃 아이템의 개수가 최대가 되기위해 남은 개수
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
        //해당 아이템 오브젝트 삭제
        
    }

    
}
