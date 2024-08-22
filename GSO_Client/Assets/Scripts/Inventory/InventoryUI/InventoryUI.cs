using Doozy.Runtime.Common.Extensions;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    /*
     * �κ��丮 UI�� �ڽİ�ü�� ������Ʈ
     * �����κ��� ���� �κ��丮 �����͸� ���� �κ��丮�� ������
     */
    public GameObject gridPref; //�׸����� ������
    public InvenData invenData; //������ ���� ���� �ش� �κ��丮�� ��ġ�� ũ�� �� ������ �׸����� ������
    public List<InventoryGrid> instantGridList; // ������ �κ��丮�� �׸��� ���
    
    private Transform grids; //�׸������ �θ� ������Ʈ

    //�׸���� �������� ������
    public const float offsetX = 20;
    public const float offsetY = 20;

    public float invenWeight = 0; //�׸����� ���� �� ���.

    protected virtual float InvenWeight
    {
        get { return invenWeight; }
        set { invenWeight = value; }
    }

    protected virtual void Awake()
    {
        instantGridList = new List<InventoryGrid>();
    }

    public virtual void InventorySet()
    {
        grids = transform.GetChildByName("Grids");
        
        if (invenData == null ) 
        {
            Debug.Log("�κ��丮 ������������ �κ������Ͱ� ����");
            return; 
        }

        //�κ��丮 �����͸� ������� ���� ������Ʈ
        InvenDataSet();

        //�÷��� �κ��丮�� ����.
        //if (invenData.isFloatInven)
        //{
        //    AdjustInventorySize();
        //}
    }

    /// <summary>
    /// �κ��丮�� �׸��嵥���͵�� �׸��带 ����
    /// </summary>
    private void InvenDataSet()
    {
        foreach (GridData data in invenData.gridList) 
        {
            //�׸��带 �����Ͽ� Grids��ü�� �ڽ����� ����
            InventoryGrid newGrid = Instantiate(gridPref, grids).GetComponent<InventoryGrid>();
            newGrid.gridData = data;
            newGrid.ownInven = this;
            newGrid.InitializeGrid();
            instantGridList.Add(newGrid);
        }
        
        UpdateInvenWeight();
    }
    
    /// <summary>
    /// ������ �׸������ ���� ���Ը� ��� ���Ͽ� ������ �ݿ�
    /// </summary>
    public void UpdateInvenWeight()
    {
        float weightIndex = 0;
        for (int i = 0; i < instantGridList.Count; i++)
        {
            weightIndex += instantGridList[i].GridWeight;
        }
        InvenWeight = weightIndex;
    }

    /// <summary>
    /// 
    /// </summary>
    public bool CanAffordWeight(float addWeight, ItemObject overlap)
    {
        UpdateInvenWeight();
        float curWeight = invenWeight;

        if(overlap != null)
        {
            curWeight -= overlap.itemData.item_weight;
        }

        if (curWeight + addWeight > invenData.limitWeight)
        {
            Debug.Log("�κ��丮�� ���԰� �Ѱ踦 ����");
            return false;
        }

        return true;
    }

    
}

/* �÷��� �κ��丮�� �κ��丮�� ��Ȱ��ȭ�ϴ� �ڵ� (�÷��� �κ��丮 ������)
public void InventoryClose()
{
    if (InventoryController.invenInstance.isItemSelected == false)
    {
        gameObject.SetActive(false);
        InventoryController.invenInstance.SelectedInven = null;
    }

// �κ��丮�� ũ�⸦ �����ϴ� �޼���
public void AdjustInventorySize()
{
    if (instantGridList.Count == 0)
        return;

    float minX = float.MaxValue;
    float maxX = float.MinValue;
    float minY = float.MaxValue;
    float maxY = float.MinValue;

    // �� �׸����� ũ��� ��ġ�� ������� �κ��丮 ũ�� ���
    foreach (InventoryGrid newGrid in instantGridList)
    {
        RectTransform gridRect = newGrid.GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        gridRect.GetWorldCorners(corners);

        // �׸����� ��ǥ�� ũ��
        Vector2 gridPos = newGrid.gridData.gridPos;
        Vector2 gridSize = new Vector2(newGrid.gridData.gridSize.x * InventoryGrid.WidthOfTile, newGrid.gridData.gridSize.y * InventoryGrid.WidthOfTile) ;

        // �׸����� �ڳ� ��ġ ���
        float gridMinX = gridPos.x;
        float gridMaxX = gridPos.x + gridSize.x;
        float gridMinY = gridPos.y - gridSize.y; // �Ʒ��� �������Ƿ� y�� gridSize.y�� ����
        float gridMaxY = gridPos.y;

        minX = Mathf.Min(minX, gridMinX);
        maxX = Mathf.Max(maxX, gridMaxX);
        minY = Mathf.Min(minY, gridMinY);
        maxY = Mathf.Max(maxY, gridMaxY);

    }

    // �κ��丮 ������Ʈ�� ũ�⸦ ����
    float width = maxX - minX + offsetX*2;
    float height = maxY - minY + offsetY*2;

    inventoryRect.sizeDelta = new Vector2(width, height);

    Debug.Log($"InvenData Size - Width: {width}, Height: {height}");
}*/

