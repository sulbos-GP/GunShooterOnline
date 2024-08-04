using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class InvenData
{
    /// <summary>
    /// InvenDataInfo 해당 스크립트의 변수에 적용
    /// </summary>
    public void SetInvenData(InvenDataInfo invenDataInfo)
    {
        inventoryId = invenDataInfo.InventoryId;
        limitWeight = invenDataInfo.LimitWeight;

        foreach (GridDataInfo data in invenDataInfo.GridData)
        {
            GridData newGridData = new GridData();
            newGridData.SetGridData(data);
            gridList.Add(newGridData);
        }
    }

    /// <summary>
    /// 현재 스크립트의 변수를 InvenDataInfo 변환
    /// </summary>
    public InvenDataInfo GetInvenData()
    {
        InvenDataInfo invenDataInfo = new InvenDataInfo();
        invenDataInfo.InventoryId = inventoryId;
        invenDataInfo.LimitWeight = limitWeight;

        foreach (GridData data in gridList)
        {
            GridDataInfo newGridData = data.GetGridData();
            invenDataInfo.GridData.Add(newGridData);
        }
        return invenDataInfo;
    }

    public int inventoryId;
    public float limitWeight; //해당 인벤토리의 한계무게
    public List<GridData> gridList; //이 인벤토리에서 생성될 그리드의 데이터
}
