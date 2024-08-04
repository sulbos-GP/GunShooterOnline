using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class InvenData
{
    /// <summary>
    /// InvenDataInfo �ش� ��ũ��Ʈ�� ������ ����
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
    /// ���� ��ũ��Ʈ�� ������ InvenDataInfo ��ȯ
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
    public float limitWeight; //�ش� �κ��丮�� �Ѱ蹫��
    public List<GridData> gridList; //�� �κ��丮���� ������ �׸����� ������
}
