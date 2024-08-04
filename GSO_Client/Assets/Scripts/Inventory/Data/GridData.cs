using Google.Protobuf.Protocol;
using JetBrains.Annotations;
using NPOI.POIFS.Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using static UnityEditor.Progress;
using Vector2 = System.Numerics.Vector2;


//[CreateAssetMenu(fileName = "gridData", menuName = "InventoryUI/gridData")]
public class GridData : MonoBehaviour
{
    /// <summary>
    /// GridDataInfo 해당 스크립트의 변수에 적용
    /// </summary>
    public void SetGridData(GridDataInfo gridDataInfo)
    {
        gridId = gridDataInfo.GridId;
        gridSize = new Vector2Int(gridDataInfo.GridSizeX, gridDataInfo.GridSizeY);
        gridPos = new Vector2(gridDataInfo.GridPosX, gridDataInfo.GridPosY);
        foreach(ItemDataInfo data in gridDataInfo.ItemList)
        {
            ItemData newItemData = null;
            newItemData.SetItemData(data);
            itemList.Add(newItemData);
        }
        createRandomItem = gridDataInfo.CreateRandomItem;
        randomItemAmount = gridDataInfo.RandomItemAmount;
    }

    /// <summary>
    /// 현재 스크립트의 변수를 GridDataInfo 변환
    /// </summary>
    public GridDataInfo GetGridData()
    {
        GridDataInfo gridDataInfo = new GridDataInfo();

        gridDataInfo.GridId = gridId;
        gridDataInfo.GridSizeX = gridSize.x;
        gridDataInfo.GridSizeY = gridSize.y;
        gridDataInfo.GridPosX = gridPos.X;
        gridDataInfo.GridPosY = gridPos.Y;
        foreach (ItemData data in itemList)
        {
            ItemDataInfo newItemDataInfo = data.GetItemData();
            gridDataInfo.ItemList.Add(newItemDataInfo);
        }
        gridDataInfo.CreateRandomItem = createRandomItem;
        gridDataInfo.RandomItemAmount = randomItemAmount;

        return gridDataInfo;
    }


    public int gridId;
    public Vector2Int gridSize;
    public Vector2 gridPos;
    public List<ItemData> itemList;

    public bool createRandomItem;
    public int randomItemAmount;
}
